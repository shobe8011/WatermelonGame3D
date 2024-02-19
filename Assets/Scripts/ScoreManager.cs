using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _scoreViewer;
    private int _score { get; set; } = 0;

    // �X�R�A��������
    public void IniciateScore()
    {
        _score = 0;
        _scoreViewer.text = "score:" + _score;
    }

    // �X�R�A�����Z
    public void AddScore(int getScore)
    {
        _score += getScore;
        _scoreViewer.text = "score:" + _score;
    }

    // TODO: �ߋ��̍ō��X�R�A�Ɣ�r����
}
