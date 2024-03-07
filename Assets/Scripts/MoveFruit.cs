using UnityEngine;
using System.Threading;  // cancellationTokenSource ���g�����߂ɕK�v
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
    /// ���ɗ��Ƃ��t���[�c���ړ�������
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
    /// �t���[�c�𗎂Ƃ�
    /// </summary>
    public async UniTask FallFruit(GameObject fallFruit, CancellationToken token)
    {
        try
        {
            // �L�����Z���̖��߂��o���珈���������Ȃ�
            if (fallFruit == null || _setFruit == null || token.IsCancellationRequested) return;
            _audioClip.PlayOneShot(_FallSE);
            Rigidbody rb = fallFruit.GetComponent<Rigidbody>();
            rb.useGravity = true;

            // ������̂��x������A���Ƃ��Ƃ��ɉ������̗͂�������
            rb.AddForce(rb.mass * Vector3.down * _fallPower, ForceMode.Impulse);
            _setFruit = null;
            _spawnFruits.SetCreateFruit(true);

            // ���Ƃ����t���[�c�����̃t���[�c�ƂԂ���Ȃ��悤�ɑ҂�
            await UniTask.Delay(System.TimeSpan.FromSeconds(1.5f));
        }
        catch
        {
            Debug.Log("���Ƃ��̃L�����Z�����ꂽ");
            Destroy(fallFruit);
            _setFruit = null;
        }
    }

    public void SetFallFruit(GameObject fruit)
    {
        _setFruit = fruit;
    }
}
