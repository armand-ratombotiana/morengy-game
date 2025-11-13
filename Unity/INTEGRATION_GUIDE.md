# 🔗 MORENGY - System Integration Guide

**How to connect all the systems together for a complete game**

---

## 📋 Overview

This guide shows you how to integrate all the individual systems (Combat, AI, UI, VFX, Audio, etc.) into a cohesive fighting game experience.

---

## 🎯 Integration Checklist

Use this checklist to ensure all systems are properly connected:

- [ ] Fighters have all required components
- [ ] Combat system triggers VFX
- [ ] Combat system triggers audio
- [ ] Combat system updates UI
- [ ] Combat system tracks combos
- [ ] AI references opponent correctly
- [ ] Camera tracks both fighters
- [ ] Game Manager controls match flow
- [ ] Round Announcer receives events
- [ ] Pause menu functions properly

---

## 🔧 Step-by-Step Integration

### Step 1: Core Fighter Setup

**Each fighter needs these components:**

```
Fighter GameObject
├── Character Controller (Unity built-in)
├── FighterController (movement)
├── FighterStats (health, stamina)
├── CombatSystem (attacks)
├── ComboTracker ⭐ NEW
├── InputManager (player only) OR AIBehavior (AI only)
└── Animator (when you have animations)
```

**Setup Order:**
1. Add Character Controller first
2. Add FighterController (references auto-fill)
3. Add FighterStats
4. Add CombatSystem
5. Add ComboTracker
6. Add InputManager OR AIBehavior

---

### Step 2: Connect Combat to VFX

**In CombatSystem.cs - ProcessHit() method:**

Add after damage is applied:

```csharp
// Play hit visual effects
if (Core.VFXManager.Instance != null)
{
    Vector3 hitPoint = strikePoint.position;
    Vector3 normal = (opponent.transform.position - transform.position).normalized;

    if (isCritical)
    {
        Core.VFXManager.Instance.PlayCriticalHitEffect(hitPoint, normal);
    }
    else if (currentAttackType == AttackType.Heavy)
    {
        Core.VFXManager.Instance.PlayHeavyHitEffect(hitPoint, normal);
    }
    else
    {
        Core.VFXManager.Instance.PlayLightHitEffect(hitPoint, normal);
    }
}
```

---

### Step 3: Connect Combat to Audio

**In CombatSystem.cs - ExecuteAttack() method:**

Add at attack startup:

```csharp
// Play attack sound
if (Managers.AudioManager.Instance != null)
{
    switch (attackType)
    {
        case AttackType.Light:
            Managers.AudioManager.Instance.PlayLightAttackSound();
            break;
        case AttackType.Heavy:
            Managers.AudioManager.Instance.PlayHeavyAttackSound();
            break;
        case AttackType.Special:
            Managers.AudioManager.Instance.PlayCriticalHitSound();
            break;
    }
}
```

**In ProcessHit() method:**

```csharp
// Play impact sound
if (Managers.AudioManager.Instance != null)
{
    Managers.AudioManager.Instance.PlayBodyImpactSound();
}
```

---

### Step 4: Connect Combat to Combo Tracker

**In CombatSystem.cs - ProcessHit() method:**

Add after hit is confirmed:

```csharp
// Track combo
ComboTracker comboTracker = GetComponent<ComboTracker>();
if (comboTracker != null)
{
    comboTracker.RegisterHit(finalDamage);
}
```

**When fighter takes damage (breaks combo):**

```csharp
// In FighterStats.cs - TakeDamage() method
ComboTracker comboTracker = GetComponent<ComboTracker>();
if (comboTracker != null)
{
    comboTracker.BreakCombo();
}
```

---

### Step 5: Connect Combat to Damage Popups

**In CombatSystem.cs - ProcessHit() method:**

Add after damage calculation:

```csharp
// Show damage popup
if (UI.DamagePopupManager.Instance != null)
{
    Vector3 popupPosition = opponent.transform.position + Vector3.up * 2f;
    UI.DamagePopupManager.Instance.SpawnDamagePopup(popupPosition, finalDamage, isCritical);
}
```

---

### Step 6: Setup Scene Managers

**Create empty GameObjects for managers:**

