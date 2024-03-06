using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIViewer : MonoBehaviour
{
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private GameObject _titleText;
    [SerializeField] private GameObject _startButton;
    [SerializeField] private GameObject _MoveFruitsText;
    [SerializeField] private GameObject _WallRotateText;
    [SerializeField] private GameObject _ShakeText;
    [SerializeField] private GameObject _DropText;
    [SerializeField] private GameObject _ActiveCameraText;
    [SerializeField] private GameObject _gameOverText;
    [SerializeField] private GameObject _ReplayButton;

    [SerializeField] private TextMeshProUGUI _frontScoreViewer;
    [SerializeField] private TextMeshProUGUI _topScoreViewer;


    public void SetBeforeGameStart()
    {
        SetBeforeGameTextUI(true);
    }

    public void SetDuringGame()
    {
        SetBeforeGameTextUI(false);
        SetGameOverUI(false);
    }

    public void SetGameOver()
    {
        SetGameOverUI(true);
    }

    public void SetScore(int score)
    {
        _frontScoreViewer.text = "score:" + score;
        _topScoreViewer.text = "score:" + score;
    }

    private void SetBeforeGameTextUI(bool isBefore)
    {
        _titleText.SetActive(isBefore);
        _startButton.SetActive(isBefore);
        _MoveFruitsText.SetActive(isBefore);
        _WallRotateText.SetActive(isBefore);
        _ShakeText.SetActive(isBefore);
        _DropText.SetActive(isBefore);
        _ActiveCameraText.SetActive(isBefore);
    }

    private void SetGameOverUI(bool isGameOver)
    {
        _gameOverText.SetActive(isGameOver);
        _ReplayButton.SetActive(isGameOver);
    }

    public void PushGameStartButton()
    {
        SetDuringGame();
        _gameManager.CallGameStart();
    }
}
