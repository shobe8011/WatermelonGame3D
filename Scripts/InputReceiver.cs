using UnityEngine;

public class InputReceiver : MonoBehaviour
{
    private GameManager _gameManager = null;
    private SpawnFruits _spawnFruit = null;

    private void Start()
    {
        _gameManager = GetComponent<GameManager>();
        _spawnFruit = GetComponent<SpawnFruits>();
    }

    private void Update()
    {
        // 左クリックでフルーツを落とす
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            _gameManager.SetFallFrag(true);
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
    }
}
