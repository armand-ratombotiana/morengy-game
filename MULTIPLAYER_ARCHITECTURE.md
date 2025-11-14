# 🌐 Multiplayer Architecture - Unity Rollback Netcode

**Project:** Morengy - The Spirit of the North
**Date:** January 13, 2025
**Decision:** Include online multiplayer at launch

---

## 📋 Strategic Pivot: Multiplayer at Launch

### Decision Update
**Original Plan:** Single-player first, multiplayer post-launch DLC
**New Plan:** Online multiplayer included at launch

**Timeline Impact:** 4-5 months → **8-10 months**
**Budget Impact:** +$3,000-5,000 (backend services, testing, networking specialist)

---

## 🏗️ Architecture Overview

### Technology Stack

**Networking Layer:**
- **Unity Netcode for GameObjects** (formerly MLAPI) - Foundation
- **GGPO-style Rollback** - Custom implementation for fighting game precision
- **Unity Transport** - Low-level networking (UDP)

**Backend Services:**
- **PlayFab** - Authentication, matchmaking, leaderboards ($0-200/month)
- **Photon Voice** (optional) - Voice chat ($95/month for 100 CCU)
- **Unity Cloud** - Analytics and crash reporting

**Infrastructure:**
- **Peer-to-Peer** for matches (no dedicated servers needed)
- **PlayFab relay** for NAT traversal
- **Discord RPC** for social integration

### Why This Stack?

**Unity Netcode for GameObjects:**
- ✅ Native Unity integration
- ✅ Free and open-source
- ✅ Lower latency than Mirror
- ✅ Good documentation

**Custom Rollback vs Library:**
- ✅ Full control over determinism
- ✅ Optimized for our specific combat system
- ✅ No licensing fees
- ❌ More development time (3-4 weeks)

**PlayFab vs Custom Backend:**
- ✅ $0 up to 100K users
- ✅ Handles authentication, matchmaking, leaderboards
- ✅ No server maintenance
- ✅ Scales automatically
- ❌ $200/month after 100K users (acceptable)

---

## 🎮 Rollback Netcode Implementation

### What is Rollback Netcode?

Fighting games require **frame-perfect precision**. Traditional netcode adds input delay (50-100ms). Rollback netcode:

1. **Predicts** opponent inputs locally
2. **Confirms** actual inputs when they arrive
3. **Rolls back** if prediction was wrong
4. **Resimulates** from correct state

**Result:** Near-zero input delay, smooth online play up to 150ms ping

### Requirements for Rollback

#### 1. Deterministic Simulation
**Problem:** Unity's physics is non-deterministic (float precision, random seeds)

**Solution:** Custom deterministic layer

```csharp
// Replace UnityEngine.Random with deterministic random
public class DeterministicRandom
{
    private uint seed;

    public void SetSeed(uint newSeed)
    {
        seed = newSeed;
    }

    public float Range(float min, float max)
    {
        seed = (seed * 1103515245 + 12345) & 0x7fffffff;
        float normalized = (float)seed / 0x7fffffff;
        return min + (max - min) * normalized;
    }
}
```

**Refactor Needed:**
- Replace all `Random.Range()` calls
- Replace all `Random.value` calls
- Use fixed-point math for critical calculations
- Remove all `Time.deltaTime` dependencies (use fixed timestep)

#### 2. Game State Snapshots

```csharp
[System.Serializable]
public struct GameStateSnapshot
{
    public int frame;

    // Fighter 1 state
    public Vector3 p1Position;
    public Quaternion p1Rotation;
    public float p1Health;
    public float p1Stamina;
    public float p1SpecialMeter;
    public FighterAnimationState p1AnimState;
    public GrappleState p1GrappleState;

    // Fighter 2 state
    public Vector3 p2Position;
    public Quaternion p2Rotation;
    public float p2Health;
    public float p2Stamina;
    public float p2SpecialMeter;
    public FighterAnimationState p2AnimState;
    public GrappleState p2GrappleState;

    // Match state
    public int currentRound;
    public float roundTimer;
    public int p1Score;
    public int p2Score;
}
```

**Snapshot Strategy:**
- Save state every frame (60 FPS)
- Keep last 60 frames in buffer (1 second rollback window)
- Compress data to minimize memory (~200 bytes per frame = 12KB total)

#### 3. Input Prediction & Confirmation

