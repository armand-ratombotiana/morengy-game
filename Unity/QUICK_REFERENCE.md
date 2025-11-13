# 🎮 MORENGY - Quick Reference Guide

Quick lookup for implemented systems, controls, and key information.

---

## 🎯 Player Controls (Default)

### Movement
- **WASD / Arrow Keys** - Move fighter
- **Shift** - Sprint (drains stamina)
- **C / Left Ctrl** - Crouch

### Combat
- **J** - Light Attack (fast, low damage)
- **K** - Heavy Attack (slow, high damage)
- **L** - Special Attack (requires full meter)
- **I** - Block (hold to maintain)
- **Space** - Dodge Roll (i-frames)

### Utility
- **R** - Rest Stance (fast stamina regen, vulnerable)
- **T** - Taunt (gain special meter)

### Alternative Controls
- **LMB** - Light Attack
- **RMB** - Heavy Attack
- **Left Ctrl** - Block

---

## 📊 Fighter Stats Explained

### Core Attributes (1-100)

| Stat | Effect |
|------|--------|
| **Power** | Increases damage output by up to 50% |
| **Speed** | Increases movement and attack speed by up to 50% |
| **Defense** | Reduces incoming damage by up to 30% |
| **Stamina** | Increases stamina pool size |
| **Technique** | Improves combo damage and critical hit chance |
| **Charisma** | Faster special meter gain from crowd |

### Health System
- **Max Health:** 100
- **No regeneration** - damage is permanent per round
- **Critical Health:** <15% (triggers comeback mechanics)
- **KO Condition:** Health reaches 0

### Stamina System
- **Max Stamina:** 100 + (Stamina Stat / 2)
- **Passive Regen:** 2% per second
- **Fast Regen:** 5% per second (when resting)
- **Exhausted:** <10% (cannot use heavy/special attacks)

**Stamina Costs:**
- Light Attack: 5%
- Heavy Attack: 15%
- Special Attack: 22.5%
- Dodge: 8%
- Blocking: 2% per second
- Sprinting: 3% per second

### Special Meter
- **Max:** 100%
- **Gain Sources:**
  - Landing attacks: +5%
  - Taking damage: +3%
  - Taunts/crowd: +2%
- **Usage:** Special moves cost 100%

---

## ⚔️ Combat Mechanics

### Attack Types

| Attack | Damage | Speed | Stamina | Recovery |
|--------|--------|-------|---------|----------|
| **Light** | 10 | 0.3s | 5% | 0.4s |
| **Heavy** | 25 | 0.8s | 15% | 0.4s |
| **Special** | 40 | 1.2s | 22.5% | 0.6s |

### Combo System
- **Combo Window:** 0.5s between attacks
- **Max Combo:** 5 hits
- **Damage Bonus:** +10% per combo hit (max +50%)
- **Common Chains:**
  - Light → Light → Heavy
  - Heavy → Special
  - Parry → Auto Combo

### Blocking
- **Damage Reduction:** 50%
- **Perfect Block Window:** 0.2s (first frames)
- **Perfect Block Bonus:** 30% stamina restore
- **Stamina Drain:** 2% per second while blocking

### Dodging
- **Duration:** 0.4s
- **Invincibility Frames:** First 60% (0.24s)
- **Distance:** 3 meters
- **Cooldown:** None (limited by stamina)

### Critical Hits
- **Base Chance:** 5%
- **Damage Multiplier:** 1.75x
- **Bonuses:**
  - +10% chance from Technique stat (at 100)
  - +15% when health <15% (comeback mechanic)
  - 100% on successful parry/counter

### Damage Formula
```
Base Damage = Attack Power × Type Multiplier
Type Multiplier = 1.0 (Light), 2.5 (Heavy), 4.0 (Special)

Final Damage = Base × Fighter Power × Stamina Modifier × Combo Multiplier × (1 - Defense)

Stamina Modifier:
- >50% stamina = 1.0x damage
- 20-50% stamina = 0.8x damage
- <20% stamina = 0.6x damage

Combo Multiplier = 1.0 + (0.1 × Combo Count) [Max: 1.5]
```

