using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] UIViewer _UIViewer;
    private int _score { get; set; } = 0;
    private int _highScore = 0;

    private string _scoreKey = "HighScore";

    public void SetHighScore()
    {
        _highScore = PlayerPrefs.GetInt(_scoreKey);
    }

    // スコアを初期化
    public void IniciateScore()
    {
        _score = 0;
        _UIViewer.SetScore(_score);
    }

    // スコアを加算
    public void AddScore(int getScore)
    {
        _score += getScore;
        _UIViewer.SetScore(_score);
    }

    // TODO: 過去の最高スコアと比較する
    public void CheckHighScore()
    {
        if(_highScore < _score)
        {
            PlayerPrefs.SetInt(_scoreKey, _highScore);
        }
    }
}