```csharp
public struct PlayerInput
{
    public int frame;
    public byte buttons; // Bitfield for 8 buttons
    public Vector2 movement; // Fixed-point 16.16

    public bool IsLightAttack => (buttons & (1 << 0)) != 0;
    public bool IsHeavyAttack => (buttons & (1 << 1)) != 0;
    public bool IsSpecialAttack => (buttons & (1 << 2)) != 0;
    public bool IsBlock => (buttons & (1 << 3)) != 0;
    public bool IsDodge => (buttons & (1 << 4)) != 0;
    public bool IsGrapple => (buttons & (1 << 5)) != 0;
}

public class RollbackManager
{
    private const int ROLLBACK_WINDOW = 60; // 1 second at 60 FPS

    private GameStateSnapshot[] stateHistory;
    private PlayerInput[] localInputHistory;
    private PlayerInput[] remoteInputHistory;

    private int currentFrame = 0;
    private int confirmedFrame = 0; // Last frame with confirmed remote input

    public void Update()
    {
        // Get local input
        PlayerInput localInput = InputManager.GetCurrentInput();
        localInputHistory[currentFrame % ROLLBACK_WINDOW] = localInput;

        // Send local input to remote
        NetworkManager.SendInput(localInput);

        // Check for remote input
        if (NetworkManager.HasRemoteInput(currentFrame, out PlayerInput remoteInput))
        {
            // Confirmed input received
            remoteInputHistory[currentFrame % ROLLBACK_WINDOW] = remoteInput;

            // Check if prediction was wrong
            if (confirmedFrame < currentFrame - 1)
            {
                // Need to rollback
                Rollback(confirmedFrame + 1);
            }

            confirmedFrame = currentFrame;
        }
        else
        {
            // Predict remote input (repeat last input)
            remoteInput = remoteInputHistory[(currentFrame - 1) % ROLLBACK_WINDOW];
            remoteInputHistory[currentFrame % ROLLBACK_WINDOW] = remoteInput;
        }

        // Simulate frame
        SimulateFrame(localInput, remoteInput);

        // Save snapshot
        stateHistory[currentFrame % ROLLBACK_WINDOW] = CaptureGameState();

        currentFrame++;
    }

    private void Rollback(int toFrame)
    {
        // Restore state
        RestoreGameState(stateHistory[toFrame % ROLLBACK_WINDOW]);

        // Resimulate all frames up to current
        for (int frame = toFrame; frame < currentFrame; frame++)
        {
            PlayerInput localInput = localInputHistory[frame % ROLLBACK_WINDOW];
            PlayerInput remoteInput = remoteInputHistory[frame % ROLLBACK_WINDOW];

            SimulateFrame(localInput, remoteInput);
            stateHistory[frame % ROLLBACK_WINDOW] = CaptureGameState();
        }
    }
}
```

---

## 🔧 System Refactors Required

### 1. Combat System - Deterministic

**Current Issues:**
```csharp
// ❌ Non-deterministic
float damage = Random.Range(baseDamage * 0.9f, baseDamage * 1.1f);

// ❌ Uses Time.deltaTime
stamina += staminaRegenRate * Time.deltaTime;

// ❌ Float precision issues
if (distance < attackRange) // Could desync
```

**Refactored:**
```csharp
// ✅ Deterministic
float damage = DeterministicRandom.Range(baseDamage * 0.9f, baseDamage * 1.1f);

// ✅ Fixed timestep
stamina += staminaRegenRate * FIXED_DELTA_TIME; // 1/60 = 0.01666f

// ✅ Fixed-point math
if (FixedPoint.Distance(posA, posB) < attackRange)
```

### 2. Fighter Controller - Deterministic Movement

**Replace CharacterController** with custom kinematic movement:
```csharp
public class NetworkedFighterController : MonoBehaviour
{
    private Vector3 velocity;
    private const float FIXED_DELTA = 1f / 60f;

    public void Move(Vector2 input)
    {
        // Deterministic movement
        Vector3 moveDir = new Vector3(input.x, 0, input.y);
        velocity = moveDir * moveSpeed;

        // Manual position update (no CharacterController)
        transform.position += velocity * FIXED_DELTA;
    }
}
```

### 3. AI Behavior - Optional for Online

**Key Decision:** AI only runs in local/training modes, not online versus

```csharp
public class AIBehavior : MonoBehaviour
{
    private void Update()
    {
        // Skip AI in online matches
        if (NetworkManager.IsOnlineMatch) return;

        // Regular AI logic
        // ...
    }
}
```

### 4. Animation Synchronization

**Challenge:** Animations must be deterministic

