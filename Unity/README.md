# MORENGY - Unity Game Project

## 🥊 Overview

This is the main Unity game project for MORENGY, a Malagasy cultural fighting game that blends traditional Morengy martial arts with modern UFC/Def Jam hybrid combat mechanics.

---

## 📁 Project Structure

```
Unity/
│
├── Assets/
│   ├── Scripts/
│   │   ├── Character/          # Fighter controllers and stats
│   │   ├── Combat/             # Combat system and damage
│   │   ├── AI/                 # AI behavior and difficulty
│   │   ├── Core/               # Input, managers, utilities
│   │   ├── UI/                 # HUD, menus, health bars
│   │   └── Managers/           # Game, round, audio managers
│   │
│   ├── Prefabs/
│   │   ├── Fighters/           # Fighter character prefabs
│   │   ├── Arenas/             # Fighting arena prefabs
│   │   └── Effects/            # VFX and particle effects
│   │
│   ├── Materials/              # PBR materials and shaders
│   ├── Textures/               # Textures and sprites
│   ├── Audio/
│   │   ├── Music/              # Background music tracks
│   │   └── SFX/                # Sound effects (hits, impacts)
│   │
│   ├── Animations/
│   │   ├── Fighters/           # Character animations
│   │   └── UI/                 # UI animations
│   │
│   ├── Scenes/                 # Unity scenes (menus, arenas)
│   └── Resources/              # Runtime loadable assets
│
├── ProjectSettings/            # Unity project configuration
└── Packages/                   # Package dependencies
```

---

## 🎮 Core Systems Implemented

### ✅ Character System
- **[FighterController.cs](Assets/Scripts/Character/FighterController.cs)** - Movement, rotation, dodging
- **[FighterStats.cs](Assets/Scripts/Character/FighterStats.cs)** - Health, stamina, special meter, stats

### ✅ Combat System
- **[CombatSystem.cs](Assets/Scripts/Combat/CombatSystem.cs)** - Attacks, blocking, damage calculation, combos
- Hit detection with sphere overlap
- Combo system with input buffering
- Critical hit mechanics
- Knockback and impact physics

### ✅ Input System
- **[InputManager.cs](Assets/Scripts/Core/InputManager.cs)** - Player input handling
- Support for keyboard and alternative controls
- Input buffering for responsive combat

### ✅ AI System
- **[AIBehavior.cs](Assets/Scripts/AI/AIBehavior.cs)** - Computer opponent behavior
- 4 difficulty levels (Easy, Medium, Hard, Expert)
- 5 personality types (Balanced, Brawler, Tactician, Showman, Technical)
- State machine architecture (Aggressive, Defensive, Observing, etc.)

---

## 🚀 Getting Started

### Prerequisites
- **Unity Version:** 2022.3 LTS or newer (recommended)
- **Platform:** Windows, macOS, Linux
- **Minimum Specs:**
  - 4GB RAM
  - DirectX 11 compatible GPU
  - 2GB free disk space

### Setup Instructions

1. **Open the Project in Unity Hub**
   ```
   Add > Select "Unity" folder > Open with Unity 2022.3 LTS
   ```

2. **Import Required Packages** (via Package Manager)
   - Unity Input System
   - Cinemachine (for camera)
   - ProBuilder (for level prototyping)
   - Shader Graph (for custom shaders)

3. **Project Settings**
   - Set target platform: PC, Mac & Linux Standalone
   - Set rendering pipeline: URP (Universal Render Pipeline) recommended
   - Configure input settings to support both keyboard and gamepad

---

## 🎯 Quick Start - Creating Your First Fight

### Step 1: Create Fighter Prefabs

1. Create two empty GameObjects in the scene
2. Add the following components to each:
   - `Character Controller` (built-in Unity)
   - `FighterController` (custom)
   - `FighterStats` (custom)
   - `CombatSystem` (custom)
   - `InputManager` (custom - for player only)
   - `AIBehavior` (custom - for AI opponent only)

3. Configure FighterStats:
   - Set Fighter Name, Region, Fighting Style
   - Adjust stats (Power, Speed, Defense, etc.)

### Step 2: Set Up Input

For the **Player Fighter**:
- Enable `InputManager` component
- Set key bindings (default: J=Light, K=Heavy, L=Special, I=Block, Space=Dodge)

For the **AI Fighter**:
- Enable `AIBehavior` component
- Set difficulty and personality
- Assign opponent reference (drag player fighter)

### Step 3: Configure Combat

Both fighters need:
- Set `strikePoint` transform (hand/fist position)
- Configure hit layers
- Adjust damage values if needed

### Step 4: Link Opponents

- Set `FighterController.Opponent` for both fighters
- This ensures they face each other and track distance

### Step 5: Test

- Press Play
- Use WASD to move, J/K/L for attacks, I for block, Space to dodge
- Watch the AI react and counter your moves!

---

## 🎨 Customization Guide

### Adding New Fighters

1. **Create Fighter Data**
   ```csharp
   // In FighterStats component
   Fighter Name: "Diego Warrior"
   Fighting Style: "Northern Morengy"
   Region: "Diego Suarez"

   Stats:
   - Power: 70
   - Speed: 60
   - Defense: 50
   - Stamina: 65
   - Technique: 55
   - Charisma: 75
   ```

