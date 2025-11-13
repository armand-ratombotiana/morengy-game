using UnityEngine;
using Morengy.Character;
using Morengy.Combat;

namespace Morengy.Core
{
    /// <summary>
    /// Handles player input and maps to fighter actions.
    /// Can be replaced with Unity's new Input System for more flexibility.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        [Header("Target Fighter")]
        [SerializeField] private FighterController fighter;
        [SerializeField] private CombatSystem combatSystem;

        [Header("Input Settings")]
        [SerializeField] private bool useAlternativeControls = false;
        [SerializeField] private float inputBufferTime = 0.2f;

        [Header("Key Bindings - Primary")]
        [SerializeField] private KeyCode lightAttackKey = KeyCode.J;
        [SerializeField] private KeyCode heavyAttackKey = KeyCode.K;
        [SerializeField] private KeyCode specialAttackKey = KeyCode.L;
        [SerializeField] private KeyCode blockKey = KeyCode.I;
        [SerializeField] private KeyCode dodgeKey = KeyCode.Space;
        [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
        [SerializeField] private KeyCode restKey = KeyCode.R;

        [Header("Alternative Bindings (Gamepad-style)")]
        [SerializeField] private KeyCode altLightAttack = KeyCode.Mouse0;
        [SerializeField] private KeyCode altHeavyAttack = KeyCode.Mouse1;
        [SerializeField] private KeyCode altBlock = KeyCode.LeftControl;

        // Input state
        private Vector2 moveInput;
        private bool blockPressed;
        private float lastInputTime;

        private void Awake()
        {
            // Auto-find components if not assigned
            if (fighter == null)
                fighter = GetComponent<FighterController>();

            if (combatSystem == null)
                combatSystem = GetComponent<CombatSystem>();

            if (fighter == null || combatSystem == null)
            {
                Debug.LogError($"InputManager on {gameObject.name} missing required components!");
            }
        }

        private void Update()
        {
            if (fighter == null || combatSystem == null) return;

            HandleMovementInput();
            HandleCombatInput();
            HandleDefensiveInput();
            HandleUtilityInput();
        }

        #region Movement Input

        /// <summary>
        /// Handle WASD/Arrow key movement input
        /// </summary>
        private void HandleMovementInput()
        {
            // Read movement axes
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            moveInput = new Vector2(horizontal, vertical);

            // Send to fighter controller
            fighter.SetMoveInput(moveInput);

            // Sprint input
            bool isSprinting = Input.GetKey(sprintKey);
            fighter.SetSprintInput(isSprinting);

            // Crouch input (can be toggle or hold)
            bool isCrouching = Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.LeftControl);
            fighter.SetCrouchInput(isCrouching);
        }

        #endregion

        #region Combat Input

        /// <summary>
        /// Handle attack inputs with buffering
        /// </summary>
        private void HandleCombatInput()
        {
            // Light Attack
            if (GetAttackInput(lightAttackKey, altLightAttack))
            {
                combatSystem.PerformLightAttack();
                RegisterInput();
            }

            // Heavy Attack
            if (GetAttackInput(heavyAttackKey, altHeavyAttack))
            {
                combatSystem.PerformHeavyAttack();
                RegisterInput();
            }

            // Special Attack
            if (Input.GetKeyDown(specialAttackKey))
            {
                combatSystem.PerformSpecialAttack();
                RegisterInput();
            }
        }

        /// <summary>
        /// Get attack input with primary/alternative key support
        /// </summary>
        private bool GetAttackInput(KeyCode primary, KeyCode alternative)
        {
            if (useAlternativeControls)
            {
                return Input.GetKeyDown(alternative);
            }
            return Input.GetKeyDown(primary);
        }

        #endregion

        #region Defensive Input