**Solution:** Sync animation states, not timings
```csharp
public struct FighterAnimationState
{
    public int currentStateHash; // Animator.StringToHash("Idle")
    public float normalizedTime; // 0.0 - 1.0 within animation
}

public void SyncAnimation(FighterAnimationState state)
{
    animator.Play(state.currentStateHash, 0, state.normalizedTime);
}
```

---

## 🎯 Matchmaking System

### PlayFab Integration

**Features:**
- Skill-based matchmaking (MMR)
- Regional matching (minimize ping)
- Ranked and casual queues
- Party system (invite friends)

**Implementation:**

```csharp
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;

public class MatchmakingManager : MonoBehaviour
{
    public void StartMatchmaking(MatchmakingMode mode)
    {
        var request = new CreateMatchmakingTicketRequest
        {
            CreatorId = PlayFabSettings.staticPlayer.PlayFabId,
            GiveUpAfterSeconds = 120, // 2 minute timeout
            QueueName = mode == MatchmakingMode.Ranked ? "Ranked" : "Casual",

            // Include player skill rating
            CreatorAttributes = new MatchmakingPlayerAttributes
            {
                DataObject = new
                {
                    skill = PlayerProfile.Instance.GetMMR(),
                    region = GetRegion()
                }
            }
        };

        PlayFabMultiplayerAPI.CreateMatchmakingTicket(request, OnTicketCreated, OnError);
    }

    private void OnTicketCreated(CreateMatchmakingTicketResult result)
    {
        Debug.Log($"Ticket created: {result.TicketId}");
        StartCoroutine(PollTicketStatus(result.TicketId));
    }

    private IEnumerator PollTicketStatus(string ticketId)
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);

            var request = new GetMatchmakingTicketRequest { TicketId = ticketId };
            PlayFabMultiplayerAPI.GetMatchmakingTicket(request, OnTicketStatus, OnError);
        }
    }

    private void OnTicketStatus(GetMatchmakingTicketResult result)
    {
        if (result.Status == "Matched")
        {
            // Match found! Get opponent details
            string opponentId = result.MatchId;
            ConnectToMatch(opponentId);
        }
        else if (result.Status == "Canceled")
        {
            Debug.Log("Matchmaking canceled");
        }
    }
}
```

### MMR (Matchmaking Rating) System

**Simple ELO-based:**
```csharp
public class MMRSystem
{
    private const int STARTING_MMR = 1000;
    private const int K_FACTOR = 32; // Rating change speed

    public int CalculateNewMMR(int currentMMR, int opponentMMR, bool won)
    {
        // Expected score
        float expected = 1f / (1f + Mathf.Pow(10, (opponentMMR - currentMMR) / 400f));

        // Actual score
        float actual = won ? 1f : 0f;

        // New rating
        int change = Mathf.RoundToInt(K_FACTOR * (actual - expected));
        return currentMMR + change;
    }
}
```

**Matchmaking Rules:**
- Search for ±100 MMR
- Expand to ±200 MMR after 30 seconds
- Expand to ±400 MMR after 60 seconds
- Give up after 120 seconds

---

## 📡 Network Protocol

### Message Types

```csharp
public enum MessageType : byte
{
    Input = 1,           // Player input
    StateSync = 2,       // Full state snapshot (lobby)
    MatchStart = 3,      // Match begin
    MatchEnd = 4,        // Match result
    Ping = 5,            // Connection test
    Disconnect = 6       // Player left
}

[System.Serializable]
public struct NetworkMessage
{
    public MessageType type;
    public int frame;
    public byte[] data;
}
```

### Bandwidth Optimization

**Input Messages:** ~12 bytes/frame × 60 FPS = 720 bytes/second = **5.76 Kbps**
**State Sync:** 200 bytes × occasional = minimal
**Total:** ~6-8 Kbps per player (extremely low)

**Optimization Techniques:**
- Delta compression (only send changed inputs)
- Huffman encoding for common inputs
- Input buffering (send 3 frames at once)

---

## 🛡️ Anti-Cheat Measures

### Client-Side Validation