2. **Customize Combat Style**
   - Adjust attack damage in `CombatSystem`
   - Modify stamina costs for unique play styles

3. **AI Personality**
   - Set `AIBehavior.Personality` to match fighter's style
   - Brawler = aggressive close-range
   - Tactician = defensive counter-fighter
   - Showman = special move focused

### Creating New Arenas

1. Create ground plane
2. Add walls/boundaries with colliders
3. Set layer to "Ground"
4. Add environmental objects (breakables, hazards)
5. Configure lighting and atmosphere

### Adjusting Difficulty

```csharp
// In AIBehavior component
Difficulty: Easy / Medium / Hard / Expert

Easy:
- Reaction time: 0.6-1.0s
- Low combo focus
- Predictable patterns

Expert:
- Reaction time: 0.1-0.2s
- High combo focus
- Adaptive behavior
```

---

## 🔧 Technical Details

### Combat Calculations

**Damage Formula:**
```
Base Damage = Attack Power × Damage Multiplier
Final Damage = Base Damage × (1 - Defense%) × Stamina Modifier × Combo Multiplier
```

**Stamina System:**
- Passive regen: 2% per second
- Fast regen (resting): 5% per second
- Attacks drain stamina (Light: 5%, Heavy: 15%)
- Exhausted at <10% (cannot use heavy/special moves)

**Special Meter:**
- Gain 5% on successful hit
- Gain 3% when taking damage
- Gain 2% from crowd interaction (taunts)
- Full meter = 100% (one special attack)

### State Management

**Fighter States:**
- Idle, Moving, Crouching, Dodging
- Attacking, Blocking, Stunned
- Knocked Down, Grabbing, Being Grabbed

**AI States:**
- Observing - neutral positioning
- Aggressive - attacking and pressuring
- Defensive - blocking and counter-attacking
- Retreating - low health escape
- Recovering - stamina regeneration

---

## 📊 Performance Optimization

### Recommended Settings

- **Physics Update:** 50Hz (0.02s fixed timestep)
- **Target FPS:** 60
- **V-Sync:** On (for consistent timing)
- **Quality:** Medium-High for development

### Optimization Tips

1. **Use Object Pooling** for particle effects
2. **Bake Lighting** for static arenas
3. **LOD System** for character models
4. **Reduce Draw Calls** with material batching

---

## 🐛 Debugging & Testing

### Debug Features

The scripts include several debug helpers:

**FighterStats:**
- Context Menu: "Reset All Stats"
- Context Menu: "Level Up"
- Context Menu: "Take 25 Damage"
- Context Menu: "Fill Special Meter"

**Visual Gizmos:**
- `FighterController` shows ground check, facing direction, dodge range
- `CombatSystem` shows strike point and range

### Console Commands (Editor Only)

Press the keys while in Play mode:
- **T** - Taunt (gain special meter)
- **R** - Rest stance (fast stamina regen)
- **Tab** - Toggle control scheme

---

## 🎓 Next Steps

### Phase 1 - Core Prototype (CURRENT)
- ✅ Character movement and controls
- ✅ Combat system with combos
- ✅ AI behavior system
- ⏳ Animation integration
- ⏳ VFX and audio feedback

### Phase 2 - Content Creation
- [ ] Create 3D character models (Blender)
- [ ] Mocap/animate fighting moves
- [ ] Design 3-5 arenas
- [ ] Record Malagasy music and SFX

### Phase 3 - Game Modes
- [ ] Story mode / Campaign
- [ ] Arcade mode
- [ ] Training mode
- [ ] Tournament mode

### Phase 4 - Polish & UI
- [ ] Main menu and HUD
- [ ] Health/stamina bars
- [ ] Combo counter display
- [ ] Round system and scoring

### Phase 5 - Multiplayer (Future)
- [ ] Local versus mode
- [ ] Online netcode (rollback)
- [ ] Matchmaking system

---

## 📚 Additional Documentation

- **[Core Combat System GDD](../Docs/GDD/CoreCombatSystem.md)** - Complete combat design
- **[Project Summary](../PROJECT_SUMMARY.md)** - Overall vision and roadmap
- **[Fighter Profiles](../Docs/Fighter_Profiles/)** - Character backstories

---

## 🤝 Contributing

### Code Style
- Use **C# naming conventions**
- Add **XML documentation** for public methods
- Include **#region** tags for organization
- Add **Gizmos** for visual debugging

### Testing Checklist
- [ ] Test on different resolutions
- [ ] Verify input on keyboard and gamepad
- [ ] Check AI behavior on all difficulty levels
- [ ] Profile performance (aim for 60 FPS)

---

## 📞 Support

- **Issues:** Report bugs via GitHub Issues
- **Discussions:** Join development discussions
- **Documentation:** Check `/Docs` folder for detailed guides

---

## 🏆 Credits

**Lead Developer:** Armand Judicael Ratombotiana
**Game Engine:** Unity 2022.3 LTS
**Inspired By:** Traditional Malagasy Morengy martial arts

---

**"Morengy is not just a fight — it's our heritage."** 🇲🇬
