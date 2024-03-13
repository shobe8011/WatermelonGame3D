using UnityEngine;
using System.Threading;  // cancellationTokenSource を使うために必要
using Cysharp.Threading.Tasks;

public class MoveFruit : MonoBehaviour
{

    [SerializeField] private AudioSource _audioClip;
    [SerializeField] private AudioClip _FallSE;

    private SpawnFruits _spawnFruits = null;
    private GameObject _setFruit = null;

    private readonly Vector3 k_firstCreatePosition = new Vector3(0.0f, 275.0f, 450.0f);
    private readonly float k_moveSpeed = 3.0f;
    private readonly float k_fallPower = 500.0f;

    private void Awake()
    {
        _spawnFruits = GetComponent<SpawnFruits>();
    }


    /// <summary>
    /// 次に落とすフルーツを移動させる
    /// </summary>
    /// <param name="isRight"></param>
    public void MoveNextFruitPositionX(bool isRight)
    {
        if (_setFruit == null) return;
        var moveLength = new Vector3(k_moveSpeed, 0.0f, 0.0f);
        _setFruit.transform.position += isRight ? moveLength : -moveLength;
    }

    public void MoveNextFruitPositionZ(bool isfront)
    {
        if (_setFruit == null) return;
        var moveLength = new Vector3(0.0f, 0.0f, k_moveSpeed);
        _setFruit.transform.position += isfront ? -moveLength : moveLength;
    }

    /// <summary>
    /// フルーツが制限範囲を超えたら中心に戻す
    /// </summary>
    public void SetCenterPosition()
    {
        if (_setFruit != null)
        {
            _setFruit.transform.position = k_firstCreatePosition;
        }
    }

    /// <summary>
    /// フルーツを落とす
    /// </summary>
    public async UniTask FallFruit(GameObject fallFruit, CancellationToken token)
    {
        try
        {
            // キャンセルの命令が出たら処理をさせない
            if (fallFruit == null || _setFruit == null || token.IsCancellationRequested) return;
            Rigidbody rb = fallFruit.GetComponent<Rigidbody>();
            rb.useGravity = true;

            // 落ちるのが遅いから、落とすときに下方向の力を加える
            rb.AddForce(rb.mass * Vector3.down * k_fallPower, ForceMode.Impulse);
            _audioClip.PlayOneShot(_FallSE);
            _setFruit = null;
            _spawnFruits.ReleaseFruit();

            // 落としたフルーツが次のフルーツとぶつからないように待つ
            await UniTask.Delay(System.TimeSpan.FromSeconds(1.5f));
        }
        catch
        {
            Debug.Log("落とすのキャンセルされた");
            Destroy(fallFruit);
            _setFruit = null;
        }
    }

    public void SetFallFruit(GameObject fruit)
    {
        _setFruit = fruit;
    }
}
