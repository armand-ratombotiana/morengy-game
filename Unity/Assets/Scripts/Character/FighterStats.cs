using UnityEngine;
using System;

namespace Morengy.Character
{
    /// <summary>
    /// Manages fighter statistics, health, stamina, and progression.
    /// Represents core attributes that define a fighter's abilities.
    /// </summary>
    public class FighterStats : MonoBehaviour
    {
        [Header("Fighter Identity")]
        [SerializeField] private string fighterName = "Unknown Fighter";
        [SerializeField] private string fightingStyle = "Traditional Morengy";
        [SerializeField] private string region = "Diego Suarez";
        [SerializeField] private int level = 1;

        [Header("Core Stats (1-100)")]
        [SerializeField] [Range(1, 100)] private float power = 50f;
        [SerializeField] [Range(1, 100)] private float speed = 50f;
        [SerializeField] [Range(1, 100)] private float defense = 50f;
        [SerializeField] [Range(1, 100)] private float stamina = 50f;
        [SerializeField] [Range(1, 100)] private float technique = 50f;
        [SerializeField] [Range(1, 100)] private float charisma = 50f;

        [Header("Health System")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth = 100f;
        [SerializeField] private float criticalHealthThreshold = 15f;

        [Header("Stamina System")]
        [SerializeField] private float maxStaminaBase = 100f;
        [SerializeField] private float currentStamina = 100f;
        [SerializeField] private float staminaRegenRate = 2f; // % per second
        [SerializeField] private float staminaFastRegenRate = 5f; // % per second when resting
        [SerializeField] private float exhaustedThreshold = 10f;

        [Header("Stamina Costs")]
        [SerializeField] private float lightAttackStaminaCost = 5f;
        [SerializeField] private float heavyAttackStaminaCost = 15f;
        [SerializeField] private float dodgeStaminaCost = 8f;
        [SerializeField] private float blockStaminaDrain = 2f; // per second
        [SerializeField] private float sprintStaminaDrain = 3f; // per second

        [Header("Special Meter")]
        [SerializeField] private float specialMeter = 0f;
        [SerializeField] private float specialMeterMax = 100f;
        [SerializeField] private float specialGainOnHit = 5f;
        [SerializeField] private float specialGainOnReceive = 3f;
        [SerializeField] private float specialGainFromCrowd = 2f;

        [Header("Damage Modifiers")]
        [SerializeField] private float criticalHitChance = 5f; // base %
        [SerializeField] private float criticalHitMultiplier = 1.75f;
        [SerializeField] private float comebackCritBonus = 15f; // added when health < 15%

        [Header("Combat Multipliers")]
        [SerializeField] private float damageMultiplier = 1.0f;
        [SerializeField] private float defenseMultiplier = 1.0f;
        [SerializeField] private float speedMultiplier = 1.0f;

        // Events
        public event Action<float, float> OnHealthChanged; // current, max
        public event Action<float, float> OnStaminaChanged; // current, max
        public event Action<float, float> OnSpecialMeterChanged; // current, max
        public event Action OnFighterKnockedOut;
        public event Action OnFighterExhausted;
        public event Action OnCriticalHealth;

        // Properties
        public string FighterName => fighterName;
        public string FightingStyle => fightingStyle;
        public string Region => region;
        public int Level => level;

        public float Power => power;
        public float Speed => speed;
        public float Defense => defense;
        public float Stamina => stamina;
        public float Technique => technique;
        public float Charisma => charisma;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public float HealthPercentage => (currentHealth / maxHealth) * 100f;

        public float CurrentStamina => currentStamina;
        public float MaxStamina => maxStaminaBase * (1 + stamina / 100f);
        public float StaminaPercentage => (currentStamina / MaxStamina) * 100f;

        public float SpecialMeter => specialMeter;
        public float SpecialMeterMax => specialMeterMax;
        public bool CanUseSpecial => specialMeter >= specialMeterMax;

        public bool IsAlive => currentHealth > 0;
        public bool IsCriticalHealth => currentHealth <= criticalHealthThreshold;
        public bool IsExhausted => currentStamina <= exhaustedThreshold;

        // Stamina costs (public for other systems)
        public float LightAttackStaminaCost => lightAttackStaminaCost;
        public float HeavyAttackStaminaCost => heavyAttackStaminaCost;
        public float DodgeStaminaCost => dodgeStaminaCost;
        public float BlockStaminaDrain => blockStaminaDrain;
        public float SprintStaminaDrain => sprintStaminaDrain;

        // Multipliers
        public float DamageMultiplier => damageMultiplier * (1 + power / 200f);
        public float DefenseMultiplier => defenseMultiplier * (1 + defense / 200f);
        public float SpeedMultiplier => speedMultiplier * (1 + speed / 200f);
        public float TechniqueBonus => technique / 100f;

        private void Awake()
        {
            // Initialize health and stamina to max
            currentHealth = maxHealth;
            currentStamina = MaxStamina;
            specialMeter = 0f;
        }

        private void Update()
        {
            // Passive stamina regeneration
            RegenerateStamina();
        }

        #region Health Management

        /// <summary>
        /// Apply damage to the fighter
        /// </summary>
        public void TakeDamage(float damage, bool isCritical = false)
        {
            if (!IsAlive) return;

            // Apply defense reduction
            float reducedDamage = damage * (1f - DefenseMultiplier * 0.3f);

            // Apply critical multiplier
            if (isCritical)
            {
                reducedDamage *= criticalHitMultiplier;
            }

            // Apply damage
            currentHealth = Mathf.Max(0, currentHealth - reducedDamage);

            // Trigger events
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            // Check for critical health
            if (IsCriticalHealth && IsAlive)
            {
                OnCriticalHealth?.Invoke();
            }

            // Check for knockout
            if (!IsAlive)
            {
                OnFighterKnockedOut?.Invoke();
            }

            // Gain special meter when taking damage
            GainSpecialMeter(specialGainOnReceive);
        }

        /// <summary>
        /// Heal the fighter (used for round resets or power-ups)
        /// </summary>
        public void Heal(float amount)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        /// <summary>
        /// Reset health to max (for new rounds)
        /// </summary>
        public void ResetHealth()
        {
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        #endregion

        #region Stamina Management

        /// <summary>
        /// Drain stamina for actions
        /// </summary>
        public bool DrainStamina(float amount)
        {
            if (currentStamina < amount)
            {
                // Not enough stamina
                return false;
            }

            currentStamina = Mathf.Max(0, currentStamina - amount);
            OnStaminaChanged?.Invoke(currentStamina, MaxStamina);

            if (IsExhausted)
            {
                OnFighterExhausted?.Invoke();
            }

            return true;
        }

        /// <summary>
        /// Passive stamina regeneration
        /// </summary>
        private void RegenerateStamina()
        {
            if (currentStamina < MaxStamina)
            {
                float regenAmount = staminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Min(MaxStamina, currentStamina + regenAmount);
                OnStaminaChanged?.Invoke(currentStamina, MaxStamina);
            }
        }

        /// <summary>
        /// Fast stamina regen when actively resting (holding rest button)
        /// </summary>
        public void FastRegenerateStamina()
        {
            if (currentStamina < MaxStamina)
            {
                float regenAmount = staminaFastRegenRate * Time.deltaTime;
                currentStamina = Mathf.Min(MaxStamina, currentStamina + regenAmount);
                OnStaminaChanged?.Invoke(currentStamina, MaxStamina);
            }
        }

        /// <summary>
        /// Reset stamina (for round resets)
        /// </summary>
        public void ResetStamina(float percentage = 1.0f)
        {
            currentStamina = MaxStamina * percentage;
            OnStaminaChanged?.Invoke(currentStamina, MaxStamina);
        }

        /// <summary>
        /// Get stamina modifier for damage calculations
        /// </summary>
        public float GetStaminaDamageModifier()
        {
            float staminaPercent = StaminaPercentage;

            if (staminaPercent > 50f)
                return 1.0f;
            else if (staminaPercent > 20f)
                return 0.8f;
            else
                return 0.6f;
        }

        #endregion

        #region Special Meter Management

        /// <summary>
        /// Gain special meter
        /// </summary>
        public void GainSpecialMeter(float amount)
        {
            specialMeter = Mathf.Min(specialMeterMax, specialMeter + amount);
            OnSpecialMeterChanged?.Invoke(specialMeter, specialMeterMax);
        }

        /// <summary>
        /// Use special meter for special moves
        /// </summary>
        public bool UseSpecialMeter(float amount)
        {
            if (specialMeter < amount)
            {
                return false;
            }

            specialMeter = Mathf.Max(0, specialMeter - amount);
            OnSpecialMeterChanged?.Invoke(specialMeter, specialMeterMax);
            return true;
        }

        /// <summary>
        /// Reset special meter (for new rounds)
        /// </summary>
        public void ResetSpecialMeter()
        {
            specialMeter = 0f;
            OnSpecialMeterChanged?.Invoke(specialMeter, specialMeterMax);
        }

        /// <summary>
        /// Called when fighter lands an attack
        /// </summary>
        public void OnAttackLanded()
        {
            GainSpecialMeter(specialGainOnHit);
        }

        #endregion

        #region Critical Hit System

        /// <summary>
        /// Calculate if attack is a critical hit
        /// </summary>
        public bool RollForCritical()
        {
            float totalCritChance = criticalHitChance;

            // Comeback mechanic: increase crit chance at low health
            if (IsCriticalHealth)
            {
                totalCritChance += comebackCritBonus;
            }

            // Technique stat bonus
            totalCritChance += technique * 0.1f;

            return UnityEngine.Random.Range(0f, 100f) < totalCritChance;
        }

        #endregion

        #region Stats Progression

        /// <summary>
        /// Increase fighter level and stats
        /// </summary>
        public void LevelUp()
        {
            level++;

            // Increase random stats
            power += UnityEngine.Random.Range(1f, 3f);
            speed += UnityEngine.Random.Range(1f, 3f);
            defense += UnityEngine.Random.Range(1f, 3f);
            stamina += UnityEngine.Random.Range(1f, 3f);
            technique += UnityEngine.Random.Range(1f, 3f);
            charisma += UnityEngine.Random.Range(1f, 3f);

            // Cap at 100
            power = Mathf.Min(100, power);
            speed = Mathf.Min(100, speed);
            defense = Mathf.Min(100, defense);
            stamina = Mathf.Min(100, stamina);
            technique = Mathf.Min(100, technique);
            charisma = Mathf.Min(100, charisma);

            // Increase health
            maxHealth += 5f;
            currentHealth = maxHealth;
        }

        /// <summary>
        /// Manually set stat values (for custom fighters)
        /// </summary>
        public void SetStats(float pow, float spd, float def, float stam, float tech, float char)
        {
            power = Mathf.Clamp(pow, 1, 100);
            speed = Mathf.Clamp(spd, 1, 100);
            defense = Mathf.Clamp(def, 1, 100);
            stamina = Mathf.Clamp(stam, 1, 100);
            technique = Mathf.Clamp(tech, 1, 100);
            charisma = Mathf.Clamp(char, 1, 100);
        }

        #endregion

        #region Round Management

        /// <summary>
        /// Reset fighter for new round
        /// </summary>
        public void PrepareForNewRound()
        {
            // Health carries over (no healing between rounds)
            // Stamina partially restores
            ResetStamina(0.6f);
            // Special meter carries over
        }

        /// <summary>
        /// Full reset for new match
        /// </summary>
        public void ResetForNewMatch()
        {
            ResetHealth();
            ResetStamina();
            ResetSpecialMeter();
        }

        #endregion

        #region Debug & Editor

#if UNITY_EDITOR
        [ContextMenu("Reset All Stats")]
        private void ResetAllStats()
        {
            ResetForNewMatch();
            Debug.Log($"{fighterName} stats reset.");
        }

        [ContextMenu("Level Up")]
        private void DebugLevelUp()
        {
            LevelUp();
            Debug.Log($"{fighterName} leveled up to {level}!");
        }

        [ContextMenu("Take 25 Damage")]
        private void DebugTakeDamage()
        {
            TakeDamage(25f);
        }

        [ContextMenu("Fill Special Meter")]
        private void DebugFillSpecial()
        {
            specialMeter = specialMeterMax;
            OnSpecialMeterChanged?.Invoke(specialMeter, specialMeterMax);
        }
#endif

        #endregion
    }
}
