using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] GameManager _gameManager;

    // �R���C�_�[�Ƀt���[�c���G�ꂽ�u�ԁA�Q�[���I�[�o�[
    private void OnCollisionEnter(Collision collision)
    {
        var outFruit = collision.gameObject;
        _gameManager.GameOver();
        Destroy(outFruit, 1.0f);
    }
}
