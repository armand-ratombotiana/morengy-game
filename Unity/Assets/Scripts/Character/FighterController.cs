using UnityEngine;
using System.Collections;

namespace Morengy.Character
{
    /// <summary>
    /// Main character controller for Morengy fighters.
    /// Handles movement, rotation, and basic state management.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(FighterStats))]
    public class FighterController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float walkSpeed = 3f;
        [SerializeField] private float sprintSpeed = 5f;
        [SerializeField] private float crouchSpeed = 1.5f;
        [SerializeField] private float backpedalSpeed = 2f;
        [SerializeField] private float rotationSpeed = 10f;

        [Header("Dodge Settings")]
        [SerializeField] private float dodgeDistance = 3f;
        [SerializeField] private float dodgeDuration = 0.4f;
        [SerializeField] private AnimationCurve dodgeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Physics")]
        [SerializeField] private float gravity = -20f;
        [SerializeField] private float groundCheckDistance = 0.2f;
        [SerializeField] private LayerMask groundLayer;

        // Components
        private CharacterController characterController;
        private FighterStats stats;
        private Animator animator;

        // State
        private Vector3 velocity;
        private bool isGrounded;
        private FighterState currentState = FighterState.Idle;
        private Transform opponent;

        // Input (will be replaced with Unity Input System)
        private Vector2 moveInput;
        private bool sprintInput;
        private bool crouchInput;

