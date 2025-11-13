using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Morengy.Character;
using Morengy.Combat;

namespace Morengy.AI
{
    /// <summary>
    /// AI behavior system for computer-controlled fighters.
    /// Implements difficulty levels and personality-based fighting styles.
    /// </summary>
    [RequireComponent(typeof(FighterController))]
    [RequireComponent(typeof(CombatSystem))]
    public class AIBehavior : MonoBehaviour
    {
        [Header("AI Configuration")]
        [SerializeField] private AIDifficulty difficulty = AIDifficulty.Medium;
        [SerializeField] private AIPersonality personality = AIPersonality.Balanced;
        [SerializeField] private Transform opponent;

        [Header("Decision Making")]
        [SerializeField] private float decisionInterval = 0.5f;
        [SerializeField] private float reactionTimeMin = 0.2f;
        [SerializeField] private float reactionTimeMax = 0.8f;

        [Header("Combat Preferences")]
        [Range(0, 100)]
        [SerializeField] private float aggression = 50f;
        [Range(0, 100)]
        [SerializeField] private float defensiveness = 50f;
        [Range(0, 100)]
        [SerializeField] private float comboFocus = 50f;
        [Range(0, 100)]
        [SerializeField] private float specialUsage = 50f;

        [Header("Spacing & Positioning")]
        [SerializeField] private float optimalRange = 2.5f;
        [SerializeField] private float retreatRange = 1.0f;
        [SerializeField] private float chargeRange = 4.0f;

        // Components
        private FighterController controller;
        private CombatSystem combatSystem;
        private FighterStats stats;
        private FighterStats opponentStats;

        // AI State
        private AIState currentState = AIState.Observing;
        private float nextDecisionTime;
        private float reactionTime;
        private bool isProcessingAction = false;

        // Decision tracking
        private Queue<string> recentActions = new Queue<string>();
        private int consecutiveAttacks = 0;
        private float lastDodgeTime = -10f;
        private float lastBlockTime = -10f;

        private void Awake()
        {
            controller = GetComponent<FighterController>();
            combatSystem = GetComponent<CombatSystem>();
            stats = GetComponent<FighterStats>();

            ConfigureDifficulty();
            ConfigurePersonality();
        }

        private void Start()
        {
            if (opponent != null)
            {
                controller.Opponent = opponent;
                opponentStats = opponent.GetComponent<FighterStats>();
            }

            nextDecisionTime = Time.time + decisionInterval;
        }

        private void Update()
        {
            if (opponent == null || !stats.IsAlive) return;

            // Make decisions at intervals
            if (Time.time >= nextDecisionTime && !isProcessingAction)
            {
                MakeDecision();
                nextDecisionTime = Time.time + decisionInterval;
            }

            // Update positioning
            UpdatePositioning();

            // Emergency reactions
            HandleEmergencyReactions();
        }

        #region Decision Making

        /// <summary>
        /// Main AI decision-making logic
        /// </summary>
        private void MakeDecision()
        {
            // Assess situation
            float distanceToOpponent = Vector3.Distance(transform.position, opponent.position);
            float healthPercentage = stats.HealthPercentage;
            float staminaPercentage = stats.StaminaPercentage;

            // Determine state
            currentState = DetermineState(distanceToOpponent, healthPercentage, staminaPercentage);

            // Execute action based on state and personality
            switch (currentState)
            {
                case AIState.Aggressive:
                    ExecuteAggressiveAction(distanceToOpponent);
                    break;

                case AIState.Defensive:
                    ExecuteDefensiveAction(distanceToOpponent);
                    break;

                case AIState.Observing:
                    ExecuteObservingAction(distanceToOpponent);
                    break;

                case AIState.Retreating:
                    ExecuteRetreatAction();
                    break;

                case AIState.Recovering:
                    ExecuteRecoveryAction();
                    break;
            }
        }

