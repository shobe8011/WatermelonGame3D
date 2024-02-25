using UnityEngine;

public class InputReceiver : MonoBehaviour
{
    [SerializeField] GameObject _wall;
    [SerializeField] MoveWall _moveWall;

    private GameManager _gameManager = null;
    private SpawnFruits _spawnFruit = null;
    private readonly float _kMaxRotate = 25.0f;
    private readonly float _kMinimamRotate = -25.0f;

    private float _xRotate = 0.0f;
    private bool _isMainCameraView = true;
    private bool _isCoolTime = false;
    private float _clickCoolTime = 1.5f;
    private float rotationSpeed = 45.0f;

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
            _clickCoolTime = 1.5f;
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
            if(_xRotate < _kMaxRotate)
            {
                //var beforePosition = _wall.transform.position;
                //var moveLength = new Vector3(0.0f, 3.0f, 1.0f);
                //var rotation = Quaternion.Euler(new Vector3(30, 0, 0));
                //var scale = Vector3.one;

                //�@�s��
                //var matrix = Matrix4x4.TRS(moveLength, rotation, scale);

                // �ϊ��O�̍��W�ɍs���������
                //beforePosition = matrix.MultiplyPoint(beforePosition);
                //beforeRotate = matrix.MultiplyPoint(beforeRotate);

                //_xRotate += 1.0f;
                float angle = rotationSpeed * Time.deltaTime;
                //_wall.transform.position = beforePosition;
                _wall.transform.rotation *= Quaternion.AngleAxis(angle, Vector3.left);

                // �ړ��ʂ̌v�Z
                Matrix4x4 matrix = _wall.transform.localToWorldMatrix;
                // �ړ���̍��W
                var rightVector = new Vector3();
                rightVector.x = matrix.m30;
                rightVector.y = matrix.m31;
                rightVector.z = matrix.m32;
                _wall.transform.position = rightVector;
                Debug.Log(rightVector);

                //_wall.transform.rotation = Quaternion.Euler(_xRotate, 0.0f, 0.0f);
                //foreach (Transform wall in _wall.GetComponentInChildren<Transform>())
                //{
                //    //var rotationX = wall.eulerAngles.x;
                //    //Debug.Log(wall.name + rotationX);
                //    //rotationX += 0.01f;
                //    //rotation.x += _xRotate;
                //    //wall.rotation = Quaternion.Euler(rotationX, 0.0f, 0.0f);
                //    Quaternion rot = Quaternion.AngleAxis(0.5f, Vector3.right);
                //    wall.rotation *= rot;
                //}
            }
        }

        // �� �Ŕ���X����]
        if (Input.GetKey(KeyCode.DownArrow))
        {
            // �ǂ̍s����擾 (
            //Matrix4x4 matrix = _wall.transform.localToWorldMatrix;
            if (_xRotate > _kMinimamRotate)
            {
                //_xRotate -= 1.0f;
                float angle = rotationSpeed * Time.deltaTime;
                _wall.transform.rotation *= Quaternion.AngleAxis(angle, Vector3.right);


                //_wall.transform.rotation = Quaternion.Euler(_xRotate, 0.0f, 0.0f);
                //foreach (Transform wall in _wall.GetComponentInChildren<Transform>())
                //{
                //    //var rotationX = wall.eulerAngles.x;
                //    //Debug.Log(wall.name + rotationX);
                //    //rotationX += 0.01f;
                //    //rotation.x += _xRotate;
                //    //wall.rotation = Quaternion.Euler(rotationX, 0.0f, 0.0f);
                //    Quaternion rot = Quaternion.AngleAxis(0.5f, Vector3.left);
                //    wall.rotation *= rot;
                //}
            }
        }

        // ���@�Ŕ���Z������]
        if (Input.GetKey(KeyCode.RightArrow))
        {
            var rotate = _wall.transform.rotation.z;
            if (rotate < _kMaxRotate)
            {
                float angle = rotationSpeed * Time.deltaTime;
                _wall.transform.rotation *= Quaternion.AngleAxis(angle, Vector3.back);
            }
        }

        // ���@�Ŕ���Z������]
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            float angle = rotationSpeed * Time.deltaTime;
            _wall.transform.rotation *= Quaternion.AngleAxis(angle, Vector3.forward);
        }

        // ����h�炷
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            //var moveWall = new MoveWall();
            _moveWall.ShakeWalls();
        }
    }
}
