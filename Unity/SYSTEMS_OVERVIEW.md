# 🎮 Morengy Game - Systems Overview

**Quick Reference for All 19 Game Systems**

---

## 📊 System Categories

### 1️⃣ Character Systems (2 scripts, 900 lines)

#### FighterController.cs
**Purpose:** Character movement, dodging, knockback
**Key Methods:**
- `HandleMovement()` - WASD/Arrow key movement
- `PerformDodge(Vector2 direction)` - Dodge rolls with stamina cost
- `ApplyKnockback(Vector3 force)` - Physics-based knockback

**Stats:**
- Movement Speed: 5 m/s
- Dodge Cost: 20 stamina
- Dodge Cooldown: 1 second

#### FighterStats.cs
**Purpose:** Health, stamina, stats management
**Key Methods:**
- `TakeDamage(float damage, bool isCritical)` - Apply damage with defense
- `DrainStamina(float amount)` - Reduce stamina
- `BuildSpecialMeter(float amount)` - Fill special meter

**Stats:**
- Health: 100 HP
- Stamina: 100 (regens 10/sec)
- Special Meter: 0-100
- 6 Core Stats: Power, Speed, Defense, Stamina, Technique, Charisma

**Events:**
- `OnHealthChanged(float current, float max)`
- `OnStaminaChanged(float current, float max)`
- `OnSpecialMeterChanged(float current)`
- `OnDeath()`

---

### 2️⃣ Combat Systems (2 scripts, 700 lines)

#### CombatSystem.cs
**Purpose:** Attack processing, blocking, damage calculation
**Key Methods:**
- `PerformAttack(AttackType type)` - Execute attack
- `PerformBlock()` - Raise guard
- `ProcessHit(attacker, target, attackType)` - Damage calculation

**Attack Types:**
| Type | Damage | Stamina | Special Gain | Speed |
|------|--------|---------|--------------|-------|
| Light | 10 | 5 | 5 | Fast |
| Heavy | 25 | 15 | 10 | Slow |
| Special | 40 | 30 | 0 | Medium |

**Damage Formula:**
```
Final = Base × (1 - Defense%) × StaminaMod × ComboMult
```

#### ComboTracker.cs
**Purpose:** Track combo chains and milestones
**Key Methods:**
- `RegisterHit()` - Add hit to combo
- `BreakCombo()` - Reset combo
- `GetStats()` - Get combo statistics

**Milestones:**
- 3 hits → "Great Combo!"
- 5 hits → "Amazing Combo!"
- 10 hits → "ULTRA COMBO!"

---

### 3️⃣ AI Systems (3 scripts, 1,445 lines)

#### AIBehavior.cs
**Purpose:** AI decision making and behavior
**Difficulties:**
- Easy: 0.6-1.0s reaction time
- Medium: 0.3-0.6s reaction time
- Hard: 0.2-0.4s reaction time
- Expert: 0.1-0.2s reaction time

**Personalities:**
- Balanced: Well-rounded
- Brawler: Aggressive heavy attacks
- Tactician: Defensive and patient
- Showman: Flashy combos
- Technical: Precise and methodical

#### AILearningSystem.cs
**Purpose:** Pattern recognition and adaptive difficulty
**Features:**
- Tracks 50 recent player actions
- Identifies 6 attack patterns
- 6 counter strategies
- Dynamic difficulty adjustment

**Patterns Detected:**
- LightCombo, MixedCombo, PowerCombo
- EvasiveStyle, DefensiveStyle, Aggressive

**Counter Strategies:**
- PerfectBlock, Dodge, Interrupt, Predict, Grab, Counter

#### RivalAI.cs
**Purpose:** Evolving AI opponent
**Evolution Stages:**
0. Basic (start)
1. Combo Learner (3 matches)
2. Tactical Predictor (6 matches)
3. Counter Master (9 matches)
4. Adaptive Expert (12+ matches)

**Level-up Bonus:** 5% all stats per level

---

### 4️⃣ Manager Systems (3 scripts, 1,435 lines)

#### GameManager.cs
**Purpose:** Match flow, rounds, win conditions
**Match Structure:**
- Best of 3 rounds
- 2 minutes per round
- Win conditions: KO, TKO, Time Decision

**Key Methods:**
- `StartMatch()` - Initialize fight
- `StartRound()` - Begin round
- `EndRound(winner)` - Process round result
- `EndMatch()` - Determine match winner

**Events:**
- `OnMatchStart()`, `OnMatchEnd(winner)`
- `OnRoundStart(roundNum)`, `OnRoundEnd(winner)`
- `OnCountdown(seconds)`

#### AudioManager.cs
**Purpose:** Music and sound effects
**Features:**
- Music with fade transitions
- SFX pooling (10 audio sources)
- Dynamic intensity system
- Volume controls (master, music, SFX, ambience)

