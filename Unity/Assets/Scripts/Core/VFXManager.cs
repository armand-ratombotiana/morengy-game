using UnityEngine;
using System.Collections.Generic;

namespace Morengy.Core
{
    /// <summary>
    /// Manages visual effects for the fighting game.
    /// Handles particle effects, screen effects, and impact visuals.
    /// </summary>
    public class VFXManager : MonoBehaviour
    {
        [Header("Hit Effect Prefabs")]
        [SerializeField] private GameObject lightHitEffect;
        [SerializeField] private GameObject heavyHitEffect;
        [SerializeField] private GameObject criticalHitEffect;
        [SerializeField] private GameObject blockEffect;
        [SerializeField] private GameObject perfectBlockEffect;

        [Header("Movement Effects")]
        [SerializeField] private GameObject dodgeTrailEffect;
        [SerializeField] private GameObject sprintDustEffect;
        [SerializeField] private GameObject groundImpactEffect;

        [Header("Special Effects")]
        [SerializeField] private GameObject specialMoveChargeEffect;
        [SerializeField] private GameObject specialMoveReleaseEffect;
        [SerializeField] private GameObject knockdownEffect;
        [SerializeField] private GameObject koEffect;

        [Header("Environment Effects")]
        [SerializeField] private GameObject bloodSprayEffect;
        [SerializeField] private GameObject sweatDropEffect;
        [SerializeField] private GameObject dustCloudEffect;

        [Header("UI Effects")]
        [SerializeField] private GameObject comboFlashEffect;
        [SerializeField] private GameObject healthLowEffect;

        [Header("Pool Settings")]
        [SerializeField] private int poolSize = 20;
        [SerializeField] private float effectLifetime = 2f;

        // Object pools
        private Dictionary<string, Queue<GameObject>> effectPools;
        private Transform poolContainer;

        // Singleton
        public static VFXManager Instance { get; private set; }

        private void Awake()
        {
            // Singleton
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitializePools();
        }

        /// <summary>
        /// Initialize object pools for effects
        /// </summary>
        private void InitializePools()
        {
            poolContainer = new GameObject("VFX_Pool").transform;
            poolContainer.SetParent(transform);

            effectPools = new Dictionary<string, Queue<GameObject>>();
        }

        #region Hit Effects

        /// <summary>
        /// Play light hit effect at position
        /// </summary>
        public void PlayLightHitEffect(Vector3 position, Vector3 normal)
        {
            PlayEffect(lightHitEffect, position, Quaternion.LookRotation(normal));
        }

        /// <summary>
        /// Play heavy hit effect at position
        /// </summary>
        public void PlayHeavyHitEffect(Vector3 position, Vector3 normal)
        {
            PlayEffect(heavyHitEffect, position, Quaternion.LookRotation(normal));

            // Additional screen shake for heavy hits
            if (Camera.main != null)
            {
                var cameraController = Camera.main.GetComponent<FightingCameraController>();
                if (cameraController != null)
                {
                    cameraController.Shake(0.3f, 0.2f);
                }
            }
        }

        /// <summary>
        /// Play critical hit effect with enhanced visuals
        /// </summary>
        public void PlayCriticalHitEffect(Vector3 position, Vector3 normal)
        {
            PlayEffect(criticalHitEffect, position, Quaternion.LookRotation(normal));

            // Heavy screen shake for critical
            if (Camera.main != null)
            {
                var cameraController = Camera.main.GetComponent<FightingCameraController>();
                if (cameraController != null)
                {
                    cameraController.Shake(0.8f, 0.4f);
                }
            }

            // Play critical hit sound
            if (Managers.AudioManager.Instance != null)
            {
                Managers.AudioManager.Instance.PlayCriticalHitSound();
            }
        }

        /// <summary>
        /// Play block effect at position
        /// </summary>
        public void PlayBlockEffect(Vector3 position, Vector3 normal)
        {
            PlayEffect(blockEffect, position, Quaternion.LookRotation(normal));
        }

        /// <summary>
        /// Play perfect block effect with enhanced visuals
        /// </summary>
        public void PlayPerfectBlockEffect(Vector3 position, Vector3 normal)
        {
            PlayEffect(perfectBlockEffect, position, Quaternion.LookRotation(normal));
        }

        #endregion

        #region Movement Effects

        /// <summary>
        /// Play dodge trail effect following transform
        /// </summary>
        public void PlayDodgeTrail(Transform followTarget)
        {
            if (dodgeTrailEffect == null) return;

            GameObject trail = PlayEffect(dodgeTrailEffect, followTarget.position, followTarget.rotation);

            // Make trail follow for duration
            if (trail != null)
            {
                StartCoroutine(FollowTransformCoroutine(trail.transform, followTarget, 0.4f));
            }
        }

        /// <summary>
        /// Play sprint dust effect at feet position
        /// </summary>
        public void PlaySprintDust(Vector3 footPosition)
        {
            if (sprintDustEffect != null)
            {
                PlayEffect(sprintDustEffect, footPosition, Quaternion.identity);
            }
        }

        /// <summary>
        /// Play ground impact effect (knockdown, heavy landing)
        /// </summary>
        public void PlayGroundImpact(Vector3 position)
        {
            PlayEffect(groundImpactEffect, position, Quaternion.identity);
        }

        #endregion

        #region Special Move Effects

