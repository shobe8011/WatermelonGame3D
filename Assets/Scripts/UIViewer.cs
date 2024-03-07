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

    /// <summary>
    /// �Q�[�����̕s�v��UI�͏���
    /// </summary>
    public void SetDuringGame()
    {
        SetBeforeGameTextUI(false);
        SetGameOverUI(false);
    }

    /// <summary>
    /// �Q�[���I�[�o�[�̕\�����o��
    /// </summary>
    public void SetGameOver()
    {
        SetGameOverUI(true);
    }

    /// <summary>
    /// �X�R�A�\���X�V
    /// </summary>
    /// <param name="score"></param>
    public void SetScore(int score)
    {
        _frontScoreViewer.text = "score:" + score;
        _topScoreViewer.text = "score:" + score;
    }

    /// <summary>
    /// �Q�[���J�n�O�ɏo�Ă���UI�̕\���̐؂�ւ�
    /// </summary>
    /// <param name="isBefore"></param>
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

    /// <summary>
    /// �{�^���������ꂽ�Ƃ��̏���
    /// </summary>
    public void PushGameStartButton()
    {
        SetDuringGame();
        _gameManager.CallGameStart();
    }
    public void PushReplayButton()
    {
        _gameManager.Replay();
    }

}
