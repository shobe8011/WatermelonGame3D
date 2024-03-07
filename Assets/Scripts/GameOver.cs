using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] GameManager _gameManager;
    private bool _canActive = false;
    private float _coolTime = 1.0f;

    // コライダーにフルーツが触れた瞬間、ゲームオーバー
    private void OnCollisionEnter(Collision collision)
    {
        _gameManager.GameOver();
    }

    private void Update()
    {
        if (_canActive)
        {
            _coolTime = 1.0f;
            _canActive = false;
        }
        else
        {
            _coolTime -= Time.deltaTime;
            this.gameObject.SetActive(false);
            if (_coolTime <= 0)
            {
                _canActive = true;
                this.gameObject.SetActive(true);
            }
        }
    }

    public void SetGameOverFlag(bool canActiveFlag)
    {
        if (_canActive)
        {
            //this.gameObject.SetActive(canActiveFlag);
        }
        else
        {
            //this.gameObject.SetActive(false);
        }
    }
}
