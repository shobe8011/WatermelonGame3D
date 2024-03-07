using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] UIViewer _UIViewer;
    private int _score { get; set; } = 0;

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
}
