using UnityEngine;

namespace MyAssets.MyScripts
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource bgSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private AudioSource timerSource;

        [Header("Clips")]
        [SerializeField] private AudioClip bgMusic;
        [SerializeField] private AudioClip timerTickClip;
        [SerializeField] private AudioClip gameOverClip;
        [SerializeField] private AudioClip[] moveClips;


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            if (bgSource != null && bgMusic != null)
            {
                bgSource.clip = bgMusic;
                bgSource.loop = true;
                bgSource.Play();
            }
        }

        public void PlayTimerTick()
        {
            if (timerSource != null && timerTickClip != null && !timerSource.isPlaying)
            {
                timerSource.clip = timerTickClip;
                timerSource.loop = true;
                timerSource.Play();
            }
        }

        public void StopTimerTick()
        {
            if (timerSource != null)
                timerSource.Stop();
        }

        public void PlayGameOver()
        {
            if (sfxSource != null && gameOverClip != null)
                sfxSource.PlayOneShot(gameOverClip);
        }
        
        public void PlayRandomMoveSound()
        {
            if (moveClips == null || moveClips.Length == 0) return;
            var clip = moveClips[Random.Range(0, moveClips.Length)];
            sfxSource.PlayOneShot(clip);
        }

        public bool IsTimerTickingPlaying()
        {
            return timerSource != null && timerSource.isPlaying;
        }
    }
}