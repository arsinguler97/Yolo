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

        var audio = FindFirstObjectByType<AudioManager>();
        if (audio != null && !audio.IsTimerTickingPlaying())
            audio.PlayTimerTick();
    }

    private void OnTimerFinished(GameTimerFinishedEvent evt)
    {
        if (fadePanel != null)
            fadePanel.ForceFullAlpha();

        if (playAgainButton != null)
            playAgainButton.SetActive(true);

        var audio = FindFirstObjectByType<AudioManager>();
        if (audio != null)
        {
            audio.StopTimerTick();
            audio.PlayGameOver();
        }

        if (gameManager != null)
        {
            var finalText = gameManager.GetComponentInChildren<TextMeshProUGUI>(true);
            if (finalText != null)
            {
                finalText.text = $"You died as a {gameManager.Score}-year-old {gameManager.CurrentStage.ToLower()} :(";
                finalText.gameObject.SetActive(true);
            }
        }

        if (gridInputController != null)
            gridInputController.DisableInput();
    }

    private void OnDestroy()
    {
        EventBus.Instance.Unsubscribe<GameTimerTickEvent>(this);
        EventBus.Instance.Unsubscribe<GameTimerFinishedEvent>(this);
    }
}