```csharp
public class CheatDetection
{
    private const float MAX_HEALTH = 100f;
    private const float MAX_STAMINA = 100f;
    private const float MAX_SPEED = 10f;

    public bool ValidateGameState(GameStateSnapshot state)
    {
        // Health bounds
        if (state.p1Health > MAX_HEALTH || state.p2Health > MAX_HEALTH)
            return false;

        // Stamina bounds
        if (state.p1Stamina > MAX_STAMINA || state.p2Stamina > MAX_STAMINA)
            return false;

        // Speed hacks (position delta check)
        float distance = Vector3.Distance(state.p1Position, lastState.p1Position);
        float maxDistance = MAX_SPEED * FIXED_DELTA * 2; // 2x for safety
        if (distance > maxDistance)
            return false;

        return true;
    }
}
```

### Server-Side Validation (PlayFab Cloud Scripts)

```javascript
// PlayFab Cloud Script
handlers.ValidateMatchResult = function(args, context) {
    var player1Id = args.player1Id;
    var player2Id = args.player2Id;
    var winner = args.winner;
    var player1Stats = args.player1Stats;
    var player2Stats = args.player2Stats;

    // Validate stats are within reasonable bounds
    if (player1Stats.damageDealt > 500) return { valid: false, reason: "Impossible damage" };
    if (player1Stats.comboHits > 50) return { valid: false, reason: "Impossible combo" };

    // Update MMR
    var player1Data = server.GetUserData({ PlayFabId: player1Id });
    var player2Data = server.GetUserData({ PlayFabId: player2Id });

    var player1MMR = player1Data.Data.MMR ? parseInt(player1Data.Data.MMR.Value) : 1000;
    var player2MMR = player2Data.Data.MMR ? parseInt(player2Data.Data.MMR.Value) : 1000;

    // Calculate new MMR (ELO)
    // ... (ELO calculation)

    // Save new MMR
    server.UpdateUserData({
        PlayFabId: player1Id,
        Data: { MMR: newPlayer1MMR.toString() }
    });

    return { valid: true, newMMR1: newPlayer1MMR, newMMR2: newPlayer2MMR };
}
```

---

## 🎨 UI/UX for Multiplayer

### Lobby System

**Features:**
- Quick match (automatic matchmaking)
- Ranked match
- Create private lobby (invite friend)
- Join friend's lobby
- Training mode (local AI)

**UI Flow:**
```
Main Menu
  └─> Online
       ├─> Quick Match → Matchmaking → Fight
       ├─> Ranked → Matchmaking → Fight
       ├─> Private Lobby → Create/Join → Fight
       └─> Training → Local AI Fight
```

### Connection Quality Indicator

```csharp
public class ConnectionDisplay : MonoBehaviour
{
    [SerializeField] private Image qualityIcon;
    [SerializeField] private Text pingText;

    public void UpdateConnectionQuality(float ping)
    {
        pingText.text = $"{ping:F0}ms";

        if (ping < 50)
        {
            qualityIcon.color = Color.green; // Excellent
        }
        else if (ping < 100)
        {
            qualityIcon.color = Color.yellow; // Good
        }
        else if (ping < 150)
        {
            qualityIcon.color = Color.orange; // Playable
        }
        else
        {
            qualityIcon.color = Color.red; // Poor
        }
    }
}
```

### Rematch System

```csharp
public class RematchManager : MonoBehaviour
{
    public void RequestRematch()
    {
        NetworkManager.SendMessage(MessageType.RematchRequest);
        ShowWaitingForOpponent();
    }

    public void OnRematchRequestReceived()
    {
        ShowRematchPrompt(); // "Opponent wants rematch. Accept?"
    }

    public void AcceptRematch()
    {
        NetworkManager.SendMessage(MessageType.RematchAccept);
        StartNewMatch();
    }
}
```

---

## 📊 Analytics & Telemetry

### Metrics to Track

**Match Data:**
- Match duration
- Damage dealt by each player
- Combos landed
- Special moves used
- Grapples attempted
- Winner/loser

**Network Data:**
- Average ping
- Packet loss
- Rollback frequency
- Disconnects

**Player Data:**
- Win/loss ratio
- Favorite fighter
- Ranked progression
- Play time

### PlayFab Analytics Integration

```csharp
using PlayFab;
using PlayFab.ClientModels;

public class AnalyticsManager : MonoBehaviour
{
    public void LogMatchResult(MatchResult result)
    {
        var request = new WritePlayerEventRequest
        {
            EventName = "match_completed",
            Body = new Dictionary<string, object>
            {
                { "winner", result.winnerId },
                { "loser", result.loserId },
                { "duration", result.duration },
                { "rounds", result.totalRounds },
                { "fighter1", result.fighter1Name },
                { "fighter2", result.fighter2Name },
                { "avgPing", result.averagePing },
                { "rollbacks", result.rollbackCount }
            }
        };

        PlayFabClientAPI.WritePlayerEvent(request, OnEventLogged, OnError);
    }
}
```