---

## 🤖 AI Behavior

### Difficulty Levels

| Difficulty | Reaction Time | Block Rate | Combo Focus | Description |
|------------|---------------|------------|-------------|-------------|
| **Easy** | 0.6-1.0s | 20% | Low | Predictable, beginner-friendly |
| **Medium** | 0.3-0.6s | 40% | Medium | Balanced challenge |
| **Hard** | 0.15-0.4s | 60% | High | Adaptive, skilled |
| **Expert** | 0.1-0.2s | 85% | Very High | Near-perfect execution |

### AI Personalities

| Personality | Playstyle | Range | Special Focus |
|-------------|-----------|-------|---------------|
| **Balanced** | All-around | 2.5m | Normal |
| **Brawler** | Aggressive close-range | 1.8m | High aggression |
| **Tactician** | Defensive counter-fighter | 3.0m | High defense |
| **Showman** | Special move focused | 2.5m | High special usage |
| **Technical** | Combo specialist | 2.5m | High combo focus |

### AI States
- **Observing** - Neutral positioning, looking for openings
- **Aggressive** - Pressuring with attacks
- **Defensive** - Blocking and counter-attacking
- **Retreating** - Low health escape
- **Recovering** - Stamina regeneration

---

## 🎬 Animation States (For Reference)

### Required Animations
```
Locomotion:
- Idle
- Walk Forward
- Walk Backward
- Strafe Left/Right
- Sprint
- Crouch Idle
- Crouch Walk

Combat:
- Light Attack 1-3 (combo chain)
- Heavy Punch
- Heavy Kick
- Flying Knee
- Block Idle
- Block Impact
- Dodge Roll (4 directions)

Reactions:
- Hit Light
- Hit Heavy
- Stunned
- Knockdown
- Get Up
- KO

Special:
- Taunt 1-3
- Special Move (region specific)
- Victory Pose
- Defeat Pose
```

### Animation Events (Add in Unity)
- **HitFrame** - Enable hit detection
- **HitEnd** - Disable hit detection
- **AttackEnd** - Return to idle
- **PlaySound** - Trigger audio (whoosh, impact)
- **CameraShake** - Trigger screen shake

---

## 🔧 Unity Component Setup

### Fighter Setup (Required Components)

#### Player Fighter
```
GameObject: "Player"
├── Character Controller (Unity built-in)
├── FighterController (custom)
├── FighterStats (custom)
├── CombatSystem (custom)
├── InputManager (custom)
└── Animator (Unity built-in)
    └── Child: Fighter Model
```

#### AI Fighter
```
GameObject: "AI Opponent"
├── Character Controller (Unity built-in)
├── FighterController (custom)
├── FighterStats (custom)
├── CombatSystem (custom)
├── AIBehavior (custom)
└── Animator (Unity built-in)
    └── Child: Fighter Model
```

### Inspector Settings Quick Config

**Character Controller:**
- Center: (0, 1, 0)
- Radius: 0.4
- Height: 1.8
- Skin Width: 0.08

**FighterController:**
- Walk Speed: 3
- Sprint Speed: 5
- Dodge Distance: 3
- Rotation Speed: 10

**FighterStats:**
- Max Health: 100
- Stamina: 50-70 (depends on fighter)
- Power/Speed/Defense: 40-80 (balanced)

**CombatSystem:**
- Strike Range: 1.5
- Light Attack Damage: 10
- Heavy Attack Damage: 25
- Hit Layer: "Fighter"

**AIBehavior:**
- Difficulty: Medium (for testing)
- Personality: Balanced
- Optimal Range: 2.5

---

## 🐛 Common Issues & Solutions

### Issue: Attacks Not Hitting
**Solutions:**
- Check "Hit Layer" matches opponent's layer
- Verify strike point position (should be at fist)
- Check strike range (increase if needed)
- Ensure both fighters have colliders

### Issue: AI Not Moving
**Solutions:**
- Assign opponent reference in AIBehavior
- Set FighterController.Opponent
- Check ground layer mask

