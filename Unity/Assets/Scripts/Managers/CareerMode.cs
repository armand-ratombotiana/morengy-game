using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Morengy.Character;
using Morengy.AI;

namespace Morengy.Managers
{
    /// <summary>
    /// Career mode manager handling progression, unlocks, and story events.
    /// Provides structured gameplay with increasing difficulty and rewards.
    /// </summary>
    public class CareerMode : MonoBehaviour
    {
        [Header("Career Settings")]
        [SerializeField] private int totalCareerFights = 20;
        [SerializeField] private int fightsPerTier = 5;
        [SerializeField] private float difficultyIncreaseRate = 0.15f;

        [Header("Progression")]
        [SerializeField] private int requiredWinsToProgress = 3;
        [SerializeField] private bool allowRetries = true;
        [SerializeField] private int maxRetriesPerFight = 3;

        [Header("Rewards")]
        [SerializeField] private int baseRewardPerWin = 100;
        [SerializeField] private int bonusPerPerfectRound = 50;
        [SerializeField] private int bonusPerComboMilestone = 25;

        // Career state
        private CareerData currentCareer;
        private int currentFightIndex = 0;
        private int retriesUsed = 0;
        private List<FightResult> fightHistory = new List<FightResult>();

        // Current fight data
        private FightSetup currentFight;
        private bool isFightActive = false;

        // Events
        public event System.Action<CareerData> OnCareerStarted;
        public event System.Action<FightSetup> OnFightStarted;
        public event System.Action<FightResult> OnFightCompleted;
        public event System.Action<int> OnTierCompleted;
        public event System.Action<CareerData> OnCareerCompleted;
        public event System.Action<string> OnUnlockEarned;

        // Singleton
        public static CareerMode Instance { get; private set; }

        // Properties
        public CareerData CurrentCareer => currentCareer;
        public int CurrentFightIndex => currentFightIndex;
        public bool IsCareerActive => currentCareer != null;

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

        #region Career Management

        /// <summary>
        /// Start new career mode
        /// </summary>
        public void StartNewCareer(string fighterName)
        {
            currentCareer = new CareerData
            {
                fighterName = fighterName,
                startDate = System.DateTime.Now.ToString(),
                totalFights = totalCareerFights,
                currentFight = 0,
                wins = 0,
                losses = 0,
                currentTier = CareerTier.Amateur,
                currency = 0,
                reputation = 0
            };

            currentFightIndex = 0;
            fightHistory.Clear();

            OnCareerStarted?.Invoke(currentCareer);

            Debug.Log($"Career started for {fighterName}");

            // Start first fight
            SetupNextFight();
        }

        /// <summary>
        /// Load existing career
        /// </summary>
        public void LoadCareer(CareerData data)
        {
            currentCareer = data;
            currentFightIndex = data.currentFight;

            OnCareerStarted?.Invoke(currentCareer);

            Debug.Log($"Career loaded for {data.fighterName}: Fight {currentFightIndex}/{totalCareerFights}");
        }

        /// <summary>
        /// Setup next fight in career
        /// </summary>
        public void SetupNextFight()
        {
            if (currentFightIndex >= totalCareerFights)
            {
                CompleteCareer();
                return;
            }

            // Generate fight based on progression
            currentFight = GenerateFight(currentFightIndex);

            // Update career data
            currentCareer.currentFight = currentFightIndex;

            OnFightStarted?.Invoke(currentFight);

            Debug.Log($"Fight {currentFightIndex + 1}: {currentFight.opponentName} ({currentFight.difficulty})");
        }

        /// <summary>
        /// Generate fight setup for current progression
        /// </summary>
        private FightSetup GenerateFight(int fightIndex)
        {
            // Determine tier
            CareerTier tier = GetTierForFight(fightIndex);
            float difficultyMultiplier = 1f + (fightIndex * difficultyIncreaseRate);

            // Create opponent
            FightSetup fight = new FightSetup
            {
                fightNumber = fightIndex + 1,
                tier = tier,
                opponentName = GenerateOpponentName(tier, fightIndex),
                difficulty = CalculateDifficulty(tier, difficultyMultiplier),
                specialConditions = GetSpecialConditions(fightIndex),
                rewardMultiplier = CalculateRewardMultiplier(tier, fightIndex),
                isRivalFight = IsRivalFight(fightIndex),
                isBossFight = IsBossFight(fightIndex)
            };

            return fight;
        }

