using UnityEngine;
using System.Collections;
using Morengy.Character;
using Morengy.Core;

namespace Morengy.Combat
{
    /// <summary>
    /// Grappling system implementing UFC-style clinch, takedowns, and ground game.
    /// Integrates with existing combat system for complete MMA-style fighting.
    /// </summary>
    [RequireComponent(typeof(FighterStats))]
    [RequireComponent(typeof(CombatSystem))]
    public class GrapplingSystem : MonoBehaviour
    {
        [Header("Grapple Settings")]
        [SerializeField] private float grappleRange = 2f;
        [SerializeField] private float clinchBreakThreshold = 30f; // Stamina to break clinch
        [SerializeField] private float takedownStaminaCost = 40f;
        [SerializeField] private float submissionEscapeWindow = 1.5f;

        [Header("Damage Settings")]
        [SerializeField] private float clinchKneeDamage = 8f;
        [SerializeField] private float clinchElbowDamage = 12f;
        [SerializeField] private float groundPoundDamage = 6f;
        [SerializeField] private float submissionDamage = 15f;

        [Header("Timing Windows")]
        [SerializeField] private float takedownDefenseWindow = 0.5f;
        [SerializeField] private float reversalWindow = 0.3f;
        [SerializeField] private float escapeWindow = 1.0f;

        // Components
        private FighterStats stats;
        private CombatSystem combatSystem;
        private FighterController controller;

        // State tracking
        private GrappleState currentState = GrappleState.None;
        private GroundPosition currentPosition = GroundPosition.None;
        private Transform opponent;
        private GrapplingSystem opponentGrapple;

        // Grapple data
        private bool isInitiator = false;
        private float clinchDuration = 0f;
        private float groundDuration = 0f;
        private bool isAttemptingSubmission = false;
        private float submissionProgress = 0f;

        // Input tracking
        private bool escapePressed = false;
        private float escapeButtonPresses = 0f;
        private float lastEscapeTime = 0f;

        // Events
        public event System.Action<GrappleState> OnGrappleStateChanged;
        public event System.Action<GroundPosition> OnGroundPositionChanged;
        public event System.Action<bool> OnTakedownAttempt; // bool = successful
        public event System.Action<float> OnSubmissionProgress; // 0-1 progress
        public event System.Action OnGrappleBreak;

        // Properties
        public GrappleState CurrentState => currentState;
        public GroundPosition CurrentPosition => currentPosition;
        public bool IsGrappling => currentState != GrappleState.None;
        public bool IsGrounded => currentState == GrappleState.Ground;
        public Transform Opponent => opponent;

        private void Awake()
        {
            stats = GetComponent<FighterStats>();
            combatSystem = GetComponent<CombatSystem>();
            controller = GetComponent<FighterController>();
        }

        private void Update()
        {
            if (!IsGrappling) return;

            // Update grapple duration
            if (currentState == GrappleState.Clinch)
            {
                clinchDuration += Time.deltaTime;
            }
            else if (currentState == GrappleState.Ground)
            {
                groundDuration += Time.deltaTime;

                // Update submission progress
                if (isAttemptingSubmission)
                {
                    UpdateSubmission();
                }
            }

            // Check for escape input
            TrackEscapeAttempts();
        }

        #region Grapple Initiation

        /// <summary>
        /// Attempt to initiate grapple with nearby opponent
        /// </summary>
        public bool AttemptGrapple(Transform target)
        {
            if (IsGrappling) return false;
            if (stats.CurrentStamina < takedownStaminaCost * 0.5f) return false;

            // Check range
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance > grappleRange)
            {
                Debug.Log($"{gameObject.name}: Too far to grapple ({distance:F2}m)");
                return false;
            }

            // Get opponent grappling system
            opponentGrapple = target.GetComponent<GrapplingSystem>();
            if (opponentGrapple == null)
            {
                Debug.LogWarning($"{target.name} has no GrapplingSystem!");
                return false;
            }

