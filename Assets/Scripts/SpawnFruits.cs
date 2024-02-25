using UnityEngine;
using System.Threading;  // cancellationTokenSource ���g�����߂ɕK�v
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class SpawnFruits : MonoBehaviour
{
    [SerializeField] private float move = 3.0f;
    private GameManager _gameManager = null;
    private GameManager.FruitsKinds _fruitsKind1 = GameManager.FruitsKinds.none;
    private GameManager.FruitsKinds _fruitsKind2 = GameManager.FruitsKinds.none;

    private readonly int FIRST_CREATE_FRUIT_KINDS = 4;
    private readonly Vector3 k_firstCreatePosition = new Vector3(0.0f, 240.0f, 450.0f);
    private readonly Vector3 k_beforeExplosionSize = new Vector3(0.5f, 0.5f, 0.5f);

    // null���e�^�@����������Ƃ���null�ɂ���
    private Vector3? _fruits1Pos = null;
    private Vector3? _fruits2Pos = null;
    private Vector3? _HalfPoint = null;

    private GameObject _fruitParent = null;
    private GameObject _nextFruit = null;
    [SerializeField] private GameObject _baseSphere;
    private InitializeFruits _initializeFruits = null;
    private ScoreManager _scoreManager = null;

    private void Start()
    {
        _gameManager = GetComponent<GameManager>();
        _scoreManager = GetComponent<ScoreManager>();
        _initializeFruits = _gameManager._initializeFruits;

        var cts = new CancellationTokenSource();
    }

    // ���ɗ��Ƃ��t���[�c�����߁A�����ʒu�ɃZ�b�g
    public async UniTask<GameObject> SetNextFruit(CancellationToken token)
    {
        try
        {
            // �܂����Ƃ���Ă��Ȃ��t���[�c����������return
            if (_nextFruit != null || token.IsCancellationRequested) return null;

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
            _nextFruit.transform.SetParent(_fruitParent.transform);
            return _nextFruit;
        }
        catch
        {
            Debug.Log("�����L�����Z��");
            return null;
        }
    }

    // ���ɗ��Ƃ��t���[�c���ړ�������
    public void MoveNextFruitPositionX(bool isRight)
    {
        if (_nextFruit == null) return;
        var moveLength = new Vector3(move, 0.0f, 0.0f);
        _nextFruit.transform.position += isRight ? moveLength : -moveLength;
        // TODO: �ړ�������t����
    }

    public void MoveNextFruitPositionZ(bool isfront)
    {
        if (_nextFruit == null) return;
        var moveLength = new Vector3(0.0f, 0.0f, move);
        _nextFruit.transform.position += isfront ? -moveLength : moveLength;
        // TODO: �ړ�������t����
    }

    /// <summary>
    /// �t���[�c�𗎂Ƃ�
    /// </summary>
    public async UniTask FallFruit(GameObject fallFruit, CancellationToken token)
    {
        try
        {
            // �L�����Z���̖��߂��o���珈���������Ȃ�
            if (fallFruit == null || token.IsCancellationRequested) return;
            Rigidbody rb = fallFruit.GetComponent<Rigidbody>();
            rb.useGravity = true;

            // ������̂��x������A���Ƃ��Ƃ��ɉ������̗͂�������
            rb.AddForce(rb.mass * Vector3.down * 500.0f, ForceMode.Impulse);
            _nextFruit = null;

            // ���Ƃ����t���[�c�����̃t���[�c�ƂԂ���Ȃ��悤�ɑ҂�
            await UniTask.Delay(System.TimeSpan.FromSeconds(1.5f));
        }
        catch
        {
            Debug.Log("���Ƃ��̃L�����Z�����ꂽ");
        }
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
        var fruitBase = _initializeFruits.GetFruitsBase(nextFruit);
        // �t���[�c�̃X�R�A���Z
        _scoreManager.AddScore(fruitBase.score);

        // TODO:�X�C�J��2���������Ƃ��̑Ώ�

        // �}�e���A������Ƃ�
        var material = fruitBase.fruitMaterial;

        // ����
        GameObject evolusionFruit = Instantiate(_baseSphere, createPosition, Quaternion.identity);
        // �������o����邽�߂̏k���T�C�Y�ݒ�
        evolusionFruit.transform.localScale = k_beforeExplosionSize;

        // gameObject�̖��O���t���[�c�̎�ނɂ���
        evolusionFruit.name = fruitBase.fruitName;

        // �t���[�c�̎�ނ�^����
        evolusionFruit.GetComponent<CollisionFruit>().SetFruitKind(fruitBase.fruitsKinds);
        evolusionFruit.GetComponent<MeshRenderer>().material = material;
        evolusionFruit.GetComponent<Rigidbody>().useGravity = true;

        // GameManager�̎q�I�u�W�F�N�g�ɂ���
        evolusionFruit.transform.SetParent(_fruitParent.transform);

        // ��C�Ƀt���[�c�����̃T�C�Y�܂Ŋg�傷��
        float fruitsSize = fruitBase.fruitSize;
        Vector3 fruitSize = new Vector3(fruitsSize, fruitsSize, fruitsSize);
        evolusionFruit.transform.DOScale(fruitSize, 0.1f);

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

    // gameManager�ō쐬���ꂽ�t���[�c�̐e�I�u�W�F�����炤
    public void Initialization(GameObject fruitsParent)
    {
        _fruitParent = fruitsParent;
        _nextFruit = null;
    }
}
