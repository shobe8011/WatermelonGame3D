using UnityEngine;
using System.Threading;  // cancellationTokenSource を使うために必要
using Cysharp.Threading.Tasks;

public class MoveFruit : MonoBehaviour
{

    [SerializeField] private AudioSource _audioClip;
    [SerializeField] private AudioClip _FallSE;

    private SpawnFruits _spawnFruits = null;
    private GameObject _setFruit = null;
    private float _moveSpeed = 1.0f;
    private float _fallPower = 500.0f;

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
        var moveLength = new Vector3(_moveSpeed, 0.0f, 0.0f);
        _setFruit.transform.position += isRight ? moveLength : -moveLength;
    }

    public void MoveNextFruitPositionZ(bool isfront)
    {
        if (_setFruit == null) return;
        var moveLength = new Vector3(0.0f, 0.0f, _moveSpeed);
        _setFruit.transform.position += isfront ? -moveLength : moveLength;
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
            _audioClip.PlayOneShot(_FallSE);
            Rigidbody rb = fallFruit.GetComponent<Rigidbody>();
            rb.useGravity = true;

            // 落ちるのが遅いから、落とすときに下方向の力を加える
            rb.AddForce(rb.mass * Vector3.down * _fallPower, ForceMode.Impulse);
            _setFruit = null;
            _spawnFruits.SetCreateFruit(true);

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
