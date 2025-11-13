using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Morengy.AI
{
    /// <summary>
    /// Machine learning system that makes AI opponents adapt to player behavior.
    /// Tracks player patterns and adjusts AI strategy accordingly.
    /// </summary>
    public class AILearningSystem : MonoBehaviour
    {
        [Header("Learning Settings")]
        [SerializeField] private bool enableLearning = true;
        [SerializeField] private int maxPatternMemory = 50;
        [SerializeField] private float learningRate = 0.1f;
        [SerializeField] private float adaptationSpeed = 1.0f;

        [Header("Pattern Detection")]
        [SerializeField] private int patternLength = 3;
        [SerializeField] private float patternConfidenceThreshold = 0.6f;

        // Learning data
        private Queue<PlayerAction> recentActions = new Queue<PlayerAction>();
        private Dictionary<string, int> patternFrequency = new Dictionary<string, int>();
        private Dictionary<AttackPattern, CounterStrategy> learnedCounters = new Dictionary<AttackPattern, CounterStrategy>();

        // Player tendencies
        private PlayerTendencies tendencies = new PlayerTendencies();

        // Components
        private AIBehavior aiBehavior;

        // Singleton for easy access
        public static AILearningSystem Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            aiBehavior = GetComponent<AIBehavior>();
        }

        private void Start()
        {
            InitializeLearning();
        }

        /// <summary>
        /// Initialize learning system
        /// </summary>
        private void InitializeLearning()
        {
            tendencies = new PlayerTendencies();
            patternFrequency.Clear();
            learnedCounters.Clear();
        }

        #region Action Recording

        /// <summary>
        /// Record player action for pattern analysis
        /// </summary>
        public void RecordPlayerAction(ActionType action, bool wasSuccessful)
        {
            if (!enableLearning) return;

            PlayerAction playerAction = new PlayerAction
            {
                action = action,
                timestamp = Time.time,
                wasSuccessful = wasSuccessful
            };

            recentActions.Enqueue(playerAction);

            // Keep memory limited
            if (recentActions.Count > maxPatternMemory)
            {
                recentActions.Dequeue();
            }

            // Update tendencies
            UpdateTendencies(action, wasSuccessful);

            // Analyze patterns
            if (recentActions.Count >= patternLength)
            {
                AnalyzePatterns();
            }
        }

        /// <summary>
        /// Update player tendency statistics
        /// </summary>
        private void UpdateTendencies(ActionType action, bool wasSuccessful)
        {
            tendencies.totalActions++;

            switch (action)
            {
                case ActionType.LightAttack:
                    tendencies.lightAttackCount++;
                    if (wasSuccessful) tendencies.successfulLightAttacks++;
                    break;
                case ActionType.HeavyAttack:
                    tendencies.heavyAttackCount++;
                    if (wasSuccessful) tendencies.successfulHeavyAttacks++;
                    break;
                case ActionType.SpecialAttack:
                    tendencies.specialAttackCount++;
                    if (wasSuccessful) tendencies.successfulSpecials++;
                    break;
                case ActionType.Block:
                    tendencies.blockCount++;
                    break;
                case ActionType.Dodge:
                    tendencies.dodgeCount++;
                    break;
                case ActionType.Aggressive:
                    tendencies.aggressiveActions++;
                    break;
                case ActionType.Defensive:
                    tendencies.defensiveActions++;
                    break;
            }
        }

        #endregion

        #region Pattern Analysis

        /// <summary>
        /// Analyze recent actions for patterns
        /// </summary>
        private void AnalyzePatterns()
        {
            // Get last N actions
            var lastActions = recentActions.TakeLast(patternLength).ToList();
            if (lastActions.Count < patternLength) return;

            // Create pattern string
            string pattern = string.Join("-", lastActions.Select(a => a.action.ToString()));

            // Track frequency
            if (!patternFrequency.ContainsKey(pattern))
            {
                patternFrequency[pattern] = 0;
            }
            patternFrequency[pattern]++;

            // Learn counter if pattern is frequent
            float confidence = (float)patternFrequency[pattern] / recentActions.Count;
            if (confidence >= patternConfidenceThreshold)
            {
                LearnCounter(lastActions);
            }
        }

        /// <summary>
        /// Learn counter-strategy for detected pattern
        /// </summary>
        private void LearnCounter(List<PlayerAction> pattern)
        {
            AttackPattern attackPattern = IdentifyPattern(pattern);
            CounterStrategy counter = DetermineCounter(attackPattern);

            learnedCounters[attackPattern] = counter;

            Debug.Log($"AI learned counter: {attackPattern} → {counter}");
        }

        /// <summary>
        /// Identify pattern type from actions
        /// </summary>
        private AttackPattern IdentifyPattern(List<PlayerAction> actions)
        {
            bool hasLight = actions.Any(a => a.action == ActionType.LightAttack);
            bool hasHeavy = actions.Any(a => a.action == ActionType.HeavyAttack);
            bool hasSpecial = actions.Any(a => a.action == ActionType.SpecialAttack);
            bool hasDodge = actions.Any(a => a.action == ActionType.Dodge);

            if (actions.All(a => a.action == ActionType.LightAttack))
                return AttackPattern.LightCombo;
            else if (hasLight && hasHeavy)
                return AttackPattern.MixedCombo;
            else if (hasHeavy && hasSpecial)
                return AttackPattern.PowerCombo;
            else if (hasDodge)
                return AttackPattern.EvasiveStyle;
            else if (actions.Count(a => a.action == ActionType.Block) > 1)
                return AttackPattern.DefensiveStyle;
            else
                return AttackPattern.Aggressive;
        }

        /// <summary>
        /// Determine best counter for pattern
        /// </summary>
        private CounterStrategy DetermineCounter(AttackPattern pattern)
        {
            switch (pattern)
            {
                case AttackPattern.LightCombo:
                    return CounterStrategy.PerfectBlock; // Block and counter
                case AttackPattern.MixedCombo:
                    return CounterStrategy.Dodge; // Evade and punish
                case AttackPattern.PowerCombo:
                    return CounterStrategy.Interrupt; // Attack first
                case AttackPattern.EvasiveStyle:
                    return CounterStrategy.Predict; // Anticipate movement
                case AttackPattern.DefensiveStyle:
                    return CounterStrategy.Grab; // Break guard
                case AttackPattern.Aggressive:
                    return CounterStrategy.Counter; // Defensive counter
                default:
                    return CounterStrategy.Adapt;
            }
        }

        #endregion

        #region AI Adaptation

        /// <summary>
        /// Get recommended action based on learning
        /// </summary>
        public ActionType GetAdaptedAction(float distanceToPlayer)
        {
            // Analyze player tendencies
            float aggressionRatio = tendencies.GetAggressionRatio();
            float lightAttackPreference = tendencies.GetLightAttackPreference();
            float blockingFrequency = tendencies.GetBlockingFrequency();

            // Adapt strategy
            if (aggressionRatio > 0.7f)
            {
                // Player is aggressive - be defensive
                return blockingFrequency < 0.3f ? ActionType.Block : ActionType.Dodge;
            }
            else if (lightAttackPreference > 0.6f)
            {
                // Player favors light attacks - prepare perfect block
                return ActionType.Block;
            }
            else if (distanceToPlayer < 2f && tendencies.dodgeCount > tendencies.blockCount)
            {
                // Player dodges more than blocks - use grabs
                return ActionType.Aggressive;
            }
            else
            {
                // Default adaptive behavior
                return ActionType.Aggressive;
            }
        }

        /// <summary>
        /// Get counter for current situation
        /// </summary>
        public CounterStrategy GetRecommendedCounter()
        {
            // Get most recent pattern
            var lastActions = recentActions.TakeLast(patternLength).ToList();
            if (lastActions.Count < patternLength)
                return CounterStrategy.Adapt;

            AttackPattern currentPattern = IdentifyPattern(lastActions);

            // Return learned counter if available
            if (learnedCounters.ContainsKey(currentPattern))
            {
                return learnedCounters[currentPattern];
            }

            return CounterStrategy.Adapt;
        }

        /// <summary>
        /// Adjust AI difficulty based on learning
        /// </summary>
        public void AdaptDifficulty(float playerWinRate)
        {
            if (!enableLearning || aiBehavior == null) return;

            // Make AI harder if player is winning too much
            if (playerWinRate > 0.7f)
            {
                // Increase difficulty
                if (aiBehavior.GetType().GetProperty("Difficulty") != null)
                {
                    Debug.Log("AI adapting: Increasing difficulty");
                }
            }
            else if (playerWinRate < 0.3f)
            {
                // Decrease difficulty slightly
                Debug.Log("AI adapting: Making fight more balanced");
            }
        }

        #endregion

        #region Data Export/Import

        /// <summary>
        /// Export learning data for persistence
        /// </summary>
        public LearningData ExportLearningData()
        {
            return new LearningData
            {
                tendencies = this.tendencies,
                patternFrequency = new Dictionary<string, int>(this.patternFrequency),
                totalActionsRecorded = recentActions.Count
            };
        }

        /// <summary>
        /// Import learning data from previous session
        /// </summary>
        public void ImportLearningData(LearningData data)
        {
            if (data == null) return;

            this.tendencies = data.tendencies;
            this.patternFrequency = new Dictionary<string, int>(data.patternFrequency);

            Debug.Log($"AI learning data imported: {data.totalActionsRecorded} actions");
        }

        /// <summary>
        /// Reset all learning
        /// </summary>
        public void ResetLearning()
        {
            InitializeLearning();
            recentActions.Clear();
            Debug.Log("AI learning reset");
        }

        #endregion

        #region Statistics

        /// <summary>
        /// Get learning statistics for display
        /// </summary>
        public string GetLearningStats()
        {
            return $"Actions Tracked: {recentActions.Count}\n" +
                   $"Patterns Learned: {learnedCounters.Count}\n" +
                   $"Player Aggression: {(tendencies.GetAggressionRatio() * 100):F1}%\n" +
                   $"Light Attack Preference: {(tendencies.GetLightAttackPreference() * 100):F1}%\n" +
                   $"Most Frequent Pattern: {GetMostFrequentPattern()}";
        }

        private string GetMostFrequentPattern()
        {
            if (patternFrequency.Count == 0) return "None";

            var mostFrequent = patternFrequency.OrderByDescending(kv => kv.Value).First();
            return $"{mostFrequent.Key} ({mostFrequent.Value}x)";
        }

        #endregion
    }

    #region Data Structures

    /// <summary>
    /// Player action record
    /// </summary>
    [System.Serializable]
    public struct PlayerAction
    {
        public ActionType action;
        public float timestamp;
        public bool wasSuccessful;
    }

    /// <summary>
    /// Action types for learning
    /// </summary>
    public enum ActionType
    {
        LightAttack,
        HeavyAttack,
        SpecialAttack,
        Block,
        Dodge,
        Aggressive,
        Defensive
    }

    /// <summary>
    /// Identified attack patterns
    /// </summary>
    public enum AttackPattern
    {
        LightCombo,
        MixedCombo,
        PowerCombo,
        EvasiveStyle,
        DefensiveStyle,
        Aggressive
    }

    /// <summary>
    /// Counter strategies
    /// </summary>
    public enum CounterStrategy
    {
        PerfectBlock,
        Dodge,
        Interrupt,
        Predict,
        Grab,
        Counter,
        Adapt
    }

    /// <summary>
    /// Player tendency statistics
    /// </summary>
    [System.Serializable]
    public class PlayerTendencies
    {
        public int totalActions;
        public int lightAttackCount;
        public int heavyAttackCount;
        public int specialAttackCount;
        public int blockCount;
        public int dodgeCount;
        public int aggressiveActions;
        public int defensiveActions;
        public int successfulLightAttacks;
        public int successfulHeavyAttacks;
        public int successfulSpecials;

        public float GetAggressionRatio()
        {
            if (totalActions == 0) return 0.5f;
            return (float)aggressiveActions / totalActions;
        }

        public float GetLightAttackPreference()
        {
            int totalAttacks = lightAttackCount + heavyAttackCount + specialAttackCount;
            if (totalAttacks == 0) return 0.33f;
            return (float)lightAttackCount / totalAttacks;
        }

        public float GetBlockingFrequency()
        {
            if (totalActions == 0) return 0f;
            return (float)blockCount / totalActions;
        }
    }

    /// <summary>
    /// Exportable learning data
    /// </summary>
    [System.Serializable]
    public class LearningData
    {
        public PlayerTendencies tendencies;
        public Dictionary<string, int> patternFrequency;
        public int totalActionsRecorded;
    }

    #endregion
}