        /// <summary>
        /// Handle blocking and dodging
        /// </summary>
        private void HandleDefensiveInput()
        {
            // Block (hold to maintain)
            KeyCode currentBlockKey = useAlternativeControls ? altBlock : blockKey;

            if (Input.GetKeyDown(currentBlockKey))
            {
                combatSystem.StartBlocking();
                blockPressed = true;
            }

            if (Input.GetKey(currentBlockKey) && blockPressed)
            {
                // Maintain block
                if (!combatSystem.IsBlocking)
                {
                    combatSystem.StartBlocking();
                }
            }

            if (Input.GetKeyUp(currentBlockKey))
            {
                combatSystem.StopBlocking();
                blockPressed = false;
            }

            // Dodge/Roll
            if (Input.GetKeyDown(dodgeKey))
            {
                Vector2 dodgeDirection = moveInput.magnitude > 0.1f ? moveInput : Vector2.down;
                fighter.PerformDodge(dodgeDirection);
                RegisterInput();
            }
        }

        #endregion

        #region Utility Input

        /// <summary>
        /// Handle rest, taunt, and other utility actions
        /// </summary>
        private void HandleUtilityInput()
        {
            // Rest stance (fast stamina regen, but vulnerable)
            if (Input.GetKey(restKey))
            {
                // Fast stamina regeneration (handled in FighterStats)
                fighter.GetComponent<FighterStats>().FastRegenerateStamina();
            }

            // Taunt (gain crowd meter)
            if (Input.GetKeyDown(KeyCode.T))
            {
                PerformTaunt();
            }
        }

        /// <summary>
        /// Perform taunt action
        /// </summary>
        private void PerformTaunt()
        {
            // Only taunt if not in combat action
            if (combatSystem.IsAttacking || combatSystem.IsBlocking) return;

            // Gain special meter from crowd
            FighterStats stats = fighter.GetComponent<FighterStats>();
            stats.GainSpecialMeter(10f);

            Debug.Log($"{stats.FighterName} taunts! Crowd loves it!");
            // Trigger taunt animation
        }

        #endregion

        #region Input Buffering

        /// <summary>
        /// Register input timestamp for buffering
        /// </summary>
        private void RegisterInput()
        {
            lastInputTime = Time.time;
        }

        /// <summary>
        /// Check if input is within buffer window
        /// </summary>
        public bool IsInputRecent()
        {
            return Time.time - lastInputTime < inputBufferTime;
        }

        #endregion

        #region Control Scheme Switching

        /// <summary>
        /// Switch between primary and alternative control schemes
        /// </summary>
        public void ToggleControlScheme()
        {
            useAlternativeControls = !useAlternativeControls;
            Debug.Log($"Switched to {(useAlternativeControls ? "Alternative" : "Primary")} controls");
        }

        /// <summary>
        /// Set control scheme explicitly
        /// </summary>
        public void SetControlScheme(bool useAlternative)
        {
            useAlternativeControls = useAlternative;
        }

        #endregion

        #region Public Accessors

        /// <summary>
        /// Get current movement input (for AI or replay systems)
        /// </summary>
        public Vector2 GetMoveInput() => moveInput;

        /// <summary>
        /// Check if blocking
        /// </summary>
        public bool IsBlockingInput() => blockPressed;

        #endregion

#if UNITY_EDITOR
        private void OnGUI()
        {
            // Display control hints in editor
            if (!Application.isPlaying) return;

            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.white;
            style.fontSize = 12;

            string controls = useAlternativeControls ?
                "ALT CONTROLS: LMB-Light | RMB-Heavy | LCtrl-Block | Space-Dodge" :
                "CONTROLS: J-Light | K-Heavy | L-Special | I-Block | Space-Dodge | Shift-Sprint";

            GUI.Label(new Rect(10, 10, 500, 25), controls, style);

            FighterStats stats = fighter.GetComponent<FighterStats>();
            GUI.Label(new Rect(10, 30, 300, 25),
                $"Health: {stats.HealthPercentage:F0}% | Stamina: {stats.StaminaPercentage:F0}%", style);
        }
#endif
    }
}
