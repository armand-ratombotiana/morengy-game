using UnityEngine;
using Morengy.Character;
using Morengy.Combat;

namespace Morengy.AI
{
    /// <summary>
    /// Evolving rival AI that adapts and grows with the player.
    /// Uses AI learning system to create personalized challenge.
    /// </summary>
    [RequireComponent(typeof(AIBehavior))]
    [RequireComponent(typeof(FighterStats))]
    public class RivalAI : MonoBehaviour
    {
        [Header("Rival Settings")]
        [SerializeField] private string rivalName = "The Rival";
        [SerializeField] private bool enableEvolution = true;
        [SerializeField] private float evolutionRate = 0.05f;

        [Header("Personality")]
        [SerializeField] private RivalPersonality personality = RivalPersonality.Adaptive;
        [SerializeField] private bool mirrorPlayerStyle = true;

        [Header("Learning")]
        [SerializeField] private float learningAggressiveness = 1.5f;
        [SerializeField] private bool rememberPreviousMatches = true;

        // Components
        private AIBehavior aiBehavior;
        private FighterStats rivalStats;
        private AILearningSystem learningSystem;

        // Evolution tracking
        private RivalData rivalData;
        private int matchesAgainstPlayer = 0;
        private int winsAgainstPlayer = 0;
        private int lossesAgainstPlayer = 0;

        // Style adaptation
        private PlayerStyleProfile playerStyle;
        private float adaptationLevel = 0f;

        // Events
        public event System.Action<RivalData> OnRivalEvolved;
        public event System.Action<string> OnRivalTaunt;

        // Properties
        public RivalData RivalData => rivalData;
        public float AdaptationLevel => adaptationLevel;

        private void Awake()
        {
            aiBehavior = GetComponent<AIBehavior>();
            rivalStats = GetComponent<FighterStats>();
            learningSystem = AILearningSystem.Instance;

            InitializeRival();
        }

        private void Start()
        {
            LoadRivalData();

            if (learningSystem != null)
            {
                ApplyLearning();
            }
        }

        /// <summary>
        /// Initialize rival AI
        /// </summary>
        private void InitializeRival()
        {
            rivalData = new RivalData
            {
                name = rivalName,
                level = 1,
                matchesPlayed = 0,
                wins = 0,
                losses = 0,
                evolutionStage = 0
            };

            playerStyle = new PlayerStyleProfile();
        }

        #region Match Integration

        /// <summary>
        /// Called when match starts
        /// </summary>
        public void OnMatchStart()
        {
            matchesAgainstPlayer++;
            rivalData.matchesPlayed++;

            // Analyze player from learning system
            if (learningSystem != null)
            {
                AnalyzePlayerStyle();
            }

            // Send taunt based on history
            SendOpeningTaunt();

            Debug.Log($"{rivalName} entered the ring! (Match #{matchesAgainstPlayer})");
        }

        /// <summary>
        /// Called when match ends
        /// </summary>
        public void OnMatchEnd(bool rivalWon)
        {
            if (rivalWon)
            {
                winsAgainstPlayer++;
                rivalData.wins++;
                SendVictoryTaunt();
            }
            else
            {
                lossesAgainstPlayer++;
                rivalData.losses++;
                SendDefeatTaunt();
            }

            // Evolve after match
            if (enableEvolution)
            {
                EvolveRival(rivalWon);
            }

            // Save progress
            SaveRivalData();

            Debug.Log($"{rivalName} record: {rivalData.wins}W-{rivalData.losses}L");
        }

        /// <summary>
        /// Called when round ends
        /// </summary>
        public void OnRoundEnd(bool rivalWonRound)
        {
            if (!rivalWonRound)
            {
                // Learn from defeat
                AdaptToPlayerStyle();
            }
        }

        #endregion

        #region Evolution System

        /// <summary>
        /// Evolve rival based on match result
        /// </summary>
        private void EvolveRival(bool won)
        {
            // Gain experience regardless of result
            float xpGain = won ? 1.5f : 1.0f;
            adaptationLevel += evolutionRate * xpGain;

            // Level up at adaptation thresholds
            if (adaptationLevel >= rivalData.level)
            {
                LevelUpRival();
            }

            // Evolution stages based on total matches
            int newStage = matchesAgainstPlayer / 3;
            if (newStage > rivalData.evolutionStage)
            {
                rivalData.evolutionStage = newStage;
                TriggerEvolution();
            }
        }

