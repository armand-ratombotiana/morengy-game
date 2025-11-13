using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Morengy.Core
{
    /// <summary>
    /// Player profile system tracking statistics, progression, and achievements.
    /// Persists across sessions and provides comprehensive player analytics.
    /// </summary>
    public class PlayerProfile : MonoBehaviour
    {
        [Header("Profile Settings")]
        [SerializeField] private string defaultPlayerName = "Fighter";
        [SerializeField] private bool autoSave = true;
        [SerializeField] private float autoSaveInterval = 60f;

        // Profile data
        private ProfileData profile;
        private float lastSaveTime = 0f;

        // Session tracking
        private SessionData currentSession;
        private bool sessionActive = false;

        // Events
        public event System.Action<ProfileData> OnProfileLoaded;
        public event System.Action<ProfileData> OnProfileSaved;
        public event System.Action<Achievement> OnAchievementUnlocked;
        public event System.Action<int> OnLevelUp;

        // Singleton
        public static PlayerProfile Instance { get; private set; }

        // Properties
        public ProfileData Profile => profile;
        public bool IsProfileLoaded => profile != null;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadProfile();
        }

        private void Start()
        {
            StartSession();
        }

        private void Update()
        {
            // Auto-save
            if (autoSave && Time.time - lastSaveTime > autoSaveInterval)
            {
                SaveProfile();
                lastSaveTime = Time.time;
            }
        }

        private void OnApplicationQuit()
        {
            EndSession();
            SaveProfile();
        }

        #region Profile Management

        /// <summary>
        /// Create new player profile
        /// </summary>
        public void CreateNewProfile(string playerName = null)
        {
            profile = new ProfileData
            {
                playerName = string.IsNullOrEmpty(playerName) ? defaultPlayerName : playerName,
                createdDate = System.DateTime.Now.ToString(),
                level = 1,
                experience = 0,
                totalPlayTime = 0f
            };

            OnProfileLoaded?.Invoke(profile);

            Debug.Log($"New profile created: {profile.playerName}");
        }

        /// <summary>
        /// Load profile from PlayerPrefs
        /// </summary>
        public void LoadProfile()
        {
            if (PlayerPrefs.HasKey("PlayerProfile"))
            {
                string json = PlayerPrefs.GetString("PlayerProfile");
                profile = JsonUtility.FromJson<ProfileData>(json);

                OnProfileLoaded?.Invoke(profile);

                Debug.Log($"Profile loaded: {profile.playerName} (Level {profile.level})");
            }
            else
            {
                CreateNewProfile();
            }
        }

        /// <summary>
        /// Save profile to PlayerPrefs
        /// </summary>
        public void SaveProfile()
        {
            if (profile == null) return;

            // Update last played
            profile.lastPlayed = System.DateTime.Now.ToString();

            string json = JsonUtility.ToJson(profile);
            PlayerPrefs.SetString("PlayerProfile", json);
            PlayerPrefs.Save();

            OnProfileSaved?.Invoke(profile);

            Debug.Log($"Profile saved: {profile.playerName}");
        }

        /// <summary>
        /// Delete current profile
        /// </summary>
        public void DeleteProfile()
        {
            PlayerPrefs.DeleteKey("PlayerProfile");
            profile = null;

            Debug.Log("Profile deleted");
        }

        #endregion

        #region Statistics Tracking

        /// <summary>
        /// Record match result
        /// </summary>
        public void RecordMatchResult(bool won, int roundsWon, int roundsLost)
        {
            if (profile == null) return;

            profile.stats.totalMatches++;

            if (won)
            {
                profile.stats.matchesWon++;
                profile.stats.currentWinStreak++;

                if (profile.stats.currentWinStreak > profile.stats.longestWinStreak)
                {
                    profile.stats.longestWinStreak = profile.stats.currentWinStreak;
                }
            }
            else
            {
                profile.stats.matchesLost++;
                profile.stats.currentWinStreak = 0;
            }

            profile.stats.roundsWon += roundsWon;
            profile.stats.roundsLost += roundsLost;

            // Update session
            if (sessionActive)
            {
                currentSession.matchesPlayed++;
                if (won) currentSession.matchesWon++;
            }

            // Award experience
            int xpGained = won ? 100 : 25;
            AddExperience(xpGained);

            // Check achievements
            CheckMatchAchievements();
        }

        /// <summary>
        /// Record combat statistics
        /// </summary>
        public void RecordCombatStats(int hits, int heavyHits, int specials, int blocks, int dodges, float damage)
        {
            if (profile == null) return;

            profile.stats.totalHitsLanded += hits;
            profile.stats.heavyAttacksLanded += heavyHits;
            profile.stats.specialMovesUsed += specials;
            profile.stats.successfulBlocks += blocks;
            profile.stats.successfulDodges += dodges;
            profile.stats.totalDamageDealt += damage;

            // Update session
            if (sessionActive)
            {
                currentSession.damageDealt += damage;
            }
        }

        /// <summary>
        /// Record combo achievement
        /// </summary>
        public void RecordCombo(int comboCount, float comboDamage)
        {
            if (profile == null) return;

            profile.stats.totalCombos++;

            if (comboCount > profile.stats.longestCombo)
            {
                profile.stats.longestCombo = comboCount;
                CheckComboAchievements(comboCount);
            }

            // Session tracking
            if (sessionActive)
            {
                if (comboCount > currentSession.longestCombo)
                {
                    currentSession.longestCombo = comboCount;
                }
            }
        }

        /// <summary>
        /// Record knockout
        /// </summary>
        public void RecordKnockout(bool isPerfect)
        {
            if (profile == null) return;

            profile.stats.knockouts++;

            if (isPerfect)
            {
                profile.stats.perfectVictories++;
                CheckPerfectVictoryAchievements();
            }
        }

        #endregion

        #region Experience & Leveling

        /// <summary>
        /// Add experience points
        /// </summary>
        public void AddExperience(int amount)
        {
            if (profile == null) return;

            profile.experience += amount;

            // Check for level up
            int requiredXP = GetRequiredExperience(profile.level);
            if (profile.experience >= requiredXP)
            {
                LevelUp();
            }
        }

        /// <summary>
        /// Level up player
        /// </summary>
        private void LevelUp()
        {
            profile.level++;
            profile.experience = 0;

            OnLevelUp?.Invoke(profile.level);

            Debug.Log($"Level Up! Now level {profile.level}");

            // Award level-up rewards
            profile.skillPoints += 2;

            // Check level achievements
            CheckLevelAchievements();
        }

        /// <summary>
        /// Get required XP for level
        /// </summary>
        private int GetRequiredExperience(int level)
        {
            return 100 * level + (level * level * 50);
        }

        /// <summary>
        /// Get progress to next level
        /// </summary>
        public float GetLevelProgress()
        {
            if (profile == null) return 0f;

            int required = GetRequiredExperience(profile.level);
            return (float)profile.experience / required;
        }

        #endregion

        #region Achievements

        /// <summary>
        /// Check match-related achievements
        /// </summary>
        private void CheckMatchAchievements()
        {
            // First win
            if (profile.stats.matchesWon == 1)
            {
                UnlockAchievement("FIRST_VICTORY", "First Victory", "Win your first match");
            }

            // Win milestones
            if (profile.stats.matchesWon == 10)
            {
                UnlockAchievement("WIN_10", "Rising Fighter", "Win 10 matches");
            }
            else if (profile.stats.matchesWon == 50)
            {
                UnlockAchievement("WIN_50", "Experienced Fighter", "Win 50 matches");
            }
            else if (profile.stats.matchesWon == 100)
            {
                UnlockAchievement("WIN_100", "Champion", "Win 100 matches");
            }

            // Win streak
            if (profile.stats.currentWinStreak == 5)
            {
                UnlockAchievement("STREAK_5", "Hot Streak", "Win 5 matches in a row");
            }
            else if (profile.stats.currentWinStreak == 10)
            {
                UnlockAchievement("STREAK_10", "Unstoppable", "Win 10 matches in a row");
            }
        }

        /// <summary>
        /// Check combo achievements
        /// </summary>
        private void CheckComboAchievements(int comboCount)
        {
            if (comboCount >= 10)
            {
                UnlockAchievement("COMBO_10", "Combo Master", "Land a 10-hit combo");
            }
            else if (comboCount >= 20)
            {
                UnlockAchievement("COMBO_20", "Ultra Combo", "Land a 20-hit combo");
            }
        }

        /// <summary>
        /// Check perfect victory achievements
        /// </summary>
        private void CheckPerfectVictoryAchievements()
        {
            if (profile.stats.perfectVictories == 1)
            {
                UnlockAchievement("PERFECT_1", "Flawless", "Win a match without losing a round");
            }
            else if (profile.stats.perfectVictories == 10)
            {
                UnlockAchievement("PERFECT_10", "Untouchable", "Win 10 perfect victories");
            }
        }

        /// <summary>
        /// Check level achievements
        /// </summary>
        private void CheckLevelAchievements()
        {
            if (profile.level == 10)
            {
                UnlockAchievement("LEVEL_10", "Dedicated", "Reach level 10");
            }
            else if (profile.level == 25)
            {
                UnlockAchievement("LEVEL_25", "Expert", "Reach level 25");
            }
            else if (profile.level == 50)
            {
                UnlockAchievement("LEVEL_50", "Master", "Reach level 50");
            }
        }

        /// <summary>
        /// Unlock achievement
        /// </summary>
        private void UnlockAchievement(string id, string title, string description)
        {
            // Check if already unlocked
            if (profile.achievements.Any(a => a.id == id))
                return;

            Achievement achievement = new Achievement
            {
                id = id,
                title = title,
                description = description,
                unlockedDate = System.DateTime.Now.ToString()
            };

            profile.achievements.Add(achievement);

            OnAchievementUnlocked?.Invoke(achievement);

            Debug.Log($"Achievement Unlocked: {title}");
        }

        /// <summary>
        /// Get achievement progress
        /// </summary>
        public float GetAchievementProgress()
        {
            int totalAchievements = 15; // Total available
            return (float)profile.achievements.Count / totalAchievements;
        }

        #endregion

        #region Session Management

        /// <summary>
        /// Start play session
        /// </summary>
        private void StartSession()
        {
            currentSession = new SessionData
            {
                startTime = Time.time,
                matchesPlayed = 0,
                matchesWon = 0,
                damageDealt = 0f,
                longestCombo = 0
            };

            sessionActive = true;

            Debug.Log("Session started");
        }

        /// <summary>
        /// End play session
        /// </summary>
        private void EndSession()
        {
            if (!sessionActive) return;

            currentSession.duration = Time.time - currentSession.startTime;

            // Update total play time
            profile.totalPlayTime += currentSession.duration;

            // Record session
            profile.sessions.Add(currentSession);

            sessionActive = false;

            Debug.Log($"Session ended: {currentSession.duration / 60f:F1} minutes");
        }

        /// <summary>
        /// Get current session stats
        /// </summary>
        public SessionData GetCurrentSession()
        {
            if (!sessionActive) return new SessionData();

            currentSession.duration = Time.time - currentSession.startTime;
            return currentSession;
        }

        #endregion

        #region Statistics Queries

        /// <summary>
        /// Get win rate
        /// </summary>
        public float GetWinRate()
        {
            if (profile == null || profile.stats.totalMatches == 0) return 0f;
            return (float)profile.stats.matchesWon / profile.stats.totalMatches;
        }

        /// <summary>
        /// Get formatted statistics
        /// </summary>
        public string GetStatsSummary()
        {
            if (profile == null) return "No profile loaded";

            return $"Level: {profile.level}\n" +
                   $"Matches: {profile.stats.totalMatches} ({profile.stats.matchesWon}W-{profile.stats.matchesLost}L)\n" +
                   $"Win Rate: {(GetWinRate() * 100):F1}%\n" +
                   $"Win Streak: {profile.stats.currentWinStreak}\n" +
                   $"Longest Combo: {profile.stats.longestCombo}\n" +
                   $"Perfect Victories: {profile.stats.perfectVictories}\n" +
                   $"Play Time: {(profile.totalPlayTime / 3600f):F1}h\n" +
                   $"Achievements: {profile.achievements.Count}/15";
        }

        #endregion
    }

    #region Data Structures

    /// <summary>
    /// Complete player profile data
    /// </summary>
    [System.Serializable]
    public class ProfileData
    {
        public string playerName;
        public string createdDate;
        public string lastPlayed;
        public int level;
        public int experience;
        public int skillPoints;
        public float totalPlayTime;

        public PlayerStatistics stats = new PlayerStatistics();
        public List<Achievement> achievements = new List<Achievement>();
        public List<SessionData> sessions = new List<SessionData>();
    }

    /// <summary>
    /// Player statistics
    /// </summary>
    [System.Serializable]
    public class PlayerStatistics
    {
        // Match stats
        public int totalMatches;
        public int matchesWon;
        public int matchesLost;
        public int roundsWon;
        public int roundsLost;
        public int currentWinStreak;
        public int longestWinStreak;

        // Combat stats
        public int totalHitsLanded;
        public int heavyAttacksLanded;
        public int specialMovesUsed;
        public int successfulBlocks;
        public int successfulDodges;
        public float totalDamageDealt;

        // Achievement stats
        public int longestCombo;
        public int totalCombos;
        public int knockouts;
        public int perfectVictories;
    }

    /// <summary>
    /// Achievement data
    /// </summary>
    [System.Serializable]
    public struct Achievement
    {
        public string id;
        public string title;
        public string description;
        public string unlockedDate;
    }

    /// <summary>
    /// Play session data
    /// </summary>
    [System.Serializable]
    public struct SessionData
    {
        public float startTime;
        public float duration;
        public int matchesPlayed;
        public int matchesWon;
        public float damageDealt;
        public int longestCombo;
    }

    #endregion
}
