using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class SpawnFruits : MonoBehaviour
{
    private GameManager _gameManager = null;
    private GameManager.FruitsKinds _fruitsKind1 = GameManager.FruitsKinds.none;
    private GameManager.FruitsKinds _fruitsKind2 = GameManager.FruitsKinds.none;

    private readonly int FIRST_CREATE_FRUIT_KINDS = 4;
    private readonly Vector3 k_firstCreatePosition = new Vector3(-20.0f, 225.0f, 450.0f);
    private readonly Vector3 k_beforeExplosionSize = new Vector3(0.5f, 0.5f, 0.5f);

    // null許容型　初期化するときもnullにする
    private Vector3? _fruits1Pos = null;
    private Vector3? _fruits2Pos = null;
    private Vector3? _HalfPoint = null;

    private GameObject _nextFruit = null;
    [SerializeField] private GameObject _baseSphere;
    private InitializeFruits _initializeFruits = null;

    private void Start()
    {
        _gameManager = GetComponent<GameManager>();
        _initializeFruits = _gameManager._initializeFruits;
    }

    // 次に落とすフルーツを決め、初期位置にセット
    public async UniTask<GameObject> SetNextFruit()
    {
        // まだ落とされていないフルーツがあったらreturn
        if (_nextFruit != null) return null;
        
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
        _nextFruit.transform.SetParent(this.transform);
        return _nextFruit;
    }

    // 次に落とすフルーツを移動させる
    public void MoveNextFruitPositionX(bool isRight)
    {
        if (_nextFruit == null) return;
        var moveLength = new Vector3(0.5f, 0.0f, 0.0f);
        _nextFruit.transform.position += isRight ? moveLength : -moveLength;
        // TODO: 移動制限を付ける
    }

    public void MoveNextFruitPositionZ(bool isfront)
    {
        if (_nextFruit == null) return;
        var moveLength = new Vector3(0.0f, 0.0f, 0.5f);
        _nextFruit.transform.position += isfront ? -moveLength : moveLength;
        // TODO: 移動制限を付ける
    }

    /// <summary>
    /// フルーツを落とす
    /// </summary>
    public async UniTask<bool> FallFruit(GameObject fallFruit)
    {
        // TODO:フルーツを落とすフラグを作る
        if (fallFruit == null) return false;
        Rigidbody rb = fallFruit.GetComponent<Rigidbody>();
        //fallFruit.AddComponent<Rigidbody>();
        rb.useGravity = true;
        //rb.isKinematic = true;
        //fallFruit.GetComponent<Rigidbody>().mass = 1000.0f;
        // 落ちるのが遅いから、落とすときに、下方向の力を加えた方がいいかも
        //fallFruit.GetComponent<Rigidbody>().AddForce(0.0f, -1000.0f, 0.0f, ForceMode.Impulse);
        //rb.AddForce(0.0f, -500.0f, 0.0f, ForceMode.Impulse);
        rb.AddForce(rb.mass * Vector3.down * 500.0f, ForceMode.Impulse);
        //rb.AddForceAtPosition

        _nextFruit = null;

        // 落としたフルーツが次のフルーツとぶつからないように待つ
        await UniTask.Delay(System.TimeSpan.FromSeconds(2.0f));
        //await UniTask.DelayFrame(1);
        return false;
    }

    // 同じフルーツ同士がくっついたときに進化先のフルーツの生成地点を求める
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
        int nextFruit = (int)beforeFruit + 1;
        // TODO:スイカが2つくっついたときの対処

        // マテリアルしゅとく
        var material = await _initializeFruits.GetFruitMaterial(nextFruit);

        // 生成
        GameObject evolusionFruit = Instantiate(_baseSphere, createPosition, Quaternion.identity);
        // 爆発演出を作るための縮小サイズ設定
        evolusionFruit.transform.localScale = k_beforeExplosionSize;

        // gameObjectの名前をフルーツの種類にする
        evolusionFruit.name = _initializeFruits.GetFruitName(nextFruit);

        // フルーツの種類を与える
        var fruitKind = _initializeFruits.GetFruitKind(nextFruit);
        evolusionFruit.GetComponent<CollisionFruit>().SetFruitKind(fruitKind);
        evolusionFruit.GetComponent<MeshRenderer>().material = material;
        //evolusionFruit.AddComponent<Rigidbody>();
        evolusionFruit.GetComponent<Rigidbody>().useGravity = true;

        // GameManagerの子オブジェクトにする
        evolusionFruit.transform.SetParent(this.transform);

        // 一気にフルーツを元のサイズまで拡大する
        Vector3 fruitsSize = await _initializeFruits.GetFruitSize(nextFruit);
        evolusionFruit.transform.DOScale(fruitsSize, 0.1f);

        // 使った変数の初期化
        Initiate();
    }

    // 初期化用関数
    private void Initiate()
    {
        _fruits1Pos = null;
        _fruits2Pos = null;
        _HalfPoint = null;
        _fruitsKind1 = GameManager.FruitsKinds.none;
        _fruitsKind2 = GameManager.FruitsKinds.none;
    }
}
