# MORENGY - Core Combat System Design

## Game Design Document: Combat Mechanics
**Version:** 1.0
**Date:** 2025-01-13
**Focus:** Malagasy Martial Arts (Morengy) with UFC/Def Jam Hybrid Mechanics

---

## 1. Combat Philosophy

### Cultural Foundation
Morengy is a traditional Malagasy martial art emphasizing:
- **Respect & Honor** - fighters bow before and after matches
- **Raw Power** - explosive strikes and devastating blows
- **Agility** - quick footwork and evasive movement
- **Endurance** - matches test stamina and willpower
- **Spectacle** - crowd engagement and showmanship

### Gameplay Translation
- **UFC Realism:** Stamina management, damage accumulation, realistic physics
- **Def Jam Style:** Environmental interactions, contextual finishers, personality-driven combat
- **Morengy Authenticity:** Traditional techniques, cultural animations, honor system

---

## 2. Core Combat Mechanics

### 2.1 Fighting Stance System
```
STANCES:
├── Traditional Morengy (Balanced)
│   ├── High Guard - Protection focused
│   ├── Low Crouch - Evasion & counterattack
│   └── Open Stance - Aggressive striking
├── Northern Style (Diego Suarez)
│   └── Quick jabs, lateral movement
└── Southern Style (Rural regions)
    └── Power-based, grounded attacks
```

### 2.2 Attack Types

#### **Light Attacks** (Fast, Low Damage)
- **Jab** - Quick straight punch (inspired by Morengy tsimbola)
- **Leg Kick** - Low sweeping kick
- **Body Blow** - Mid-level strike
- **Energy Cost:** Low (5% stamina)
- **Recovery:** Fast (0.3s)

#### **Heavy Attacks** (Slow, High Damage)
- **Haymaker** - Devastating hook punch
- **Flying Knee** - Aerial knee strike
- **Ground Slam** - Powerful downward strike
- **Energy Cost:** High (15% stamina)
- **Recovery:** Slow (1.2s)

#### **Special Moves** (Signature Techniques)
- **Morengy Finisher** - Region-specific ultimate move
- **Cultural Taunts** - Restore meter, intimidate opponent
- **Reversal Strikes** - Counter-based special attacks
- **Energy Cost:** Very High (25% stamina + special meter)
- **Conditions:** Requires full special meter

#### **Grapples & Throws**
- **Clinch Fighting** - Close-range wrestling
- **Takedowns** - Ground transitions
- **Submission Attempts** - Ground grappling (can be escaped)
- **Environmental Throws** - Use arena objects

### 2.3 Defense Mechanics

#### **Blocking**
- **High Block** - Protects head, drains stamina slowly
- **Low Block** - Protects body/legs
- **Perfect Block** - Timed block with stamina restore bonus

#### **Evasion**
- **Dodge Roll** - I-frames, repositioning
- **Duck** - Avoid high attacks
- **Sidestep** - Lateral evasion
- **Stamina Cost:** 8% per dodge

#### **Counters**
- **Parry** - Timing-based deflection (opens opponent for combo)
- **Counter Strike** - Automatic attack after successful parry
- **Reversal** - Special move triggered during opponent attack

---

## 3. Health & Stamina System

### Health Bar
```
████████████████████ 100% (Full Health)
█████████░░░░░░░░░░░ 45% (Danger Zone - Critical animations)
███░░░░░░░░░░░░░░░░░ 15% (Near KO - Wobble effects)
```

- **Regeneration:** None (permanent damage model)
- **Visual Feedback:** Bruises, blood, sweat increase over fight
- **KO Condition:** Health reaches 0% OR stamina-depleted critical hit

### Stamina Bar
```
⚡⚡⚡⚡⚡⚡⚡⚡⚡⚡ 100% (Full Energy)
⚡⚡⚡⚡⚡░░░░░░ 50% (Slowed attacks)
⚡⚡░░░░░░░░░░ 20% (Exhausted - vulnerable)
```

- **Regeneration:** Passive (2% per second when not attacking)
- **Fast Regen:** Active rest stance (5% per second, vulnerable)
- **Depletion Effects:**
  - < 30%: Attack speed -20%, damage -10%
  - < 10%: Cannot use heavy attacks or specials
  - 0%: Fighter is stunned for 3 seconds (free hits)

### Special Meter (Ultimate/Finisher)
```
🔥🔥🔥🔥🔥 READY (Can execute finisher)
🔥🔥🔥░░ 60% (Building)
```

- **Gain:** Landing attacks (+5%), taking damage (+3%), crowd hype (+2%)
- **Use:** Special moves, environmental finishers, ultimate attacks
- **Max Storage:** 100% (one full finisher)

---

## 4. Combo System

### Combo Chain Rules
1. **Light → Light → Heavy** (Basic 3-hit combo)
2. **Heavy → Special** (High-risk combo ender)
3. **Grapple → Ground Attacks** (Takedown combo)
4. **Counter → Auto Combo** (Parry reward)

### Combo Breakers
- **Cost:** 50% special meter
- **Window:** 0.5s timing during opponent combo
- **Effect:** Interrupts combo, pushes opponent back

