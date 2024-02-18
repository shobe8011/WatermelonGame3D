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

    // null���e�^�@����������Ƃ���null�ɂ���
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

    // ���ɗ��Ƃ��t���[�c�����߁A�����ʒu�ɃZ�b�g
    public async UniTask<GameObject> SetNextFruit()
    {
        // �܂����Ƃ���Ă��Ȃ��t���[�c����������return
        if (_nextFruit != null) return null;
        
        // �����O�ɕK�v�Ȃ��̂��擾
        int creatFruit;
        creatFruit = Random.Range(0, FIRST_CREATE_FRUIT_KINDS);
        Vector3 fruitsSize = await _initializeFruits.GetFruitSize(creatFruit);
        // �}�e���A���擾
        var material = await _initializeFruits.GetFruitMaterial(creatFruit);

        // �Q�[���I�u�W�F�N�g�𐶐�
        _nextFruit = Instantiate(_baseSphere, k_firstCreatePosition, Quaternion.identity);

        // �擾�������̂𔽉f
        _nextFruit.name = _initializeFruits.GetFruitName(creatFruit);

        // �t���[�c�̎�ނ�^����
        var fruitKind = _initializeFruits.GetFruitKind(creatFruit);
        _nextFruit.GetComponent<CollisionFruit>().SetFruitKind(fruitKind);
        _nextFruit.transform.localScale = fruitsSize;
        _nextFruit.GetComponent<MeshRenderer>().material = material;

        // GameManager�̎q�I�u�W�F�N�g�ɂ���
        _nextFruit.transform.SetParent(this.transform);
        return _nextFruit;
    }

    // ���ɗ��Ƃ��t���[�c���ړ�������
    public void MoveNextFruitPositionX(bool isRight)
    {
        if (_nextFruit == null) return;
        var moveLength = new Vector3(0.5f, 0.0f, 0.0f);
        _nextFruit.transform.position += isRight ? moveLength : -moveLength;
        // TODO: �ړ�������t����
    }

    public void MoveNextFruitPositionZ(bool isfront)
    {
        if (_nextFruit == null) return;
        var moveLength = new Vector3(0.0f, 0.0f, 0.5f);
        _nextFruit.transform.position += isfront ? -moveLength : moveLength;
        // TODO: �ړ�������t����
    }

    /// <summary>
    /// �t���[�c�𗎂Ƃ�
    /// </summary>
    public async UniTask<bool> FallFruit(GameObject fallFruit)
    {
        // TODO:�t���[�c�𗎂Ƃ��t���O�����
        if (fallFruit == null) return false;
        Rigidbody rb = fallFruit.GetComponent<Rigidbody>();
        //fallFruit.AddComponent<Rigidbody>();
        rb.useGravity = true;
        //rb.isKinematic = true;
        //fallFruit.GetComponent<Rigidbody>().mass = 1000.0f;
        // ������̂��x������A���Ƃ��Ƃ��ɁA�������̗͂�������������������
        //fallFruit.GetComponent<Rigidbody>().AddForce(0.0f, -1000.0f, 0.0f, ForceMode.Impulse);
        //rb.AddForce(0.0f, -500.0f, 0.0f, ForceMode.Impulse);
        rb.AddForce(rb.mass * Vector3.down * 500.0f, ForceMode.Impulse);
        //rb.AddForceAtPosition

        _nextFruit = null;

        // ���Ƃ����t���[�c�����̃t���[�c�ƂԂ���Ȃ��悤�ɑ҂�
        await UniTask.Delay(System.TimeSpan.FromSeconds(2.0f));
        //await UniTask.DelayFrame(1);
        return false;
    }

    // �����t���[�c���m�����������Ƃ��ɐi����̃t���[�c�̐����n�_�����߂�
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
            Debug.LogError("�����|�W�V���������܂��Ă��܂�");
        }

        // �������܂�����t���[�c�̐����ʒu��n��
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
    /// �����t���[�c���m�����������Ƃ��ɐi��������
    /// </summary>
    /// <param name="beforeFruit"> �i���O�̃t���[�c </param>
    /// <param name="createPosition"> ������̈ʒu </param>
    public async void EvolutionFruit(GameManager.FruitsKinds beforeFruit, Vector3 createPosition)
    {
        int nextFruit = (int)beforeFruit + 1;
        // TODO:�X�C�J��2���������Ƃ��̑Ώ�

        // �}�e���A������Ƃ�
        var material = await _initializeFruits.GetFruitMaterial(nextFruit);

        // ����
        GameObject evolusionFruit = Instantiate(_baseSphere, createPosition, Quaternion.identity);
        // �������o����邽�߂̏k���T�C�Y�ݒ�
        evolusionFruit.transform.localScale = k_beforeExplosionSize;

        // gameObject�̖��O���t���[�c�̎�ނɂ���
        evolusionFruit.name = _initializeFruits.GetFruitName(nextFruit);

        // �t���[�c�̎�ނ�^����
        var fruitKind = _initializeFruits.GetFruitKind(nextFruit);
        evolusionFruit.GetComponent<CollisionFruit>().SetFruitKind(fruitKind);
        evolusionFruit.GetComponent<MeshRenderer>().material = material;
        //evolusionFruit.AddComponent<Rigidbody>();
        evolusionFruit.GetComponent<Rigidbody>().useGravity = true;

        // GameManager�̎q�I�u�W�F�N�g�ɂ���
        evolusionFruit.transform.SetParent(this.transform);

        // ��C�Ƀt���[�c�����̃T�C�Y�܂Ŋg�傷��
        Vector3 fruitsSize = await _initializeFruits.GetFruitSize(nextFruit);
        evolusionFruit.transform.DOScale(fruitsSize, 0.1f);

        // �g�����ϐ��̏�����
        Initiate();
    }

    // �������p�֐�
    private void Initiate()
    {
        _fruits1Pos = null;
        _fruits2Pos = null;
        _HalfPoint = null;
        _fruitsKind1 = GameManager.FruitsKinds.none;
        _fruitsKind2 = GameManager.FruitsKinds.none;
    }
}
