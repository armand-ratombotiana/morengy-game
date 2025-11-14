# 🎯 Morengy Game - Implementation Roadmap

**Decision:** Fast to Market - Unity Premium Single-Player (4-5 months)
**Focus:** Complete single-player experience with local versus
**Date:** January 13, 2025

---

## 📋 Strategic Decision Summary

### ✅ Chosen Path: Unity Premium Single-Player

**Why This Path:**
1. **Existing Foundation:** 6,864 lines of quality C# code (52% complete)
2. **Exceptional AI Systems:** Production-ready learning and adaptation (90% complete)
3. **Faster Time to Market:** 4-5 months vs 12+ months for Unreal rebuild
4. **Lower Risk:** Proven technology stack and architecture
5. **Cultural Mission:** Focus on celebrating Malagasy heritage, not just tech showcase

**Strategic Tradeoffs Accepted:**
- ❌ Not using Unreal Engine 5 (Lumen, Nanite, native motion matching)
- ❌ Multiplayer deferred to post-launch DLC
- ❌ F2P conversion only if game proves successful
- ✅ Will upgrade URP → HDRP for better visuals
- ✅ Motion matching alternatives via Unity plugins/Mixamo
- ✅ Focus on what makes game unique: AI and cultural authenticity

---

## 🎯 Implementation Priorities

### Phase 1.5: Critical Infrastructure (Weeks 1-2) 🔴 HIGH PRIORITY

#### 1. HDRP Migration (Week 1)
**Goal:** Upgrade from URP to HDRP for AAA visual quality

**Tasks:**
- [ ] Install HDRP package via Unity Package Manager
- [ ] Convert project render pipeline settings
- [ ] Update materials to HDRP shaders
- [ ] Configure HDRP volume profiles
- [ ] Test all existing systems for compatibility
- [ ] Update lighting to take advantage of HDRP features

**Benefits:**
- Ray-traced reflections and shadows
- Better post-processing
- Volumetric lighting and fog
- Screen Space Global Illumination (SSGI)
- Closer to "AAA visual fidelity" goal

**Risks:**
- Performance impact (target optimization in Phase 7)
- Some scripts may need shader property updates

#### 2. Technical Architecture Documentation (Week 2)
**Goal:** Document all technical decisions and tradeoffs

**Tasks:**
- [ ] Create formal Technical Design Document (TDD)
- [ ] Document Unity vs Unreal decision rationale
- [ ] Explain HDRP choice and visual targets
- [ ] Document deferred features (multiplayer, F2P)
- [ ] Outline post-launch roadmap for online play
- [ ] Create architecture diagrams

**Deliverables:**
- `TECHNICAL_DESIGN_DOCUMENT.md` (comprehensive TDD)
- `ARCHITECTURE_DECISIONS.md` (ADR - Architecture Decision Records)

---

### Phase 2: Visual Assets & Animation (Weeks 3-6) 🟡 MEDIUM PRIORITY

#### 1. 3D Character Models (Weeks 3-4)
**Goal:** Create 4 initial fighters with Malagasy cultural authenticity

**Approach Options:**
- **Option A:** Outsource to 3D artist (Fiverr, ArtStation) - $400-800/fighter
- **Option B:** Use Blender + Adobe Fuse/Character Creator 4 - DIY
- **Option C:** Hybrid: Base models from Character Creator + custom Blender details

**Recommended: Option C**

**Tasks:**
- [ ] Create base body types in Character Creator 4
  - Lightweight striker physique (Diego Warrior)
  - Muscular brawler build (Nosy Be Champion)
  - Athletic balanced build (Mahajanga Veteran)
  - Technical lean build (Antananarivo Technician)
- [ ] Export to Blender for customization
- [ ] Add Malagasy cultural features:
  - Regional clothing (lambas, salaka)
  - Tattoos and tribal markings
  - Hairstyles (traditional and modern)
  - Facial features representing Madagascar regions
- [ ] Rig with Mixamo Auto-Rigger
- [ ] Import to Unity with optimized LODs

**Quality Targets:**
- 15K-25K polygons per character
- 2K texture maps (diffuse, normal, roughness, AO)
- Blend shapes for facial expressions

#### 2. Fighting Animations (Weeks 4-5)
**Goal:** 50+ animations per fighter covering all combat scenarios

