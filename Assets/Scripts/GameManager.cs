using Cysharp.Threading.Tasks;
using System.Threading;  // cancellationTokenSource を使うために必要
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private enum GameState
    {
        none,
        Start,
        Set,
        Fall,
        GameOver,
    }
    private GameState _gameState = GameState.none;

    // 数字が小さいほうが小さいフルーツ
    public enum FruitsKinds
    {
        cherry,
        strawberry,
        grape, 
        dekopon,
        persimmon,
        apple,
        pear,
        peach,
        pinapple,
        melon,
        watermelon,
        none,
    }

    [SerializeField] Camera _MainCamera;
    [SerializeField] Camera _TopCamera;
    [SerializeField] GameObject _gameOverText;
    [SerializeField] GameObject _ReplayButton;

    private CancellationTokenSource _cancelTokenSource;
    private CancellationToken _cancelToken;
    private SpawnFruits _spawnFruits = null;
    public InitializeFruits _initializeFruits { get; } = new InitializeFruits();
    private ScoreManager _scoreManager = null;
    private GameObject _nextFruit = null;
    private GameObject _fruitParent = null;
    private bool _canFall = false;

    async void Start()
    {
        _spawnFruits = GetComponent<SpawnFruits>();
        _scoreManager = GetComponent<ScoreManager>();
        await _initializeFruits.InitializeList();
        await _initializeFruits.SetFruitsMaterial();
        _gameState = GameState.Start;
    }

    // Update is called once per frame
    async void Update()
    {
        switch(_gameState)
        {
            case GameState.none:
                break;

            // ゲーム開始前
            case GameState.Start:
            {
                // ゲームを初期化する
                InitializeGame();
                break; 
            }

            // フルーツをセット
            case GameState.Set:
            {
                // フルーツをセット
                _nextFruit = await _spawnFruits.SetNextFruit(_cancelToken);
                _gameState = GameState.Fall;
                break;
            }

            // 玉を落とす
            case GameState.Fall:
            {
                // 玉を落とす
                // フラグが true になるまで待機
                //await UniTask.WaitUntil(() => _canFall == true);
                if (!_canFall || _nextFruit == null) return;
                _canFall = false;
                await _spawnFruits.FallFruit(_nextFruit, _cancelToken);
                _nextFruit = null;
                _gameState = GameState.Set;

                // フルーツがバーを超えたらゲームオーバー
                break;
            }

            // ゲームオーバー
            case GameState.GameOver:
            {
                // ゲームオーバーの画面表示
                // TODO:ハイスコアの場合は保存
                // リスタートボタンが押されたらステートを"start"に変える
                break;
            }
        }
    }

    // ゲームの初期化
    private async void InitializeGame()
    {
        _cancelTokenSource = new CancellationTokenSource();
        _cancelToken = _cancelTokenSource.Token;
        // ゲームに必要なUIをセットする
        _scoreManager.IniciateScore();
        ChangeViewCamera(true);
        GameObject newFruitsObj = new GameObject();
        _fruitParent = Instantiate(newFruitsObj, this.transform.position, Quaternion.identity);
        _fruitParent.transform.SetParent(this.transform);
        _spawnFruits.Initialization(_fruitParent);

        // ゲーム開始
        _gameState = GameState.Set;
    }

    // ゲームオーバーになったときの処理
    public void CallGameOver()
    {
        ChangeViewCamera(true);
        _gameState = GameState.GameOver;
    }

    // フルーツを落とすことができるかどうかのフラグ
    public void SetFallFrag(bool canFall)
    {
        if(_gameState == GameState.Fall)
        {
            _canFall = canFall;
        }
    }

    // カメラ切り替えのフラグ受け取り
    public void ChangeViewCamera(bool isMainCamera)
    {
        _MainCamera.enabled = isMainCamera;
        _TopCamera.enabled = !isMainCamera;
    }

    // ゲームオーバーを検知
    public void GameOver()
    {
        _gameState = GameState.GameOver;
        _cancelTokenSource.Cancel();
        _gameOverText.SetActive(true);
        _ReplayButton.SetActive(true);
        _cancelTokenSource.Dispose();
        Debug.Log("gameOver");
    }

    // replay　ボタンがおされたとき
    public void PushReplayButton()
    {
        _gameOverText.SetActive(false);
        _ReplayButton.SetActive(false);
        Destroy(_fruitParent);
        _gameState = GameState.Start;
    }
}
