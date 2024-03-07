using UnityEngine;
using System.Threading;  // cancellationTokenSource を使うために必要
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class SpawnFruits : MonoBehaviour
{
    [SerializeField] private AudioSource _audioClip;
    [SerializeField] private AudioClip _BombSE;
    private GameManager _gameManager = null;
    private MoveFruit _moveFruit = null;


    private GameManager.FruitsKinds _fruitsKind1 = GameManager.FruitsKinds.none;
    private GameManager.FruitsKinds _fruitsKind2 = GameManager.FruitsKinds.none;

    private readonly int FIRST_CREATE_FRUIT_KINDS = 4;
    private readonly Vector3 k_firstCreatePosition = new Vector3(0.0f, 275.0f, 450.0f);
    private readonly Vector3 k_beforeExplosionSize = new Vector3(0.5f, 0.5f, 0.5f);

    // null許容型　初期化するときもnullにする
    private Vector3? _fruits1Pos = null;
    private Vector3? _fruits2Pos = null;
    private Vector3? _HalfPoint = null;

    private GameObject _fruitParent = null;
    private GameObject _nextFruit = null;
    [SerializeField] private GameObject _baseSphere;
    private InitializeFruits _initializeFruits = null;
    private ScoreManager _scoreManager = null;
    private bool _canCreateFruit = true;

    private void Start()
    {
        _gameManager = GetComponent<GameManager>();
        _scoreManager = GetComponent<ScoreManager>();
        _moveFruit = GetComponent<MoveFruit>();
        _initializeFruits = _gameManager._initializeFruits;

        var cts = new CancellationTokenSource();
    }

    /// <summary>
    /// 次に落とすフルーツを決め、初期位置にセット
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async UniTask<GameObject> SetNextFruit(CancellationToken token)
    {
        try
        {
            if (!_canCreateFruit && token.IsCancellationRequested) return null;
            _canCreateFruit = true;

            // 生成前に必要なものを取得
            int creatFruit;
            creatFruit = Random.Range(0, FIRST_CREATE_FRUIT_KINDS);
            Vector3 fruitsSize = await _initializeFruits.GetFruitSize(creatFruit);
            // マテリアル取得
            var material = await _initializeFruits.GetFruitMaterial(creatFruit);

            // ゲームオブジェクトを生成
            _nextFruit = Instantiate(_baseSphere, k_firstCreatePosition, Quaternion.identity);

            // 取得したものを反映
            _nextFruit.name = _initializeFruits.GetFruitName(creatFruit);

            // フルーツの種類を与える
            var fruitKind = _initializeFruits.GetFruitKind(creatFruit);
            _nextFruit.GetComponent<CollisionFruit>().SetFruitKind(fruitKind);
            _nextFruit.transform.localScale = fruitsSize;
            _nextFruit.GetComponent<MeshRenderer>().material = material;

            // GameManagerの子オブジェクトにする
            _nextFruit.transform.SetParent(_fruitParent.transform);
            _moveFruit.SetFallFruit(_nextFruit);
            return _nextFruit;
        }
        catch
        {
            Debug.Log("生成キャンセル");
            Destroy(_nextFruit);
            return null;
        }
    }

    /// <summary>
    /// 同じフルーツ同士がくっついたときに進化先のフルーツの生成地点を求める
    /// </summary>
    /// <param name="fruitsKinds"></param>
    /// <param name="fruitPosition"></param>
    public void CalcurationHalfPoint(GameManager.FruitsKinds fruitsKinds, Vector3 fruitPosition)
    {
        if(_fruits1Pos == null)
        {
            _fruits1Pos = fruitPosition;
            _fruitsKind1 = fruitsKinds;
        }
        else if(_fruits2Pos == null)
        {
            _fruits2Pos = fruitPosition;
            _fruitsKind2 = fruitsKinds;
        }
        else
        {
            Debug.LogError("両方ポジションが埋まっています");
        }

        // 両方埋まったらフルーツの生成位置を渡す
        if(_fruits1Pos != null 
            && _fruits2Pos != null
            && _fruitsKind1 == _fruitsKind2)
        {
            _HalfPoint = (_fruits1Pos + _fruits2Pos) / 2;
            Vector3 halfPosition = _HalfPoint ?? Vector3.zero;
            EvolutionFruit(_fruitsKind1, halfPosition);
        }
    }

    /// <summary>
    /// 同じフルーツ同士がくっついたときに進化させる
    /// </summary>
    /// <param name="beforeFruit"> 進化前のフルーツ </param>
    /// <param name="createPosition"> 生成後の位置 </param>
    public async void EvolutionFruit(GameManager.FruitsKinds beforeFruit, Vector3 createPosition)
    {
        // スコアの加算
        _scoreManager.AddScore(_initializeFruits.GetFruitScore((int)beforeFruit));

        if(beforeFruit != GameManager.FruitsKinds.watermelon)
        {
            int nextFruit = (int)beforeFruit + 1;
            var fruitBase = _initializeFruits.GetFruitsBase(nextFruit);

            // マテリアルしゅとく
            var material = fruitBase.fruitMaterial;

            // 生成
            GameObject evolusionFruit = Instantiate(_baseSphere, createPosition, Quaternion.identity);
            // 爆発演出を作るための縮小サイズ設定
            evolusionFruit.transform.localScale = k_beforeExplosionSize;

            // gameObjectの名前をフルーツの種類にする
            evolusionFruit.name = fruitBase.fruitName;

            // フルーツの種類を与える
            evolusionFruit.GetComponent<CollisionFruit>().SetFruitKind(fruitBase.fruitsKinds);
            evolusionFruit.GetComponent<MeshRenderer>().material = material;
            evolusionFruit.GetComponent<Rigidbody>().useGravity = true;

            // GameManagerの子オブジェクトにする
            evolusionFruit.transform.SetParent(_fruitParent.transform);

            // 一気にフルーツを元のサイズまで拡大する
            float fruitsSize = fruitBase.fruitSize;
            Vector3 fruitSize = new Vector3(fruitsSize, fruitsSize, fruitsSize);
            evolusionFruit.transform.DOScale(fruitSize, 0.1f);
            _audioClip.PlayOneShot(_BombSE);
        }

        // 使った変数の初期化
        Initiate();
    }

    /// <summary>
    /// 初期化用関数
    /// </summary>
    private void Initiate()
    {
        _fruits1Pos = null;
        _fruits2Pos = null;
        _HalfPoint = null;
        _fruitsKind1 = GameManager.FruitsKinds.none;
        _fruitsKind2 = GameManager.FruitsKinds.none;
    }

    /// <summary>
    /// gameManagerで作成されたフルーツの親オブジェをもらう
    /// </summary>
    /// <param name="fruitsParent"></param>
    public void Initialization(GameObject fruitsParent)
    {
        _fruitParent = fruitsParent;
        if(_nextFruit != null)
        {
            Destroy(_nextFruit);
            _nextFruit = null;
        }
    }

    /// <summary>
    /// フルーツの親オブジェクトを設定
    /// </summary>
    public void SetCenterPosition()
    {
        if(_nextFruit != null)
        {
            _nextFruit.transform.position = k_firstCreatePosition;
        }
    }

    public void SetCreateFruit(bool createFlag)
    {
        _canCreateFruit = createFlag;
    }
}