---

## 🔒 Security Considerations

### Input Validation
- Clamp all movement inputs to [-1, 1]
- Validate button inputs (only allowed buttons)
- Rate limit inputs (max 60/second)

### State Validation
- Both clients validate game state
- Mismatch = disconnect and report
- Server validates final result

### DDoS Protection
- Use PlayFab relay (not direct IP)
- Rate limit connection attempts
- Implement reconnection grace period

---

## 🚀 Deployment Strategy

### Beta Testing

**Phase 1: Closed Alpha** (Week 24-25)
- 10-20 testers
- Test rollback netcode stability
- Measure rollback frequency
- Test matchmaking

**Phase 2: Open Beta** (Week 26-27)
- 100-500 players
- Stress test matchmaking
- Gather feedback on netcode quality
- Monitor server costs

**Phase 3: Launch** (Week 28+)
- Full release with online multiplayer
- Monitor and adjust MMR algorithm
- Fix any critical bugs
- Scale PlayFab usage based on demand

### Rollout Plan

**Soft Launch:**
- Release on itch.io first (smaller audience)
- Monitor for 1 week
- Fix critical issues

**Full Launch:**
- Release on Steam
- Marketing campaign
- Prepare for potential server load

---

## 💰 Cost Breakdown (Multiplayer)

### Development Costs
- Networking specialist (80 hours × $50/hour): **$4,000**
- Extended testing (2 extra weeks): **$0** (your time)
- Playtesting (50 beta testers): **$0** (volunteers)

### Monthly Operating Costs
**Year 1 (0-1,000 concurrent users):**
- PlayFab: **$0** (free tier)
- Unity Cloud: **$0** (free tier)
- Domain/hosting: **$10**/month

**Year 2 (1,000-10,000 CCU):**
- PlayFab: **$200**/month (after 100K users)
- Unity Cloud: **$0** (free tier sufficient)
- Total: **$210/month = $2,520/year**

**Total First Year:** $4,000 (dev) + $120 (hosting) = **$4,120**

---

## ⏱️ Updated Timeline

### Extended Development: **8-10 months** (20 weeks → 28 weeks)

**Added Phases:**

**Phase 3.5: Rollback Netcode (Weeks 10-13)** - 4 weeks
- Deterministic combat refactor
- Snapshot system
- Rollback manager
- Input prediction

**Phase 5.5: Matchmaking (Weeks 15-16)** - 2 weeks
- PlayFab integration
- MMR system
- Lobby UI
- Connection quality display

**Phase 7.5: Network Testing (Weeks 23-24)** - 2 weeks
- Closed alpha
- Netcode stress testing
- Ping simulation testing
- Rollback tuning

**Phase 8.5: Open Beta (Weeks 25-26)** - 2 weeks
- Public beta
- Matchmaking refinement
- Balance adjustments
- Bug fixes

**Total:** 28 weeks = **7 months** from start

---

## ✅ Deliverables Checklist

### Network Layer
- [ ] Unity Netcode for GameObjects integrated
- [ ] Custom rollback system implemented
- [ ] Deterministic combat system
- [ ] Input prediction & confirmation
- [ ] State snapshot system

### Backend
- [ ] PlayFab account system
- [ ] Matchmaking integration
- [ ] MMR/ELO system
- [ ] Leaderboards
- [ ] Anti-cheat validation

### UI
- [ ] Online menu with modes
- [ ] Matchmaking UI with timer
- [ ] Lobby system
- [ ] Connection quality indicator
- [ ] Rematch system

### Testing
- [ ] 50+ hours netcode testing
- [ ] Closed alpha (10-20 testers)
- [ ] Open beta (100+ players)
- [ ] Performance profiling
- [ ] Anti-cheat validation

---

## 🎯 Success Metrics

### Technical Goals
- ✅ Rollback working up to 150ms ping
- ✅ < 5 rollback frames per match on good connections
- ✅ 99.5% uptime
- ✅ < 30 second matchmaking time

### Player Experience
- ✅ 90%+ report smooth online play
- ✅ Active player base (100+ concurrent)
- ✅ Ranked ladder with 500+ players
- ✅ Positive reviews mention netcode quality

---

**This multiplayer implementation will position Morengy as a legitimate competitive fighting game with exceptional netcode quality.**

🥊 **Ready to compete online!** 🥊
