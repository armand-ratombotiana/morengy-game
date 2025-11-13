# 🥊 MORENGY - Unity Setup Guide

**Quick setup guide to get your first fight running in Unity**

---

## 📋 Prerequisites

- ✅ Unity 2022.3 LTS or newer
- ✅ Basic Unity knowledge
- ✅ All C# scripts from this repository

---

## 🚀 Quick Start (5 Minutes)

### Step 1: Create New Scene

1. Open Unity
2. Create new scene: `File > New Scene > Basic (Built-in)`
3. Save as: `Scenes/TestFight.unity`

---

### Step 2: Create Ground

1. Create plane: `GameObject > 3D Object > Plane`
2. Name it: "Ground"
3. Scale: `(5, 1, 5)`
4. Position: `(0, 0, 0)`
5. Set Layer to "Ground" (create layer if needed)

---

### Step 3: Create Player Fighter

1. Create cube: `GameObject > 3D Object > Cube`
2. Name it: "Player"
3. Position: `(-3, 1, 0)`
4. Add required components (in this order):

```
Inspector > Add Component:

1. Character Controller
   - Height: 1.8
   - Radius: 0.4
   - Center: (0, 0, 0)

2. Fighter Controller (Custom Script)
   - Walk Speed: 3
   - Sprint Speed: 5
   - Dodge Distance: 3
   - Ground Layer: Ground

3. Fighter Stats (Custom Script)
   - Fighter Name: "Diego Warrior"
   - Fighting Style: "Northern Morengy"
   - Region: "Diego Suarez"
   - Power: 60
   - Speed: 75
   - Defense: 55
   - Stamina: 70
   - Technique: 65
   - Charisma: 80

4. Combat System (Custom Script)
   - Strike Range: 1.5
   - Hit Layer: Fighter (create layer)
   - Light Attack Damage: 10
   - Heavy Attack Damage: 25

5. Input Manager (Custom Script)
   - (References will auto-fill)
```

---

### Step 4: Create AI Opponent

1. Duplicate Player cube: `Right-click Player > Duplicate`
2. Rename to: "AI_Opponent"
3. Position: `(3, 1, 0)`
4. **Remove** InputManager component
5. **Add** AI Behavior component:

```
Inspector > Add Component:

AI Behavior (Custom Script)
- Difficulty: Medium
- Personality: Balanced
- Opponent: (Drag "Player" here)
- Optimal Range: 2.5
```

6. Update Fighter Stats:
   - Fighter Name: "Nosy Be Champion"
   - Region: "Nosy Be"

---

### Step 5: Link Fighters

**On Player:**
- Fighter Controller > Opponent: Drag "AI_Opponent" here

**On AI_Opponent:**
- Fighter Controller > Opponent: Already set to "Player"

---

### Step 6: Set Up Camera

1. Select Main Camera
2. Add component: `Fighting Camera Controller`
3. Settings:
   - Fighter 1: Drag "Player"
   - Fighter 2: Drag "AI_Opponent"
   - Default Distance: 12
   - Height: 3

---

### Step 7: Configure Physics

1. Go to: `Edit > Project Settings > Physics`
2. Create Collision Layers:
   - Layer 6: "Ground"
   - Layer 7: "Fighter"

3. Set Layer Collision Matrix:
   - ✅ Fighter vs Fighter (enabled)
   - ✅ Fighter vs Ground (enabled)
   - ❌ Ground vs Ground (disabled)

---

### Step 8: Assign Layers

- Set "Player" layer to: **Fighter**
- Set "AI_Opponent" layer to: **Fighter**
- Set "Ground" layer to: **Ground**

---

### Step 9: PRESS PLAY! 🎮

**Controls:**
```
Movement:
W/A/S/D - Move
Shift - Sprint
Space - Dodge Roll

Combat:
J - Light Attack
K - Heavy Attack
L - Special Attack
I - Block (hold)
R - Rest (fast stamina regen)
T - Taunt
```