1. **GameManager** (controls match)
   ```
   GameObject: "GameManager"
   Component: GameManager
   Settings:
   - Player 1: Drag player fighter
   - Player 2: Drag AI fighter
   - Max Rounds: 3
   - Round Duration: 120
   - Spawn points: Create empties at start positions
   ```

2. **AudioManager** (sounds & music)
   ```
   GameObject: "AudioManager"
   Component: AudioManager
   Settings:
   - Assign audio clips when available
   - Master Volume: 1.0
   - Music Volume: 0.7
   - SFX Volume: 1.0
   ```

3. **VFXManager** (visual effects)
   ```
   GameObject: "VFXManager"
   Component: VFXManager
   Settings:
   - Assign particle prefabs when available
   - Pool Size: 20
   ```

4. **DamagePopupManager** (floating numbers)
   ```
   GameObject: "DamagePopupManager"
   Component: DamagePopupManager
   Settings:
   - Damage Popup Prefab: Create UI prefab with DamagePopup component
   ```

---

### Step 7: Setup UI Canvas

**Create UI hierarchy:**

```
Canvas (Screen Space - Overlay)
├── FighterHUD_Player1
│   └── Component: FighterHUD
│       - Fighter Stats: Player 1
│       - Health Bar Fill: Create Image
│       - Stamina Bar Fill: Create Image
│       - Special Meter Fill: Create Image
│
├── FighterHUD_Player2
│   └── Same as above for Player 2
│
├── RoundAnnouncer
│   └── Panel
│       ├── Component: RoundAnnouncer
│       ├── AnnouncementText (TextMeshPro)
│       └── SubText (TextMeshPro)
│
└── PauseMenu
    ├── Component: PauseMenu
    ├── Panel (pause menu)
    │   ├── Resume Button
    │   ├── Restart Button
    │   ├── Settings Button
    │   └── Quit Button
    └── Settings Panel
        ├── Master Volume Slider
        ├── Music Volume Slider
        └── SFX Volume Slider
```

---

### Step 8: Camera Setup

**Main Camera:**

```
Component: FightingCameraController
Settings:
- Fighter 1: Player
- Fighter 2: AI
- Default Distance: 12
- Min Distance: 8
- Max Distance: 18
- Height: 3
```

---

## 🎮 Event Flow Diagram

```
MATCH START
    ↓
GameManager.StartMatch()
    ↓
RoundAnnouncer.OnRoundStart("ROUND 1")
    ↓
RoundAnnouncer.OnCountdown("3...2...1...FIGHT!")
    ↓
AudioManager.PlayFightMusic()
    ↓
ROUND ACTIVE
    ↓
┌───────────────────────────────────┐
│  Player Input → CombatSystem      │
│      ↓                            │
│  Hit Detection                    │
│      ↓                            │
│  ├─ VFXManager.PlayHitEffect()   │
│  ├─ AudioManager.PlayHitSound()  │
│  ├─ ComboTracker.RegisterHit()   │
│  ├─ DamagePopup.Spawn()          │
│  ├─ FighterStats.TakeDamage()    │
│  └─ FighterHUD.UpdateHealth()    │
└───────────────────────────────────┘
    ↓
Health reaches 0
    ↓
RoundAnnouncer.AnnounceKnockout()
    ↓
VFXManager.PlayKOEffect()
    ↓
GameManager.EndRound(winner)
    ↓
Check if match over (2 rounds won)
    ↓
    Yes → GameManager.EndMatch()
          RoundAnnouncer.OnMatchEnd()
          AudioManager.PlayVictoryMusic()
    ↓
    No → GameManager.StartNextRound()
```

---

## 🔗 Critical Connections

### Combat → Everything

The CombatSystem is the central hub that triggers most events:

**Required integrations:**
1. ✅ Combat → VFX (hit effects)
2. ✅ Combat → Audio (hit sounds)
3. ✅ Combat → ComboTracker (track hits)
4. ✅ Combat → DamagePopup (show damage)
5. ✅ Combat → FighterStats (deal damage)

### GameManager → UI

GameManager controls match flow and notifies UI:

**Event connections:**
1. ✅ OnRoundStart → RoundAnnouncer
2. ✅ OnRoundEnd → RoundAnnouncer
3. ✅ OnMatchEnd → RoundAnnouncer
4. ✅ OnCountdown → RoundAnnouncer
5. ✅ OnRoundTimerUpdate → UI timer display

