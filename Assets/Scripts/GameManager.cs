using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private enum GameState
    {
        none,
        Start,
        Set,
        Fall,
        wait,
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

    private SpawnFruits _spawnFruits = null;
    public InitializeFruits _initializeFruits { get; } = new InitializeFruits();
    private GameObject _nextFruit = null;
    private bool _canFall = false;
    private float _waitTime = 10.0f;

    // Start is called before the first frame update
    async void Awake()
    {
        await _initializeFruits.InitializeList();
    }
    async void Start()
    {
        _spawnFruits = GetComponent<SpawnFruits>();
        //await _initializeFruits.InitializeList();
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
                _waitTime = 10.0f;
                // フルーツをセット
                _nextFruit = await _spawnFruits.SetNextFruit();
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
                _canFall = await _spawnFruits.FallFruit(_nextFruit);
                _nextFruit = null;
                _gameState = GameState.Set;

                // フルーツがバーを超えたらゲームオーバー
                break;
            }

            // フルーツが落ちるのを待つ
            case GameState.wait:
            {
                _waitTime -= Time.deltaTime;
                if(_waitTime <= 0.0f)
                {
                    _gameState = GameState.Set;
                }
                break;
            }

            // ゲームオーバー
            case GameState.GameOver:
            {
                // ゲームオーバーの画面表示
                // リスタートボタンが押されたらステートを"start"に変える
                break;
            }
        }
    }

    // ゲームの初期化
    private async void InitializeGame()
    {
        // ゲームに必要なUIをセットする
        // await

        // ゲーム開始
        //_canFall = true;
        _gameState = GameState.Set;
    }

    // ゲームオーバーになったときの処理
    public void CallGameOver()
    {
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
}
