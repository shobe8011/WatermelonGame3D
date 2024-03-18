using System.Threading.Tasks;
using System.Threading;  // cancellationTokenSource を使うために必要
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        Start,
        Set,
        Fall,
        GameOver,
        WaitReplay,
    }
    private GameState _gameState = GameState.WaitReplay;

    // 数字が小さいほうが小さいフルーツ
    public enum FruitsKinds
    {
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
    [SerializeField] private UIViewer _UIViewer;

    private CancellationTokenSource _cancelTokenSource;
    private CancellationToken _cancelToken;
    private SpawnFruits _spawnFruits = null;
    private MoveFruit _moveFruit = null;
    public InitializeFruits _initializeFruits { get; } = new InitializeFruits();
    private ScoreManager _scoreManager = null;
    private GameObject _nextFruit = null;
    private GameObject _fruitParent = null;
    private bool _canChangeCamera = false;
    private bool _canFall = false;



    async void Awake()
    {
        Application.targetFrameRate = 30;
        _spawnFruits = GetComponent<SpawnFruits>();
        _scoreManager = GetComponent<ScoreManager>();
        _moveFruit = GetComponent<MoveFruit>();

        // 初期のロード
        await _initializeFruits.InitializeList();
        await _initializeFruits.SetFruitsMaterial();
    }


    // Update is called once per frame
    async void Update()
    {
        switch(_gameState)
        {
            case GameState.Start:
            {
                    // ゲームを初期化する
                    await InitializeGame();
                    // ゲーム開始
                    _gameState = GameState.Set;
                    break;
            }

            // フルーツをセット
            case GameState.Set:
            {
                // フルーツをセット
                _nextFruit = await _spawnFruits.SetNextFruit(_cancelToken);
                if (_nextFruit != null)
                {
                    _gameState = GameState.Fall;
                }
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
                await _moveFruit.FallFruit(_nextFruit, _cancelToken);
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
    private async Task InitializeGame()
    {
        // UniTaskのキャンセルフラグ初期化
        _cancelTokenSource = new CancellationTokenSource();
        _cancelToken = _cancelTokenSource.Token;

        // ゲームに必要なUIをセットする
        _UIViewer.SetDuringGame();
        _scoreManager.IniciateScore();
        ChangeViewCamera(true);

        // フルールの親オブジェクトをつくる
        if(_fruitParent != null)
        {
            Destroy(_fruitParent);
        }
        GameObject newFruitsObj = new GameObject();
        _fruitParent = Instantiate(newFruitsObj, this.transform.position, Quaternion.identity);
        _fruitParent.transform.SetParent(this.transform);
        _spawnFruits.Initialization(_fruitParent);

        // ゲーム開始前にタイトル画面で動かせる箱の初期化
        _moveWall.InitializeWall();

        // フラグの初期化
        _canFall = false;
        _canChangeCamera = true;
        _moveWall.SetCanFallFlag(true);
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
        _UIViewer.SetGameOver();
        _cancelTokenSource.Dispose();
        _canFall = _canChangeCamera = false;
        _moveWall.SetCanFallFlag(false);
    }

    /// <summary>
    /// カメラ切り替えのフラグ受け取り
    /// </summary>
    /// <param name="isMainCamera"></param>
    public void ChangeViewCamera(bool isMainCamera)
    {
        if (!_canChangeCamera) return;
        _MainCamera.enabled = isMainCamera;
        _TopCamera.enabled = !isMainCamera;
    }

    public void CallGameStart()
    {
        _gameState = GameState.Start;
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
    }

    /// <summary>
    /// replay　ボタンがおされたとき
    /// </summary>
    public void Replay()
    {
        _gameState = GameState.Start;
    }
}