        /// <summary>
        /// Process fight result
        /// </summary>
        public void ProcessFightResult(bool playerWon, int roundsWon, int roundsLost, ComboStats comboStats)
        {
            FightResult result = new FightResult
            {
                fightNumber = currentFightIndex + 1,
                opponentName = currentFight.opponentName,
                playerWon = playerWon,
                roundsWon = roundsWon,
                roundsLost = roundsLost,
                highestCombo = comboStats.HighestCombo,
                totalHits = comboStats.TotalHits,
                isPerfectVictory = playerWon && roundsLost == 0
            };

            // Calculate rewards
            if (playerWon)
            {
                int reward = CalculateReward(result);
                currentCareer.currency += reward;
                currentCareer.wins++;
                currentCareer.reputation += 10;

                result.currencyEarned = reward;

                Debug.Log($"Victory! Earned {reward} currency");

                // Check for unlocks
                CheckUnlocks(result);

                // Move to next fight
                currentFightIndex++;
                retriesUsed = 0;

                // Check tier progression
                CheckTierProgression();
            }
            else
            {
                currentCareer.losses++;
                currentCareer.reputation -= 5;

                Debug.Log($"Defeat. Retries: {retriesUsed}/{maxRetriesPerFight}");

                // Handle retry
                if (allowRetries && retriesUsed < maxRetriesPerFight)
                {
                    retriesUsed++;
                }
                else
                {
                    // Game over or forced progression
                    Debug.LogWarning("Career ended due to too many losses");
                }
            }

            // Record fight
            fightHistory.Add(result);

            OnFightCompleted?.Invoke(result);

            // Update AI learning
            if (AILearningSystem.Instance != null)
            {
                float winRate = (float)currentCareer.wins / (currentCareer.wins + currentCareer.losses);
                AILearningSystem.Instance.AdaptDifficulty(winRate);
            }
        }

        /// <summary>
        /// Calculate reward for fight result
        /// </summary>
        private int CalculateReward(FightResult result)
        {
            int reward = baseRewardPerWin;

            // Perfect victory bonus
            if (result.isPerfectVictory)
            {
                reward += bonusPerPerfectRound * result.roundsWon;
            }

            // Combo bonuses
            if (result.highestCombo >= 10)
                reward += bonusPerComboMilestone * 3;
            else if (result.highestCombo >= 5)
                reward += bonusPerComboMilestone * 2;
            else if (result.highestCombo >= 3)
                reward += bonusPerComboMilestone;

            // Tier multiplier
            reward = Mathf.RoundToInt(reward * currentFight.rewardMultiplier);

            return reward;
        }

        /// <summary>
        /// Check for tier progression
        /// </summary>
        private void CheckTierProgression()
        {
            int fightsInCurrentTier = currentFightIndex % fightsPerTier;

            if (fightsInCurrentTier == 0 && currentFightIndex > 0)
            {
                // Advance tier
                currentCareer.currentTier = GetTierForFight(currentFightIndex);
                OnTierCompleted?.Invoke((int)currentCareer.currentTier);

                Debug.Log($"Advanced to {currentCareer.currentTier} tier!");
            }
        }

