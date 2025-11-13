using UnityEngine;
using System.Collections;
using Morengy.Character;

namespace Morengy.Combat
{
    /// <summary>
    /// Tracks combo chains and triggers combo-related events.
    /// Integrates with CombatSystem to provide combo feedback.
    /// </summary>
    public class ComboTracker : MonoBehaviour
    {
        [Header("Combo Settings")]
        [SerializeField] private float comboResetTime = 2f;
        [SerializeField] private int minComboForDisplay = 2;
        [SerializeField] private int[] comboMilestones = { 3, 5, 10 };

        [Header("References")]
        [SerializeField] private FighterStats fighterStats;
        [SerializeField] private CombatSystem combatSystem;

        // State
        private int currentCombo = 0;
        private float lastHitTime = 0f;
        private int totalHitsLanded = 0;
        private int highestCombo = 0;
        private float totalDamageInCombo = 0f;

        // Events
        public event System.Action<int> OnComboIncreased;
        public event System.Action<int, float> OnComboFinished; // combo count, total damage
        public event System.Action<int> OnComboMilestone;
        public event System.Action OnComboBreak;

        // Properties
        public int CurrentCombo => currentCombo;
        public int HighestCombo => highestCombo;
        public int TotalHitsLanded => totalHitsLanded;

        private void Awake()
        {
            if (fighterStats == null)
                fighterStats = GetComponent<FighterStats>();

            if (combatSystem == null)
                combatSystem = GetComponent<CombatSystem>();
        }

        private void Start()
        {
            // Subscribe to combat events
            if (fighterStats != null)
            {
                fighterStats.OnHealthChanged += OnOpponentDamaged;
            }
        }

        private void OnDestroy()
        {
            if (fighterStats != null)
            {
                fighterStats.OnHealthChanged -= OnOpponentDamaged;
            }
        }

        private void Update()
        {
            CheckComboTimeout();
        }

        /// <summary>
        /// Called when opponent takes damage (indicating we landed a hit)
        /// </summary>
        private void OnOpponentDamaged(float current, float max)
        {
            // This is called on the opponent's stats
            // We need to track when WE deal damage
            // This will be called from CombatSystem instead
        }

        /// <summary>
        /// Register a successful hit
        /// </summary>
        public void RegisterHit(float damage)
        {
            lastHitTime = Time.time;
            currentCombo++;
            totalHitsLanded++;
            totalDamageInCombo += damage;

            // Update highest combo
            if (currentCombo > highestCombo)
            {
                highestCombo = currentCombo;
            }

            // Trigger events
            OnComboIncreased?.Invoke(currentCombo);

            // Check for milestones
            CheckMilestones();

            // Update UI if combo is worth displaying
            if (currentCombo >= minComboForDisplay)
            {
                UpdateComboDisplay();
            }

            Debug.Log($"Combo: {currentCombo} hits! Total damage: {totalDamageInCombo:F1}");
        }

        /// <summary>
        /// Check if combo has timed out
        /// </summary>
        private void CheckComboTimeout()
        {
            if (currentCombo > 0 && Time.time - lastHitTime > comboResetTime)
            {
                FinishCombo();
            }
        }

        /// <summary>
        /// Check if combo has reached a milestone
        /// </summary>
        private void CheckMilestones()
        {
            foreach (int milestone in comboMilestones)
            {
                if (currentCombo == milestone)
                {
                    OnComboMilestone?.Invoke(milestone);
                    TriggerMilestoneEffect(milestone);
                    break;
                }
            }
        }

        /// <summary>
        /// Trigger special effect for combo milestone
        /// </summary>
        private void TriggerMilestoneEffect(int milestone)
        {
            // Play announcer message
            if (UI.RoundAnnouncer.Instance != null)
            {
                UI.RoundAnnouncer.Instance.AnnounceCombo(milestone);
            }

            // Play audio
            if (Managers.AudioManager.Instance != null)
            {
                Managers.AudioManager.Instance.PlayCrowdRoar();
            }

            Debug.Log($"COMBO MILESTONE: {milestone} HITS!");
        }

        /// <summary>
        /// Update combo display on UI
        /// </summary>
        private void UpdateComboDisplay()
        {
            // Find FighterHUD and update combo
            UI.FighterHUD hud = FindObjectOfType<UI.FighterHUD>();
            if (hud != null)
            {
                hud.ShowCombo(currentCombo);
            }
        }

        /// <summary>
        /// Finish current combo
        /// </summary>
        private void FinishCombo()
        {
            if (currentCombo > 0)
            {
                OnComboFinished?.Invoke(currentCombo, totalDamageInCombo);

                Debug.Log($"Combo finished: {currentCombo} hits, {totalDamageInCombo:F1} damage");

                // Reset
                currentCombo = 0;
                totalDamageInCombo = 0f;
            }
        }

        /// <summary>
        /// Break combo (when hit by opponent)
        /// </summary>
        public void BreakCombo()
        {
            if (currentCombo > 0)
            {
                OnComboBreak?.Invoke();
                Debug.Log($"Combo broken at {currentCombo} hits!");

                FinishCombo();
            }
        }

        /// <summary>
        /// Reset all combo stats
        /// </summary>
        public void ResetStats()
        {
            currentCombo = 0;
            totalDamageInCombo = 0f;
            totalHitsLanded = 0;
            highestCombo = 0;
            lastHitTime = 0f;
        }

        /// <summary>
        /// Get combo statistics
        /// </summary>
        public ComboStats GetStats()
        {
            return new ComboStats
            {
                CurrentCombo = currentCombo,
                HighestCombo = highestCombo,
                TotalHits = totalHitsLanded,
                CurrentDamage = totalDamageInCombo
            };
        }
    }

    /// <summary>
    /// Combo statistics structure
    /// </summary>
    [System.Serializable]
    public struct ComboStats
    {
        public int CurrentCombo;
        public int HighestCombo;
        public int TotalHits;
        public float CurrentDamage;
    }
}
