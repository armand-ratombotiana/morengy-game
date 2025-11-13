using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace Morengy.UI
{
    /// <summary>
    /// Displays round announcements, countdowns, and match messages.
    /// Provides cinematic UI feedback for match events.
    /// </summary>
    public class RoundAnnouncer : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject announcementPanel;
        [SerializeField] private TextMeshProUGUI announcementText;
        [SerializeField] private TextMeshProUGUI subText;
        [SerializeField] private Image backgroundOverlay;

        [Header("Animation Settings")]
        [SerializeField] private float fadeInDuration = 0.3f;
        [SerializeField] private float displayDuration = 2f;
        [SerializeField] private float fadeOutDuration = 0.3f;
        [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Text Colors")]
        [SerializeField] private Color countdownColor = Color.yellow;
        [SerializeField] private Color fightColor = Color.red;
        [SerializeField] private Color roundStartColor = Color.cyan;
        [SerializeField] private Color roundEndColor = Color.white;
        [SerializeField] private Color victoryColor = Color.green;
        [SerializeField] private Color defeatColor = Color.red;

        [Header("Audio")]
        [SerializeField] private bool playAudioOnAnnounce = true;

        // State
        private Coroutine currentAnnouncementCoroutine;
        private CanvasGroup canvasGroup;

        // Singleton
        public static RoundAnnouncer Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Get or add CanvasGroup
            canvasGroup = announcementPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = announcementPanel.AddComponent<CanvasGroup>();
            }

            // Hide initially
            HideImmediate();
        }

        private void Start()
        {
            // Subscribe to game manager events if exists
            if (Managers.GameManager.Instance != null)
            {
                Managers.GameManager.Instance.OnRoundStart += OnRoundStart;
                Managers.GameManager.Instance.OnRoundEnd += OnRoundEnd;
                Managers.GameManager.Instance.OnMatchEnd += OnMatchEnd;
                Managers.GameManager.Instance.OnCountdown += OnCountdown;
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (Managers.GameManager.Instance != null)
            {
                Managers.GameManager.Instance.OnRoundStart -= OnRoundStart;
                Managers.GameManager.Instance.OnRoundEnd -= OnRoundEnd;
                Managers.GameManager.Instance.OnMatchEnd -= OnMatchEnd;
                Managers.GameManager.Instance.OnCountdown -= OnCountdown;
            }
        }

        #region Event Handlers

        /// <summary>
        /// Handle round start event
        /// </summary>
        private void OnRoundStart(int roundNumber)
        {
            string message = $"ROUND {roundNumber}";
            string sub = "Get Ready!";
            AnnounceMessage(message, sub, roundStartColor, 1.5f);
        }

        /// <summary>
        /// Handle round end event
        /// </summary>
        private void OnRoundEnd(Character.FighterStats winner)
        {
            if (winner != null)
            {
                string message = "ROUND OVER";
                string sub = $"{winner.FighterName} Wins!";
                AnnounceMessage(message, sub, roundEndColor, 2f);
            }
        }

        /// <summary>
        /// Handle match end event
        /// </summary>
        private void OnMatchEnd(Character.FighterStats winner)
        {
            if (winner != null)
            {
                string message = "VICTORY!";
                string sub = $"{winner.FighterName} is the Champion!";
                AnnounceMessage(message, sub, victoryColor, 3f);

                // Play victory music
                if (Managers.AudioManager.Instance != null)
                {
                    Managers.AudioManager.Instance.PlayVictoryMusic();
                }
            }
        }

        /// <summary>
        /// Handle countdown event
        /// </summary>
        private void OnCountdown(string countdownText)
        {
            Color color = countdownText == "FIGHT!" ? fightColor : countdownColor;
            float scale = countdownText == "FIGHT!" ? 1.5f : 1.2f;

            AnnounceMessage(countdownText, "", color, 0.8f, scale);

            // Play appropriate sound
            if (playAudioOnAnnounce && Managers.AudioManager.Instance != null)
            {
                if (countdownText == "FIGHT!")
                {
                    Managers.AudioManager.Instance.PlayFightAnnounce();
                }
                else
                {
                    Managers.AudioManager.Instance.PlayCountdownBeep();
                }
            }
        }

        #endregion

        #region Public Announcement Methods

        /// <summary>
        /// Announce custom message
        /// </summary>
        public void AnnounceMessage(string message, string subtitle = "", Color? textColor = null, float duration = -1f, float textScale = 1f)
        {
            // Stop current announcement if any
            if (currentAnnouncementCoroutine != null)
            {
                StopCoroutine(currentAnnouncementCoroutine);
            }

            // Start new announcement
            currentAnnouncementCoroutine = StartCoroutine(
                AnnouncementCoroutine(
                    message,
                    subtitle,
                    textColor ?? Color.white,
                    duration < 0 ? displayDuration : duration,
                    textScale
                )
            );
        }

        /// <summary>
        /// Announce "KNOCKOUT!"
        /// </summary>
        public void AnnounceKnockout()
        {
            AnnounceMessage("KNOCKOUT!", "Brutal Finish!", Color.red, 2.5f, 1.5f);

            if (playAudioOnAnnounce && Managers.AudioManager.Instance != null)
            {
                Managers.AudioManager.Instance.PlayKOSound();
            }
        }

        /// <summary>
        /// Announce combo
        /// </summary>
        public void AnnounceCombo(int comboCount)
        {
            if (comboCount < 3) return;

            string message = "";
            if (comboCount >= 5)
                message = "ULTRA COMBO!";
            else if (comboCount >= 4)
                message = "SUPER COMBO!";
            else
                message = "COMBO!";

            AnnounceMessage(message, $"{comboCount} Hits!", Color.yellow, 1f, 0.8f);
        }

        /// <summary>
        /// Announce critical hit
        /// </summary>
        public void AnnounceCritical()
        {
            AnnounceMessage("CRITICAL HIT!", "", Color.red, 1f, 1.2f);
        }

        /// <summary>
        /// Announce perfect block
        /// </summary>
        public void AnnouncePerfectBlock()
        {
            AnnounceMessage("PERFECT!", "Block", Color.cyan, 1f, 0.9f);
        }

        #endregion

        #region Animation Coroutines

        /// <summary>
        /// Main announcement animation coroutine
        /// </summary>
        private IEnumerator AnnouncementCoroutine(string message, string subtitle, Color color, float duration, float textScale)
        {
            // Set text
            announcementText.text = message;
            announcementText.color = color;
            announcementText.transform.localScale = Vector3.zero;

            if (subText != null)
            {
                subText.text = subtitle;
                subText.color = color * 0.8f;
            }

            // Show panel
            announcementPanel.SetActive(true);

            // Fade in
            yield return StartCoroutine(FadeIn());

            // Scale up text
            yield return StartCoroutine(ScaleText(textScale));

            // Hold
            yield return new WaitForSeconds(duration);

            // Fade out
            yield return StartCoroutine(FadeOut());

            // Hide panel
            announcementPanel.SetActive(false);
        }

        /// <summary>
        /// Fade in animation
        /// </summary>
        private IEnumerator FadeIn()
        {
            float elapsed = 0f;

            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeInDuration;

                canvasGroup.alpha = t;

                if (backgroundOverlay != null)
                {
                    Color bgColor = backgroundOverlay.color;
                    bgColor.a = t * 0.5f; // Semi-transparent background
                    backgroundOverlay.color = bgColor;
                }

                yield return null;
            }

            canvasGroup.alpha = 1f;
        }

        /// <summary>
        /// Scale text animation
        /// </summary>
        private IEnumerator ScaleText(float targetScale)
        {
            float elapsed = 0f;
            float duration = 0.3f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = scaleCurve.Evaluate(elapsed / duration);

                float scale = Mathf.Lerp(0f, targetScale, t);
                announcementText.transform.localScale = Vector3.one * scale;

                yield return null;
            }

            announcementText.transform.localScale = Vector3.one * targetScale;
        }

        /// <summary>
        /// Fade out animation
        /// </summary>
        private IEnumerator FadeOut()
        {
            float elapsed = 0f;

            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                float t = 1f - (elapsed / fadeOutDuration);

                canvasGroup.alpha = t;

                if (backgroundOverlay != null)
                {
                    Color bgColor = backgroundOverlay.color;
                    bgColor.a = t * 0.5f;
                    backgroundOverlay.color = bgColor;
                }

                yield return null;
            }

            canvasGroup.alpha = 0f;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Hide announcement immediately
        /// </summary>
        private void HideImmediate()
        {
            announcementPanel.SetActive(false);
            canvasGroup.alpha = 0f;

            if (backgroundOverlay != null)
            {
                Color bgColor = backgroundOverlay.color;
                bgColor.a = 0f;
                backgroundOverlay.color = bgColor;
            }
        }

        /// <summary>
        /// Stop current announcement
        /// </summary>
        public void StopAnnouncement()
        {
            if (currentAnnouncementCoroutine != null)
            {
                StopCoroutine(currentAnnouncementCoroutine);
                currentAnnouncementCoroutine = null;
            }

            HideImmediate();
        }

        #endregion
    }
}
