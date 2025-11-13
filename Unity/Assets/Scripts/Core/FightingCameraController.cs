using UnityEngine;

namespace Morengy.Core
{
    /// <summary>
    /// Camera controller for fighting games.
    /// Dynamically frames both fighters and adjusts based on distance.
    /// </summary>
    public class FightingCameraController : MonoBehaviour
    {
        [Header("Fighter References")]
        [SerializeField] private Transform fighter1;
        [SerializeField] private Transform fighter2;

        [Header("Camera Settings")]
        [SerializeField] private float defaultDistance = 12f;
        [SerializeField] private float minDistance = 8f;
        [Sertml:parameter name="maxDistance">18f;
        [SerializeField] private float height = 3f;
        [SerializeField] private float heightDamping = 2f;
        [SerializeField] private float rotationDamping = 3f;

        [Header("Framing")]
        [SerializeField] private float borderPadding = 2f;
        [SerializeField] private float zoomSpeed = 5f;
        [SerializeField] private float minZoom = 5f;
        [SerializeField] private float maxZoom = 15f;

        [Header("Screen Shake")]
        [SerializeField] private float shakeDecay = 2f;
        [SerializeField] private float shakeIntensity = 0.5f;

        // State
        private Vector3 targetPosition;
        private float currentDistance;
        private float shakeAmount = 0f;
        private Vector3 shakeOffset;

        private void Start()
        {
            currentDistance = defaultDistance;

            if (fighter1 == null || fighter2 == null)
            {
                Debug.LogWarning("FightingCameraController missing fighter references!");
            }
        }

        private void LateUpdate()
        {
            if (fighter1 == null || fighter2 == null) return;

            UpdateCameraPosition();
            ApplyScreenShake();
        }

        /// <summary>
        /// Update camera position to frame both fighters
        /// </summary>
        private void UpdateCameraPosition()
        {
            // Calculate midpoint between fighters
            Vector3 midpoint = (fighter1.position + fighter2.position) / 2f;

            // Calculate distance between fighters
            float fighterDistance = Vector3.Distance(fighter1.position, fighter2.position);

            // Calculate target distance based on fighter separation
            float targetDistance = Mathf.Clamp(
                defaultDistance + fighterDistance * 0.5f,
                minDistance,
                maxDistance
            );

            // Smoothly adjust distance
            currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * zoomSpeed);

            // Calculate camera position behind midpoint
            Vector3 direction = (fighter2.position - fighter1.position).normalized;
            Vector3 perpendicular = Vector3.Cross(direction, Vector3.up).normalized;

            // Position camera offset from midpoint
            Vector3 basePosition = midpoint - perpendicular * currentDistance;
            basePosition.y = midpoint.y + height;

            // Smooth camera movement
            transform.position = Vector3.Lerp(
                transform.position,
                basePosition,
                Time.deltaTime * heightDamping
            );

            // Look at midpoint
            Quaternion targetRotation = Quaternion.LookRotation(midpoint - transform.position);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * rotationDamping
            );
        }

        /// <summary>
        /// Apply screen shake effect
        /// </summary>
        private void ApplyScreenShake()
        {
            if (shakeAmount > 0)
            {
                shakeOffset = Random.insideUnitSphere * shakeAmount * shakeIntensity;
                transform.position += shakeOffset;

                shakeAmount -= Time.deltaTime * shakeDecay;
                shakeAmount = Mathf.Max(0, shakeAmount);
            }
        }

        /// <summary>
        /// Trigger screen shake (call on heavy hit, KO, etc.)
        /// </summary>
        public void Shake(float intensity = 1f, float duration = 0.3f)
        {
            shakeAmount = duration;
            shakeIntensity = intensity;
        }

        /// <summary>
        /// Set fighter references at runtime
        /// </summary>
        public void SetFighters(Transform f1, Transform f2)
        {
            fighter1 = f1;
            fighter2 = f2;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (fighter1 == null || fighter2 == null) return;

            // Draw midpoint
            Vector3 midpoint = (fighter1.position + fighter2.position) / 2f;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(midpoint, 0.5f);

            // Draw camera target
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, midpoint);
        }
#endif
    }
}