**Animation Sources:**
- **Mixamo:** Free library for basic moves
- **Motion Capture Alternatives:** Rokoko Smartsuit (if budget allows)
- **Custom Animation:** Blender for signature moves

**Required Animation Sets:**

**Locomotion (8 animations):**
- Idle stance (fighting stance)
- Walk forward/backward
- Run forward
- Strafe left/right
- Crouch
- Jump

**Attacks (15 animations):**
- Light punch combo (3 variations)
- Heavy punch combo (2 variations)
- Low kicks (2 variations)
- High kicks (2 variations)
- Special moves (4 unique per fighter)
- Spinning attacks (2)

**Defense (8 animations):**
- Block high
- Block low
- Perfect block (special timing)
- Dodge left/right
- Dodge backward
- Roll recovery

**Grappling (12 animations):**
- Clinch enter/exit
- Takedown attempts (3 variations)
- Ground transitions (3)
- Submission attempts (2)
- Reversal escapes (3)

**Hit Reactions (10 animations):**
- Light hit reactions (head, body, legs)
- Heavy hit reactions (knockback)
- Stun/dazed state
- Knockdown (front, back)
- Get-up animations
- KO animation

**Tasks:**
- [ ] Download Mixamo animations for base moveset
- [ ] Blend and customize in Blender for Morengy style
- [ ] Create signature moves from scratch
- [ ] Implement motion matching alternative (Unity Kinematica or Animation Rigging)
- [ ] Set up blend trees in Unity Animator
- [ ] Configure inverse kinematics (IK) for foot placement
- [ ] Add root motion for realistic movement

#### 3. VFX Integration (Week 6)
**Goal:** HDRP-quality particle effects

**Tasks:**
- [ ] Create hit effects (sparks, blood spray - stylized not realistic)
- [ ] Impact VFX for different attack types
- [ ] Special move effects (fire, lightning, energy)
- [ ] Environmental effects (dust, water splashes)
- [ ] Screen distortion on heavy hits
- [ ] Implement using Unity VFX Graph (HDRP)
- [ ] Integrate with existing VFXManager.cs

---

### Phase 3: Combat Expansion (Weeks 7-9) 🔴 HIGH PRIORITY

#### 1. Grappling System (Weeks 7-8)
**Goal:** Implement UFC-style grappling mechanics

**New Script: `GrapplingSystem.cs`**

```csharp
public class GrapplingSystem : MonoBehaviour
{
    // States
    public enum GrappleState
    {
        None,
        Clinch,
        Takedown,
        Ground,
        Submission
    }

    // Core mechanics
    - Clinch initiation (close range proximity)
    - Takedown attempts with resistance
    - Ground positions (mount, guard, side control)
    - Submission mini-game (button mashing + timing)
    - Reversal mechanics
    - Stamina cost for grappling

    // Integration with existing systems
    - Links to FighterStats for stamina drain
    - Links to CombatSystem for damage
    - Links to AIBehavior for grappling AI
}
```

**Tasks:**
- [ ] Create GrapplingSystem.cs (estimated 400 lines)
- [ ] Implement clinch mechanics
- [ ] Add takedown system with defense
- [ ] Create ground game state machine
- [ ] Build submission system (QTE-style)
- [ ] Add AI grappling behavior to AIBehavior.cs
- [ ] Update InputManager for grapple controls
- [ ] Create grappling UI indicators

**Controls:**
- G key: Initiate grapple (when close)
- WASD: Position control during clinch
- J/K/L: Grapple attacks (punches, knees, elbows)
- Space: Escape/Reversal attempt

#### 2. Environmental Combat (Week 9)
**Goal:** Def Jam-style environmental interactions

**New Script: `EnvironmentalCombat.cs`**

```csharp
public class EnvironmentalCombat : MonoBehaviour
{
    // Arena hazards
    - Wall collision detection
    - Breakable objects (barrels, crates)
    - Ring boundaries (ropes in Diego Suarez ring)
    - Water hazards (Nosy Be beach)
    - Crowd interactions

    // Contextual finishers
    - Wall slam → extra damage
    - Object throw → projectile attack
    - Arena-specific moves
    - Environmental knockouts
}
```

**Tasks:**
- [ ] Create EnvironmentalCombat.cs (estimated 350 lines)
- [ ] Implement wall bounce physics
- [ ] Add destructible object system
- [ ] Create contextual finisher triggers
- [ ] Design 3 arena hazards per location
- [ ] Add crowd reaction system
- [ ] Integrate with CombatSystem for damage
- [ ] Create special VFX for environmental hits