### FighterStats → UI

Stats changes update UI in real-time:

**Event connections:**
1. ✅ OnHealthChanged → FighterHUD.UpdateHealth()
2. ✅ OnStaminaChanged → FighterHUD.UpdateStamina()
3. ✅ OnSpecialMeterChanged → FighterHUD.UpdateSpecial()
4. ✅ OnCriticalHealth → FighterHUD.ShowWarning()
5. ✅ OnFighterKnockedOut → GameManager

---

## 🐛 Testing Integration

### Test Checklist:

1. **Combat Integration**
   - [ ] Attacks play sound effects
   - [ ] Hits show visual effects
   - [ ] Damage numbers appear
   - [ ] Health bars update
   - [ ] Combos are tracked

2. **UI Integration**
   - [ ] Health/stamina bars work
   - [ ] Round announcements appear
   - [ ] Combo counter displays
   - [ ] Pause menu functions

3. **Audio Integration**
   - [ ] Background music plays
   - [ ] Hit sounds trigger
   - [ ] UI sounds on button press
   - [ ] Volume sliders work

4. **Match Flow**
   - [ ] Countdown works
   - [ ] Rounds start/end correctly
   - [ ] Match ends after 2 rounds won
   - [ ] Timer counts down

5. **Camera**
   - [ ] Frames both fighters
   - [ ] Zooms based on distance
   - [ ] Screen shake on hits

---

## 📝 Quick Integration Script

**Add this helper to your fighter:**

```csharp
using UnityEngine;

namespace Morengy.Core
{
    /// <summary>
    /// Helper component that auto-integrates all systems.
    /// Attach to fighter for automatic setup.
    /// </summary>
    public class FighterIntegrator : MonoBehaviour
    {
        private void Start()
        {
            IntegrateSystems();
        }

        private void IntegrateSystems()
        {
            var combatSystem = GetComponent<CombatSystem>();
            var comboTracker = GetComponent<ComboTracker>();
            var fighterStats = GetComponent<FighterStats>();

            if (combatSystem == null || fighterStats == null)
            {
                Debug.LogError($"Missing required components on {gameObject.name}!");
                return;
            }

            Debug.Log($"Fighter {fighterStats.FighterName} systems integrated!");
        }
    }
}
```

---

## 🎯 Common Integration Issues

### Issue: VFX not showing
**Solution:**
- Ensure VFXManager exists in scene
- Check VFXManager.Instance is not null before calling
- Assign particle prefabs in VFXManager

### Issue: Audio not playing
**Solution:**
- Ensure AudioManager exists and has DontDestroyOnLoad
- Assign audio clips in AudioManager
- Check volume levels are > 0

### Issue: UI not updating
**Solution:**
- Verify FighterHUD has correct FighterStats reference
- Check events are being subscribed (OnHealthChanged, etc.)
- Ensure Canvas is set to Screen Space - Overlay

### Issue: Combos not tracking
**Solution:**
- Add ComboTracker component to fighter
- Call RegisterHit() from CombatSystem.ProcessHit()
- Call BreakCombo() when fighter takes damage

---

## 🚀 Optimization Tips

1. **Object Pooling**
   - VFXManager already pools effects
   - Consider pooling damage popups for high combo counts

2. **Event Unsubscription**
   - All scripts properly unsubscribe in OnDestroy()
   - Prevents memory leaks

3. **Null Checks**
   - Always check Instance != null before using singletons
   - Graceful degradation if systems missing

4. **Performance**
   - VFX pool size can be adjusted
   - Damage popups auto-destroy after lifetime
   - Audio uses pooled sources

---

## 📚 Next Steps

After integration:

1. ✅ Test all systems together
2. ✅ Verify event flow works correctly
3. ✅ Check for console errors
4. ⏳ Add 3D models and animations
5. ⏳ Create particle effect prefabs
6. ⏳ Add audio clips
7. ⏳ Polish and balance

---

**All systems are designed to work together seamlessly!**

**Follow this guide and your Morengy game will be fully integrated!** 🥊

---

*Last Updated: January 13, 2025*
*Compatible with all Phase 1 systems*
