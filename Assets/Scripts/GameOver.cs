using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] GameManager _gameManager;
    private bool _canActive = true;
    private float _coolTime = 1.0f;

    // コライダーにフルーツが触れた瞬間、ゲームオーバー
    private void OnCollisionEnter(Collision collision)
    {
        _gameManager.GameOver();
        // 一回発火させたら消す
        this.gameObject.GetComponent<BoxCollider>().enabled = false;
        _coolTime = 1.0f;
        _canActive = false;
    }

    void Update()
    {
        if (!_canActive)
        {
            _coolTime -= Time.deltaTime;
            if (_coolTime <= 0)
            {
                _canActive = true;
            }
            return;
        }

        _coolTime = 1.0f;
        this.gameObject.GetComponent<BoxCollider>().enabled = true;
    }

    public void SetGameOverFlag(bool canActiveFlag)
    {
        // 動ける状態で、クールタイム中でない
        if (canActiveFlag)
        {
            if(!_canActive)
            {
                _canActive = true;
            }
        }
        else
        {
            // 動けない状態で、クールタイム中 ⇒　クールタイムリセット
            if (!_canActive)
            {
                _coolTime = 1.0f;
            }
            else
            {
                _canActive = false;
                this.gameObject.GetComponent<BoxCollider>().enabled = false;
            }
        }
    }
}