### Juggle System
- **Max Air Hits:** 3 consecutive aerial strikes
- **Gravity Scale:** Increases per hit (prevents infinite combos)
- **Finisher Bonus:** Last hit deals 1.5x damage

---

## 5. Environmental Combat (Def Jam Influence)

### Arena Interactions
- **Wall Bounces** - Slam opponent into walls for extended combos
- **Crowd Shoves** - Spectators push fighters back into ring
- **Object Weapons** - Use benches, bottles (temporary bonus damage)
- **Hazards** - Water, mud, fire (status effects)

### Contextual Finishers
When opponent health < 15% and near hazard:
- **Harbor Arena** - Throw into ocean
- **Zebu Ring** - Trampled by cattle
- **Street Fight** - Slam into market stall

### Dynamic Destructibles
- Breakable barriers
- Destructible crowd stands
- Environmental state changes (dry → muddy after rain)

---

## 6. AI Behavior System

### Difficulty Tiers

#### **Easy (Training)**
- Predictable attack patterns
- Slow reaction time (0.8s)
- Rarely blocks or dodges
- No combo chains

#### **Medium (Local Fighter)**
- Mixed attack patterns
- Moderate reaction (0.5s)
- Blocks 40% of attacks
- Basic 2-3 hit combos

#### **Hard (Regional Champion)**
- Adaptive pattern recognition
- Fast reaction (0.3s)
- Blocks 60% + parries
- Complex combos + feints

#### **Expert (Legendary Warrior)**
- Machine learning adaptive AI
- Instant reaction (0.15s)
- Reads player patterns
- Perfect execution + mind games

### AI Personality Types
1. **Brawler** - Aggressive, high-pressure offense
2. **Tactician** - Defensive, counter-based
3. **Showman** - Taunt-heavy, crowd interaction
4. **Technical** - Combo specialist, precision strikes

---

## 7. Damage Calculation Formula

```
Base Damage = Attack Power × Damage Multiplier
Final Damage = Base Damage × (1 - Defense%) × Stamina Modifier × Combo Multiplier

Where:
- Attack Power: Fighter stat (50-100)
- Damage Multiplier: Attack type (Light: 1.0, Heavy: 2.5, Special: 4.0)
- Defense%: Blocking (0.5), Partial Block (0.7), No Block (1.0)
- Stamina Modifier: Attacker stamina (>50%: 1.0, <50%: 0.8, <20%: 0.6)
- Combo Multiplier: 1.0 + (0.1 × Combo Count) [Max: 1.5]
```

### Critical Hit System
- **Chance:** 5% base + precision stat
- **Effect:** 1.75x damage, special visual effect
- **Conditions:**
  - Perfect timing window (0.2s)
  - Counter attacks (automatic critical)
  - Low health comeback mechanic (15% health = 20% crit chance)

---

## 8. Match Conditions & Victory

### Win Conditions
1. **Knockout (KO)** - Reduce opponent health to 0%
2. **Technical Knockout (TKO)** - 3 knockdowns in one round
3. **Submission** - Successful ground submission
4. **Time Decision** - Highest health at time limit wins
5. **Disqualification** - Repeated rule violations

### Round System
- **Rounds:** 3 rounds (2 minutes each)
- **Rest Period:** 30 seconds between rounds (stamina restores to 60%)
- **Health:** Carries between rounds (no healing)
- **Victory:** Best of 3 rounds

### Post-Match Respect System
- **Win with Honor:** No taunting when opponent < 20% health
- **Bow Respect:** Both fighters bow post-match
- **Crowd Reaction:** Influences XP and reputation gain

---

## 9. Progression & Customization

### Fighter Stats (Upgradeable)
- **Power** - Damage output
- **Speed** - Attack/movement speed
- **Defense** - Damage reduction
- **Stamina** - Energy pool size
- **Technique** - Combo execution bonus
- **Charisma** - Crowd meter gain rate

### Unlockable Techniques
- Regional fighting styles
- Signature moves from real Morengy fighters
- Cultural animations and taunts
- Ultimate finishers

### Cosmetic Customization
- Traditional Malagasy clothing
- Body tattoos and scars
- Victory poses
- Entrance animations

---

## 10. Technical Implementation Notes

### Unity Systems Required
1. **Character Controller** - Custom physics-based movement
2. **Animation State Machine** - Blend trees for smooth transitions
3. **Hit Detection** - Capsule colliders + raycast validation
4. **Combo Manager** - Input buffer system (0.3s window)
5. **AI State Machine** - Behavior tree architecture
6. **VFX System** - Hit particles, sweat, blood decals
7. **Audio Manager** - 3D positional impact sounds

### Performance Targets
- **60 FPS** minimum on mid-range hardware
- **Physics Updates:** Fixed timestep (50Hz)
- **Input Polling:** 120Hz for responsive controls
- **Network Latency:** <100ms for online play (rollback netcode)

---

## Next Steps
1. Prototype basic movement and single attack type
2. Implement health/stamina system with UI
3. Create hit detection and damage calculation
4. Build combo system with input buffering
5. Develop AI behavior tree (easy difficulty first)
6. Add animation state machine
7. Integrate VFX and audio feedback

---

**Status:** Ready for Prototype Phase
**Priority:** Core combat loop (movement, attack, damage, KO)
