using UnityEngine;
using System.Collections;
using Morengy.Character;
using Morengy.Combat;

namespace Morengy.Managers
{
    /// <summary>
    /// Main game manager controlling match flow, rounds, and win conditions.
    /// Implements UFC-style round system with best-of-3 rounds.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Fighters")]
        [SerializeField] private FighterStats player1;
        [SerializeField] private FighterStats player2;
        [SerializeField] private Transform player1SpawnPoint;
        [SerializeField] private Transform player2SpawnPoint;

        [Header("Match Settings")]
        [SerializeField] private int maxRounds = 3;
        [SerializeField] private float roundDuration = 120f; // 2 minutes
        [SerializeField] private float restPeriod = 30f;
        [SerializeField] private float countdownTime = 3f;

        [Header("Match State")]
        [SerializeField] private int currentRound = 1;
        [SerializeField] private int player1RoundsWon = 0;
        [SerializeField] private int player2RoundsWon = 0;
        [SerializeField] private float roundTimer;

        // State
        private MatchState currentState = MatchState.PreMatch;
        private bool matchInProgress = false;

        // Events
        public event System.Action<int> OnRoundStart;
        public event System.Action<FighterStats> OnRoundEnd;
        public event System.Action<FighterStats> OnMatchEnd;
        public event System.Action<float> OnRoundTimerUpdate;
        public event System.Action<string> OnCountdown;