            // Check if opponent is already grappling
            if (opponentGrapple.IsGrappling)
            {
                Debug.Log($"{target.name} is already grappling");
                return false;
            }

            // Success - initiate clinch
            opponent = target;
            isInitiator = true;

            EnterClinch(target);

            return true;
        }

        /// <summary>
        /// Enter clinch state (called by both fighters)
        /// </summary>
        private void EnterClinch(Transform target)
        {
            opponent = target;
            currentState = GrappleState.Clinch;
            clinchDuration = 0f;

            // Drain stamina for initiator
            if (isInitiator)
            {
                stats.DrainStamina(takedownStaminaCost * 0.3f);
            }

            // Disable movement
            if (controller != null)
            {
                controller.SetCanMove(false);
            }

            // Face opponent
            Vector3 directionToOpponent = (opponent.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(directionToOpponent);

            OnGrappleStateChanged?.Invoke(currentState);

            Debug.Log($"{gameObject.name} entered clinch with {opponent.name}");

            // Notify opponent to enter clinch
            if (isInitiator && opponentGrapple != null)
            {
                opponentGrapple.RespondToClinch(transform);
            }
        }

        /// <summary>
        /// Called by opponent when clinched
        /// </summary>
        private void RespondToClinch(Transform initiator)
        {
            opponent = initiator;
            isInitiator = false;
            currentState = GrappleState.Clinch;
            clinchDuration = 0f;

            if (controller != null)
            {
                controller.SetCanMove(false);
            }

            Vector3 directionToOpponent = (opponent.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(directionToOpponent);

            OnGrappleStateChanged?.Invoke(currentState);

            Debug.Log($"{gameObject.name} responded to clinch from {opponent.name}");
        }

        #endregion

        #region Clinch Mechanics

        /// <summary>
        /// Perform knee strike from clinch
        /// </summary>
        public bool ClinchKnee()
        {
            if (currentState != GrappleState.Clinch) return false;
            if (stats.CurrentStamina < 15f) return false;

            stats.DrainStamina(15f);

            // Apply damage to opponent
            if (opponentGrapple != null)
            {
                opponentGrapple.stats.TakeDamage(clinchKneeDamage, false);
            }

            Debug.Log($"{gameObject.name} landed clinch knee for {clinchKneeDamage} damage");

            return true;
        }

        /// <summary>
        /// Perform elbow strike from clinch
        /// </summary>
        public bool ClinchElbow()
        {
            if (currentState != GrappleState.Clinch) return false;
            if (stats.CurrentStamina < 20f) return false;

            stats.DrainStamina(20f);

            // Apply damage to opponent
            if (opponentGrapple != null)
            {
                opponentGrapple.stats.TakeDamage(clinchElbowDamage, false);
            }

            Debug.Log($"{gameObject.name} landed clinch elbow for {clinchElbowDamage} damage");

            return true;
        }

        /// <summary>
        /// Attempt to break clinch
        /// </summary>
        public bool AttemptClinchBreak()
        {
            if (currentState != GrappleState.Clinch) return false;

            float breakCost = clinchBreakThreshold - (clinchDuration * 5f); // Easier over time
            breakCost = Mathf.Max(breakCost, 20f); // Minimum 20 stamina

            if (stats.CurrentStamina < breakCost)
            {
                Debug.Log($"{gameObject.name} insufficient stamina to break clinch ({stats.CurrentStamina}/{breakCost})");
                return false;
            }

            stats.DrainStamina(breakCost);

            // Success - break clinch for both fighters
            BreakGrapple();

            return true;
        }

        #endregion

        #region Takedown Mechanics

        /// <summary>
        /// Attempt takedown from clinch
        /// </summary>
        public bool AttemptTakedown()
        {
            if (currentState != GrappleState.Clinch) return false;
            if (stats.CurrentStamina < takedownStaminaCost) return false;

            stats.DrainStamina(takedownStaminaCost);

            // Check opponent defense
            bool opponentDefended = false;
            if (opponentGrapple != null)
            {
                opponentDefended = opponentGrapple.DefendTakedown();
            }

            if (opponentDefended)
            {
                // Takedown failed
                OnTakedownAttempt?.Invoke(false);
                Debug.Log($"{gameObject.name} takedown defended by {opponent.name}");
                return false;
            }

            // Success - transition to ground
            TransitionToGround(isInitiator ? GroundPosition.Mount : GroundPosition.Guard);

            OnTakedownAttempt?.Invoke(true);
            Debug.Log($"{gameObject.name} successful takedown!");

            return true;
        }

        /// <summary>
        /// Defend against takedown (called by defender)
        /// </summary>
        private bool DefendTakedown()
        {
            // Stamina-based defense chance
            float defenseChance = stats.CurrentStamina / 100f;
            defenseChance *= stats.DefenseMultiplier;

            // Technique stat helps defense
            defenseChance += (stats.Technique / 100f) * 0.3f;

            bool defended = Random.value < defenseChance;

            if (defended)
            {
                stats.DrainStamina(20f); // Cost to defend
                Debug.Log($"{gameObject.name} defended takedown! (chance: {defenseChance:F2})");
            }

            return defended;
        }

        #endregion

        #region Ground Game

        /// <summary>
        /// Transition to ground position
        /// </summary>
        private void TransitionToGround(GroundPosition position)
        {
            currentState = GrappleState.Ground;
            currentPosition = position;
            groundDuration = 0f;

            OnGrappleStateChanged?.Invoke(currentState);
            OnGroundPositionChanged?.Invoke(currentPosition);

            Debug.Log($"{gameObject.name} transitioned to ground position: {position}");

            // Set opponent position
            if (opponentGrapple != null)
            {
                GroundPosition opponentPos = GetOppositePosition(position);
                opponentGrapple.SetGroundPosition(opponentPos);
            }
        }

        /// <summary>
        /// Set ground position (called by opponent)
        /// </summary>
        private void SetGroundPosition(GroundPosition position)
        {
            currentState = GrappleState.Ground;
            currentPosition = position;
            groundDuration = 0f;

            OnGrappleStateChanged?.Invoke(currentState);
            OnGroundPositionChanged?.Invoke(currentPosition);

            Debug.Log($"{gameObject.name} set to ground position: {position}");
        }

        /// <summary>
        /// Get opposite ground position
        /// </summary>
        private GroundPosition GetOppositePosition(GroundPosition position)
        {
            return position switch
            {
                GroundPosition.Mount => GroundPosition.Mounted,
                GroundPosition.Mounted => GroundPosition.Mount,
                GroundPosition.Guard => GroundPosition.InGuard,
                GroundPosition.InGuard => GroundPosition.Guard,
                GroundPosition.SideControl => GroundPosition.Controlled,
                GroundPosition.Controlled => GroundPosition.SideControl,
                _ => GroundPosition.None
            };
        }

        /// <summary>
        /// Perform ground and pound attack
        /// </summary>
        public bool GroundPound()
        {
            if (currentState != GrappleState.Ground) return false;

            // Only dominant positions can ground pound
            if (currentPosition != GroundPosition.Mount &&
                currentPosition != GroundPosition.SideControl &&
                currentPosition != GroundPosition.Guard)
            {
                return false;
            }

            if (stats.CurrentStamina < 10f) return false;

            stats.DrainStamina(10f);

            // Apply damage
            if (opponentGrapple != null)
            {
                opponentGrapple.stats.TakeDamage(groundPoundDamage, false);
            }

            Debug.Log($"{gameObject.name} ground and pound for {groundPoundDamage} damage");

            return true;
        }

        #endregion

        #region Submission System

        /// <summary>
        /// Attempt submission hold
        /// </summary>
        public bool AttemptSubmission()
        {
            if (currentState != GrappleState.Ground) return false;

            // Only certain positions can attempt submission
            if (currentPosition != GroundPosition.Mount &&
                currentPosition != GroundPosition.Guard)
            {
                return false;
            }

            if (stats.CurrentStamina < 30f) return false;

            stats.DrainStamina(30f);

            isAttemptingSubmission = true;
            submissionProgress = 0f;

            Debug.Log($"{gameObject.name} attempting submission!");

            return true;
        }

        /// <summary>
        /// Update submission progress
        /// </summary>
        private void UpdateSubmission()
        {
            // Submission builds over time
            float buildRate = 0.15f * Time.deltaTime;

            // Modified by technique stat
            buildRate *= (1f + stats.Technique / 100f);

            // Opponent's escape attempts slow progress
            if (opponentGrapple != null && opponentGrapple.escapePressed)
            {
                buildRate *= 0.5f; // Half speed if opponent escaping
            }

            submissionProgress += buildRate;
            submissionProgress = Mathf.Clamp01(submissionProgress);

            OnSubmissionProgress?.Invoke(submissionProgress);

            // Success
            if (submissionProgress >= 1f)
            {
                SubmissionSuccess();
            }

            // Drain stamina while holding
            stats.DrainStamina(5f * Time.deltaTime);

            // Release if out of stamina
            if (stats.CurrentStamina <= 0f)
            {
                ReleaseSubmission();
            }
        }

        /// <summary>
        /// Submission successful - tap out
        /// </summary>
        private void SubmissionSuccess()
        {
            Debug.Log($"{gameObject.name} submission successful! {opponent.name} tapped out!");

            // Apply significant damage
            if (opponentGrapple != null)
            {
                opponentGrapple.stats.TakeDamage(submissionDamage, false);
            }

            isAttemptingSubmission = false;
            submissionProgress = 0f;

            // Break grapple after submission
            BreakGrapple();
        }

        /// <summary>
        /// Release submission hold
        /// </summary>
        private void ReleaseSubmission()
        {
            Debug.Log($"{gameObject.name} released submission");

            isAttemptingSubmission = false;
            submissionProgress = 0f;
        }

        #endregion

        #region Escape & Reversal

        /// <summary>
        /// Track escape attempts (button mashing)
        /// </summary>
        private void TrackEscapeAttempts()
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
            {
                escapePressed = true;
                escapeButtonPresses++;
                lastEscapeTime = Time.time;
            }

            // Reset escape count after timeout
            if (Time.time - lastEscapeTime > escapeWindow)
            {
                escapeButtonPresses = 0;
            }

            // Attempt escape if enough presses
            if (escapeButtonPresses >= 5f)
            {
                AttemptEscape();
                escapeButtonPresses = 0;
            }
        }

        /// <summary>
        /// Attempt to escape from ground position
        /// </summary>
        public bool AttemptEscape()
        {
            if (!IsGrounded) return false;

            // Check if in disadvantaged position
            if (currentPosition != GroundPosition.Mounted &&
                currentPosition != GroundPosition.Controlled &&
                currentPosition != GroundPosition.InGuard)
            {
                return false; // Already in good position
            }

            float escapeCost = 40f;

            // Harder to escape submission
            if (opponentGrapple != null && opponentGrapple.isAttemptingSubmission)
            {
                escapeCost += 20f;
            }

            if (stats.CurrentStamina < escapeCost)
            {
                Debug.Log($"{gameObject.name} insufficient stamina to escape ({stats.CurrentStamina}/{escapeCost})");
                return false;
            }

            stats.DrainStamina(escapeCost);

            // Success - stand up
            Debug.Log($"{gameObject.name} escaped to standing!");

            BreakGrapple();

            return true;
        }

        /// <summary>
        /// Attempt reversal (position improvement)
        /// </summary>
        public bool AttemptReversal()
        {
            if (!IsGrounded) return false;
            if (stats.CurrentStamina < 35f) return false;

            stats.DrainStamina(35f);

            // Technique-based success chance
            float reversalChance = 0.3f + (stats.Technique / 100f) * 0.4f;

            bool success = Random.value < reversalChance;

            if (success)
            {
                // Swap positions
                GroundPosition newPosition = GetOppositePosition(currentPosition);
                TransitionToGround(newPosition);

                Debug.Log($"{gameObject.name} successful reversal! Now in {newPosition}");
            }
            else
            {
                Debug.Log($"{gameObject.name} reversal failed (chance: {reversalChance:F2})");
            }

            return success;
        }

        #endregion

        #region Grapple Management

        /// <summary>
        /// Break grapple and return to standing
        /// </summary>
        public void BreakGrapple()
        {
            currentState = GrappleState.None;
            currentPosition = GroundPosition.None;
            isInitiator = false;
            clinchDuration = 0f;
            groundDuration = 0f;
            isAttemptingSubmission = false;
            submissionProgress = 0f;
            escapeButtonPresses = 0f;

            // Re-enable movement
            if (controller != null)
            {
                controller.SetCanMove(true);
            }

            OnGrappleStateChanged?.Invoke(currentState);
            OnGrappleBreak?.Invoke();

            Debug.Log($"{gameObject.name} broke grapple");

            // Notify opponent
            if (opponentGrapple != null && opponentGrapple.IsGrappling)
            {
                opponentGrapple.BreakGrapple();
            }

            opponent = null;
            opponentGrapple = null;
        }

        /// <summary>
        /// Force break grapple (called externally, e.g., round end)
        /// </summary>
        public void ForceBreakGrapple()
        {
            if (!IsGrappling) return;

            Debug.Log($"{gameObject.name} grapple force broken");

            BreakGrapple();
        }

        #endregion

        #region Queries

        /// <summary>
        /// Check if can initiate grapple
        /// </summary>
        public bool CanInitiateGrapple()
        {
            if (IsGrappling) return false;
            if (stats.CurrentStamina < takedownStaminaCost * 0.5f) return false;
            return true;
        }

        /// <summary>
        /// Get grapple advantage (for AI decision making)
        /// </summary>
        public float GetGrappleAdvantage()
        {
            if (!IsGrappling) return 0f;

            float advantage = 0f;

            // Position advantage
            if (currentPosition == GroundPosition.Mount) advantage += 0.5f;
            else if (currentPosition == GroundPosition.SideControl) advantage += 0.3f;
            else if (currentPosition == GroundPosition.Guard) advantage += 0.1f;
            else if (currentPosition == GroundPosition.Mounted) advantage -= 0.5f;
            else if (currentPosition == GroundPosition.Controlled) advantage -= 0.3f;
            else if (currentPosition == GroundPosition.InGuard) advantage -= 0.1f;

            // Stamina advantage
            if (opponentGrapple != null)
            {
                float staminaDiff = (stats.CurrentStamina - opponentGrapple.stats.CurrentStamina) / 100f;
                advantage += staminaDiff * 0.3f;
            }

            return Mathf.Clamp(advantage, -1f, 1f);
        }

        #endregion
    }

    #region Enums

    /// <summary>
    /// Grappling state
    /// </summary>
    public enum GrappleState
    {
        None,           // Not grappling
        Clinch,         // Standing clinch
        Takedown,       // Transition to ground
        Ground,         // On the ground
        Submission      // Submission attempt
    }

    /// <summary>
    /// Ground positions
    /// </summary>
    public enum GroundPosition
    {
        None,           // Standing
        Mount,          // Top position, dominant
        Mounted,        // Bottom of mount
        Guard,          // Top position in guard
        InGuard,        // Bottom, defending in guard
        SideControl,    // Dominant side position
        Controlled      // Being controlled from side
    }

    #endregion
}