        /// <summary>
        /// Play special move charge effect around fighter
        /// </summary>
        public GameObject PlaySpecialChargeEffect(Transform fighter)
        {
            if (specialMoveChargeEffect == null) return null;

            GameObject charge = PlayEffect(specialMoveChargeEffect, fighter.position, Quaternion.identity);

            if (charge != null)
            {
                charge.transform.SetParent(fighter);
            }

            return charge;
        }

        /// <summary>
        /// Play special move release effect
        /// </summary>
        public void PlaySpecialReleaseEffect(Vector3 position, Vector3 direction)
        {
            PlayEffect(specialMoveReleaseEffect, position, Quaternion.LookRotation(direction));

            // Big screen shake
            if (Camera.main != null)
            {
                var cameraController = Camera.main.GetComponent<FightingCameraController>();
                if (cameraController != null)
                {
                    cameraController.Shake(1.2f, 0.5f);
                }
            }
        }

        /// <summary>
        /// Play knockdown effect
        /// </summary>
        public void PlayKnockdownEffect(Vector3 position)
        {
            PlayEffect(knockdownEffect, position, Quaternion.identity);
            PlayGroundImpact(position);
        }

        /// <summary>
        /// Play KO effect with enhanced visuals
        /// </summary>
        public void PlayKOEffect(Vector3 position)
        {
            PlayEffect(koEffect, position, Quaternion.identity);

            // Massive screen shake
            if (Camera.main != null)
            {
                var cameraController = Camera.main.GetComponent<FightingCameraController>();
                if (cameraController != null)
                {
                    cameraController.Shake(1.5f, 0.8f);
                }
            }
        }

        #endregion

        #region Environment Effects

        /// <summary>
        /// Play blood spray effect (subtle for cultural sensitivity)
        /// </summary>
        public void PlayBloodSpray(Vector3 position, Vector3 direction)
        {
            if (bloodSprayEffect != null)
            {
                PlayEffect(bloodSprayEffect, position, Quaternion.LookRotation(direction));
            }
        }

        /// <summary>
        /// Play sweat drop effect
        /// </summary>
        public void PlaySweatDrop(Vector3 position)
        {
            if (sweatDropEffect != null)
            {
                PlayEffect(sweatDropEffect, position, Quaternion.identity);
            }
        }

        /// <summary>
        /// Play dust cloud effect
        /// </summary>
        public void PlayDustCloud(Vector3 position)
        {
            if (dustCloudEffect != null)
            {
                PlayEffect(dustCloudEffect, position, Quaternion.identity);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Play effect from pool or instantiate new
        /// </summary>
        private GameObject PlayEffect(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (prefab == null) return null;

            GameObject effect = GetPooledEffect(prefab);

            if (effect == null)
            {
                effect = Instantiate(prefab, position, rotation);
            }
            else
            {
                effect.transform.position = position;
                effect.transform.rotation = rotation;
                effect.SetActive(true);
            }

            // Auto-return to pool after lifetime
            StartCoroutine(ReturnToPoolAfterDelay(effect, prefab.name, effectLifetime));

            // Play particle system if exists
            ParticleSystem particles = effect.GetComponent<ParticleSystem>();
            if (particles != null)
            {
                particles.Play();
            }

            return effect;
        }

        /// <summary>
        /// Get effect from pool
        /// </summary>
        private GameObject GetPooledEffect(GameObject prefab)
        {
            string key = prefab.name;

            if (!effectPools.ContainsKey(key))
            {
                effectPools[key] = new Queue<GameObject>();
            }

            if (effectPools[key].Count > 0)
            {
                return effectPools[key].Dequeue();
            }

            return null;
        }

        /// <summary>
        /// Return effect to pool after delay
        /// </summary>
        private System.Collections.IEnumerator ReturnToPoolAfterDelay(GameObject effect, string poolKey, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (effect != null)
            {
                effect.SetActive(false);
                effect.transform.SetParent(poolContainer);

                if (!effectPools.ContainsKey(poolKey))
                {
                    effectPools[poolKey] = new Queue<GameObject>();
                }

                effectPools[poolKey].Enqueue(effect);
            }
        }

        /// <summary>
        /// Make effect follow transform for duration
        /// </summary>
        private System.Collections.IEnumerator FollowTransformCoroutine(Transform effect, Transform target, float duration)
        {
            float elapsed = 0f;

            while (elapsed < duration && effect != null && target != null)
            {
                effect.position = target.position;
                effect.rotation = target.rotation;
                elapsed += Time.deltaTime;
                yield return null;
            }
        }

        #endregion

        #region Public Utilities

        /// <summary>
        /// Stop all effects immediately
        /// </summary>
        public void StopAllEffects()
        {
            foreach (var pool in effectPools.Values)
            {
                foreach (var effect in pool)
                {
                    if (effect != null)
                    {
                        effect.SetActive(false);
                    }
                }
            }
        }

        /// <summary>
        /// Clear all effect pools
        /// </summary>
        public void ClearPools()
        {
            foreach (var pool in effectPools.Values)
            {
                foreach (var effect in pool)
                {
                    if (effect != null)
                    {
                        Destroy(effect);
                    }
                }
                pool.Clear();
            }
            effectPools.Clear();
        }

        #endregion

#if UNITY_EDITOR
        /// <summary>
        /// Create simple placeholder effects for testing
        /// </summary>
        [ContextMenu("Create Placeholder Effects")]
        private void CreatePlaceholderEffects()
        {
            Debug.Log("Creating placeholder particle effects...");
            // This would create simple particle systems for testing
            // In actual implementation, designers would create proper VFX
        }
#endif
    }
}
