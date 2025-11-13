using UnityEngine;
using TMPro;
using System.Collections;

namespace Morengy.UI
{
    /// <summary>
    /// Floating damage number that appears when fighters take damage.
    /// Provides visual feedback for damage dealt.
    /// </summary>
    public class DamagePopup : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float lifetime = 1.5f;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float fadeSpeed = 2f;
        [SerializeField] private AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Visual Settings")]
        [SerializeField] private Color normalDamageColor = Color.white;
        [SerializeField] private Color criticalDamageColor = Color.red;
        [SerializeField] private Color healColor = Color.green;
        [SerializeField] private float criticalScale = 1.5f;

        // Components
        private TextMeshProUGUI textMesh;
        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;

        // Animation state
        private Vector3 startPosition;
        private Vector3 targetPosition;
        private float elapsedTime = 0f;

        private void Awake()
        {
            textMesh = GetComponent<TextMeshProUGUI>();
            canvasGroup = GetComponent<CanvasGroup>();
            rectTransform = GetComponent<RectTransform>();

            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        /// <summary>
        /// Initialize and display damage popup
        /// </summary>
        public void Initialize(float damage, bool isCritical = false, bool isHeal = false)
        {
            // Set text
            string damageText = Mathf.Abs(damage).ToString("F0");

            if (isCritical)
            {
                damageText = "CRIT! " + damageText;
                textMesh.color = criticalDamageColor;
                transform.localScale = Vector3.one * criticalScale;
            }
            else if (isHeal)
            {
                damageText = "+" + damageText;
                textMesh.color = healColor;
            }
            else
            {
                textMesh.color = normalDamageColor;
            }

            textMesh.text = damageText;

            // Set animation
            startPosition = rectTransform.anchoredPosition;
            targetPosition = startPosition + Vector3.up * 100f + Random.insideUnitCircle * 30f;

            // Start animation
            StartCoroutine(AnimatePopup());
        }

        /// <summary>
        /// Animate popup movement and fade
        /// </summary>
        private IEnumerator AnimatePopup()
        {
            elapsedTime = 0f;

            while (elapsedTime < lifetime)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / lifetime;

                // Move using curve
                float curveValue = moveCurve.Evaluate(t);
                rectTransform.anchoredPosition = Vector3.Lerp(startPosition, targetPosition, curveValue);

                // Fade out
                canvasGroup.alpha = 1f - (t * fadeSpeed);

                yield return null;
            }

            // Destroy when done
            Destroy(gameObject);
        }

        /// <summary>
        /// Stop animation and destroy immediately
        /// </summary>
        public void StopAndDestroy()
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Manager for creating and pooling damage popups
    /// </summary>
    public class DamagePopupManager : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private GameObject damagePopupPrefab;

        [Header("Positioning")]
        [SerializeField] private Canvas worldCanvas;
        [SerializeField] private Vector3 popupOffset = new Vector3(0, 2f, 0);

        // Singleton
        public static DamagePopupManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Create world canvas if not assigned
            if (worldCanvas == null)
            {
                CreateWorldCanvas();
            }
        }

        /// <summary>
        /// Create world space canvas for popups
        /// </summary>
        private void CreateWorldCanvas()
        {
            GameObject canvasObj = new GameObject("DamagePopupCanvas");
            canvasObj.transform.SetParent(transform);

            worldCanvas = canvasObj.AddComponent<Canvas>();
            worldCanvas.renderMode = RenderMode.WorldSpace;
            worldCanvas.sortingOrder = 100;

            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            // Scale canvas
            canvasObj.transform.localScale = Vector3.one * 0.01f;
        }

        /// <summary>
        /// Spawn damage popup at world position
        /// </summary>
        public void SpawnDamagePopup(Vector3 worldPosition, float damage, bool isCritical = false)
        {
            if (damagePopupPrefab == null)
            {
                Debug.LogWarning("DamagePopup prefab not assigned!");
                return;
            }

            // Instantiate popup
            GameObject popupObj = Instantiate(damagePopupPrefab, worldCanvas.transform);
            popupObj.transform.position = worldPosition + popupOffset;

            // Initialize
            DamagePopup popup = popupObj.GetComponent<DamagePopup>();
            if (popup != null)
            {
                popup.Initialize(damage, isCritical);
            }

            // Make popup face camera
            if (Camera.main != null)
            {
                popupObj.transform.rotation = Quaternion.LookRotation(
                    popupObj.transform.position - Camera.main.transform.position
                );
            }
        }

        /// <summary>
        /// Spawn heal popup
        /// </summary>
        public void SpawnHealPopup(Vector3 worldPosition, float healAmount)
        {
            if (damagePopupPrefab == null) return;

            GameObject popupObj = Instantiate(damagePopupPrefab, worldCanvas.transform);
            popupObj.transform.position = worldPosition + popupOffset;

            DamagePopup popup = popupObj.GetComponent<DamagePopup>();
            if (popup != null)
            {
                popup.Initialize(healAmount, false, true);
            }

            if (Camera.main != null)
            {
                popupObj.transform.rotation = Quaternion.LookRotation(
                    popupObj.transform.position - Camera.main.transform.position
                );
            }
        }

        /// <summary>
        /// Clear all active popups
        /// </summary>
        public void ClearAllPopups()
        {
            DamagePopup[] popups = FindObjectsOfType<DamagePopup>();
            foreach (var popup in popups)
            {
                popup.StopAndDestroy();
            }
        }
    }
}
