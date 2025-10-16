using UnityEngine;
using TMPro;

namespace MyAssets.MyScripts
{
    public class MyGameManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private GameObject finalScoreText;
        [SerializeField] private GameObject playAgainButton;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GridInputController gridInputController;
        [SerializeField] private SpriteAnimatorSimple animatorSimple;

        private int _score;
        private bool _isGameOver;
        private string _currentStage = "Infant";
        
        public int Score => _score;
        public string CurrentStage => _currentStage;

        private void Start()
        {
            gridInputController.EnableInput();
            gameOverPanel.SetActive(false);

            _score = 0;
            _isGameOver = false;
            UpdateScoreUI();

            finalScoreText.SetActive(false);
        }

        public void OnSafeTile()
        {
            if (_isGameOver) return;

            _score += 1;
            UpdateScoreUI();
            UpdateStage();
        }

        public void OnDangerTile()
        {
            if (_isGameOver) return;

            FindFirstObjectByType<AudioManager>()?.PlayGameOver();
            _isGameOver = true;
            Debug.Log("Dangerous tile touched! Player died.");

            gameOverPanel.SetActive(true);
            playAgainButton.SetActive(true);

            var tmp = finalScoreText.GetComponent<TextMeshProUGUI>();
            tmp.text = $"You died as a {_score}-year-old {_currentStage.ToLower()} :(";
            finalScoreText.SetActive(true);

            gridInputController.DisableInput();
        }

        private void UpdateScoreUI()
        {
            if (scoreText != null)
                scoreText.text = "Score: " + _score;
        }

        private void UpdateStage()
        {
            if (_score < 6)
                _currentStage = "Infant";
            else if (_score < 15)
                _currentStage = "Child";
            else if (_score < 23)
                _currentStage = "Teenager";
            else if (_score < 65)
                _currentStage = "Adult";
            else
                _currentStage = "Elder";

            animatorSimple.SetSpriteSet(_currentStage);
        }
    }
}
