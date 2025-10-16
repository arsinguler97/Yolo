using _Project.Code.Core.Events;
using _Project.Code.Gameplay.GameManagement;
using MyAssets.MyScripts;
using TMPro;
using UnityEngine;

public class GameTimeDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private GameObject playAgainButton;
    [SerializeField] private TimerFadePanel fadePanel;
    [SerializeField] private GridInputController gridInputController;
    [SerializeField] private MyGameManager gameManager;

    private GameTimerService _timerService;

    private void Start()
    {
        if (timeText == null)
            timeText = GetComponent<TMP_Text>();

        _timerService = FindFirstObjectByType<GameTimerService>();
        if (_timerService != null)
        {
            _timerService.ResetTimer();
            _timerService.StartTimer();
        }

        EventBus.Instance.Subscribe<GameTimerTickEvent>(this, OnTimerTick);
        EventBus.Instance.Subscribe<GameTimerFinishedEvent>(this, OnTimerFinished);

        if (playAgainButton != null)
            playAgainButton.SetActive(false);
    }

    private void OnTimerTick(GameTimerTickEvent evt)
    {
        if (timeText != null)
            timeText.text = evt.GetFormattedTime();
    }

    private void OnTimerFinished(GameTimerFinishedEvent evt)
    {
        if (fadePanel != null)
            fadePanel.ForceFullAlpha();

        if (playAgainButton != null)
            playAgainButton.SetActive(true);

        if (gameManager != null)
        {
            var finalText = gameManager.GetComponentInChildren<TMP_Text>(true);
            if (finalText != null)
            {
                finalText.text = $"You've made it to {gameManager.Score}!";
                finalText.gameObject.SetActive(true);
            }
        }

        gridInputController.DisableInput();
    }

    private void OnDestroy()
    {
        EventBus.Instance.Unsubscribe<GameTimerTickEvent>(this);
        EventBus.Instance.Unsubscribe<GameTimerFinishedEvent>(this);
    }
}
