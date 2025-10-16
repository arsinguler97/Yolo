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
        
        private int _score;
        private bool _isGameOver;
        
        public int Score => _score;

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
        }

        public void OnDangerTile()
        {
            if (_isGameOver) return;

            _isGameOver = true;
            Debug.Log("Dangerous tile touched! Player died.");

            gameOverPanel.SetActive(true);

            playAgainButton.SetActive(true);
            
            var tmp = finalScoreText.GetComponent<TextMeshProUGUI>();
            tmp.text = $"You've made it to {_score}!";
            finalScoreText.SetActive(true);
            
            gridInputController.DisableInput();
        }

        private void UpdateScoreUI()
        {
            if (scoreText != null)
                scoreText.text = "Score: " + _score;
        }
    }
}