        /// <summary>
        /// Determine AI state based on situation
        /// </summary>
        private AIState DetermineState(float distance, float health, float stamina)
        {
            // Critical health - retreat and recover
            if (health < 20f || stamina < 15f)
            {
                return AIState.Retreating;
            }

            // Low stamina - recover
            if (stamina < 30f)
            {
                return AIState.Recovering;
            }

            // Opponent is vulnerable - be aggressive
            if (opponentStats != null && opponentStats.IsCriticalHealth)
            {
                return AIState.Aggressive;
            }

            // Within attack range and feeling aggressive
            if (distance < optimalRange && Random.value < aggression / 100f)
            {
                return AIState.Aggressive;
            }

            // Prefer defensive play
            if (Random.value < defensiveness / 100f)
            {
                return AIState.Defensive;
            }

            // Default to observing
            return AIState.Observing;
        }

        #endregion

        #region Action Execution

        /// <summary>
        /// Execute aggressive actions (attacking)
        /// </summary>
        private void ExecuteAggressiveAction(float distance)
        {
            if (combatSystem.IsAttacking) return;

            // Close distance if too far
            if (distance > optimalRange)
            {
                MoveToward(opponent.position);
                return;
            }

            // Decide attack type
            float randomValue = Random.value;

            // Try special if available
            if (stats.CanUseSpecial && randomValue < specialUsage / 100f)
            {
                StartCoroutine(DelayedAction(() => combatSystem.PerformSpecialAttack()));
                RecordAction("Special");
                return;
            }

            // Combo logic
            if (combatSystem.ComboCount > 0 && randomValue < comboFocus / 100f)
            {
                // Continue combo with light attacks
                StartCoroutine(DelayedAction(() => combatSystem.PerformLightAttack()));
                RecordAction("LightCombo");
            }
            else if (randomValue < 0.7f)
            {
                // Light attack (70% chance)
                StartCoroutine(DelayedAction(() => combatSystem.PerformLightAttack()));
                RecordAction("Light");
            }
            else
            {
                // Heavy attack (30% chance)
                StartCoroutine(DelayedAction(() => combatSystem.PerformHeavyAttack()));
                RecordAction("Heavy");
            }

            consecutiveAttacks++;
        }

        /// <summary>
        /// Execute defensive actions (blocking, dodging)
        /// </summary>
        private void ExecuteDefensiveAction(float distance)
        {
            // Maintain spacing
            if (distance < retreatRange)
            {
                // Too close, dodge back
                Vector2 dodgeDir = -(opponent.position - transform.position).normalized;
                controller.PerformDodge(new Vector2(dodgeDir.x, dodgeDir.z));
                lastDodgeTime = Time.time;
                RecordAction("Dodge");
            }
            else if (distance < optimalRange)
            {
                // Block incoming attacks
                if (!combatSystem.IsBlocking)
                {
                    combatSystem.StartBlocking();
                    lastBlockTime = Time.time;
                    StartCoroutine(HoldBlockCoroutine());
                    RecordAction("Block");
                }
            }
            else
            {
                // Circle around opponent
                CircleAroundOpponent();
            }

            consecutiveAttacks = 0;
        }

        /// <summary>
        /// Execute observing actions (positioning, feints)
        /// </summary>
        private void ExecuteObservingAction(float distance)
        {
            if (distance > chargeRange)
            {
                // Too far, move closer
                MoveToward(opponent.position);
            }
            else if (distance < retreatRange)
            {
                // Too close, back up
                MoveAway(opponent.position);
            }
            else
            {
                // Optimal range, circle and look for opening
                CircleAroundOpponent();

                // Occasional quick attack to test defenses
                if (Random.value < 0.2f)
                {
                    StartCoroutine(DelayedAction(() => combatSystem.PerformLightAttack()));
                    RecordAction("Probe");
                }
            }

            consecutiveAttacks = 0;
        }

        /// <summary>
        /// Execute retreat actions
        /// </summary>
        private void ExecuteRetreatAction()
        {
            // Move away from opponent
            MoveAway(opponent.position);

            // Dodge if opponent is close
            if (Vector3.Distance(transform.position, opponent.position) < retreatRange)
            {
                if (Time.time - lastDodgeTime > 2f)
                {
                    Vector2 dodgeDir = -(opponent.position - transform.position).normalized;
                    controller.PerformDodge(new Vector2(dodgeDir.x, dodgeDir.z));
                    lastDodgeTime = Time.time;
                }
            }

            RecordAction("Retreat");
            consecutiveAttacks = 0;
        }