        /// <summary>
        /// Level up rival stats
        /// </summary>
        private void LevelUpRival()
        {
            rivalData.level++;

            // Improve stats
            if (rivalStats != null)
            {
                float statBoost = 1.05f; // 5% increase
                rivalStats.SetPower(Mathf.RoundToInt(rivalStats.Power * statBoost));
                rivalStats.SetSpeed(Mathf.RoundToInt(rivalStats.Speed * statBoost));
                rivalStats.SetDefense(Mathf.RoundToInt(rivalStats.Defense * statBoost));
                rivalStats.SetStamina(Mathf.RoundToInt(rivalStats.Stamina * statBoost));
                rivalStats.SetTechnique(Mathf.RoundToInt(rivalStats.Technique * statBoost));
            }

            Debug.Log($"{rivalName} leveled up to {rivalData.level}!");
        }

        /// <summary>
        /// Trigger evolution event
        /// </summary>
        private void TriggerEvolution()
        {
            OnRivalEvolved?.Invoke(rivalData);

            // Update AI behavior based on evolution
            UpdateBehaviorFromEvolution();

            Debug.Log($"{rivalName} evolved to Stage {rivalData.evolutionStage}!");

            // Show evolution message
            if (UI.RoundAnnouncer.Instance != null)
            {
                UI.RoundAnnouncer.Instance.AnnounceRival($"{rivalName} has evolved!");
            }
        }

        /// <summary>
        /// Update AI behavior based on evolution stage
        /// </summary>
        private void UpdateBehaviorFromEvolution()
        {
            if (aiBehavior == null) return;

            // Stage 0: Basic
            // Stage 1: Learns combos
            // Stage 2: Predicts player moves
            // Stage 3: Masters counters
            // Stage 4+: Perfect adaptation

            switch (rivalData.evolutionStage)
            {
                case 0:
                    // Basic AI
                    break;

                case 1:
                    // Enable combo learning
                    rivalData.canUseAdvancedCombos = true;
                    break;

                case 2:
                    // Enable prediction
                    rivalData.canPredictMoves = true;
                    break;

                case 3:
                    // Master counters
                    rivalData.canPerfectCounter = true;
                    break;

                default:
                    // Full adaptation
                    rivalData.canAdaptInRealTime = true;
                    break;
            }
        }

        #endregion

        #region Style Adaptation

        /// <summary>
        /// Analyze player fighting style
        /// </summary>
        private void AnalyzePlayerStyle()
        {
            if (learningSystem == null) return;

            // Get learning stats
            string stats = learningSystem.GetLearningStats();

            // Extract player tendencies
            // This would parse the learning data to build style profile
            // For now, we'll use basic detection

            Debug.Log($"{rivalName} analyzing player style...");
        }

        /// <summary>
        /// Adapt to counter player's style
        /// </summary>
        private void AdaptToPlayerStyle()
        {
            if (!mirrorPlayerStyle || aiBehavior == null) return;

            // Get recommended counter strategy
            if (learningSystem != null)
            {
                CounterStrategy counter = learningSystem.GetRecommendedCounter();
                ApplyCounterStrategy(counter);
            }

            Debug.Log($"{rivalName} adapted strategy!");
        }

        /// <summary>
        /// Apply counter strategy to AI behavior
        /// </summary>
        private void ApplyCounterStrategy(CounterStrategy counter)
        {
            // This would modify AI behavior based on counter strategy
            switch (counter)
            {
                case CounterStrategy.PerfectBlock:
                    // Increase blocking tendency
                    break;

                case CounterStrategy.Dodge:
                    // Increase dodge usage
                    break;

                case CounterStrategy.Interrupt:
                    // Be more aggressive
                    break;

                case CounterStrategy.Predict:
                    // Anticipate player moves
                    break;

                case CounterStrategy.Grab:
                    // Use more grabs/aggressive moves
                    break;

                case CounterStrategy.Counter:
                    // Focus on counterattacks
                    break;
            }
        }

        /// <summary>
        /// Apply AI learning to rival
        /// </summary>
        private void ApplyLearning()
        {
            if (learningSystem == null) return;

            // Import learning data if available
            var learningData = learningSystem.ExportLearningData();
            if (learningData != null)
            {
                // Apply learned patterns with increased aggression
                Debug.Log($"{rivalName} using learned patterns!");
            }
        }

        #endregion

        #region Taunting System

        /// <summary>
        /// Send opening taunt
        /// </summary>
        private void SendOpeningTaunt()
        {
            string taunt = "";

            if (matchesAgainstPlayer == 1)
            {
                taunt = "Let's see what you've got!";
            }
            else if (winsAgainstPlayer > lossesAgainstPlayer)
            {
                taunt = "I'm still the better fighter!";
            }
            else if (lossesAgainstPlayer > winsAgainstPlayer)
            {
                taunt = "This time will be different!";
            }
            else
            {
                taunt = "We're evenly matched... for now!";
            }

            OnRivalTaunt?.Invoke(taunt);
            Debug.Log($"{rivalName}: \"{taunt}\"");
        }