**Arena-Specific Features:**

**Diego Suarez Harbor:**
- Rope boundaries (wall bounce like boxing ring)
- Ship crates (destructible)
- Water barrels (splash effect)

**Nosy Be Beach:**
- Sand slows movement slightly
- Palm trees (can be crashed into)
- Beach umbrellas (obstacles)

**Rural Zebu Arena:**
- Wooden fence boundaries
- Hay bales (destructible)
- Dust clouds on impact

---

### Phase 4: Audio & Music (Weeks 10-11) 🟡 MEDIUM PRIORITY

#### 1. Malagasy Music Integration (Week 10)
**Goal:** Authentic cultural soundtrack

**Approach:**
- **Option A:** Collaborate with Malagasy musicians (ideal)
- **Option B:** License existing Malagasy music
- **Option C:** Create fusion tracks with local producers

**Music Needed:**

**Menu Music (2 tracks):**
- Traditional valiha instrumental (main menu)
- Modern salegy remix (character select)

**Fight Music (6 tracks):**
- 2 per tier (Amateur/Regional, Professional/Champion)
- Dynamic layers that increase with fight intensity
- Blend traditional instruments (kabosy, jejy) with electronic beats

**Cultural Elements:**
- Valiha (bamboo tube zither)
- Kabosy (box guitar)
- Jejy (traditional lute)
- Salegy rhythm patterns
- Tsapiky influences from southwest

**Tasks:**
- [ ] Research Malagasy music producers
- [ ] Commission or license 8 music tracks
- [ ] Implement dynamic music layers in AudioManager
- [ ] Test intensity transitions during fights
- [ ] Add regional music variations per arena

#### 2. Combat Sound Effects (Week 11)
**Goal:** 3D positional impact sounds

**Sound Effects Needed (50+ sounds):**

**Strikes:**
- Light punch impacts (5 variations)
- Heavy punch impacts (5 variations)
- Kick impacts (body, legs, head - 8 variations)
- Block sounds (5 variations)
- Perfect block (special sound)

**Movement:**
- Footsteps (sand, wood, concrete - 6 variations)
- Dodge swishes (3)
- Jump/land sounds (2)

**Grappling:**
- Clinch grunts (4)
- Takedown impacts (3)
- Ground grappling (5)

**Special:**
- Special move charge-up (4)
- Special move impacts (4)
- Combo milestone sounds (3)

**Environmental:**
- Wall impacts (3)
- Object breaks (5)
- Crowd reactions (10 variations)

**Voice Acting:**
- Fighter grunts (20 per fighter)
- Pain reactions (10 per fighter)
- Victory taunts (5 per fighter)
- KO screams (2 per fighter)

**Tasks:**
- [ ] Record or license sound effects
- [ ] Implement 3D audio positioning
- [ ] Add voice acting (Malagasy voice actors preferred)
- [ ] Create dynamic crowd reactions
- [ ] Integrate with existing AudioManager

---

### Phase 5: Training Mode & Polish (Weeks 12-14) 🟡 MEDIUM PRIORITY

#### 1. Training Mode (Week 12)
**Goal:** Tutorial and practice mode with coach AI

**New Script: `TrainingMode.cs`**

```csharp
public class TrainingMode : MonoBehaviour
{
    // Features
    - Step-by-step tutorial (15 lessons)
    - Practice dummy with adjustable difficulty
    - Combo practice mode
    - Timing practice (perfect blocks, dodges)
    - Coach AI with tips and feedback
    - Performance analytics
}
```

**Tutorial Lessons:**
1. Basic movement and controls
2. Light attacks and combos
3. Heavy attacks
4. Blocking and perfect blocks
5. Dodging and evasion
6. Special attacks
7. Grappling basics
8. Takedowns and ground game
9. Environmental combat
10. Stamina management
11. Advanced combos
12. Counter-attacking
13. Reading opponent patterns
14. Fighting different AI personalities
15. Career mode preparation

**Tasks:**
- [ ] Create TrainingMode.cs (estimated 500 lines)
- [ ] Build tutorial sequence system
- [ ] Create coach AI (extends AIBehavior)
- [ ] Add practice dummy with hit markers
- [ ] Implement combo challenge mode
- [ ] Create training UI overlays
- [ ] Add performance feedback system

#### 2. Game Modes & UI (Week 13)
**Goal:** Complete menu system and game modes

