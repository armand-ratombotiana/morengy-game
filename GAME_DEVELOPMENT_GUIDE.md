# 🎮 MORENGY Game Development Guide

**Complete guide for implementing the Morengy fighting game**

---

## 📋 Table of Contents

1. [Project Overview](#project-overview)
2. [Development Phases](#development-phases)
3. [Unity Setup](#unity-setup)
4. [Core Systems](#core-systems)
5. [Animation Pipeline](#animation-pipeline)
6. [Audio Integration](#audio-integration)
7. [Testing Strategy](#testing-strategy)
8. [Deployment](#deployment)

---

## 🎯 Project Overview

### Vision
A **Malagasy cultural fighting game** celebrating Madagascar's traditional Morengy martial art, combined with modern UFC realism and Def Jam's expressive personality-driven combat.

### Target Platform
- **Primary:** PC (Windows/Mac/Linux)
- **Secondary:** Consoles (PlayStation, Xbox, Switch)
- **Future:** Mobile (iOS/Android)

### Core Features
- ✅ **Implemented:** Character controller, combat system, AI, input
- 🚧 **In Progress:** Animation integration, VFX, audio
- ⏳ **Planned:** Story mode, multiplayer, arenas

---

## 📅 Development Phases

### Phase 1: Core Prototype ✅ (COMPLETED)
**Duration:** Weeks 1-2
**Status:** ✅ Done

Deliverables:
- ✅ Unity project structure created
- ✅ Character movement and controls (FighterController.cs)
- ✅ Combat system with attacks, blocking, combos (CombatSystem.cs)
- ✅ Health, stamina, special meter (FighterStats.cs)
- ✅ AI opponent with difficulty levels (AIBehavior.cs)
- ✅ Input system (InputManager.cs)
- ✅ Game Design Document

**What You Have Now:**
You can already test combat mechanics with placeholder cubes! Just set up two GameObjects with the scripts and start fighting.

---

### Phase 2: Visual & Animation 🚧 (NEXT)
**Duration:** Weeks 3-6
**Status:** Ready to start

**Tasks:**

#### 2.1 Character Modeling (Blender)
```
Priority: HIGH
Timeline: 2 weeks

Steps:
1. Model base male fighter body (1 week)
   - Research Malagasy physique references
   - Create low-poly model (15k-25k tris)
   - UV unwrap and texture

2. Create 3-5 fighter variations (1 week)
   - Different body types
   - Regional clothing styles
   - Tattoos and accessories

Tools: Blender 3.6+
Export: FBX with armature
```

#### 2.2 Animation System
```
Priority: HIGH
Timeline: 3 weeks

Steps:
1. Mocap or keyframe core animations (1.5 weeks)
   - Idle, walk, run, crouch
   - Light attack combo (3-hit)
   - Heavy attacks (punch, kick, knee)
   - Block, dodge roll
   - Hit reactions, knockdown, KO

2. Special moves (1 week)
   - Regional finishers (Diego, Nosy Be, etc.)
   - Morengy traditional techniques
   - Taunts and victory poses

3. Unity Animator setup (0.5 weeks)
   - Create Animator Controller
   - Set up blend trees
   - Configure state transitions
   - Add animation events for hit detection

Tools: Blender + Mixamo (free mocap library)
Reference: UFC games, Tekken, Street Fighter
```

#### 2.3 Visual Effects (VFX)
```
Priority: MEDIUM
Timeline: 1 week

Create particle systems:
- Hit spark/impact (using Unity Particle System)
- Blood spray (subtle, cultural sensitivity)
- Sweat drops
- Dust clouds
- Speed lines for fast attacks
- Special move auras
- Critical hit flash

Tools: Unity VFX Graph or Particle System
```

---

### Phase 3: Arenas & Environments 🏝️
**Duration:** Weeks 7-8

#### 3.1 Arena Design
Create 3-5 iconic Malagasy locations:

**Arena 1: Diego Suarez Harbor** (Priority: HIGH)
```
Setting: Waterfront ring with crowd
Features:
- Ocean backdrop with sunset
- Wooden platform arena
- Crowd of spectators
- Boats in background
- Destructible crates and barrels
```

**Arena 2: Nosy Be Beach** (Priority: MEDIUM)
```
Setting: Sandy beach with palm trees
Features:
- Turquoise water
- Beach crowd
- Fire torches at night
- Soft sand (affects movement)
```

**Arena 3: Rural Zebu Arena** (Priority: MEDIUM)
```
Setting: Village ring with cattle
Features:
- Dirt ground
- Wooden fence
- Zebu cattle as hazard
- Village huts background
```

**Arena 4: Antananarivo Street** (Priority: LOW)
```
Setting: Urban street fight
Features:
- City backdrop
- Market stalls
- Urban crowd
- Modern Malagasy culture
```

#### 3.2 Implementation
```
1. Blockout with ProBuilder (Unity)
2. Add lighting and atmosphere
3. Place crowd and props
4. Configure colliders and boundaries
5. Add environmental audio (waves, birds, etc.)
6. Test combat flow and camera angles
```

---

### Phase 4: Audio & Music 🎵
**Duration:** Weeks 9-10

#### 4.1 Sound Effects (SFX)
```
Priority: HIGH
Timeline: 1 week

Required sounds:
Combat:
- Light punch/kick (3 variations)
- Heavy impact (5 variations)
- Block/parry (2 variations)
- Dodge swoosh
- Critical hit sound
- Special move whoosh

Environmental:
- Footsteps (dirt, sand, wood)
- Crowd reactions (cheer, gasp, roar)
- Ocean waves, wind, birds

UI:
- Menu navigation
- Fight countdown
- Round announcements
- Victory fanfare

Tools: Audacity, Freesound.org, local recording
Format: .wav or .ogg (Unity compatible)
```

#### 4.2 Music System
```
Priority: MEDIUM
Timeline: 1 week

Soundtrack style: Malagasy traditional + modern fusion

Required tracks:
1. Main Menu Theme (calm, cultural)
2. Character Select (energetic)
3. Fight Music - Diego Arena (Salegy rhythm)
4. Fight Music - Beach Arena (island vibes)
5. Fight Music - Rural Arena (traditional drums)
6. Victory Theme
7. Defeat Theme

Features:
- Dynamic music layers (intensity increases during fight)
- Smooth transitions between sections
- Cultural authenticity (Malagasy instruments)

Collaboration:
- Work with Malagasy musicians
- Use traditional instruments (valiha, kabosy, drums)
- Modern electronic production
```

---

### Phase 5: Game Modes & UI 🎮
**Duration:** Weeks 11-13

#### 5.1 UI System
```
Priority: HIGH
Timeline: 2 weeks

Screens to create:
1. Main Menu
   - Start Game
   - Fighter Select
   - Options
   - Credits
   - Exit

2. Fighter Select Screen
   - Fighter portraits
   - Stats display (Power, Speed, etc.)
   - Arena selection
   - VS screen

3. In-Game HUD
   - Health bars (top screen)
   - Stamina bars
   - Special meter
   - Combo counter
   - Round timer
   - Fighter names

4. Pause Menu
5. Victory/Defeat Screen
6. Settings Menu (volume, controls, graphics)

Design Style:
- Malagasy cultural motifs
- Red/green/white color scheme (Madagascar flag)
- Bold typography
- Animated transitions

Tools: Unity UI Canvas, TextMeshPro
```

#### 5.2 Game Modes
```
Priority: MEDIUM-HIGH
Timeline: 1 week

Mode 1: Quick Fight (LOCAL VERSUS)
- 2 players on same keyboard/split
- Best of 3 rounds
- Simple and fast

Mode 2: Arcade Mode (VS AI)
- Fight through 5-10 opponents
- Increasing difficulty
- Unlock fighters and arenas

Mode 3: Story Mode (PHASE 6)
- Follow a young fighter's journey
- Cutscenes and dialogue
- Cultural storytelling

Mode 4: Training Mode
- Practice moves
- Combo trials
- Damage display
- Frame data (advanced)

Mode 5: Tournament Mode (FUTURE)
- 8-fighter bracket
- Elimination rounds
- Leaderboards
```

---

### Phase 6: Story & Content 📖
**Duration:** Weeks 14-16

#### 6.1 Story Mode Development
```
Narrative:
Title: "Rise of the North"

Plot:
- Young fighter from Diego Suarez dreams of becoming a Morengy legend
- Travels across Madagascar challenging regional champions
- Learns different fighting styles and cultural lessons
- Faces legendary Morengy master in final battle
- Themes: respect, heritage, perseverance

Structure:
- 10 story fights with cutscenes
- 5 regions to explore (Diego, Nosy Be, Mahajanga, etc.)
- Character development and dialogue
- Cultural education about Morengy traditions

Implementation:
- Dialogue system with TextMeshPro
- Simple 2D character portraits
- Narration and voice acting (if budget allows)
- Story branching (optional)
```

#### 6.2 Fighter Roster
```
Create 8-12 unique fighters:

Confirmed Fighters:
1. Diego Warrior (Protagonist)
   - Balanced stats
   - Northern style
   - Quick jabs

2. Nosy Be Champion
   - High speed
   - Island style
   - Evasive

3. Mahajanga Veteran
   - High power
   - Heavy hits
   - Slow but devastating

4. Antananarivo Technician
   - High technique
   - Combo specialist
   - Urban style

5-12: Regional representatives, legends, unlockable bosses

Each fighter needs:
- Unique model and textures
- Special moves (2-3 per fighter)
- Voice lines and personality
- Backstory and lore
```

---

### Phase 7: Polish & Testing 🔍
**Duration:** Weeks 17-18

#### 7.1 Bug Fixing & Optimization
```
Testing checklist:
- Performance profiling (maintain 60 FPS)
- Memory leak detection
- Input responsiveness
- AI behavior balance
- Collision detection accuracy
- Animation transitions
- Audio sync with actions
- UI responsiveness
```

#### 7.2 Playtesting
```
Gather feedback on:
- Combat feel (weight, impact, responsiveness)
- Difficulty balance (AI too easy/hard?)
- Fighter balance (overpowered characters?)
- UI clarity
- Cultural authenticity
- Fun factor

Methods:
- Internal testing with friends/family
- Beta testing with Madagascar community
- Online playtesting (itch.io)
```

#### 7.3 Final Polish
```
- Add screen shake for heavy hits
- Improve particle effects
- Fine-tune animation timing
- Balance damage values
- Add more sound effects
- Create trailer and screenshots
- Write credits and acknowledgments
```

---

### Phase 8: Release & Marketing 🚀
**Duration:** Week 19+

#### 8.1 Release Platforms
```
Primary Release:
- Itch.io (free or pay-what-you-want)
- GitHub (open-source code)

Future:
- Steam (with Steamworks integration)
- Epic Games Store
- Consoles (PlayStation, Xbox, Switch)
```

#### 8.2 Marketing Strategy
```
1. Build Community
   - Create Discord server
   - Social media (Twitter, Instagram, TikTok)
   - Share dev logs and behind-the-scenes

2. Content Creation
   - Gameplay trailers
   - Character reveal videos
   - Dev diaries
   - Cultural education posts

3. Press & Outreach
   - Submit to indie game sites (IndieDB, GameJolt)
   - Contact gaming YouTubers
   - Reach out to Malagasy cultural organizations
   - Submit to game festivals

4. Cultural Impact
   - Partner with Madagascar tourism board
   - Educational institutions
   - Martial arts communities
```

---

## 🛠️ Unity Setup Instructions

### Installation
1. Download Unity Hub
2. Install Unity 2022.3 LTS
3. Open the project folder `morengy-game/Unity`

### Required Packages
```
Via Package Manager (Window > Package Manager):
- Input System (for controller support)
- Cinemachine (camera system)
- TextMeshPro (UI text)
- Universal Render Pipeline (graphics)
- ProBuilder (level design)
```

### Project Settings
```
Edit > Project Settings:

Player:
- Company Name: Your Name
- Product Name: MORENGY
- Version: 0.1.0

Quality:
- Set to "High" for development
- Disable VSync if testing frame-independent code

Input Manager:
- Configure axes for movement (Horizontal, Vertical)
- Add custom axes for attacks if needed

Physics:
- Fixed Timestep: 0.02 (50Hz)
- Default Contact Offset: 0.01
```

---

## 🎨 Art & Animation Pipeline

### Recommended Workflow

**3D Modeling (Blender):**
1. Model character in T-pose
2. Keep poly count under 25k triangles
3. UV unwrap carefully
4. Export as FBX

**Texturing:**
1. Use PBR materials (Albedo, Normal, Metallic, Roughness)
2. 2048x2048 texture resolution
3. Compress textures in Unity

**Animation:**
1. Use Mixamo for free mocap
2. Or keyframe in Blender
3. Export animations as separate FBX files
4. Import to Unity and configure Animator Controller

---

## 🎵 Audio Implementation

### Unity Audio Setup
```csharp
// AudioManager.cs example structure

public class AudioManager : MonoBehaviour
{
    [Header("Music")]
    public AudioClip menuMusic;
    public AudioClip fightMusic;

    [Header("SFX")]
    public AudioClip lightPunch;
    public AudioClip heavyPunch;
    public AudioClip block;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    // Play SFX
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    // Dynamic music layers
    public void SetMusicIntensity(float intensity)
    {
        // Adjust volume or switch layers based on fight intensity
    }
}
```

### Integration Points
- Play hit SFX in `CombatSystem.OnHitLanded()`
- Play footstep sounds in animation events
- Trigger crowd reactions based on combo count
- Dynamic music based on health percentage

---

## 🧪 Testing Strategy

### Unit Testing
```csharp
// Example test for FighterStats
[Test]
public void TakeDamage_ReducesHealth()
{
    FighterStats stats = new FighterStats();
    stats.TakeDamage(25f);
    Assert.AreEqual(75f, stats.CurrentHealth);
}
```

### Playtesting Checklist
- [ ] All attacks connect properly
- [ ] Blocking reduces damage
- [ ] Dodging provides i-frames
- [ ] AI behaves naturally
- [ ] No infinite combos
- [ ] Stamina management feels fair
- [ ] Special moves are satisfying
- [ ] Frame rate stays at 60 FPS
- [ ] No game-breaking bugs

---

## 📊 Performance Targets

### Minimum Specs
- CPU: Intel i5 / AMD Ryzen 5
- GPU: GTX 1050 / Equivalent
- RAM: 4GB
- Storage: 2GB

### Optimization Goals
- 60 FPS on medium hardware
- < 100ms input latency
- < 500MB RAM usage
- Fast load times (<5s between fights)

---

## 🚀 Deployment Checklist

### Pre-Release
- [ ] All core features implemented
- [ ] No critical bugs
- [ ] Performance optimized
- [ ] UI fully functional
- [ ] Audio integrated
- [ ] Story mode complete (if applicable)
- [ ] At least 6 fighters
- [ ] 3 arenas

### Build Settings
```
File > Build Settings:
- Platform: PC, Mac & Linux Standalone
- Architecture: x86_64
- Compression: LZ4 (faster) or LZ4HC (smaller)
- Development Build: OFF (for release)
```

### Release Assets
- Game executable
- README with controls
- Soundtrack (optional download)
- Concept art gallery
- Credits document

---

## 📚 Resources & References

### Learning Unity
- Unity Learn: https://learn.unity.com
- Brackeys YouTube: Fighting game tutorials
- Game Dev Beginner: Unity C# guides

### Animation Resources
- Mixamo: Free mocap library
- OpenGameArt: Free game assets
- Sketchfab: 3D model references

### Malagasy Culture Research
- Morengy fight videos (YouTube)
- Madagascar cultural documentaries
- Interview local Morengy practitioners
- Study traditional music and instruments

---

## 🤝 Community & Support

### Get Involved
- **GitHub:** Contribute code and report issues
- **Discord:** Join development discussions
- **Social Media:** Follow for updates

### Contact
- **Developer:** Armand Judicael Ratombotiana
- **Email:** [Your email]
- **LinkedIn:** [Your LinkedIn]

---

## 📝 License & Credits

### License
MIT License - Free to use, modify, and distribute

### Credits
- **Concept & Development:** Armand Judicael Ratombotiana
- **Cultural Consultants:** Malagasy Morengy community
- **Music:** [TBD]
- **Special Thanks:** Madagascar martial arts practitioners

---

**"Morengy is not just a fight — it's our heritage."** 🇲🇬

Let's build something amazing together! 🥊
