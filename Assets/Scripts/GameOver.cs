using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] GameManager _gameManager;

    // コライダーにフルーツが触れた瞬間、ゲームオーバー
    private void OnCollisionEnter(Collision collision)
    {
        var outFruit = collision.gameObject;
        _gameManager.GameOver();
        Destroy(outFruit, 1.0f);
    }
}
