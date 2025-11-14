# 🤖 Advanced Systems Documentation

**Last Updated:** January 13, 2025
**Status:** Phase 6 Advanced Features - COMPLETE

This document details the advanced AI, progression, and career systems implemented for Morengy Fighting Game.

---

## 📋 Table of Contents

1. [AI Learning System](#ai-learning-system)
2. [Career Mode](#career-mode)
3. [Player Profile System](#player-profile-system)
4. [Rival AI System](#rival-ai-system)
5. [System Integration](#system-integration)
6. [Usage Examples](#usage-examples)

---

## 🧠 AI Learning System

**File:** `Unity/Assets/Scripts/AI/AILearningSystem.cs`
**Lines:** 479
**Purpose:** Adaptive AI that learns from player behavior and adjusts strategy

### Core Features

#### 1. Pattern Recognition
- Tracks up to 50 recent player actions
- Identifies 6 distinct attack patterns:
  - **LightCombo**: 3+ consecutive light attacks
  - **MixedCombo**: Alternating light/heavy attacks
  - **PowerCombo**: Multiple heavy attacks
  - **EvasiveStyle**: High dodge frequency
  - **DefensiveStyle**: Frequent blocking
  - **Aggressive**: Constant attacking

#### 2. Counter Strategy System
AI automatically determines optimal counter for detected patterns:
- **PerfectBlock**: Counter for light combos
- **Dodge**: Evade heavy attacks
- **Interrupt**: Break combo chains
- **Predict**: Anticipate next move
- **Grab**: Counter defensive play
- **Counter**: Punish aggressive attacks

#### 3. Adaptive Difficulty
```csharp
// Adjusts based on player performance
- Win rate < 40%: Reduce difficulty
- Win rate 40-60%: Maintain balance
- Win rate > 60%: Increase challenge
```

#### 4. Learning Analytics
```csharp
public struct PlayerTendencies
{
    public float aggressionRatio;      // Attack vs defense balance
    public float lightAttackRatio;     // Light attack preference
    public float heavyAttackRatio;     // Heavy attack preference
    public float specialAttackRatio;   // Special move usage
    public float blockingFrequency;    // Defense tendency
    public float dodgeFrequency;       // Evasion tendency
}
```

### Key Methods

```csharp
// Record player action for learning
RecordPlayerAction(ActionType action, bool wasSuccessful)

// Get AI's adapted action
GetAdaptedAction(float distanceToOpponent)

// Get recommended counter strategy
GetRecommendedCounter()

// Adjust difficulty dynamically
AdaptDifficulty(float playerWinRate)

// Export learning data
ExportLearningData()

// Import previous learning
ImportLearningData(LearningData data)
```

### Integration Points

**With CombatSystem:**
```csharp
// Record every player attack
if (AILearningSystem.Instance != null && isPlayerAttack)
{
    AILearningSystem.Instance.RecordPlayerAction(actionType, wasSuccessful);
}
```

**With AIBehavior:**
```csharp
// Use learned patterns to choose action
if (AILearningSystem.Instance != null)
{
    ActionType adaptedAction = AILearningSystem.Instance.GetAdaptedAction(distance);
}
```

---

## 🏆 Career Mode

**File:** `Unity/Assets/Scripts/Managers/CareerMode.cs`
**Lines:** 585
**Purpose:** Structured 20-fight progression through 4 competitive tiers

### Career Structure

#### Tier Progression
```
Amateur (Fights 1-5)
  ↓
Regional (Fights 6-10)
  ↓
Professional (Fights 11-15)
  ↓
Champion (Fights 16-20)
```

#### Fight Generation
- **Dynamic Opponents**: Procedurally generated based on tier
- **Difficulty Scaling**: 15% increase per fight
- **Special Events**:
  - **Rival Battles**: Every 7th fight (AI with learning)
  - **Boss Fights**: Tier finals (5th, 10th, 15th, 20th)
  - **Standard Matches**: Regular progression

### Rewards System

```csharp
Base Reward: 100 currency per win

Bonuses:
+ 50 per perfect round (no rounds lost)
+ 25 per combo milestone (3+ hit combo)
+ 25 per combo milestone (5+ hit combo)
+ 75 per combo milestone (10+ hit combo)

Tier Multipliers:
× 1.0 - Amateur
× 1.5 - Regional
× 2.0 - Professional
× 3.0 - Champion

Boss Fight Multiplier: × 2.0
```

### Unlockable Content

**Fighters:**
- 5 wins → Mahajanga Veteran
- 10 wins → Additional fighter
- 15 wins → Elite fighter

**Arenas:**
- Professional tier → Coastal Stadium
- Champion tier → Championship Arena

**Special Moves:**
- 10-hit combo → Ultra Combo
- Perfect victory → Advanced technique

### Career Data Structure

```csharp
public class CareerData
{
    public string fighterName;
    public int currentFight;          // 0-20
    public int wins;
    public int losses;
    public CareerTier currentTier;    // Amateur/Regional/Pro/Champion
    public int currency;
    public int reputation;
    public List<string> unlockedFighters;
    public List<string> unlockedArenas;
    public List<string> unlockedSpecialMoves;
}
```

### Key Methods

```csharp
// Start new career
StartNewCareer(string fighterName)

// Setup next fight
SetupNextFight()

// Process fight results
ProcessFightResult(bool playerWon, int roundsWon, int roundsLost, ComboStats stats)

// Save/Load career
SaveCareer()
LoadCareer(CareerData data)

// Get statistics
GetCareerStats()
```

### Integration

```csharp
// In GameManager.cs - After match ends
if (CareerMode.Instance != null && CareerMode.Instance.IsCareerActive)
{
    CareerMode.Instance.ProcessFightResult(
        playerWon: player1Score > player2Score,
        roundsWon: player1Score,
        roundsLost: player2Score,
        comboStats: comboTracker.GetStats()
    );
}
```

---

## 📊 Player Profile System

**File:** `Unity/Assets/Scripts/Core/PlayerProfile.cs`
**Lines:** 484
**Purpose:** Comprehensive player progression, statistics, and achievements

### Features

#### 1. Experience & Leveling
```csharp
// XP Formula
Required XP = (100 × Level) + (Level² × 50)

Level 1:  150 XP
Level 5:  1,750 XP
Level 10: 6,000 XP
Level 25: 33,750 XP
Level 50: 130,000 XP

// XP Rewards
Win: 100 XP
Loss: 25 XP
```

#### 2. Statistics Tracking

**Match Statistics:**
- Total matches played
- Wins / Losses
- Rounds won / lost
- Current win streak
- Longest win streak

**Combat Statistics:**
- Total hits landed
- Heavy attacks landed
- Special moves used
- Successful blocks
- Successful dodges
- Total damage dealt

**Achievement Statistics:**
- Longest combo
- Total combos
- Knockouts
- Perfect victories

#### 3. Achievements System

**15 Total Achievements:**

| ID | Title | Description | Requirement |
|---|---|---|---|
| FIRST_VICTORY | First Victory | Win your first match | 1 win |
| WIN_10 | Rising Fighter | Win 10 matches | 10 wins |
| WIN_50 | Experienced Fighter | Win 50 matches | 50 wins |
| WIN_100 | Champion | Win 100 matches | 100 wins |
| STREAK_5 | Hot Streak | Win 5 in a row | 5 streak |
| STREAK_10 | Unstoppable | Win 10 in a row | 10 streak |
| COMBO_10 | Combo Master | Land 10-hit combo | 10 combo |
| COMBO_20 | Ultra Combo | Land 20-hit combo | 20 combo |
| PERFECT_1 | Flawless | First perfect victory | 1 perfect |
| PERFECT_10 | Untouchable | 10 perfect victories | 10 perfect |
| LEVEL_10 | Dedicated | Reach level 10 | Level 10 |
| LEVEL_25 | Expert | Reach level 25 | Level 25 |
| LEVEL_50 | Master | Reach level 50 | Level 50 |

#### 4. Session Tracking
```csharp
public struct SessionData
{
    public float duration;         // Time played this session
    public int matchesPlayed;      // Matches this session
    public int matchesWon;         // Wins this session
    public float damageDealt;      // Total damage this session
    public int longestCombo;       // Best combo this session
}
```

#### 5. Auto-Save System
- Saves every 60 seconds
- Saves on application quit
- Uses PlayerPrefs for persistence

### Key Methods

```csharp
// Profile management
CreateNewProfile(string playerName)
LoadProfile()
SaveProfile()

// Statistics recording
RecordMatchResult(bool won, int roundsWon, int roundsLost)
RecordCombatStats(int hits, int heavyHits, int specials, int blocks, int dodges, float damage)
RecordCombo(int comboCount, float comboDamage)
RecordKnockout(bool isPerfect)

// Progression
AddExperience(int amount)
GetLevelProgress()

// Session
GetCurrentSession()
GetStatsSummary()

// Queries
GetWinRate()
GetAchievementProgress()
```

### Integration

```csharp
// Record match result
if (PlayerProfile.Instance != null)
{
    PlayerProfile.Instance.RecordMatchResult(
        playerWon,
        roundsWon,
        roundsLost
    );
}

// Record combat stats
if (PlayerProfile.Instance != null && isPlayer)
{
    PlayerProfile.Instance.RecordCombatStats(
        hits: 1,
        heavyHits: isHeavy ? 1 : 0,
        specials: isSpecial ? 1 : 0,
        blocks: 0,
        dodges: 0,
        damage: damageDealt
    );
}
```

---

## 🥊 Rival AI System

**File:** `Unity/Assets/Scripts/AI/RivalAI.cs`
**Lines:** 416
**Purpose:** Evolving AI opponent that grows with the player

### Evolution System

#### 5 Evolution Stages

**Stage 0: Basic Rival**
- Standard AI behavior
- No special abilities

**Stage 1: Combo Learner** (3 matches)
- Unlocks advanced combo usage
- `canUseAdvancedCombos = true`

**Stage 2: Tactical Predictor** (6 matches)
- Can predict player moves
- `canPredictMoves = true`

**Stage 3: Counter Master** (9 matches)
- Perfect counter timing
- `canPerfectCounter = true`

**Stage 4+: Adaptive Expert** (12+ matches)
- Real-time adaptation
- `canAdaptInRealTime = true`

### Leveling System

```csharp
// Evolution rate: 0.05 per match
// Bonus XP for wins: 1.5x
// Bonus XP for losses: 1.0x

// Level-up stat boost: 5% all stats
Power   += 5%
Speed   += 5%
Defense += 5%
Stamina += 5%
Technique += 5%
```

### Style Adaptation

**Player Style Profiling:**
```csharp
public class PlayerStyleProfile
{
    public float aggressionLevel;    // How aggressive is player
    public float defenseLevel;       // How defensive is player
    public float comboUsage;         // Combo frequency
    public float specialUsage;       // Special move usage
    public float movementPattern;    // Movement style
}
```

**Personality Types:**
- **Adaptive**: Learns and adapts (default)
- **Aggressive**: Always attacks
- **Tactical**: Patient and strategic
- **Mimic**: Copies player style
- **Unpredictable**: Random chaos

### Taunting System

**Context-Aware Taunts:**

**Opening Taunts:**
- First match: "Let's see what you've got!"
- Rival leading: "I'm still the better fighter!"
- Player leading: "This time will be different!"
- Tied record: "We're evenly matched... for now!"

**Victory Taunts:**
- "I've learned from our battles!"
- "You'll have to do better than that!"
- "This is what evolution looks like!"
- "I adapt faster than you can improve!"

**Defeat Taunts:**
- "I'll remember this..."
- "You got lucky this time!"
- "I'm learning from this defeat!"
- "Next time, I'll be stronger!"

### Persistence

```csharp
public class RivalData
{
    public string name;
    public int level;
    public int matchesPlayed;
    public int wins;
    public int losses;
    public int evolutionStage;

    // Unlocked abilities
    public bool canUseAdvancedCombos;
    public bool canPredictMoves;
    public bool canPerfectCounter;
    public bool canAdaptInRealTime;
}
```

### Key Methods

```csharp
// Match lifecycle
OnMatchStart()
OnMatchEnd(bool rivalWon)
OnRoundEnd(bool rivalWonRound)

// Evolution
EvolveRival(bool won)
LevelUpRival()
TriggerEvolution()

// Adaptation
AnalyzePlayerStyle()
AdaptToPlayerStyle()
ApplyCounterStrategy(CounterStrategy counter)

// Taunts
SendOpeningTaunt()
SendVictoryTaunt()
SendDefeatTaunt()

// Persistence
SaveRivalData()
LoadRivalData()
ResetRival()

// Statistics
GetRivalStats()
```

### Integration

```csharp
// In GameManager.cs

// Match start
RivalAI rival = FindObjectOfType<RivalAI>();
if (rival != null)
{
    rival.OnMatchStart();

    // Subscribe to taunts
    rival.OnRivalTaunt += (taunt) => {
        RoundAnnouncer.Instance?.AnnounceRival(taunt);
    };
}

// Match end
if (rival != null)
{
    bool rivalWon = player2Score > player1Score;
    rival.OnMatchEnd(rivalWon);
}

// Round end
if (rival != null)
{
    rival.OnRoundEnd(rivalWonRound);
}
```

---

## 🔗 System Integration

### How Systems Work Together

```
Player Action
    ↓
AI Learning System (records pattern)
    ↓
Combat System (processes hit)
    ↓
Player Profile (records stats)
    ↓
Career Mode (tracks progress)
    ↓
Rival AI (learns & evolves)
```

### Data Flow

**During Combat:**
1. Player attacks → CombatSystem processes
2. AILearningSystem records action & success
3. PlayerProfile updates combat stats
4. ComboTracker monitors combo
5. RivalAI adapts strategy in real-time

**After Round:**
1. GameManager determines round winner
2. RivalAI.OnRoundEnd() called
3. Stats accumulated

**After Match:**
1. GameManager calculates match result
2. PlayerProfile.RecordMatchResult()
3. CareerMode.ProcessFightResult() (if in career)
4. RivalAI.OnMatchEnd() (if vs rival)
5. AILearningSystem.AdaptDifficulty()
6. Auto-save triggered

### Event Chain

```csharp
// Match flow with all systems
GameManager.StartMatch()
    → CareerMode.SetupNextFight()
    → RivalAI.OnMatchStart()
    → PlayerProfile starts session tracking

GameManager.ProcessCombat()
    → CombatSystem.ProcessHit()
    → AILearningSystem.RecordPlayerAction()
    → PlayerProfile.RecordCombatStats()
    → ComboTracker.RegisterHit()

GameManager.EndRound()
    → RivalAI.OnRoundEnd()

GameManager.EndMatch()
    → PlayerProfile.RecordMatchResult()
    → CareerMode.ProcessFightResult()
    → RivalAI.OnMatchEnd()
    → AILearningSystem.AdaptDifficulty()
    → Auto-save all systems
```

---

## 💡 Usage Examples

### Example 1: Starting Career Mode

```csharp
using Morengy.Managers;

public class CareerMenuController : MonoBehaviour
{
    public void OnStartCareerClicked(string fighterName)
    {
        if (CareerMode.Instance != null)
        {
            // Start new career
            CareerMode.Instance.StartNewCareer(fighterName);

            // Subscribe to career events
            CareerMode.Instance.OnFightStarted += OnFightReady;
            CareerMode.Instance.OnFightCompleted += OnFightFinished;
            CareerMode.Instance.OnUnlockEarned += ShowUnlockNotification;

            // Load fight scene
            SceneManager.LoadScene("FightScene");
        }
    }

    private void OnFightReady(FightSetup fight)
    {
        Debug.Log($"Next opponent: {fight.opponentName}");
        Debug.Log($"Difficulty: {fight.difficulty}");
        Debug.Log($"Special: {fight.specialConditions}");
    }

    private void OnFightFinished(FightResult result)
    {
        if (result.playerWon)
        {
            Debug.Log($"Victory! Earned {result.currencyEarned} currency");
        }
        else
        {
            Debug.Log("Defeat. Try again!");
        }
    }

    private void ShowUnlockNotification(string unlock)
    {
        Debug.Log($"🎉 Unlocked: {unlock}");
        // Show UI notification
    }
}
```

### Example 2: Displaying Player Stats

```csharp
using Morengy.Core;
using UnityEngine.UI;

public class ProfileUI : MonoBehaviour
{
    [SerializeField] private Text playerNameText;
    [SerializeField] private Text levelText;
    [SerializeField] private Slider levelProgressBar;
    [SerializeField] private Text statsText;
    [SerializeField] private Text achievementsText;

    private void Start()
    {
        if (PlayerProfile.Instance != null)
        {
            UpdateUI();

            // Subscribe to level up
            PlayerProfile.Instance.OnLevelUp += (level) => {
                Debug.Log($"Leveled up to {level}!");
                UpdateUI();
            };
        }
    }

    private void UpdateUI()
    {
        var profile = PlayerProfile.Instance.Profile;

        playerNameText.text = profile.playerName;
        levelText.text = $"Level {profile.level}";
        levelProgressBar.value = PlayerProfile.Instance.GetLevelProgress();
        statsText.text = PlayerProfile.Instance.GetStatsSummary();

        int totalAchievements = 15;
        int unlocked = profile.achievements.Count;
        achievementsText.text = $"Achievements: {unlocked}/{totalAchievements}";
    }
}
```

### Example 3: Setting Up Rival Fight

```csharp
using Morengy.AI;

public class RivalFightSetup : MonoBehaviour
{
    [SerializeField] private GameObject rivalPrefab;

    private void SetupRivalBattle()
    {
        // Spawn rival
        GameObject rivalObj = Instantiate(rivalPrefab, spawnPoint, Quaternion.identity);
        RivalAI rival = rivalObj.GetComponent<RivalAI>();

        if (rival != null)
        {
            // Configure rival
            rival.OnMatchStart();

            // Subscribe to events
            rival.OnRivalTaunt += DisplayTaunt;
            rival.OnRivalEvolved += ShowEvolutionCutscene;

            // Get rival stats for UI
            var stats = rival.GetRivalStats();
            Debug.Log($"Facing: {stats.Name} (Level {stats.Level})");
            Debug.Log($"Record: {stats.Wins}W-{stats.Losses}L");
            Debug.Log($"Evolution Stage: {stats.EvolutionStage}");
        }
    }

    private void DisplayTaunt(string taunt)
    {
        // Show taunt in UI
        if (UI.RoundAnnouncer.Instance != null)
        {
            UI.RoundAnnouncer.Instance.AnnounceRival(taunt);
        }
    }

    private void ShowEvolutionCutscene(RivalData data)
    {
        // Play evolution animation/cutscene
        Debug.Log($"Rival evolved to Stage {data.evolutionStage}!");
    }
}
```

### Example 4: AI Learning Integration

```csharp
using Morengy.AI;
using Morengy.Combat;

public class CombatIntegration : MonoBehaviour
{
    private void ProcessPlayerAttack(AttackType attackType, bool hitSuccessful)
    {
        // Record action for AI learning
        if (AILearningSystem.Instance != null)
        {
            ActionType action = attackType switch
            {
                AttackType.Light => ActionType.LightAttack,
                AttackType.Heavy => ActionType.HeavyAttack,
                AttackType.Special => ActionType.SpecialAttack,
                _ => ActionType.Aggressive
            };

            AILearningSystem.Instance.RecordPlayerAction(action, hitSuccessful);

            // Get learning stats for debug
            string stats = AILearningSystem.Instance.GetLearningStats();
            Debug.Log(stats);
        }
    }

    private void AIChooseAction(float distanceToPlayer)
    {
        // Get AI's learned action
        if (AILearningSystem.Instance != null)
        {
            ActionType adaptedAction = AILearningSystem.Instance.GetAdaptedAction(distanceToPlayer);

            // Use adapted action for AI decision
            ExecuteAIAction(adaptedAction);
        }
    }
}
```

---

## 📈 Performance Considerations

### Memory Usage
- **AI Learning**: ~50 action records × 24 bytes = ~1.2KB per instance
- **Player Profile**: ~500 bytes for core data
- **Career Data**: ~300 bytes + unlocks list
- **Rival Data**: ~200 bytes per rival

### CPU Impact
- **Pattern Analysis**: O(n) where n = pattern length (default 50)
- **Statistics Update**: O(1) per event
- **Auto-Save**: Runs on 60-second timer, minimal impact

### Optimization Tips
1. Adjust `patternRecordLimit` in AILearningSystem if memory constrained
2. Reduce auto-save frequency if needed
3. Use object pooling for UI notifications
4. Cache frequently accessed statistics

---

## 🔧 Configuration

### AI Learning Settings
```csharp
[Header("Learning Parameters")]
[SerializeField] private int patternRecordLimit = 50;
[SerializeField] private int patternLength = 5;
[SerializeField] private float confidenceThreshold = 0.6f;
```

### Career Mode Settings
```csharp
[Header("Career Settings")]
[SerializeField] private int totalCareerFights = 20;
[SerializeField] private int fightsPerTier = 5;
[SerializeField] private float difficultyIncreaseRate = 0.15f;
```

### Player Profile Settings
```csharp
[Header("Profile Settings")]
[SerializeField] private bool autoSave = true;
[SerializeField] private float autoSaveInterval = 60f;
```

### Rival AI Settings
```csharp
[Header("Rival Settings")]
[SerializeField] private bool enableEvolution = true;
[SerializeField] private float evolutionRate = 0.05f;
[SerializeField] private RivalPersonality personality = RivalPersonality.Adaptive;
```

---

## 🐛 Troubleshooting

### AI Not Learning
- Check AILearningSystem.Instance is not null
- Verify RecordPlayerAction() is being called
- Check console for pattern detection logs

### Career Progress Not Saving
- Ensure CareerMode.SaveCareer() is called after matches
- Check PlayerPrefs permissions
- Verify CareerData serialization

### Achievements Not Unlocking
- Check achievement conditions in PlayerProfile.cs
- Verify stats are being recorded correctly
- Check OnAchievementUnlocked event subscription

### Rival Not Evolving
- Ensure RivalAI.OnMatchEnd() is called
- Check evolutionRate and matchesPlayed
- Verify rememberPreviousMatches is enabled

---

## 📚 API Reference

See [INTEGRATION_GUIDE.md](Unity/INTEGRATION_GUIDE.md) for complete integration instructions.

---

**Total Implementation:** 1,964 lines of code
**Systems:** 4 major advanced systems
**Integration Points:** 12+ connection points with existing systems

🥊 **All systems tested and ready for integration!**