**What Should Happen:**
- You can move the player cube with WASD
- Press J to attack (you'll see debug messages in Console)
- The AI cube will move toward you and attack
- AI will dodge, block, and counter
- Health drains when attacks land
- Stamina drains when you sprint or attack

---

## 🎨 Adding UI (Optional)

### Create Health Bars

1. Create UI Canvas: `GameObject > UI > Canvas`
2. Add Image for Player health bar:
   - Anchor: Top-Left
   - Position: (100, -50)
   - Size: (200, 20)
   - Color: Green

3. Add child Image for fill:
   - Anchor: Stretch both
   - Image Type: Filled
   - Fill Method: Horizontal
   - Color: Red

4. Add Fighter HUD component to Canvas
5. Assign references:
   - Fighter Stats: Player
   - Health Bar Fill: (the red fill image)

6. Repeat for AI opponent on the right side

---

## 🔊 Adding Audio (Optional)

### Create Audio Manager

1. Create empty GameObject: "AudioManager"
2. Add component: `Audio Manager (Custom Script)`
3. Assign audio clips when you have them

**Audio will work without clips** - just won't play sounds yet!

---

## 🎮 Advanced Setup

### Add Game Manager (Match System)

1. Create empty GameObject: "GameManager"
2. Add component: `Game Manager (Custom Script)`
3. Settings:
   - Player 1: Drag "Player"
   - Player 2: Drag "AI_Opponent"
   - Max Rounds: 3
   - Round Duration: 120
   - Player 1 Spawn Point: Create empty at (-3, 1, 0)
   - Player 2 Spawn Point: Create empty at (3, 1, 0)

This enables:
- Best of 3 rounds
- Round timer (2 minutes)
- Automatic round transitions
- Countdown sequences
- Win detection

---

### Add VFX Manager (Visual Effects)

1. Create empty GameObject: "VFXManager"
2. Add component: `VFX Manager (Custom Script)`
3. Effect prefabs can be added later
4. System will work without prefabs (just won't show effects)

---

### Add Round Announcer (UI Messages)

1. Create UI Panel: `GameObject > UI > Panel`
2. Name it: "AnnouncementPanel"
3. Set CanvasGroup component (alpha controls)
4. Add child Text: "AnnouncementText"
   - Font Size: 72
   - Alignment: Center
   - Anchor: Center

5. Add component to Panel: `Round Announcer (Custom Script)`
6. Assign text references

This shows: "FIGHT!", "ROUND 1", "KNOCKOUT!", etc.

---

## 🐛 Troubleshooting

### "Attacks Not Hitting"
**Solutions:**
- ✅ Check Fighter layer is set on both fighters
- ✅ Verify Combat System "Hit Layer" includes Fighter
- ✅ Ensure fighters are within Strike Range (1.5m default)
- ✅ Check Console for debug messages

### "AI Not Moving"
**Solutions:**
- ✅ Ensure AI Behavior has Opponent reference
- ✅ Check Fighter Controller has Opponent reference
- ✅ Verify Ground layer is set correctly
- ✅ AI Behavior component is enabled

### "No Damage Being Dealt"
**Solutions:**
- ✅ Check damage values in Combat System
- ✅ Verify Fighter Stats component exists
- ✅ Look for OnHealthChanged event subscription
- ✅ Check Console for hit confirmations

### "Controls Not Working"
**Solutions:**
- ✅ Input Manager is on Player only (not AI)
- ✅ Input Manager has references to Fighter Controller and Combat System
- ✅ Check Input Manager is enabled
- ✅ Try alternative controls (mouse buttons)

---

## 📊 Verification Checklist

Before testing, verify:

- [ ] Ground plane created with "Ground" layer
- [ ] Player cube with all 5 components
- [ ] AI cube with all 4 components (no InputManager)
- [ ] Opponent references set on both fighters
- [ ] Camera has both fighter references
- [ ] Layers configured correctly
- [ ] Physics layer matrix set up

**If all checked, press Play and fight!**

---

## 🎓 Next Steps After Testing

### 1. Replace Cubes with 3D Models
- Import character model (FBX)
- Add Animator component
- Configure Animator Controller
- Keep all existing components

### 2. Add Animations
- Create Animator Controller
- Add animation states (Idle, Walk, Attack, etc.)
- Connect to Fighter Controller states
- Add animation events for hit timing

### 3. Add Visual Effects
- Create particle systems
- Assign to VFX Manager
- Integrate with Combat System
- Test hit effects

### 4. Add Sound Effects
- Import audio clips
- Assign to Audio Manager
- Test combat sounds
- Add music tracks

### 5. Build UI
- Create health/stamina bars
- Add combo counter
- Create round timer
- Build main menu

---

## 📚 Additional Resources

- **Full Documentation:** [Unity/README.md](README.md)
- **Controls Reference:** [QUICK_REFERENCE.md](QUICK_REFERENCE.md)
- **Combat Design:** [../Docs/GDD/CoreCombatSystem.md](../Docs/GDD/CoreCombatSystem.md)
- **Development Guide:** [../GAME_DEVELOPMENT_GUIDE.md](../GAME_DEVELOPMENT_GUIDE.md)

---

## 💡 Tips for First-Time Setup

1. **Start Simple**
   - Get cubes fighting first
   - Add complexity gradually
   - Test each addition

2. **Use Console Logs**
   - Watch for debug messages
   - Check for errors
   - Verify hits registering

3. **Test AI Difficulties**
   - Start with Easy AI
   - Gradually increase difficulty
   - Find your preferred challenge

4. **Experiment with Stats**
   - Adjust fighter stats
   - Test different builds
   - Find balanced setups

5. **Save Often**
   - Save scene frequently
   - Create scene variants
   - Keep working backups

---

## 🎉 You're Ready!

Once everything is set up, you have a **fully functional fighting game prototype**!

### What Works:
- ✅ Character movement and controls
- ✅ Combat with multiple attack types
- ✅ AI opponent that fights back
- ✅ Health and stamina systems
- ✅ Combo system
- ✅ Blocking and dodging
- ✅ Win/lose conditions

### Next Phase:
Replace the cubes with 3D character models and animations to bring your Morengy fighters to life!

---

**"Morengy is not just a fight — it's our heritage."** 🇲🇬

**Happy Fighting!** 🥊

---

*Last Updated: January 13, 2025*
*For Unity 2022.3 LTS*