        /// <summary>
        /// Check for unlockables
        /// </summary>
        private void CheckUnlocks(FightResult result)
        {
            // Unlock special moves
            if (result.highestCombo >= 10 && !currentCareer.unlockedSpecialMoves.Contains("UltraCombo"))
            {
                currentCareer.unlockedSpecialMoves.Add("UltraCombo");
                OnUnlockEarned?.Invoke("Special Move: Ultra Combo");
            }

            // Unlock fighters
            if (currentCareer.wins >= 5 && !currentCareer.unlockedFighters.Contains("MahajangaVeteran"))
            {
                currentCareer.unlockedFighters.Add("MahajangaVeteran");
                OnUnlockEarned?.Invoke("Fighter: Mahajanga Veteran");
            }

            // Unlock arenas
            if (currentCareer.currentTier >= CareerTier.Professional && !currentCareer.unlockedArenas.Contains("CoastalStadium"))
            {
                currentCareer.unlockedArenas.Add("CoastalStadium");
                OnUnlockEarned?.Invoke("Arena: Coastal Stadium");
            }
        }

        /// <summary>
        /// Complete career mode
        /// </summary>
        private void CompleteCareer()
        {
            Debug.Log($"Career completed! Record: {currentCareer.wins}-{currentCareer.losses}");

            OnCareerCompleted?.Invoke(currentCareer);

            // Award final bonuses
            currentCareer.currency += 1000;
            currentCareer.reputation = 100;
        }

        #endregion

        #region Fight Generation

        /// <summary>
        /// Get tier for fight number
        /// </summary>
        private CareerTier GetTierForFight(int fightIndex)
        {
            int tierIndex = fightIndex / fightsPerTier;
            return (CareerTier)Mathf.Min(tierIndex, 3);
        }

        /// <summary>
        /// Generate opponent name
        /// </summary>
        private string GenerateOpponentName(CareerTier tier, int fightIndex)
        {
            string[] amateurNames = { "Rookie Rakoto", "Street Brawler Andry", "Local Hero Fara", "Newcomer Rija", "Challenger Mamy" };
            string[] regionalNames = { "Regional Champ Noro", "City Fighter Hery", "Tournament Winner Aina", "Skilled Veteran Toky", "Rising Star Vola" };
            string[] proNames = { "Pro Fighter Lalaina", "Champion Nirina", "Title Holder Miora", "Legend Tsiry", "Master Koto" };
            string[] championNames = { "Grand Champion Diego", "Undefeated Nosy", "Ultimate Fighter Mahajanga", "Supreme Master Tana", "Final Boss Rova" };

            string[] names = tier switch
            {
                CareerTier.Amateur => amateurNames,
                CareerTier.Regional => regionalNames,
                CareerTier.Professional => proNames,
                CareerTier.Champion => championNames,
                _ => amateurNames
            };

            return names[fightIndex % names.Length];
        }

        /// <summary>
        /// Calculate AI difficulty
        /// </summary>
        private AIDifficulty CalculateDifficulty(CareerTier tier, float multiplier)
        {
            return tier switch
            {
                CareerTier.Amateur => AIDifficulty.Easy,
                CareerTier.Regional => AIDifficulty.Medium,
                CareerTier.Professional => AIDifficulty.Hard,
                CareerTier.Champion => AIDifficulty.Expert,
                _ => AIDifficulty.Easy
            };
        }

        /// <summary>
        /// Get special conditions for fight
        /// </summary>
        private string GetSpecialConditions(int fightIndex)
        {
            if (IsBossFight(fightIndex))
                return "Boss Fight - No Retries!";
            else if (IsRivalFight(fightIndex))
                return "Rival Battle - Adapted AI!";
            else if (fightIndex % 5 == 4)
                return "Tier Final - Increased Stakes!";
            else
                return "Standard Match";
        }

        /// <summary>
        /// Calculate reward multiplier
        /// </summary>
        private float CalculateRewardMultiplier(CareerTier tier, int fightIndex)
        {
            float tierMultiplier = tier switch
            {
                CareerTier.Amateur => 1f,
                CareerTier.Regional => 1.5f,
                CareerTier.Professional => 2f,
                CareerTier.Champion => 3f,
                _ => 1f
            };

            if (IsBossFight(fightIndex))
                tierMultiplier *= 2f;

            return tierMultiplier;
        }

