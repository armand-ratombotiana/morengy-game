using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Morengy.Character;

namespace Morengy.Combat
{
    /// <summary>
    /// Main combat system managing attacks, blocking, and damage calculations.
    /// Implements the UFC/Def Jam hybrid combat mechanics for Morengy.
    /// </summary>
    [RequireComponent(typeof(FighterController))]
    [RequireComponent(typeof(FighterStats))]
    public class CombatSystem : MonoBehaviour
    {
        [Header("Attack Settings")]
        [SerializeField] private float lightAttackDamage = 10f;
        [SerializeField] private float heavyAttackDamage = 25f;
        [SerializeField] private float specialAttackDamage = 40f;

        [Header("Attack Timing")]
        [SerializeField] private float lightAttackDuration = 0.3f;
        [SerializeField] private float heavyAttackDuration = 0.8f;
        [SerializeField] private float attackRecoveryTime = 0.4f;

        [Header("Combo System")]
        [SerializeField] private float comboWindow = 0.5f;
        [SerializeField] private int maxComboCount = 5;
        [SerializeField] private float comboDamageBonus = 0.1f; // 10% per hit

        [Header("Hit Detection")]
        [SerializeField] private Transform strikePoint;
        [SerializeField] private float strikeRange = 1.5f;
        [SerializeField] private LayerMask hitLayer;

        [Header("Blocking")]
        [SerializeField] private float blockDamageReduction = 0.5f;
        [SerializeField] private float perfectBlockWindow = 0.2f;
        [SerializeField] private float perfectBlockBonus = 0.3f; // stamina restore

        [Header("Knockback")]
        [SerializeField] private float lightKnockbackForce = 2f;
        [SerializeField] private float heavyKnockbackForce = 5f;
        [SerializeField] private float criticalKnockbackMultiplier = 1.5f;

        // Components
        private FighterController controller;
        private FighterStats stats;
        private Animator animator;

        // Combat State
        private bool isAttacking = false;
        private bool isBlocking = false;
        private AttackType currentAttackType;
        private int comboCount = 0;
        private float lastAttackTime = 0f;
        private Queue<AttackType> attackBuffer = new Queue<AttackType>();

        // Hit Detection
        private HashSet<Collider> hitThisAttack = new HashSet<Collider>();

        // Properties
        public bool IsAttacking => isAttacking;
        public bool IsBlocking => isBlocking;
        public int ComboCount => comboCount;

        private void Awake()
        {
            controller = GetComponent<FighterController>();
            stats = GetComponent<FighterStats>();
            animator = GetComponentInChildren<Animator>();

            // Create strike point if not assigned
            if (strikePoint == null)
            {
                GameObject strikeObj = new GameObject("StrikePoint");
                strikeObj.transform.SetParent(transform);
                strikeObj.transform.localPosition = new Vector3(0, 1.2f, 0.8f);
                strikePoint = strikeObj.transform;
            }
        }

        private void Update()
        {
            // Handle combo timeout
            if (Time.time - lastAttackTime > comboWindow && comboCount > 0)
            {
                ResetCombo();
            }

            // Process buffered attacks
            ProcessAttackBuffer();

            // Update blocking state
            if (isBlocking)
            {
                stats.DrainStamina(stats.BlockStaminaDrain * Time.deltaTime);

                // Stop blocking if out of stamina
                if (stats.IsExhausted)
                {
                    StopBlocking();
                }
            }
        }

        #region Attack Methods

        /// <summary>
        /// Execute light attack
        /// </summary>
        public void PerformLightAttack()
        {
            if (!CanAttack(AttackType.Light)) return;

            // Buffer attack if currently attacking
            if (isAttacking)
            {
                BufferAttack(AttackType.Light);
                return;
            }

            StartCoroutine(ExecuteAttack(AttackType.Light));
        }

        /// <summary>
        /// Execute heavy attack
        /// </summary>
        public void PerformHeavyAttack()
        {
            if (!CanAttack(AttackType.Heavy)) return;

            if (isAttacking)
            {
                BufferAttack(AttackType.Heavy);
                return;
            }

            StartCoroutine(ExecuteAttack(AttackType.Heavy));
        }

        /// <summary>
        /// Execute special attack (requires full meter)
        /// </summary>
        public void PerformSpecialAttack()
        {
            if (!CanAttack(AttackType.Special) || !stats.CanUseSpecial) return;

            StartCoroutine(ExecuteAttack(AttackType.Special));
        }

