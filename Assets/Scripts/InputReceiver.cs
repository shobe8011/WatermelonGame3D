using UnityEngine;

public class InputReceiver : MonoBehaviour
{
    private GameManager _gameManager = null;
    private SpawnFruits _spawnFruit = null;

    private bool _isMainCameraView = true;

    private void Start()
    {
        _gameManager = GetComponent<GameManager>();
        _spawnFruit = GetComponent<SpawnFruits>();
    }

    private void Update()
    {
        // ���N���b�N�Ńt���[�c�𗎂Ƃ�
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            _gameManager.SetFallFrag(true);
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
    }
}