        /// <summary>
        /// Execute recovery actions (stamina regen)
        /// </summary>
        private void ExecuteRecoveryAction()
        {
            // Maintain safe distance
            float distance = Vector3.Distance(transform.position, opponent.position);

            if (distance < optimalRange)
            {
                MoveAway(opponent.position);
            }

            // Fast regen stamina
            stats.FastRegenerateStamina();

            // Block if opponent attacks
            if (distance < optimalRange && Random.value < 0.6f)
            {
                if (!combatSystem.IsBlocking)
                {
                    combatSystem.StartBlocking();
                    StartCoroutine(HoldBlockCoroutine());
                }
            }

            RecordAction("Recover");
            consecutiveAttacks = 0;
        }

        #endregion

        #region Movement & Positioning

        /// <summary>
        /// Move toward target position
        /// </summary>
        private void MoveToward(Vector3 target)
        {
            Vector3 direction = (target - transform.position).normalized;
            controller.SetMoveInput(new Vector2(direction.x, direction.z));
        }

        /// <summary>
        /// Move away from target position
        /// </summary>
        private void MoveAway(Vector3 target)
        {
            Vector3 direction = (transform.position - target).normalized;
            controller.SetMoveInput(new Vector2(direction.x, direction.z));
        }

        /// <summary>
        /// Circle around opponent (strafing)
        /// </summary>
        private void CircleAroundOpponent()
        {
            Vector3 toOpponent = opponent.position - transform.position;
            Vector3 right = Vector3.Cross(Vector3.up, toOpponent).normalized;

            // Randomly strafe left or right
            Vector3 strafeDirection = Random.value > 0.5f ? right : -right;
            controller.SetMoveInput(new Vector2(strafeDirection.x, strafeDirection.z));
        }

        /// <summary>
        /// Update AI positioning continuously
        /// </summary>
        private void UpdatePositioning()
        {
            // Clear movement if processing an action
            if (isProcessingAction)
            {
                controller.SetMoveInput(Vector2.zero);
            }
        }

        #endregion

        #region Emergency Reactions

