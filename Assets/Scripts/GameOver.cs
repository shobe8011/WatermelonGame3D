using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] GameManager _gameManager;

    // コライダーにフルーツが触れた瞬間、ゲームオーバー
    private void OnCollisionEnter(Collision collision)
    {
        var outFruit = collision.gameObject;
        _gameManager.GameOver();
    }

    public void SetGameOverFlag(bool isGameOver)
    {
        this.gameObject.SetActive(isGameOver);
    }
}
