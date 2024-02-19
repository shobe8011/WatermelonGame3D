using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private enum GameState
    {
        none,
        Start,
        Set,
        Fall,
        GameOver,
    }
    private GameState _gameState = GameState.none;

    // �������������ق����������t���[�c
    public enum FruitsKinds
    {
        cherry,
        strawberry,
        grape, 
        dekopon,
        persimmon,
        apple,
        pear,
        peach,
        pinapple,
        melon,
        watermelon,
        none,
    }

    [SerializeField] Camera _MainCamera;
    [SerializeField] Camera _TopCamera;

    private SpawnFruits _spawnFruits = null;
    public InitializeFruits _initializeFruits { get; } = new InitializeFruits();
    private ScoreManager _scoreManager = null;
    private GameObject _nextFruit = null;
    private bool _canFall = false;

    async void Start()
    {
        _spawnFruits = GetComponent<SpawnFruits>();
        _scoreManager = GetComponent<ScoreManager>();
        await _initializeFruits.InitializeList();
        await _initializeFruits.SetFruitsMaterial();
        _gameState = GameState.Start;
    }

    // Update is called once per frame
    async void Update()
    {
        switch(_gameState)
        {
            case GameState.none:
                break;

            // �Q�[���J�n�O
            case GameState.Start:
            {
                // �Q�[��������������
                InitializeGame();
                break; 
            }

            // �t���[�c���Z�b�g
            case GameState.Set:
            {
                // �t���[�c���Z�b�g
                _nextFruit = await _spawnFruits.SetNextFruit();
                _gameState = GameState.Fall;
                break;
            }

            // �ʂ𗎂Ƃ�
            case GameState.Fall:
            {
                // �ʂ𗎂Ƃ�
                // �t���O�� true �ɂȂ�܂őҋ@
                //await UniTask.WaitUntil(() => _canFall == true);
                if (!_canFall || _nextFruit == null) return;
                _canFall = false;
                await _spawnFruits.FallFruit(_nextFruit);
                _nextFruit = null;
                _gameState = GameState.Set;

                // �t���[�c���o�[�𒴂�����Q�[���I�[�o�[
                break;
            }

            // �Q�[���I�[�o�[
            case GameState.GameOver:
            {
                // �Q�[���I�[�o�[�̉�ʕ\��
                // ���X�^�[�g�{�^���������ꂽ��X�e�[�g��"start"�ɕς���
                break;
            }
        }
    }

    // �Q�[���̏�����
    private async void InitializeGame()
    {
        // �Q�[���ɕK�v��UI���Z�b�g����
        _scoreManager.IniciateScore();
        ChangeViewCamera(true);

        // �Q�[���J�n
        _gameState = GameState.Set;
    }

    // �Q�[���I�[�o�[�ɂȂ����Ƃ��̏���
    public void CallGameOver()
    {
        _gameState = GameState.GameOver;
    }

    // �t���[�c�𗎂Ƃ����Ƃ��ł��邩�ǂ����̃t���O
    public void SetFallFrag(bool canFall)
    {
        if(_gameState == GameState.Fall)
        {
            _canFall = canFall;
        }
    }

    // �J�����؂�ւ��̃t���O�󂯎��
    public void ChangeViewCamera(bool isMainCamera)
    {
        _MainCamera.enabled = isMainCamera;
        _TopCamera.enabled = !isMainCamera;
    }
}