**Key Methods:**
- `PlayMusic(clip, fadeTime)` - Play background music
- `PlaySFX(clip, volume, pitch)` - Play sound effect
- `PlayImpactSound(force)` - Dynamic impact sounds
- `SetMusicIntensity(level)` - Adjust music intensity

#### CareerMode.cs
**Purpose:** 20-fight career progression
**Structure:**
- 4 tiers: Amateur → Regional → Professional → Champion
- 5 fights per tier
- Boss fights at tier finals
- Rival fights every 7th fight

**Rewards:**
- Base: 100 currency per win
- Bonus: +50 per perfect round, +25 per combo milestone
- Tier Multipliers: 1.0x → 1.5x → 2.0x → 3.0x

**Unlocks:**
- Fighters (at 5, 10, 15 wins)
- Arenas (Professional, Champion tiers)
- Special Moves (10-hit combo, perfect victories)

---

### 5️⃣ UI Systems (4 scripts, 1,300 lines)

#### FighterHUD.cs
**Purpose:** Health/stamina/special meter display
**Features:**
- Color-coded health bars (green→yellow→red)
- Delayed damage visualization
- Stamina bars with color transitions
- Special meter with ready indicator
- Animated updates

#### RoundAnnouncer.cs
**Purpose:** Game announcements and countdowns
**Announcement Types:**
- Round start: "ROUND 1 - FIGHT!"
- Round end: "PLAYER 1 WINS!"
- Match end: "VICTORY!" / "DEFEAT!"
- Events: "ULTRA COMBO!", "PERFECT!"
- Countdowns: "3... 2... 1... FIGHT!"

#### DamagePopup.cs
**Purpose:** Floating damage numbers
**Features:**
- Animated upward movement
- Critical hits display "CRIT!" with 1.5x scale
- Color-coded: White (normal), Yellow (crit), Red (blocked)
- Auto-destroy after 1 second

#### PauseMenu.cs
**Purpose:** Pause system and settings
**Features:**
- ESC key pause/resume
- TimeScale management
- Settings panel (volume controls)
- PlayerPrefs persistence

---

### 6️⃣ Core Systems (5 scripts, 1,084 lines)

#### InputManager.cs
**Purpose:** Player input handling
**Controls:**
- Movement: WASD / Arrow Keys
- Light Attack: J / Numpad1
- Heavy Attack: K / Numpad2
- Special Attack: L / Numpad3
- Block: I / Numpad4
- Dodge: Space

#### VFXManager.cs
**Purpose:** Particle effects pooling
**Effect Types:**
- Hit effects (light, heavy, special)
- Movement effects (dodge, dash)
- Special effects (ultimate, power-up)
- Screen shake integration

**Pool Size:** 20 per effect type

#### FightingCameraController.cs
**Purpose:** Dynamic camera system
**Features:**
- Follows both fighters
- Dynamic zoom based on distance
- Smooth camera movement
- Impact shake effects
- Configurable bounds

#### FighterData.cs
**Purpose:** ScriptableObject fighter presets
**Properties:**
- Fighter name, description, region
- Base stats (Power, Speed, Defense, etc.)
- Special moves list
- Unlock requirements

**Presets Created:**
- Diego Warrior
- Nosy Be Champion
- Mahajanga Veteran
- Antananarivo Technician

#### PlayerProfile.cs
**Purpose:** Stats, achievements, progression
**Features:**
- XP and leveling system
- 15 achievements
- Match/combat statistics
- Session tracking
- Auto-save (60s interval)

**XP Formula:** `100 × level + level² × 50`

**Statistics Tracked:**
- Matches: total, wins, losses, streaks
- Combat: hits, blocks, dodges, damage
- Achievements: combos, knockouts, perfects

---

## 🔗 System Integration Flow

```
Player Input
    ↓
InputManager → FighterController
    ↓
CombatSystem.PerformAttack()
    ↓
├─→ CombatSystem.ProcessHit()
│   ├─→ FighterStats.TakeDamage()
│   ├─→ ComboTracker.RegisterHit()
│   ├─→ VFXManager.PlayHitEffect()
│   ├─→ AudioManager.PlaySFX()
│   ├─→ DamagePopup.Spawn()
│   └─→ AILearningSystem.RecordAction()
│
├─→ FighterHUD.UpdateDisplay()
├─→ PlayerProfile.RecordStats()
└─→ GameManager checks win conditions
        ↓
    GameManager.EndRound()
        ↓
    RoundAnnouncer.AnnounceWinner()
        ↓
    GameManager.EndMatch()
        ↓
    ├─→ CareerMode.ProcessResult()
    ├─→ PlayerProfile.RecordMatch()
    ├─→ RivalAI.OnMatchEnd()
    └─→ AILearningSystem.AdaptDifficulty()
```

