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
        // 左クリック もしくは　エンターキーでフルーツを落とす
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

        // wasdでフルーツを落とす前に移動
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

        // C でカメラ切り替え
        if(Input.GetKeyDown(KeyCode.C))
        {
            // カメラ表示のフラグを反転させる
            _isMainCameraView = _isMainCameraView ? false : true;
            _gameManager.ChangeViewCamera(_isMainCameraView);
        }

        // ↑ で箱のX軸回転
        if(Input.GetKey(KeyCode.UpArrow))
        {
            _moveWall.RotateWallX(true);
        }

        // ↓ で箱のX軸回転
        if (Input.GetKey(KeyCode.DownArrow))
        {
            _moveWall.RotateWallX(false);
        }

        // →　で箱のZ軸を回転
        if (Input.GetKey(KeyCode.RightArrow))
        {
            _moveWall.RotateWallZ(false);
        }

        // ←　で箱のZ軸を回転
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            _moveWall.RotateWallZ(true);
        }

        // 箱を揺らす
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            _moveWall.ShakeWalls();
        }
    }
}
