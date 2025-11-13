using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace Morengy.UI
{
    /// <summary>
    /// Pause menu system for pausing the game and accessing settings.
    /// Handles time scaling and input during pause.
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;

        [Header("Settings Panel")]
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;

        [Header("Input")]
        [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

        // State
        private bool isPaused = false;
        private float timeScaleBeforePause = 1f;

        // Singleton
        public static PauseMenu Instance { get; private set; }

        // Properties
        public bool IsPaused => isPaused;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            SetupButtons();
            HideMenu();
        }

        private void Start()
        {
            // Load saved settings
            LoadSettings();
        }

        private void Update()
        {
            // Check for pause input
            if (Input.GetKeyDown(pauseKey))
            {
                if (isPaused)
                    Resume();
                else
                    Pause();
            }
        }

        /// <summary>
        /// Setup button listeners
        /// </summary>
        private void SetupButtons()
        {
            if (resumeButton != null)
                resumeButton.onClick.AddListener(Resume);

            if (restartButton != null)
                restartButton.onClick.AddListener(Restart);

            if (settingsButton != null)
                settingsButton.onClick.AddListener(OpenSettings);

            if (quitButton != null)
                quitButton.onClick.AddListener(QuitToMainMenu);

            // Volume sliders
            if (masterVolumeSlider != null)
                masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);

            if (musicVolumeSlider != null)
                musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);

            if (sfxVolumeSlider != null)
                sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        #region Pause/Resume

        /// <summary>
        /// Pause the game
        /// </summary>
        public void Pause()
        {
            isPaused = true;
            timeScaleBeforePause = Time.timeScale;
            Time.timeScale = 0f;

            ShowMenu();

            // Notify game manager
            if (Managers.GameManager.Instance != null)
            {
                Managers.GameManager.Instance.PauseMatch();
            }

            // Play menu sound
            if (Managers.AudioManager.Instance != null)
            {
                Managers.AudioManager.Instance.PlayMenuSelect();
            }
        }

        /// <summary>
        /// Resume the game
        /// </summary>
        public void Resume()
        {
            isPaused = false;
            Time.timeScale = timeScaleBeforePause;

            HideMenu();
            HideSettings();

            // Notify game manager
            if (Managers.GameManager.Instance != null)
            {
                Managers.GameManager.Instance.ResumeMatch();
            }

            // Play menu sound
            if (Managers.AudioManager.Instance != null)
            {
                Managers.AudioManager.Instance.PlayMenuConfirm();
            }
        }

        #endregion

        #region Menu Actions

        /// <summary>
        /// Restart current match
        /// </summary>
        public void Restart()
        {
            Resume(); // Unpause first

            // Restart via game manager
            if (Managers.GameManager.Instance != null)
            {
                Managers.GameManager.Instance.RestartMatch();
            }
            else
            {
                // Fallback: reload scene
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        /// <summary>
        /// Open settings panel
        /// </summary>
        public void OpenSettings()
        {
            if (settingsPanel != null)
            {
                pauseMenuPanel.SetActive(false);
                settingsPanel.SetActive(true);
            }

            if (Managers.AudioManager.Instance != null)
            {
                Managers.AudioManager.Instance.PlayMenuSelect();
            }
        }

        /// <summary>
        /// Close settings panel
        /// </summary>
        public void CloseSettings()
        {
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
                pauseMenuPanel.SetActive(true);
            }

            SaveSettings();
        }

        /// <summary>
        /// Quit to main menu
        /// </summary>
        public void QuitToMainMenu()
        {
            Resume(); // Unpause
            // Load main menu scene (index 0 by convention)
            SceneManager.LoadScene(0);
        }

        #endregion

        #region Settings

        /// <summary>
        /// Set master volume
        /// </summary>
        private void SetMasterVolume(float value)
        {
            if (Managers.AudioManager.Instance != null)
            {
                Managers.AudioManager.Instance.SetMasterVolume(value);
            }
        }

        /// <summary>
        /// Set music volume
        /// </summary>
        private void SetMusicVolume(float value)
        {
            if (Managers.AudioManager.Instance != null)
            {
                Managers.AudioManager.Instance.SetMusicVolume(value);
            }
        }

        /// <summary>
        /// Set SFX volume
        /// </summary>
        private void SetSFXVolume(float value)
        {
            if (Managers.AudioManager.Instance != null)
            {
                Managers.AudioManager.Instance.SetSFXVolume(value);
            }
        }

        /// <summary>
        /// Load saved settings
        /// </summary>
        private void LoadSettings()
        {
            if (masterVolumeSlider != null)
                masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);

            if (musicVolumeSlider != null)
                musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.7f);

            if (sfxVolumeSlider != null)
                sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        }

        /// <summary>
        /// Save settings to PlayerPrefs
        /// </summary>
        private void SaveSettings()
        {
            if (masterVolumeSlider != null)
                PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);

            if (musicVolumeSlider != null)
                PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);

            if (sfxVolumeSlider != null)
                PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);

            PlayerPrefs.Save();
        }

        #endregion

        #region UI Visibility

        /// <summary>
        /// Show pause menu
        /// </summary>
        private void ShowMenu()
        {
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(true);
            }
        }

        /// <summary>
        /// Hide pause menu
        /// </summary>
        private void HideMenu()
        {
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Hide settings panel
        /// </summary>
        private void HideSettings()
        {
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
            }
        }

        #endregion

        #region Unity Events

        private void OnApplicationPause(bool pauseStatus)
        {
            // Auto-pause when application loses focus
            if (pauseStatus && !isPaused)
            {
                Pause();
            }
        }

        private void OnDestroy()
        {
            // Ensure time scale is restored
            if (isPaused)
            {
                Time.timeScale = 1f;
            }
        }

        #endregion
    }
}