        /// <summary>
        /// Main attack execution coroutine
        /// </summary>
        private IEnumerator ExecuteAttack(AttackType attackType)
        {
            isAttacking = true;
            currentAttackType = attackType;
            hitThisAttack.Clear();

            // Drain stamina
            float staminaCost = GetAttackStaminaCost(attackType);
            stats.DrainStamina(staminaCost);

            // Use special meter for special attacks
            if (attackType == AttackType.Special)
            {
                stats.UseSpecialMeter(stats.SpecialMeterMax);
            }

            // Set fighter state
            controller.SetState(FighterState.Attacking);

            // Trigger animation
            TriggerAttackAnimation(attackType);

            // Wait for attack startup (preparing phase)
            float startupTime = GetAttackDuration(attackType) * 0.3f;
            yield return new WaitForSeconds(startupTime);

            // Active frames - check for hits
            float activeTime = GetAttackDuration(attackType) * 0.4f;
            float checkInterval = 0.05f;
            float elapsed = 0f;

            while (elapsed < activeTime)
            {
                CheckForHit();
                elapsed += checkInterval;
                yield return new WaitForSeconds(checkInterval);
            }

            // Recovery phase
            float recoveryTime = GetAttackDuration(attackType) * 0.3f + attackRecoveryTime;
            yield return new WaitForSeconds(recoveryTime);

            // End attack
            isAttacking = false;
            controller.SetState(FighterState.Idle);
            lastAttackTime = Time.time;
        }

        /// <summary>
        /// Check for hit detection during active frames
        /// </summary>
        private void CheckForHit()
        {
            Collider[] hits = Physics.OverlapSphere(
                strikePoint.position,
                strikeRange,
                hitLayer
            );

            foreach (Collider hit in hits)
            {
                // Skip if already hit this attack
                if (hitThisAttack.Contains(hit)) continue;

                // Skip self
                if (hit.transform == transform || hit.transform.IsChildOf(transform)) continue;

                // Check if hit is an opponent
                CombatSystem opponentCombat = hit.GetComponentInParent<CombatSystem>();
                if (opponentCombat != null)
                {
                    ProcessHit(opponentCombat);
                    hitThisAttack.Add(hit);
                }
            }
        }

        /// <summary>
        /// Process successful hit on opponent
        /// </summary>
        private void ProcessHit(CombatSystem opponent)
        {
            // Calculate base damage
            float baseDamage = GetAttackDamage(currentAttackType);

            // Apply fighter power multiplier
            baseDamage *= stats.DamageMultiplier;

            // Apply stamina modifier
            baseDamage *= stats.GetStaminaDamageModifier();

            // Apply combo multiplier
            float comboMultiplier = 1f + (comboCount * comboDamageBonus);
            baseDamage *= comboMultiplier;

            // Check for critical hit
            bool isCritical = stats.RollForCritical();

            // Check if opponent is blocking
            float finalDamage = baseDamage;
            if (opponent.IsBlocking)
            {
                finalDamage = opponent.ProcessBlockedAttack(baseDamage, isCritical);
            }

            // Apply damage to opponent
            opponent.stats.TakeDamage(finalDamage, isCritical);

            // Apply knockback
            Vector3 knockbackDir = (opponent.transform.position - transform.position).normalized;
            float knockbackForce = currentAttackType == AttackType.Light ?
                lightKnockbackForce : heavyKnockbackForce;

            if (isCritical)
            {
                knockbackForce *= criticalKnockbackMultiplier;
            }

            opponent.controller.ApplyKnockback(knockbackDir, knockbackForce);

            // Increment combo
            IncrementCombo();

            // Gain special meter
            stats.OnAttackLanded();

            // Visual/Audio feedback (to be implemented)
            OnHitLanded(opponent, finalDamage, isCritical);
        }

        /// <summary>
        /// Process attack when blocking
        /// </summary>
        private float ProcessBlockedAttack(float incomingDamage, bool isCritical)
        {
            // Perfect block check (just pressed block)
            bool isPerfectBlock = Time.time - blockStartTime < perfectBlockWindow;

            float damageReduction = isPerfectBlock ? 0.8f : blockDamageReduction;
            float finalDamage = incomingDamage * (1f - damageReduction);

            // Perfect block restores stamina
            if (isPerfectBlock)
            {
                stats.GainSpecialMeter(perfectBlockBonus * stats.MaxStamina);
                OnPerfectBlock();
            }

            // Heavy drain for blocking critical hits
            if (isCritical)
            {
                stats.DrainStamina(stats.HeavyAttackStaminaCost);
            }

            return finalDamage;
        }

        #endregion

        #region Blocking

        private float blockStartTime = 0f;

        /// <summary>
        /// Start blocking
        /// </summary>
        public void StartBlocking()
        {
            if (isAttacking || stats.IsExhausted) return;

            isBlocking = true;
            blockStartTime = Time.time;
            controller.SetState(FighterState.Blocking);

            if (animator != null)
            {
                animator.SetBool("IsBlocking", true);
            }
        }

        /// <summary>
        /// Stop blocking
        /// </summary>
        public void StopBlocking()
        {
            isBlocking = false;
            controller.SetState(FighterState.Idle);

            if (animator != null)
            {
                animator.SetBool("IsBlocking", false);
            }
        }

