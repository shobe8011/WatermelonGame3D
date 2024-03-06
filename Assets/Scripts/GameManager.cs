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
        WaitReplay,
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

    [SerializeField] private MoveWall _moveWall;
    [SerializeField] private Camera _MainCamera;
    [SerializeField] private Camera _TopCamera;
    [SerializeField] private GameObject _gameOverText;
    [SerializeField] private GameObject _ReplayButton;

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
                if (_nextFruit == null)
                {
                    _gameState = GameState.Set;
                    _canFall = false;
                    return;
                }
                if (!_canFall)
                {
                    return;
                }

                _canFall = false;
                await _spawnFruits.FallFruit(_nextFruit, _cancelToken);
                _nextFruit = null;
                // GameOverになっている可能性をチェック
                if (_gameState == GameState.Fall)
                {
                    _gameState = GameState.Set;
                }
                break;
            }

            // ゲームオーバー
            case GameState.GameOver:
            {
                // ゲームオーバーの画面表示
                CallGameOver();
                _gameState = GameState.WaitReplay;
                break;
            }

            // リプレイ待ち
            case GameState.WaitReplay:
            {
                    break;
            }
        }
    }

    /// <summary>
    /// ゲームの初期化
    /// </summary>
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
        _moveWall.InitializeWall();
        _canFall = false;
        _moveWall.SetCanFallFlag(true);

        // ゲーム開始
        _gameState = GameState.Set;
    }

    /// <summary>
    /// フルーツを落とすことができるかどうかのフラグ
    /// </summary>
    /// <param name="canFall"></param>
    public void SetFallFrag(bool canFall)
    {
        if (_gameState == GameState.Fall)
        {
            _canFall = canFall;
        }
    }

    /// <summary>
    /// ゲームオーバーになったときの処理
    /// </summary>
    private void CallGameOver()
    {
        ChangeViewCamera(true);
        _gameState = GameState.GameOver;
        _cancelTokenSource.Cancel();
        _gameOverText.SetActive(true);
        _ReplayButton.SetActive(true);
        _cancelTokenSource.Dispose();
        _canFall = false;
        _moveWall.SetCanFallFlag(false);
    }

    /// <summary>
    /// カメラ切り替えのフラグ受け取り
    /// </summary>
    /// <param name="isMainCamera"></param>
    public void ChangeViewCamera(bool isMainCamera)
    {
        _MainCamera.enabled = isMainCamera;
        _TopCamera.enabled = !isMainCamera;
    }

    /// <summary>
    /// ゲームオーバーを検知
    /// </summary>
    public void GameOver()
    {
        _gameState = GameState.GameOver;
        if (_nextFruit != null)
        {
            Destroy(_nextFruit);
            _nextFruit = null;
        }
        Debug.Log("gameOver");
    }

    /// <summary>
    /// replay　ボタンがおされたとき
    /// </summary>
    public void PushReplayButton()
    {
        _gameOverText.SetActive(false);
        _ReplayButton.SetActive(false);
        Destroy(_fruitParent);
        _gameState = GameState.Start;
    }
}
