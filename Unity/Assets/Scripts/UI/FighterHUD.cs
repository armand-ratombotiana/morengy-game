using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Morengy.Character;

namespace Morengy.UI
{
    /// <summary>
    /// Heads-Up Display for fighter stats during combat.
    /// Shows health, stamina, special meter, and combo counter.
    /// </summary>
    public class FighterHUD : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private FighterStats fighterStats;
        [SerializeField] private bool isPlayer1 = true;

        [Header("Health Bar")]
        [SerializeField] private Image healthBarFill;
        [SerializeField] private Image healthBarDamage; // Delayed damage visualization
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private float damageBarDelay = 0.5f;

        [Header("Stamina Bar")]
        [SerializeField] private Image staminaBarFill;
        [SerializeField] private TextMeshProUGUI staminaText;
        [SerializeField] private Color normalStaminaColor = Color.cyan;
        [SerializeField] private Color lowStaminaColor = Color.yellow;
        [SerializeField] private Color exhaustedStaminaColor = Color.red;

        [Header("Special Meter")]
        [SerializeField] private Image specialMeterFill;
        [SerializeField] private TextMeshProUGUI specialMeterText;
        [SerializeField] private GameObject specialReadyIndicator;
        [SerializeField] private Color chargingColor = new Color(1f, 0.5f, 0f);
        [SerializeField] private Color readyColor = Color.yellow;

        [Header("Fighter Info")]
        [SerializeField] private TextMeshProUGUI fighterNameText;
        [SerializeField] private TextMeshProUGUI regionText;

        [Header("Combo Display")]
        [SerializeField] private TextMeshProUGUI comboCountText;
        [SerializeField] private GameObject comboPanel;
        [SerializeField] private float comboDisplayTime = 2f;

        [Header("Status Effects")]
        [SerializeField] private GameObject criticalHealthWarning;
        [SerializeField] private GameObject exhaustedWarning;

        // Internal state
        private float currentHealthDisplay;
        private float targetDamageBarHealth;
        private float damageBarTimer;
        private int lastComboCount;
        private float comboDisplayTimer;

        private void Awake()
        {
            if (fighterStats == null)
            {
                Debug.LogError($"FighterHUD on {gameObject.name} missing FighterStats reference!");
                enabled = false;
                return;
            }

            // Subscribe to stat events
            fighterStats.OnHealthChanged += UpdateHealth;
            fighterStats.OnStaminaChanged += UpdateStamina;
            fighterStats.OnSpecialMeterChanged += UpdateSpecialMeter;
            fighterStats.OnCriticalHealth += ShowCriticalHealthWarning;
            fighterStats.OnFighterExhausted += ShowExhaustedWarning;

            // Initialize displays
            InitializeUI();
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (fighterStats != null)
            {
                fighterStats.OnHealthChanged -= UpdateHealth;
                fighterStats.OnStaminaChanged -= UpdateStamina;
                fighterStats.OnSpecialMeterChanged -= UpdateSpecialMeter;
                fighterStats.OnCriticalHealth -= ShowCriticalHealthWarning;
                fighterStats.OnFighterExhausted -= ShowExhaustedWarning;
            }
        }

        private void Update()
        {
            UpdateDamageBar();
            UpdateComboDisplay();
        }

        /// <summary>
        /// Initialize UI with fighter information
        /// </summary>
        private void InitializeUI()
        {
            if (fighterNameText != null)
                fighterNameText.text = fighterStats.FighterName;

            if (regionText != null)
                regionText.text = fighterStats.Region;

            // Set initial values
            currentHealthDisplay = fighterStats.MaxHealth;
            targetDamageBarHealth = fighterStats.MaxHealth;

            UpdateHealth(fighterStats.CurrentHealth, fighterStats.MaxHealth);
            UpdateStamina(fighterStats.CurrentStamina, fighterStats.MaxStamina);
            UpdateSpecialMeter(fighterStats.SpecialMeter, fighterStats.SpecialMeterMax);

            // Hide warnings initially
            if (criticalHealthWarning != null)
                criticalHealthWarning.SetActive(false);

            if (exhaustedWarning != null)
                exhaustedWarning.SetActive(false);

            if (comboPanel != null)
                comboPanel.SetActive(false);
        }

        #region Health Display

        /// <summary>
        /// Update health bar display
        /// </summary>
        private void UpdateHealth(float current, float max)
        {
            float healthPercent = current / max;

            // Update main health bar
            if (healthBarFill != null)
            {
                healthBarFill.fillAmount = healthPercent;

                // Color shift based on health
                if (healthPercent < 0.15f)
                    healthBarFill.color = Color.red;
                else if (healthPercent < 0.5f)
                    healthBarFill.color = Color.yellow;
                else
                    healthBarFill.color = Color.green;
            }

            // Update health text
            if (healthText != null)
                healthText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";

            // Start damage bar animation
            if (current < currentHealthDisplay)
            {
                targetDamageBarHealth = current;
                damageBarTimer = damageBarDelay;
            }

            currentHealthDisplay = current;
        }