        // Properties
        public FighterState CurrentState => currentState;
        public bool IsGrounded => isGrounded;
        public Transform Opponent { get => opponent; set => opponent = value; }
        public Vector3 Velocity => velocity;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            stats = GetComponent<FighterStats>();
            animator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            CheckGroundStatus();
            HandleGravity();
            HandleMovement();
            HandleRotation();
            UpdateAnimator();
        }

        /// <summary>
        /// Check if fighter is on the ground
        /// </summary>
        private void CheckGroundStatus()
        {
            isGrounded = Physics.CheckSphere(
                transform.position + Vector3.down * groundCheckDistance,
                0.1f,
                groundLayer
            );
        }

        /// <summary>
        /// Apply gravity to the fighter
        /// </summary>
        private void HandleGravity()
        {
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f; // Small negative value to keep grounded
            }
            else
            {
                velocity.y += gravity * Time.deltaTime;
            }
        }

        /// <summary>
        /// Handle character movement based on input and state
        /// </summary>
        private void HandleMovement()
        {
            // Don't move if in attacking, stunned, or knocked down state
            if (!CanMove())
            {
                return;
            }

            // Calculate movement speed based on state
            float currentSpeed = CalculateMovementSpeed();

            // Calculate movement direction (relative to camera or facing)
            Vector3 moveDirection = CalculateMoveDirection();

            // Apply movement with stamina cost
            if (moveDirection.magnitude > 0.1f)
            {
                Vector3 movement = moveDirection * currentSpeed;
                characterController.Move(movement * Time.deltaTime);

                // Drain stamina while sprinting
                if (sprintInput && currentState == FighterState.Moving)
                {
                    stats.DrainStamina(stats.SprintStaminaDrain * Time.deltaTime);
                }
            }

            // Apply gravity
            characterController.Move(velocity * Time.deltaTime);
        }

        /// <summary>
        /// Calculate movement speed based on current state and inputs
        /// </summary>
        private float CalculateMovementSpeed()
        {
            // Apply speed multiplier from fighter stats
            float speedMultiplier = stats.SpeedMultiplier;

            if (crouchInput)
            {
                currentState = FighterState.Crouching;
                return crouchSpeed * speedMultiplier;
            }

            if (sprintInput && stats.CurrentStamina > stats.MaxStamina * 0.2f)
            {
                currentState = FighterState.Moving;
                return sprintSpeed * speedMultiplier;
            }

            // Check if moving backward (relative to opponent)
            if (opponent != null && IsMovingBackward())
            {
                currentState = FighterState.Moving;
                return backpedalSpeed * speedMultiplier;
            }

            currentState = moveInput.magnitude > 0.1f ? FighterState.Moving : FighterState.Idle;
            return walkSpeed * speedMultiplier;
        }

        /// <summary>
        /// Calculate movement direction based on input
        /// </summary>
        private Vector3 CalculateMoveDirection()
        {
            Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

            // Transform direction based on camera or world space
            // For fighting game, we typically use world space or relative to opponent
            return direction;
        }

        /// <summary>
        /// Check if fighter is moving backward relative to opponent
        /// </summary>
        private bool IsMovingBackward()
        {
            if (opponent == null) return false;

            Vector3 toOpponent = (opponent.position - transform.position).normalized;
            Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

            return Vector3.Dot(toOpponent, moveDirection) < -0.5f;
        }

        /// <summary>
        /// Handle fighter rotation to face opponent or movement direction
        /// </summary>
        private void HandleRotation()
        {
            if (opponent != null)
            {
                // Always face opponent in combat
                Vector3 directionToOpponent = (opponent.position - transform.position).normalized;
                directionToOpponent.y = 0;

                if (directionToOpponent != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(directionToOpponent);
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        targetRotation,
                        rotationSpeed * Time.deltaTime
                    );
                }
            }
            else if (moveInput.magnitude > 0.1f)
            {
                // Rotate toward movement direction if no opponent
                Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
                if (moveDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        targetRotation,
                        rotationSpeed * Time.deltaTime
                    );
                }
            }
        }

        /// <summary>
        /// Perform dodge roll maneuver
        /// </summary>
        public void PerformDodge(Vector2 direction)
        {
            if (!CanDodge()) return;

            // Drain stamina
            stats.DrainStamina(stats.DodgeStaminaCost);

            // Start dodge coroutine
            StartCoroutine(DodgeCoroutine(direction));
        }

        /// <summary>
        /// Dodge coroutine with invincibility frames
        /// </summary>
        private IEnumerator DodgeCoroutine(Vector2 direction)
        {
            currentState = FighterState.Dodging;

            Vector3 dodgeDirection = new Vector3(direction.x, 0f, direction.y).normalized;
            if (dodgeDirection == Vector3.zero)
            {
                // Default to backward dodge
                dodgeDirection = -transform.forward;
            }

            float elapsed = 0f;
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = startPosition + dodgeDirection * dodgeDistance;

            // Invincibility frames (first 60% of dodge)
            float invincibilityDuration = dodgeDuration * 0.6f;

            while (elapsed < dodgeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / dodgeDuration;
                float curveValue = dodgeCurve.Evaluate(t);

                // Move character
                Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, curveValue);
                characterController.Move((newPosition - transform.position));

                // Set invincibility
                if (elapsed <= invincibilityDuration)
                {
                    // Trigger invincibility (will be handled by combat system)
                    // This can be checked by other systems via CurrentState == Dodging
                }

                yield return null;
            }

            currentState = FighterState.Idle;
        }

        /// <summary>
        /// Check if fighter can currently move
        /// </summary>
        private bool CanMove()
        {
            return currentState == FighterState.Idle ||
                   currentState == FighterState.Moving ||
                   currentState == FighterState.Crouching;
        }

        /// <summary>
        /// Check if fighter can dodge
        /// </summary>
        private bool CanDodge()
        {
            return CanMove() &&
                   stats.CurrentStamina >= stats.DodgeStaminaCost &&
                   isGrounded;
        }

        /// <summary>
        /// Update animator parameters
        /// </summary>
        private void UpdateAnimator()
        {
            if (animator == null) return;

            animator.SetFloat("Speed", moveInput.magnitude);
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetBool("IsCrouching", crouchInput);
            animator.SetInteger("State", (int)currentState);
        }

        /// <summary>
        /// Set movement input (called from input system)
        /// </summary>
        public void SetMoveInput(Vector2 input)
        {
            moveInput = input;
        }

        /// <summary>
        /// Set sprint input
        /// </summary>
        public void SetSprintInput(bool isSprinting)
        {
            sprintInput = isSprinting;
        }

        /// <summary>
        /// Set crouch input
        /// </summary>
        public void SetCrouchInput(bool isCrouching)
        {
            crouchInput = isCrouching;
        }

        /// <summary>
        /// Force set fighter state (used by combat system)
        /// </summary>
        public void SetState(FighterState newState)
        {
            currentState = newState;
        }

        /// <summary>
        /// Knockback effect when hit
        /// </summary>
        public void ApplyKnockback(Vector3 direction, float force)
        {
            StartCoroutine(KnockbackCoroutine(direction, force));
        }

        private IEnumerator KnockbackCoroutine(Vector3 direction, float force)
        {
            currentState = FighterState.Stunned;

            float duration = 0.3f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float knockbackThisFrame = force * (1 - elapsed / duration) * Time.deltaTime;
                characterController.Move(direction.normalized * knockbackThisFrame);

                elapsed += Time.deltaTime;
                yield return null;
            }

            currentState = FighterState.Idle;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            // Draw ground check sphere
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + Vector3.down * groundCheckDistance, 0.1f);

            // Draw facing direction
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position + Vector3.up, transform.forward * 2f);

            // Draw dodge range
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, dodgeDistance);
        }
#endif
    }

    /// <summary>
    /// Fighter state enumeration
    /// </summary>
    public enum FighterState
    {
        Idle = 0,
        Moving = 1,
        Crouching = 2,
        Dodging = 3,
        Attacking = 4,
        Blocking = 5,
        Stunned = 6,
        KnockedDown = 7,
        GrabbingOpponent = 8,
        BeingGrabbed = 9
    }
}
