using UnityEngine;

public class InputReceiver : MonoBehaviour
{
    [SerializeField] GameObject _wall;
    [SerializeField] MoveWall _moveWall;

    private GameManager _gameManager = null;
    private SpawnFruits _spawnFruit = null;

    private bool _isMainCameraView = true;
    private bool _isCoolTime = false;
    private float _clickCoolTime = 2.0f;

    private void Start()
    {
        _gameManager = GetComponent<GameManager>();
        _spawnFruit = GetComponent<SpawnFruits>();
    }

    private void Update()
    {
        // ���N���b�N �������́@�G���^�[�L�[�Ńt���[�c�𗎂Ƃ�
        if(Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Return)
            && !_isCoolTime)
        {
            _isCoolTime = true;
            _gameManager.SetFallFrag(true);
            _clickCoolTime = 2.0f;
        }
        else
        {
            _clickCoolTime -= Time.deltaTime;
            if(_clickCoolTime <= 0)
            {
                _isCoolTime = false;
            }
        }

        // wasd�Ńt���[�c�𗎂Ƃ��O�Ɉړ�
        if(Input.GetKey(KeyCode.W))
        {
            _spawnFruit.MoveNextFruitPositionZ(false);
        }
        if (Input.GetKey(KeyCode.A))
        {
            _spawnFruit.MoveNextFruitPositionX(false);
        }
        if (Input.GetKey(KeyCode.S))
        {
            _spawnFruit.MoveNextFruitPositionZ(true);
        }
        if (Input.GetKey(KeyCode.D))
        {
            _spawnFruit.MoveNextFruitPositionX(true);
        }

        // C �ŃJ�����؂�ւ�
        if(Input.GetKeyDown(KeyCode.C))
        {
            // �J�����\���̃t���O�𔽓]������
            _isMainCameraView = _isMainCameraView ? false : true;
            _gameManager.ChangeViewCamera(_isMainCameraView);
        }

        // �� �Ŕ���X����]
        if(Input.GetKey(KeyCode.UpArrow))
        {
            _moveWall.RotateWallX(true);
        }

        // �� �Ŕ���X����]
        if (Input.GetKey(KeyCode.DownArrow))
        {
            _moveWall.RotateWallX(false);
        }

        // ���@�Ŕ���Z������]
        if (Input.GetKey(KeyCode.RightArrow))
        {
            _moveWall.RotateWallZ(false);
        }

        // ���@�Ŕ���Z������]
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            _moveWall.RotateWallZ(true);
        }

        // ����h�炷
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            _moveWall.ShakeWalls();
        }
    }
}