        /// <summary>
        /// Handle emergency reactions (dodge incoming attacks, etc.)
        /// </summary>
        private void HandleEmergencyReactions()
        {
            // If opponent is attacking and we're in range, react based on difficulty
            if (opponentStats != null)
            {
                CombatSystem opponentCombat = opponent.GetComponent<CombatSystem>();
                if (opponentCombat != null && opponentCombat.IsAttacking)
                {
                    float distance = Vector3.Distance(transform.position, opponent.position);

                    if (distance < optimalRange && !combatSystem.IsBlocking)
                    {
                        // React based on difficulty
                        float reactionChance = GetReactionChance();

                        if (Random.value < reactionChance)
                        {
                            // Start blocking
                            combatSystem.StartBlocking();
                            StartCoroutine(HoldBlockCoroutine());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get reaction chance based on difficulty
        /// </summary>
        private float GetReactionChance()
        {
            switch (difficulty)
            {
                case AIDifficulty.Easy:
                    return 0.2f;
                case AIDifficulty.Medium:
                    return 0.5f;
                case AIDifficulty.Hard:
                    return 0.8f;
                case AIDifficulty.Expert:
                    return 0.95f;
                default:
                    return 0.5f;
            }
        }

        #endregion

        #region Utility Coroutines

        /// <summary>
        /// Delayed action with reaction time
        /// </summary>
        private IEnumerator DelayedAction(System.Action action)
        {
            isProcessingAction = true;
            yield return new WaitForSeconds(reactionTime);
            action?.Invoke();
            isProcessingAction = false;
        }

        /// <summary>
        /// Hold block for random duration
        /// </summary>
        private IEnumerator HoldBlockCoroutine()
        {
            float blockDuration = Random.Range(0.5f, 2f);
            yield return new WaitForSeconds(blockDuration);

            if (combatSystem.IsBlocking)
            {
                combatSystem.StopBlocking();
            }
        }

        #endregion

        #region Configuration

        /// <summary>
        /// Configure AI based on difficulty
        /// </summary>
        private void ConfigureDifficulty()
        {
            switch (difficulty)
            {
                case AIDifficulty.Easy:
                    reactionTime = Random.Range(0.6f, 1.0f);
                    decisionInterval = 0.8f;
                    aggression = 30f;
                    defensiveness = 30f;
                    comboFocus = 20f;
                    specialUsage = 10f;
                    break;

                case AIDifficulty.Medium:
                    reactionTime = Random.Range(0.3f, 0.6f);
                    decisionInterval = 0.5f;
                    aggression = 50f;
                    defensiveness = 50f;
                    comboFocus = 50f;
                    specialUsage = 30f;
                    break;

                case AIDifficulty.Hard:
                    reactionTime = Random.Range(0.15f, 0.4f);
                    decisionInterval = 0.3f;
                    aggression = 70f;
                    defensiveness = 70f;
                    comboFocus = 70f;
                    specialUsage = 60f;
                    break;

                case AIDifficulty.Expert:
                    reactionTime = Random.Range(0.1f, 0.2f);
                    decisionInterval = 0.2f;
                    aggression = 85f;
                    defensiveness = 85f;
                    comboFocus = 90f;
                    specialUsage = 80f;
                    break;
            }
        }

        /// <summary>
        /// Configure AI based on personality
        /// </summary>
        private void ConfigurePersonality()
        {
            switch (personality)
            {
                case AIPersonality.Brawler:
                    aggression += 30f;
                    defensiveness -= 20f;
                    comboFocus += 20f;
                    optimalRange = 1.8f;
                    break;

                case AIPersonality.Tactician:
                    aggression -= 20f;
                    defensiveness += 30f;
                    comboFocus += 10f;
                    specialUsage += 20f;
                    optimalRange = 3.0f;
                    break;

                case AIPersonality.Showman:
                    specialUsage += 40f;
                    aggression += 10f;
                    comboFocus += 30f;
                    break;

                case AIPersonality.Technical:
                    comboFocus += 40f;
                    specialUsage += 10f;
                    defensiveness += 10f;
                    break;

                case AIPersonality.Balanced:
                    // No modifications - use default values
                    break;
            }

            // Clamp values
            aggression = Mathf.Clamp(aggression, 0, 100);
            defensiveness = Mathf.Clamp(defensiveness, 0, 100);
            comboFocus = Mathf.Clamp(comboFocus, 0, 100);
            specialUsage = Mathf.Clamp(specialUsage, 0, 100);
        }

        /// <summary>
        /// Record action for pattern tracking
        /// </summary>
        private void RecordAction(string action)
        {
            recentActions.Enqueue(action);
            if (recentActions.Count > 10)
            {
                recentActions.Dequeue();
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// Set AI difficulty at runtime
        /// </summary>
        public void SetDifficulty(AIDifficulty newDifficulty)
        {
            difficulty = newDifficulty;
            ConfigureDifficulty();
        }

        /// <summary>
        /// Set AI personality at runtime
        /// </summary>
        public void SetPersonality(AIPersonality newPersonality)
        {
            personality = newPersonality;
            ConfigurePersonality();
        }

        /// <summary>
        /// Set opponent target
        /// </summary>
        public void SetOpponent(Transform newOpponent)
        {
            opponent = newOpponent;
            controller.Opponent = newOpponent;
            opponentStats = newOpponent.GetComponent<FighterStats>();
        }

        #endregion
    }

    /// <summary>
    /// AI difficulty levels
    /// </summary>
    public enum AIDifficulty
    {
        Easy,
        Medium,
        Hard,
        Expert
    }

    /// <summary>
    /// AI personality types
    /// </summary>
    public enum AIPersonality
    {
        Balanced,
        Brawler,
        Tactician,
        Showman,
        Technical
    }

    /// <summary>
    /// AI state machine states
    /// </summary>
    public enum AIState
    {
        Observing,
        Aggressive,
        Defensive,
        Retreating,
        Recovering
    }
}