---

## 📈 Key Formulas

### Damage Calculation
```csharp
float defenseReduction = 1f - (target.Defense * 0.3f);
float staminaMod = attacker.Stamina > 30 ? 1f : 0.7f;
float comboMult = 1f + (comboCount * 0.1f); // Max 2x at 10 hits

float finalDamage = baseDamage * defenseReduction * staminaMod * comboMult;
```

### XP Required for Level
```csharp
int requiredXP = (100 * level) + (level * level * 50);
```

### Career Fight Difficulty
```csharp
float difficulty = 1f + (fightNumber * 0.15f); // 15% increase per fight
```

### Combo Timeout
```csharp
float timeout = 2.0f; // 2 seconds between hits
```

---

## 🎯 Quick Setup Guide

### 1. Create Empty GameObjects

```
Scene Hierarchy:
├── GameManager (GameManager.cs)
├── AudioManager (AudioManager.cs)
├── CareerMode (CareerMode.cs)
├── PlayerProfile (PlayerProfile.cs)
├── VFXManager (VFXManager.cs)
├── AILearningSystem (AILearningSystem.cs)
│
├── Player1
│   ├── FighterController.cs
│   ├── FighterStats.cs
│   ├── CombatSystem.cs
│   ├── ComboTracker.cs
│   └── InputManager.cs
│
├── Player2 (AI)
│   ├── FighterController.cs
│   ├── FighterStats.cs
│   ├── CombatSystem.cs
│   ├── ComboTracker.cs
│   ├── AIBehavior.cs
│   └── RivalAI.cs (optional)
│
├── UI
│   ├── Canvas
│   │   ├── Player1HUD (FighterHUD.cs)
│   │   ├── Player2HUD (FighterHUD.cs)
│   │   ├── RoundAnnouncer (RoundAnnouncer.cs)
│   │   └── PauseMenu (PauseMenu.cs)
│   └── DamagePopupContainer
│
└── Camera (FightingCameraController.cs)
```

### 2. Assign References

**GameManager:**
- Assign both fighters
- Assign UI components

**AudioManager:**
- Add music clips
- Add SFX clips

**FighterHUD:**
- Assign FighterStats reference
- Assign UI elements (health bar, stamina bar, etc.)

**CombatSystem:**
- Assign opponent FighterStats
- Assign own FighterStats
- Link to VFXManager, AudioManager

### 3. Configure Settings

**In Inspector:**
- Set AI difficulty (Easy/Medium/Hard/Expert)
- Choose AI personality
- Configure career mode settings
- Enable auto-save for profile
- Adjust camera bounds

### 4. Test

Press Play and verify:
- ✅ Movement works (WASD)
- ✅ Attacks land (J, K, L)
- ✅ Blocking works (I)
- ✅ AI responds
- ✅ HUD updates
- ✅ Combos count
- ✅ Sounds play
- ✅ Effects appear

---

## 🐛 Common Issues

### "NullReferenceException"
- Check all public references are assigned
- Verify Instance properties exist for singletons

### "AI not moving"
- Check AIBehavior is enabled
- Verify opponent reference is set
- Check AI difficulty isn't set to 0

### "HUD not updating"
- Ensure FighterStats events are subscribed
- Check FighterHUD references are assigned

### "No sound"
- Verify AudioManager has clips assigned
- Check volume settings aren't at 0
- Ensure AudioListener exists in scene

### "Career not saving"
- Check CareerMode.SaveCareer() is called
- Verify PlayerPrefs has write permissions

---

## 📚 Documentation Files

- **SETUP_GUIDE.md** - Unity setup (5 minutes)
- **INTEGRATION_GUIDE.md** - System integration with code
- **QUICK_REFERENCE.md** - Controls and formulas
- **ADVANCED_SYSTEMS.md** - Advanced features (420 lines)
- **SYSTEMS_OVERVIEW.md** - This file

---

## 🎮 Quick Commands

```csharp
// Start career mode
CareerMode.Instance.StartNewCareer("PlayerName");

// Get player stats
var stats = PlayerProfile.Instance.GetStatsSummary();

// Reset rival
RivalAI rival = FindObjectOfType<RivalAI>();
rival.ResetRival();

// Get learning stats
string aiStats = AILearningSystem.Instance.GetLearningStats();

// Play announcement
RoundAnnouncer.Instance.AnnounceRoundStart(1);

// Spawn damage popup
DamagePopup.Spawn(damage, position, isCritical);
```

---

**🥊 All 19 systems ready to fight! 🥊**

*See INTEGRATION_GUIDE.md for complete code examples*