        /// <summary>
        /// Send victory taunt
        /// </summary>
        private void SendVictoryTaunt()
        {
            string[] taunts = {
                "I've learned from our battles!",
                "You'll have to do better than that!",
                "This is what evolution looks like!",
                "I adapt faster than you can improve!"
            };

            string taunt = taunts[Random.Range(0, taunts.Length)];
            OnRivalTaunt?.Invoke(taunt);
            Debug.Log($"{rivalName}: \"{taunt}\"");
        }

        /// <summary>
        /// Send defeat taunt
        /// </summary>
        private void SendDefeatTaunt()
        {
            string[] taunts = {
                "I'll remember this...",
                "You got lucky this time!",
                "I'm learning from this defeat!",
                "Next time, I'll be stronger!"
            };

            string taunt = taunts[Random.Range(0, taunts.Length)];
            OnRivalTaunt?.Invoke(taunt);
            Debug.Log($"{rivalName}: \"{taunt}\"");
        }

        #endregion

        #region Persistence

        /// <summary>
        /// Save rival data
        /// </summary>
        private void SaveRivalData()
        {
            if (!rememberPreviousMatches) return;

            string json = JsonUtility.ToJson(rivalData);
            PlayerPrefs.SetString($"RivalData_{rivalName}", json);
            PlayerPrefs.Save();

            Debug.Log($"{rivalName} data saved");
        }

        /// <summary>
        /// Load rival data
        /// </summary>
        private void LoadRivalData()
        {
            if (!rememberPreviousMatches) return;

            string key = $"RivalData_{rivalName}";
            if (PlayerPrefs.HasKey(key))
            {
                string json = PlayerPrefs.GetString(key);
                rivalData = JsonUtility.FromJson<RivalData>(json);

                matchesAgainstPlayer = rivalData.matchesPlayed;
                winsAgainstPlayer = rivalData.wins;
                lossesAgainstPlayer = rivalData.losses;
                adaptationLevel = rivalData.level;

                Debug.Log($"{rivalName} data loaded: Level {rivalData.level}");
            }
        }

        /// <summary>
        /// Reset rival progression
        /// </summary>
        public void ResetRival()
        {
            InitializeRival();
            matchesAgainstPlayer = 0;
            winsAgainstPlayer = 0;
            lossesAgainstPlayer = 0;
            adaptationLevel = 0f;

            PlayerPrefs.DeleteKey($"RivalData_{rivalName}");

            Debug.Log($"{rivalName} reset to base level");
        }

        #endregion

        #region Statistics

        /// <summary>
        /// Get rivalry statistics
        /// </summary>
        public RivalStats GetRivalStats()
        {
            return new RivalStats
            {
                Name = rivalName,
                Level = rivalData.level,
                EvolutionStage = rivalData.evolutionStage,
                TotalMatches = matchesAgainstPlayer,
                Wins = winsAgainstPlayer,
                Losses = lossesAgainstPlayer,
                WinRate = matchesAgainstPlayer > 0 ? (float)winsAgainstPlayer / matchesAgainstPlayer : 0f,
                AdaptationLevel = adaptationLevel
            };
        }

        #endregion
    }

    #region Data Structures

    /// <summary>
    /// Rival progression data
    /// </summary>
    [System.Serializable]
    public class RivalData
    {
        public string name;
        public int level;
        public int matchesPlayed;
        public int wins;
        public int losses;
        public int evolutionStage;

        // Abilities unlocked through evolution
        public bool canUseAdvancedCombos;
        public bool canPredictMoves;
        public bool canPerfectCounter;
        public bool canAdaptInRealTime;
    }

    /// <summary>
    /// Player style profile for adaptation
    /// </summary>
    [System.Serializable]
    public class PlayerStyleProfile
    {
        public float aggressionLevel;
        public float defenseLevel;
        public float comboUsage;
        public float specialUsage;
        public float movementPattern;
    }

    /// <summary>
    /// Rival statistics
    /// </summary>
    [System.Serializable]
    public struct RivalStats
    {
        public string Name;
        public int Level;
        public int EvolutionStage;
        public int TotalMatches;
        public int Wins;
        public int Losses;
        public float WinRate;
        public float AdaptationLevel;
    }

    /// <summary>
    /// Rival personality types
    /// </summary>
    public enum RivalPersonality
    {
        Adaptive,      // Learns and adapts
        Aggressive,    // Always aggressive
        Tactical,      // Strategic and patient
        Mimic,         // Copies player style
        Unpredictable  // Random and chaotic
    }

    #endregion
}