**Main Menu System:**
- [ ] Create main menu scene
- [ ] Implement mode selection UI
- [ ] Add options/settings menu
- [ ] Create fighter selection screen
- [ ] Build arena selection screen
- [ ] Add credits and about screens

**Game Modes:**
- ✅ Career Mode (already implemented)
- [ ] Arcade Mode (ladder of 10 fights)
- [ ] Versus Mode (local 2-player)
- [ ] Training Mode (tutorials)
- [ ] Survival Mode (endless fights)
- [ ] Time Attack Mode (speed challenges)

**UI Polish:**
- [ ] Fighter stat comparison screen
- [ ] Move list viewer
- [ ] Achievement tracker UI
- [ ] Leaderboards (local high scores)
- [ ] Replay viewer
- [ ] Gallery (unlockable art/videos)

#### 3. Final Polish (Week 14)
**Goal:** Professional presentation

**Tasks:**
- [ ] Add screen transitions and animations
- [ ] Implement photo mode
- [ ] Create loading screens with tips
- [ ] Add victory cinematics
- [ ] Build result screen with stats breakdown
- [ ] Implement slow-motion on finishers
- [ ] Add dynamic camera angles for special moves
- [ ] Create fighter intro animations
- [ ] Polish all UI elements
- [ ] Add controller rumble support

---

### Phase 6: Content Expansion (Weeks 15-16) 🟢 LOW PRIORITY

#### 1. Fighter Roster Expansion (Week 15)
**Goal:** Expand from 4 to 8 fighters

**New Fighters (4 additional):**

**5. Toliara Desert Fighter**
- Region: Southwest Madagascar
- Style: Tsapiky dance-influenced (agile, rhythm-based)
- Archetype: Speed specialist
- Special: "Desert Whirlwind" spinning multi-kick

**6. Fianarantsoa Highland Warrior**
- Region: Central Highlands
- Style: Traditional Merina martial arts (disciplined, technical)
- Archetype: Technician
- Special: "Highland Fortress" defensive counter stance

**7. Toamasina Port Brawler**
- Region: East Coast
- Style: Street fighting mixed with port worker toughness
- Archetype: Powerhouse
- Special: "Dockyard Crusher" devastating throw

**8. Morondava Coastal Champion**
- Region: West Coast (Baobab Avenue)
- Style: Flowing movements inspired by ocean waves
- Archetype: Balanced
- Special: "Baobab Stance" regeneration move

**Tasks:**
- [ ] Create 3D models for 4 new fighters
- [ ] Animate signature moves for each
- [ ] Design unique stats and abilities
- [ ] Write backstories and lore
- [ ] Create fighter-specific voice lines
- [ ] Balance against existing roster
- [ ] Add to career mode progression

#### 2. Arena Expansion (Week 16)
**Goal:** 6 total arenas with unique features

**New Arenas (3 additional):**

**4. Antananarivo Royal Palace Arena**
- Setting: Historic royal palace courtyard
- Time: Sunset
- Features: Stone pillars, royal banners
- Hazard: Palace steps (height advantage)
- Music: Traditional ceremonial drums

**5. Tsarabanjina Island Paradise**
- Setting: Private island beach
- Time: Midday
- Features: Crystal clear water, luxury resort background
- Hazard: Sand affects stamina drain
- Music: Relaxed coastal salegy

**6. Ankarana Cave Arena**
- Setting: Underground limestone cave
- Time: Night (torches)
- Features: Stalactites, underground river
- Hazard: Slippery rocks
- Music: Echoing traditional chants

**Tasks:**
- [ ] Model 3 new arenas in Blender
- [ ] Texture with HDRP materials
- [ ] Add arena-specific lighting
- [ ] Implement unique hazards per arena
- [ ] Create environmental VFX
- [ ] Commission arena-specific music

---

### Phase 7: Testing & Balance (Weeks 17-18) 🔴 HIGH PRIORITY

#### 1. Technical Testing (Week 17)
**Goal:** Bug-free experience

**Testing Areas:**
- [ ] Combat system edge cases
- [ ] AI behavior in all scenarios
- [ ] Grappling transitions and edge cases
- [ ] Environmental collision detection
- [ ] UI functionality on all screens
- [ ] Save/load system reliability
- [ ] Performance profiling (60 FPS target)
- [ ] Memory leak detection
- [ ] Cross-input testing (keyboard, controller)

