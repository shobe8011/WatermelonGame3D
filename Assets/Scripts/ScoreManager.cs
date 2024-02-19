using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _scoreViewer;
    private int _score { get; set; } = 0;

    // スコアを初期化
    public void IniciateScore()
    {
        _score = 0;
        _scoreViewer.text = "score:" + _score;
    }

    // スコアを加算
    public void AddScore(int getScore)
    {
        _score += getScore;
        _scoreViewer.text = "score:" + _score;
    }

    // TODO: 過去の最高スコアと比較する
}