        /// <summary>
        /// Check if fight is rival battle
        /// </summary>
        private bool IsRivalFight(int fightIndex)
        {
            return fightIndex % 7 == 6; // Every 7th fight
        }

        /// <summary>
        /// Check if fight is boss battle
        /// </summary>
        private bool IsBossFight(int fightIndex)
        {
            return (fightIndex + 1) % fightsPerTier == 0; // Last fight of each tier
        }

        #endregion

        #region Save/Load

        /// <summary>
        /// Save career to PlayerPrefs
        /// </summary>
        public void SaveCareer()
        {
            if (currentCareer == null) return;

            string json = JsonUtility.ToJson(currentCareer);
            PlayerPrefs.SetString("CareerData", json);
            PlayerPrefs.Save();

            Debug.Log("Career saved");
        }

        /// <summary>
        /// Load career from PlayerPrefs
        /// </summary>
        public bool LoadSavedCareer()
        {
            if (PlayerPrefs.HasKey("CareerData"))
            {
                string json = PlayerPrefs.GetString("CareerData");
                CareerData data = JsonUtility.FromJson<CareerData>(json);
                LoadCareer(data);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Delete saved career
        /// </summary>
        public void DeleteCareer()
        {
            PlayerPrefs.DeleteKey("CareerData");
            currentCareer = null;
            currentFightIndex = 0;
            fightHistory.Clear();

            Debug.Log("Career deleted");
        }

        #endregion

        #region Statistics

        /// <summary>
        /// Get career statistics
        /// </summary>
        public CareerStats GetCareerStats()
        {
            if (currentCareer == null) return new CareerStats();

            return new CareerStats
            {
                TotalFights = currentCareer.wins + currentCareer.losses,
                Wins = currentCareer.wins,
                Losses = currentCareer.losses,
                WinRate = (float)currentCareer.wins / (currentCareer.wins + currentCareer.losses),
                CurrentTier = currentCareer.currentTier,
                Currency = currentCareer.currency,
                Reputation = currentCareer.reputation,
                Progress = (float)currentFightIndex / totalCareerFights
            };
        }

        #endregion
    }

    #region Data Structures

    /// <summary>
    /// Career progression data
    /// </summary>
    [System.Serializable]
    public class CareerData
    {
        public string fighterName;
        public string startDate;
        public int totalFights;
        public int currentFight;
        public int wins;
        public int losses;
        public CareerTier currentTier;
        public int currency;
        public int reputation;
        public List<string> unlockedFighters = new List<string>();
        public List<string> unlockedArenas = new List<string>();
        public List<string> unlockedSpecialMoves = new List<string>();
    }

    /// <summary>
    /// Fight setup configuration
    /// </summary>
    [System.Serializable]
    public struct FightSetup
    {
        public int fightNumber;
        public CareerTier tier;
        public string opponentName;
        public AIDifficulty difficulty;
        public string specialConditions;
        public float rewardMultiplier;
        public bool isRivalFight;
        public bool isBossFight;
    }

    /// <summary>
    /// Fight result record
    /// </summary>
    [System.Serializable]
    public struct FightResult
    {
        public int fightNumber;
        public string opponentName;
        public bool playerWon;
        public int roundsWon;
        public int roundsLost;
        public int highestCombo;
        public int totalHits;
        public bool isPerfectVictory;
        public int currencyEarned;
    }

    /// <summary>
    /// Career statistics
    /// </summary>
    [System.Serializable]
    public struct CareerStats
    {
        public int TotalFights;
        public int Wins;
        public int Losses;
        public float WinRate;
        public CareerTier CurrentTier;
        public int Currency;
        public int Reputation;
        public float Progress;
    }

    /// <summary>
    /// Career tier progression
    /// </summary>
    public enum CareerTier
    {
        Amateur = 0,
        Regional = 1,
        Professional = 2,
        Champion = 3
    }

    /// <summary>
    /// AI difficulty levels (reference from AIBehavior)
    /// </summary>
    public enum AIDifficulty
    {
        Easy,
        Medium,
        Hard,
        Expert
    }

    #endregion
}
