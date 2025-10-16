using UnityEngine;
using UnityEngine.UI;
using _Project.Code.Core.Events;
using _Project.Code.Gameplay.GameManagement;

namespace MyAssets.MyScripts
{
    public class TimerFadePanel : MonoBehaviour
    {
        [SerializeField] private Image fadePanel;
        [SerializeField] private float defaultMaxTime = 60f;
        private float _maxTime;

        private void Start()
        {
            _maxTime = defaultMaxTime;
            ResetAlpha();
        }

        private void OnEnable()
        {
            EventBus.Instance.Subscribe<GameTimerTickEvent>(this, OnTimerTick);
            EventBus.Instance.Subscribe<GameTimerFinishedEvent>(this, OnTimerFinished);
        }

        private void OnDisable()
        {
            EventBus.Instance.Unsubscribe<GameTimerTickEvent>(this);
            EventBus.Instance.Unsubscribe<GameTimerFinishedEvent>(this);
        }

        private void OnTimerTick(GameTimerTickEvent evt)
        {
            if (fadePanel == null || _maxTime <= 0f) return;

            float normalized = Mathf.Clamp01(1f - (evt.TimeRemaining / _maxTime));
            SetAlpha(normalized);
        }

        private void OnTimerFinished(GameTimerFinishedEvent evt)
        {
            ForceFullAlpha();
        }

        public void ForceFullAlpha()
        {
            SetAlpha(1f);
        }

        private void ResetAlpha()
        {
            SetAlpha(0f);
        }

        private void SetAlpha(float value)
        {
            if (fadePanel == null) return;
            Color c = fadePanel.color;
            c.a = Mathf.Clamp01(value);
            fadePanel.color = c;
        }
    }
}