        #endregion

        #region Combo System

        /// <summary>
        /// Increment combo counter
        /// </summary>
        private void IncrementCombo()
        {
            comboCount = Mathf.Min(comboCount + 1, maxComboCount);
            lastAttackTime = Time.time;
        }

        /// <summary>
        /// Reset combo counter
        /// </summary>
        private void ResetCombo()
        {
            comboCount = 0;
        }

        /// <summary>
        /// Buffer attack input for combo chaining
        /// </summary>
        private void BufferAttack(AttackType type)
        {
            if (attackBuffer.Count < 2)
            {
                attackBuffer.Enqueue(type);
            }
        }

        /// <summary>
        /// Process buffered attacks for smooth combo chains
        /// </summary>
        private void ProcessAttackBuffer()
        {
            if (!isAttacking && attackBuffer.Count > 0)
            {
                AttackType nextAttack = attackBuffer.Dequeue();

                switch (nextAttack)
                {
                    case AttackType.Light:
                        PerformLightAttack();
                        break;
                    case AttackType.Heavy:
                        PerformHeavyAttack();
                        break;
                    case AttackType.Special:
                        PerformSpecialAttack();
                        break;
                }
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Check if fighter can execute attack
        /// </summary>
        private bool CanAttack(AttackType type)
        {
            // Can't attack while knocked down or being grabbed
            if (controller.CurrentState == FighterState.KnockedDown ||
                controller.CurrentState == FighterState.BeingGrabbed)
            {
                return false;
            }

            // Check stamina requirement
            float requiredStamina = GetAttackStaminaCost(type);
            return stats.CurrentStamina >= requiredStamina;
        }

        /// <summary>
        /// Get attack damage based on type
        /// </summary>
        private float GetAttackDamage(AttackType type)
        {
            switch (type)
            {
                case AttackType.Light:
                    return lightAttackDamage;
                case AttackType.Heavy:
                    return heavyAttackDamage;
                case AttackType.Special:
                    return specialAttackDamage;
                default:
                    return 0f;
            }
        }

        /// <summary>
        /// Get attack duration based on type
        /// </summary>
        private float GetAttackDuration(AttackType type)
        {
            switch (type)
            {
                case AttackType.Light:
                    return lightAttackDuration;
                case AttackType.Heavy:
                    return heavyAttackDuration;
                case AttackType.Special:
                    return heavyAttackDuration * 1.5f;
                default:
                    return 0.5f;
            }
        }

        /// <summary>
        /// Get stamina cost for attack type
        /// </summary>
        private float GetAttackStaminaCost(AttackType type)
        {
            switch (type)
            {
                case AttackType.Light:
                    return stats.LightAttackStaminaCost;
                case AttackType.Heavy:
                    return stats.HeavyAttackStaminaCost;
                case AttackType.Special:
                    return stats.HeavyAttackStaminaCost * 1.5f;
                default:
                    return 0f;
            }
        }

        /// <summary>
        /// Trigger attack animation
        /// </summary>
        private void TriggerAttackAnimation(AttackType type)
        {
            if (animator == null) return;

            switch (type)
            {
                case AttackType.Light:
                    animator.SetTrigger("LightAttack");
                    animator.SetInteger("ComboCount", comboCount);
                    break;
                case AttackType.Heavy:
                    animator.SetTrigger("HeavyAttack");
                    break;
                case AttackType.Special:
                    animator.SetTrigger("SpecialAttack");
                    break;
            }
        }

        #endregion

        #region Events & Feedback

        /// <summary>
        /// Called when a hit lands successfully
        /// </summary>
        private void OnHitLanded(CombatSystem opponent, float damage, bool isCritical)
        {
            // Play hit effects
            // Play impact sound
            // Screen shake for heavy hits
            // Particle effects

            Debug.Log($"{stats.FighterName} hit {opponent.stats.FighterName} for {damage:F1} damage" +
                      (isCritical ? " (CRITICAL!)" : "") +
                      $" | Combo: {comboCount}");
        }

        /// <summary>
        /// Called when perfect block is executed
        /// </summary>
        private void OnPerfectBlock()
        {
            // Play perfect block sound/effect
            // Flash visual feedback
            Debug.Log($"{stats.FighterName} executed PERFECT BLOCK!");
        }

        #endregion

        #region Gizmos & Debug

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (strikePoint == null) return;

            // Draw strike range
            Gizmos.color = isAttacking ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(strikePoint.position, strikeRange);

            // Draw strike direction
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(strikePoint.position, transform.forward * strikeRange);
        }
#endif

        #endregion
    }

    /// <summary>
    /// Attack type enumeration
    /// </summary>
    public enum AttackType
    {
        Light,
        Heavy,
        Special
    }
}