**Performance Optimization:**
- [ ] Optimize draw calls (batching)
- [ ] LOD system for characters and arenas
- [ ] Occlusion culling in arenas
- [ ] Optimize particle effects
- [ ] Audio pooling verification
- [ ] Reduce garbage collection spikes

**Target Specs:**
- Minimum: GTX 1060, i5-8400, 8GB RAM
- Recommended: RTX 3060, i7-10700, 16GB RAM
- 60 FPS @ 1080p (minimum spec)
- 144 FPS @ 1080p (recommended spec)

#### 2. Balance & Playtesting (Week 18)
**Goal:** Fair and fun gameplay

**Balance Testing:**
- [ ] Fighter stat balance (no dominant fighter)
- [ ] AI difficulty curve testing
- [ ] Career mode difficulty progression
- [ ] Damage scaling verification
- [ ] Stamina consumption rates
- [ ] Special move balance (risk/reward)
- [ ] Grappling effectiveness
- [ ] Environmental hazard damage

**Playtesting:**
- [ ] Internal testing (10+ hours per tester)
- [ ] External beta testing (50+ players)
- [ ] Feedback collection and analysis
- [ ] Iterate on balance issues
- [ ] Test all game modes thoroughly
- [ ] Verify achievement unlocks
- [ ] Test career mode completion

**Analytics to Track:**
- Win rates per fighter
- Most used moves
- Average match length
- AI difficulty distribution
- Player retention (career progress)
- Most popular game modes

---

### Phase 8: Launch Preparation (Weeks 19-20) 🔴 HIGH PRIORITY

#### 1. Build & Distribution (Week 19)
**Goal:** Cross-platform builds

**Platforms:**
- [ ] Windows 64-bit (primary)
- [ ] macOS (Intel + Apple Silicon)
- [ ] Linux (Ubuntu/SteamOS)

**Build Tasks:**
- [ ] Configure build settings for each platform
- [ ] Create installer packages
- [ ] Test builds on multiple machines
- [ ] Optimize build size (target < 2GB)
- [ ] Create auto-updater system
- [ ] Add crash reporting (Unity Cloud Diagnostics)

**Distribution Platforms:**
- [ ] Steam (primary) - $100 submission fee
- [ ] Itch.io (indie friendly) - free
- [ ] Epic Games Store (application required)
- [ ] GOG (DRM-free) - curated

#### 2. Marketing & Launch (Week 20)
**Goal:** Successful launch campaign

**Marketing Materials:**
- [ ] Create launch trailer (2 minutes)
  - Showcase all 8 fighters
  - Highlight cultural authenticity
  - Show AI learning system
  - Display environmental combat
  - Feature Malagasy music
- [ ] Take promotional screenshots (20+)
- [ ] Create press kit with assets
- [ ] Write press release
- [ ] Contact gaming press (IGN, Polygon, Kotaku)
- [ ] Reach out to fighting game influencers
- [ ] Post on Reddit (r/fightinggames, r/gamedev)
- [ ] Share on social media (Twitter, Facebook, Instagram)

**Website Updates:**
- [ ] Add purchase links to website
- [ ] Create launch announcement blog post
- [ ] Update gallery with final screenshots
- [ ] Add gameplay videos
- [ ] Enable contact form for press inquiries

**Pricing Strategy:**
- **Launch Price:** $19.99 USD
- **Discount:** 10% off first week ($17.99)
- **Justification:** Premium indie quality, cultural significance, no microtransactions

**Launch Day Checklist:**
- [ ] Deploy website updates
- [ ] Publish on Steam/Itch.io
- [ ] Send press releases
- [ ] Post on social media
- [ ] Contact influencers for reviews
- [ ] Monitor community feedback
- [ ] Be ready for hotfix patches

---

## 📊 Resource Requirements

### Development Team
**Minimum:**
- 1 Unity Developer (full-time) - YOU
- 1 3D Artist (contract/part-time) - 40 hours total
- 1 Animator (contract) - 80 hours total
- 1 Sound Designer (contract) - 40 hours total
- 1 Composer (commission) - 8 tracks

**Estimated Costs:**
- 3D Artist: $1,600 (8 fighters × $200/fighter)
- Animator: $2,400 (80 hours × $30/hour or Mixamo + customization)
- Sound Designer: $1,200 (40 hours × $30/hour)
- Music: $1,600 (8 tracks × $200/track)
- Software licenses: $500 (Character Creator, plugins)
- Asset store purchases: $300
- **Total Budget: ~$7,600**

