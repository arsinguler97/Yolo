using UnityEngine;
using TMPro;
using _Project.Code.Core.ServiceLocator;
using _Project.Code.Gameplay.Scenes;

namespace MyAssets.MyScripts
{
    public class MyGameManager : MonoBehaviour
    {
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private string menuSceneName = "MenuScene";

        private int _score;
        private bool _isGameOver;

        private void Start()
        {
            _score = 0;
            _isGameOver = false;
            UpdateScoreUI();
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
            Debug.Log("Dangerous tile touched! Returning to menu...");
            ServiceLocator.Get<SceneService>().LoadScene(menuSceneName);
        }

        private void UpdateScoreUI()
        {
            if (scoreText != null)
                scoreText.text = "Score: " + _score;
        }
    }
}