        /// <summary>
        /// Animate the delayed damage bar (red bar that follows health)
        /// </summary>
        private void UpdateDamageBar()
        {
            if (healthBarDamage == null) return;

            if (damageBarTimer > 0)
            {
                damageBarTimer -= Time.deltaTime;
            }
            else
            {
                // Smoothly decrease damage bar to match health
                float currentDamagePercent = healthBarDamage.fillAmount;
                float targetPercent = targetDamageBarHealth / fighterStats.MaxHealth;

                if (currentDamagePercent > targetPercent)
                {
                    float newPercent = Mathf.Lerp(currentDamagePercent, targetPercent, Time.deltaTime * 3f);
                    healthBarDamage.fillAmount = newPercent;
                }
            }
        }

        #endregion

        #region Stamina Display

        /// <summary>
        /// Update stamina bar display
        /// </summary>
        private void UpdateStamina(float current, float max)
        {
            float staminaPercent = current / max;

            // Update stamina bar
            if (staminaBarFill != null)
            {
                staminaBarFill.fillAmount = staminaPercent;

                // Color shift based on stamina level
                if (staminaPercent < 0.1f)
                    staminaBarFill.color = exhaustedStaminaColor;
                else if (staminaPercent < 0.3f)
                    staminaBarFill.color = lowStaminaColor;
                else
                    staminaBarFill.color = normalStaminaColor;
            }

            // Update stamina text
            if (staminaText != null)
                staminaText.text = $"{Mathf.CeilToInt(current)}";
        }

        #endregion

        #region Special Meter Display

        /// <summary>
        /// Update special meter display
        /// </summary>
        private void UpdateSpecialMeter(float current, float max)
        {
            float meterPercent = current / max;

            // Update special meter fill
            if (specialMeterFill != null)
            {
                specialMeterFill.fillAmount = meterPercent;

                // Color based on charge level
                specialMeterFill.color = meterPercent >= 1f ? readyColor : chargingColor;
            }

            // Update special meter text
            if (specialMeterText != null)
                specialMeterText.text = $"{Mathf.CeilToInt(meterPercent * 100)}%";

            // Show ready indicator
            if (specialReadyIndicator != null)
            {
                specialReadyIndicator.SetActive(meterPercent >= 1f);
            }
        }

        #endregion

        #region Combo Display

        /// <summary>
        /// Show combo counter
        /// </summary>
        public void ShowCombo(int comboCount)
        {
            if (comboCount <= 1) return;

            lastComboCount = comboCount;
            comboDisplayTimer = comboDisplayTime;

            if (comboPanel != null)
                comboPanel.SetActive(true);

            if (comboCountText != null)
            {
                comboCountText.text = $"{comboCount} HIT COMBO!";

                // Scale text based on combo count
                float scale = 1f + (comboCount * 0.1f);
                comboCountText.transform.localScale = Vector3.one * Mathf.Min(scale, 2f);
            }
        }

        /// <summary>
        /// Update combo display timer
        /// </summary>
        private void UpdateComboDisplay()
        {
            if (comboDisplayTimer > 0)
            {
                comboDisplayTimer -= Time.deltaTime;

                if (comboDisplayTimer <= 0 && comboPanel != null)
                {
                    comboPanel.SetActive(false);
                }
            }
        }

        #endregion

        #region Status Warnings

        /// <summary>
        /// Show critical health warning
        /// </summary>
        private void ShowCriticalHealthWarning()
        {
            if (criticalHealthWarning != null)
            {
                criticalHealthWarning.SetActive(true);
            }
        }

        /// <summary>
        /// Show exhausted warning
        /// </summary>
        private void ShowExhaustedWarning()
        {
            if (exhaustedWarning != null)
            {
                exhaustedWarning.SetActive(true);
                StartCoroutine(HideExhaustedWarningAfterDelay());
            }
        }

        private System.Collections.IEnumerator HideExhaustedWarningAfterDelay()
        {
            yield return new WaitForSeconds(2f);

            if (exhaustedWarning != null && !fighterStats.IsExhausted)
            {
                exhaustedWarning.SetActive(false);
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// Set fighter stats reference at runtime
        /// </summary>
        public void SetFighterStats(FighterStats stats)
        {
            fighterStats = stats;
            InitializeUI();
        }

        /// <summary>
        /// Flash health bar (for dramatic effect)
        /// </summary>
        public void FlashHealthBar()
        {
            if (healthBarFill != null)
            {
                StartCoroutine(FlashBarCoroutine(healthBarFill));
            }
        }

        private System.Collections.IEnumerator FlashBarCoroutine(Image bar)
        {
            Color originalColor = bar.color;

            for (int i = 0; i < 3; i++)
            {
                bar.color = Color.white;
                yield return new WaitForSeconds(0.1f);
                bar.color = originalColor;
                yield return new WaitForSeconds(0.1f);
            }
        }

        #endregion
    }
}
