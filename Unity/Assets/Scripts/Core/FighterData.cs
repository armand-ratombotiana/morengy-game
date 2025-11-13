using UnityEngine;

namespace Morengy.Core
{
    /// <summary>
    /// ScriptableObject containing fighter configuration data.
    /// Allows easy creation and balancing of fighters without code changes.
    /// </summary>
    [CreateAssetMenu(fileName = "New Fighter", menuName = "Morengy/Fighter Data", order = 1)]
    public class FighterData : ScriptableObject
    {
        [Header("Fighter Identity")]
        public string fighterName = "Unknown Fighter";
        public string fightingStyle = "Traditional Morengy";
        public string region = "Diego Suarez";
        public string backstory = "A warrior from Madagascar...";

        [Header("Visual")]
        public Sprite portrait;
        public GameObject fighterPrefab;
        public RuntimeAnimatorController animatorController;

        [Header("Core Stats (1-100)")]
        [Range(1, 100)] public float power = 50f;
        [Range(1, 100)] public float speed = 50f;
        [Range(1, 100)] public float defense = 50f;
        [Range(1, 100)] public float stamina = 50f;
        [Range(1, 100)] public float technique = 50f;
        [Range(1, 100)] public float charisma = 50f;

        [Header("Combat Properties")]
        public float maxHealth = 100f;
        public float lightAttackDamage = 10f;
        public float heavyAttackDamage = 25f;
        public float specialAttackDamage = 40f;

        [Header("Special Moves")]
        public string specialMoveName = "Morengy Finisher";
        public string specialMoveDescription = "A devastating traditional technique";
        public AnimationClip specialMoveAnimation;

        [Header("Audio")]
        public AudioClip[] voiceLines;
        public AudioClip victoryLine;
        public AudioClip defeatLine;
        public AudioClip tauntLine;

        [Header("Unlocks")]
        public bool isUnlocked = true;
        public int unlockCost = 0;
        public string unlockRequirement = "";

        /// <summary>
        /// Get total stat rating
        /// </summary>
        public float GetTotalRating()
        {
            return (power + speed + defense + stamina + technique + charisma) / 6f;
        }

        /// <summary>
        /// Get fighter archetype based on highest stats
        /// </summary>
        public string GetArchetype()
        {
            float highestStat = Mathf.Max(power, speed, defense, stamina, technique, charisma);

            if (highestStat == power) return "Brawler";
            if (highestStat == speed) return "Speedster";
            if (highestStat == defense) return "Tank";
            if (highestStat == technique) return "Technical";
            if (highestStat == charisma) return "Showman";

            return "Balanced";
        }

        /// <summary>
        /// Validate fighter data
        /// </summary>
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(fighterName)) return false;
            if (fighterPrefab == null) return false;
            if (maxHealth <= 0) return false;

            return true;
        }
    }
}