        // Singleton
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Validate references
            if (player1 == null || player2 == null)
            {
                Debug.LogError("GameManager missing fighter references!");
                enabled = false;
            }
        }

        private void Start()
        {
            // Subscribe to fighter knockout events
            player1.OnFighterKnockedOut += () => OnFighterKnockedOut(player2);
            player2.OnFighterKnockedOut += () => OnFighterKnockedOut(player1);

            // Start match
            StartCoroutine(StartMatchSequence());
        }

        private void Update()
        {
            if (currentState == MatchState.RoundActive)
            {
                UpdateRoundTimer();
            }
        }

        #region Match Flow

        /// <summary>
        /// Start match sequence with countdown
        /// </summary>
        private IEnumerator StartMatchSequence()
        {
            currentState = MatchState.PreMatch;

            // Position fighters at spawn points
            PositionFighters();

            // Reset stats
            player1.ResetForNewMatch();
            player2.ResetForNewMatch();

            yield return new WaitForSeconds(1f);

            // Start first round
            StartCoroutine(StartRoundSequence());
        }

        /// <summary>
        /// Start round with countdown
        /// </summary>
        private IEnumerator StartRoundSequence()
        {
            currentState = MatchState.PreRound;

            OnRoundStart?.Invoke(currentRound);

            // Countdown
            yield return StartCoroutine(CountdownSequence());

            // Start round
            currentState = MatchState.RoundActive;
            matchInProgress = true;
            roundTimer = roundDuration;

            // Enable fighter controls
            EnableFighters(true);
        }

        /// <summary>
        /// Countdown before round starts
        /// </summary>
        private IEnumerator CountdownSequence()
        {
            DisableFighters();

            for (int i = (int)countdownTime; i > 0; i--)
            {
                OnCountdown?.Invoke(i.ToString());
                yield return new WaitForSeconds(1f);
            }

            OnCountdown?.Invoke("FIGHT!");
            yield return new WaitForSeconds(0.5f);
        }

        /// <summary>
        /// End current round
        /// </summary>
        private IEnumerator EndRoundSequence(FighterStats winner)
        {
            currentState = MatchState.PostRound;
            matchInProgress = false;

            // Disable fighters
            DisableFighters();

            // Update round wins
            if (winner == player1)
                player1RoundsWon++;
            else if (winner == player2)
                player2RoundsWon++;

            OnRoundEnd?.Invoke(winner);

            Debug.Log($"Round {currentRound} winner: {winner.FighterName}");

            // Check if match is over
            if (IsMatchOver())
            {
                yield return new WaitForSeconds(2f);
                EndMatch(DetermineMatchWinner());
                yield break;
            }

            // Rest period
            yield return new WaitForSeconds(restPeriod);

            // Prepare for next round
            currentRound++;
            PrepareNextRound();

            yield return new WaitForSeconds(1f);

            // Start next round
            StartCoroutine(StartRoundSequence());
        }

        /// <summary>
        /// End match and declare winner
        /// </summary>
        private void EndMatch(FighterStats winner)
        {
            currentState = MatchState.MatchEnd;

            OnMatchEnd?.Invoke(winner);

            Debug.Log($"MATCH WINNER: {winner.FighterName}!");
            Debug.Log($"Final Score - {player1.FighterName}: {player1RoundsWon} | {player2.FighterName}: {player2RoundsWon}");
        }

        #endregion

        #region Round Management

        /// <summary>
        /// Update round timer
        /// </summary>
        private void UpdateRoundTimer()
        {
            roundTimer -= Time.deltaTime;
            OnRoundTimerUpdate?.Invoke(roundTimer);

            if (roundTimer <= 0)
            {
                // Time's up - determine winner by health
                FighterStats winner = DetermineRoundWinnerByHealth();
                StartCoroutine(EndRoundSequence(winner));
            }
        }

        /// <summary>
        /// Prepare fighters for next round
        /// </summary>
        private void PrepareNextRound()
        {
            // Position fighters
            PositionFighters();

            // Partial stamina restore (60%)
            player1.PrepareForNewRound();
            player2.PrepareForNewRound();

            // Health and special meter carry over
        }

        /// <summary>
        /// Position fighters at spawn points
        /// </summary>
        private void PositionFighters()
        {
            if (player1SpawnPoint != null)
            {
                player1.transform.position = player1SpawnPoint.position;
                player1.transform.rotation = player1SpawnPoint.rotation;
            }

            if (player2SpawnPoint != null)
            {
                player2.transform.position = player2SpawnPoint.position;
                player2.transform.rotation = player2SpawnPoint.rotation;
            }
        }

        #endregion

        #region Win Conditions

        /// <summary>
        /// Called when a fighter is knocked out
        /// </summary>
        private void OnFighterKnockedOut(FighterStats winner)
        {
            if (currentState != MatchState.RoundActive) return;

            StartCoroutine(EndRoundSequence(winner));
        }

        /// <summary>
        /// Check if match is over (best of 3)
        /// </summary>
        private bool IsMatchOver()
        {
            // Best of 3: first to win 2 rounds
            return player1RoundsWon >= 2 || player2RoundsWon >= 2;
        }

        /// <summary>
        /// Determine round winner by health (time decision)
        /// </summary>
        private FighterStats DetermineRoundWinnerByHealth()
        {
            return player1.HealthPercentage > player2.HealthPercentage ? player1 : player2;
        }

        /// <summary>
        /// Determine overall match winner
        /// </summary>
        private FighterStats DetermineMatchWinner()
        {
            return player1RoundsWon > player2RoundsWon ? player1 : player2;
        }

        #endregion

        #region Fighter Control

        /// <summary>
        /// Enable/disable fighter controls
        /// </summary>
        private void EnableFighters(bool enable)
        {
            // Enable/disable InputManager for player
            var player1Input = player1.GetComponent<Morengy.Core.InputManager>();
            if (player1Input != null)
                player1Input.enabled = enable;

            // Enable/disable AI for opponent
            var player2AI = player2.GetComponent<Morengy.AI.AIBehavior>();
            if (player2AI != null)
                player2AI.enabled = enable;
        }

        /// <summary>
        /// Disable all fighter controls
        /// </summary>
        private void DisableFighters()
        {
            EnableFighters(false);
        }

        #endregion

        #region Public API

        /// <summary>
        /// Restart match
        /// </summary>
        public void RestartMatch()
        {
            // Reset state
            currentRound = 1;
            player1RoundsWon = 0;
            player2RoundsWon = 0;

            // Restart match
            StartCoroutine(StartMatchSequence());
        }

        /// <summary>
        /// Pause match
        /// </summary>
        public void PauseMatch()
        {
            Time.timeScale = 0f;
            currentState = MatchState.Paused;
        }

        /// <summary>
        /// Resume match
        /// </summary>
        public void ResumeMatch()
        {
            Time.timeScale = 1f;
            currentState = MatchState.RoundActive;
        }

        /// <summary>
        /// Get current round
        /// </summary>
        public int GetCurrentRound() => currentRound;

        /// <summary>
        /// Get round timer
        /// </summary>
        public float GetRoundTimer() => roundTimer;

        /// <summary>
        /// Get match state
        /// </summary>
        public MatchState GetMatchState() => currentState;

        #endregion

        #region Debug

#if UNITY_EDITOR
        [ContextMenu("Skip to Next Round")]
        private void DebugSkipToNextRound()
        {
            if (Application.isPlaying)
            {
                roundTimer = 0f;
            }
        }

        [ContextMenu("End Match")]
        private void DebugEndMatch()
        {
            if (Application.isPlaying)
            {
                player1RoundsWon = 2;
                EndMatch(player1);
            }
        }
#endif

        #endregion
    }

    /// <summary>
    /// Match state enumeration
    /// </summary>
    public enum MatchState
    {
        PreMatch,
        PreRound,
        RoundActive,
        PostRound,
        MatchEnd,
        Paused
    }
}