### Software & Tools
- Unity 2022.3 LTS (free)
- Visual Studio (free)
- Blender (free)
- Character Creator 4 ($199 or subscription)
- Mixamo (free)
- Unity Asset Store plugins (~$300)
  - Kinematica or animation solution
  - Enhanced HDRP effects
  - Additional VFX packs

---

## 🎯 Success Metrics

### Technical Goals
- ✅ 60 FPS @ 1080p on recommended specs
- ✅ < 2 second loading times
- ✅ Zero critical bugs at launch
- ✅ 8 fully animated fighters
- ✅ 6 complete arenas
- ✅ 5 game modes functional

### Quality Goals
- ✅ 90%+ positive reviews on Steam
- ✅ Featured on indie game sites
- ✅ Cultural authenticity praised by Malagasy community
- ✅ AI system recognized as innovative
- ✅ Fighting game community engagement

### Business Goals
- 🎯 1,000 copies sold in first month
- 🎯 $20,000 revenue in first 3 months
- 🎯 Community of 500+ Discord members
- 🎯 10+ content creator videos
- 🎯 Featured on Steam's "New & Trending"

---

## 🔄 Post-Launch Roadmap (Months 6-12)

### Update 1.1 - Quality of Life (Month 6)
- Bug fixes and balance patches
- Additional achievements
- New cosmetic options (alternate costumes)
- Replay system enhancements

### Update 1.2 - Content Pack (Month 8)
- 2 new fighters
- 2 new arenas
- New game mode (Boss Rush)
- Additional music tracks

### Update 2.0 - Multiplayer (Month 10-12)
- Online versus with rollback netcode
- Ranked matchmaking
- Leaderboards
- Replay sharing
- Spectator mode

### Update 2.5 - Story Mode (Month 12+)
- Narrative campaign (10 chapters)
- Voice acting
- Cutscenes
- Character development arcs

---

## ✅ Key Deliverables by Phase

| Phase | Week | Deliverable | Priority |
|-------|------|-------------|----------|
| 1.5 | 1 | HDRP Migration Complete | 🔴 HIGH |
| 1.5 | 2 | Technical Documentation | 🔴 HIGH |
| 2 | 3-4 | 8 Fighter Models | 🟡 MEDIUM |
| 2 | 4-5 | 50+ Animations per Fighter | 🟡 MEDIUM |
| 2 | 6 | HDRP VFX Integration | 🟡 MEDIUM |
| 3 | 7-8 | Grappling System | 🔴 HIGH |
| 3 | 9 | Environmental Combat | 🔴 HIGH |
| 4 | 10 | Malagasy Music (8 tracks) | 🟡 MEDIUM |
| 4 | 11 | Combat SFX (50+ sounds) | 🟡 MEDIUM |
| 5 | 12 | Training Mode | 🟡 MEDIUM |
| 5 | 13 | All Game Modes | 🟡 MEDIUM |
| 5 | 14 | UI/UX Polish | 🟡 MEDIUM |
| 6 | 15 | 8 Fighters Total | 🟢 LOW |
| 6 | 16 | 6 Arenas Total | 🟢 LOW |
| 7 | 17 | Technical Testing | 🔴 HIGH |
| 7 | 18 | Balance & Playtesting | 🔴 HIGH |
| 8 | 19 | Cross-Platform Builds | 🔴 HIGH |
| 8 | 20 | Marketing & Launch | 🔴 HIGH |

---

## 🎉 Summary

This roadmap transforms the current prototype (52% complete) into a **premium single-player fighting game** in 20 weeks, focusing on:

1. **Visual Excellence** - HDRP + high-quality 3D assets
2. **Complete Combat** - Strikes + grappling + environmental
3. **Cultural Authenticity** - Malagasy music, art, and lore
4. **Exceptional AI** - Already industry-leading
5. **Premium Experience** - 8 fighters, 6 arenas, 5 game modes

**Launch Target:** June 2025
**Price Point:** $19.99 USD
**Platforms:** Steam, Itch.io, Epic, GOG

**Post-Launch:** Multiplayer and story mode as paid DLC or updates based on success.

---

**🥊 Let's make Morengy: The Spirit of the North a reality! 🥊**

*Celebrating Madagascar's martial art through world-class game design.*