### Issue: Stamina Draining Too Fast
**Solutions:**
- Reduce stamina costs in FighterStats
- Increase stamina regen rate
- Adjust fighter Stamina stat (higher = larger pool)

### Issue: Controls Not Working
**Solutions:**
- Check InputManager is enabled
- Verify key bindings in Inspector
- Ensure InputManager has references to controller and combat system

### Issue: Animation Not Playing
**Solutions:**
- Verify Animator Controller is assigned
- Check animation triggers match code
- Ensure animation clips are imported

---

## 📝 Script Reference

### Key Public Methods

**FighterController:**
- `SetMoveInput(Vector2)` - Set movement direction
- `PerformDodge(Vector2)` - Execute dodge roll
- `SetState(FighterState)` - Force state change

**FighterStats:**
- `TakeDamage(float, bool)` - Apply damage
- `DrainStamina(float)` - Use stamina
- `GainSpecialMeter(float)` - Add to special meter
- `ResetForNewMatch()` - Reset all stats

**CombatSystem:**
- `PerformLightAttack()` - Execute light attack
- `PerformHeavyAttack()` - Execute heavy attack
- `PerformSpecialAttack()` - Execute special (requires meter)
- `StartBlocking()` - Begin blocking
- `StopBlocking()` - End blocking

**AIBehavior:**
- `SetDifficulty(AIDifficulty)` - Change difficulty
- `SetPersonality(AIPersonality)` - Change personality
- `SetOpponent(Transform)` - Set target

---

## 🎨 Visual Debugging

### Gizmos (Editor Only)

**FighterController** (Selected):
- Yellow sphere: Ground check
- Blue ray: Facing direction
- Green sphere: Dodge range

**CombatSystem** (Selected):
- Yellow/Red sphere: Strike range (red when attacking)
- Blue ray: Strike direction

### Debug Console Commands

**In FighterStats Context Menu:**
- Reset All Stats
- Level Up
- Take 25 Damage
- Fill Special Meter

---

## 📊 Balance Guidelines

### Fighter Stat Templates

**Balanced Fighter:**
```
Power: 50 | Speed: 50 | Defense: 50
Stamina: 50 | Technique: 50 | Charisma: 50
```

**Power Fighter (Brawler):**
```
Power: 80 | Speed: 35 | Defense: 60
Stamina: 70 | Technique: 40 | Charisma: 60
```

**Speed Fighter (Technical):**
```
Power: 40 | Speed: 85 | Defense: 45
Stamina: 55 | Technique: 75 | Charisma: 55
```

**Tank Fighter (Defensive):**
```
Power: 60 | Speed: 40 | Defense: 85
Stamina: 80 | Technique: 50 | Charisma: 40
```

### Testing Checklist
- [ ] Can defeat Easy AI consistently
- [ ] Medium AI provides challenge
- [ ] Hard AI is difficult but fair
- [ ] No infinite combos possible
- [ ] All attacks connect at correct range
- [ ] Blocking feels responsive
- [ ] Dodging has clear i-frames
- [ ] Special moves feel powerful but balanced

---

## 🚀 Next Steps

### Immediate (This Week)
1. Test combat with placeholder cubes
2. Set up two fighters in a scene
3. Verify all attacks work
4. Test AI on different difficulties

### Short Term (Next Month)
1. Add basic 3D character models
2. Integrate animations
3. Create first arena
4. Add sound effects

### Medium Term (2-3 Months)
1. Complete fighter roster (6-8 fighters)
2. Add 3-4 arenas
3. Implement UI and menus
4. Create story mode basics

---

## 📚 Additional Resources

- **Full GDD:** [Docs/GDD/CoreCombatSystem.md](../Docs/GDD/CoreCombatSystem.md)
- **Unity Setup:** [Unity/README.md](README.md)
- **Development Guide:** [GAME_DEVELOPMENT_GUIDE.md](../GAME_DEVELOPMENT_GUIDE.md)

---

**Happy Fighting! 🥊**
