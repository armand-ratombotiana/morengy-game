using UnityEngine;
using System.Collections.Generic;

namespace Morengy.Managers
{
    /// <summary>
    /// Manages all audio in the game including music and sound effects.
    /// Supports dynamic music layers and 3D positional audio.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource ambienceSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private int sfxPoolSize = 10;

        [Header("Music")]
        [SerializeField] private AudioClip menuMusic;
        [SerializeField] private AudioClip fightMusicLowIntensity;
        [SerializeField] private AudioClip fightMusicHighIntensity;
        [SerializeField] private AudioClip victoryMusic;
        [SerializeField] private AudioClip defeatMusic;

        [Header("Ambience")]
        [SerializeField] private AudioClip crowdAmbience;
        [SerializeField] private AudioClip oceanWaves;
        [SerializeField] private AudioClip villageAmbience;

        [Header("Combat SFX")]
        [SerializeField] private AudioClip[] lightPunchSounds;
        [SerializeField] private AudioClip[] heavyPunchSounds;
        [SerializeField] private AudioClip[] kickSounds;
        [SerializeField] private AudioClip[] blockSounds;
        [SerializeField] private AudioClip criticalHitSound;
        [SerializeField] private AudioClip dodgeWhoosh;

        [Header("Impact SFX")]
        [SerializeField] private AudioClip[] bodyImpactSounds;
        [SerializeField] private AudioClip[] headImpactSounds;
        [SerializeField] private AudioClip knockdownSound;
        [SerializeField] private AudioClip koSound;

        [Header("UI SFX")]
        [SerializeField] private AudioClip menuSelect;
        [SerializeField] private AudioClip menuConfirm;
        [SerializeField] private AudioClip countdownBeep;
        [SerializeField] private AudioClip fightAnnounce;
        [SerializeField] private AudioClip roundEndBell;

        [Header("Crowd SFX")]
        [SerializeField] private AudioClip crowdCheer;
        [SerializeField] private AudioClip crowdGasp;
        [SerializeField] private AudioClip crowdRoar;

        [Header("Settings")]
        [SerializeField] [Range(0f, 1f)] private float masterVolume = 1f;
        [SerializeField] [Range(0f, 1f)] private float musicVolume = 0.7f;
        [SerializeField] [Range(0f, 1f)] private float sfxVolume = 1f;
        [SerializeField] [Range(0f, 1f)] private float ambienceVolume = 0.5f;
        [SerializeField] private float musicFadeTime = 2f;

        // SFX pool
        private List<AudioSource> sfxPool;
        private int currentSFXIndex = 0;

        // Music state
        private float currentMusicIntensity = 0f;
        private AudioClip currentMusicClip;

        // Singleton
        public static AudioManager Instance { get; private set; }

        private void Awake()
        {
            // Singleton
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeAudioSources();
            CreateSFXPool();
        }

        private void Start()
        {
            PlayMenuMusic();
        }

        #region Initialization

        /// <summary>
        /// Initialize main audio sources
        /// </summary>
        private void InitializeAudioSources()
        {
            // Create music source if not assigned
            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.SetParent(transform);
                musicSource = musicObj.AddComponent<AudioSource>();
            }

            musicSource.loop = true;
            musicSource.volume = musicVolume * masterVolume;
            musicSource.playOnAwake = false;

            // Create ambience source if not assigned
            if (ambienceSource == null)
            {
                GameObject ambienceObj = new GameObject("AmbienceSource");
                ambienceObj.transform.SetParent(transform);
                ambienceSource = ambienceObj.AddComponent<AudioSource>();
            }

            ambienceSource.loop = true;
            ambienceSource.volume = ambienceVolume * masterVolume;
            ambienceSource.playOnAwake = false;

            // Create SFX source if not assigned
            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.SetParent(transform);
                sfxSource = sfxObj.AddComponent<AudioSource>();
            }

            sfxSource.volume = sfxVolume * masterVolume;
            sfxSource.playOnAwake = false;
        }

        /// <summary>
        /// Create pool of audio sources for overlapping SFX
        /// </summary>
        private void CreateSFXPool()
        {
            sfxPool = new List<AudioSource>();

            for (int i = 0; i < sfxPoolSize; i++)
            {
                GameObject sfxObj = new GameObject($"SFX_Pool_{i}");
                sfxObj.transform.SetParent(transform);
                AudioSource source = sfxObj.AddComponent<AudioSource>();
                source.volume = sfxVolume * masterVolume;
                source.playOnAwake = false;
                sfxPool.Add(source);
            }
        }

        #endregion

        #region Music Control

        /// <summary>
        /// Play menu music
        /// </summary>
        public void PlayMenuMusic()
        {
            FadeToMusic(menuMusic);
        }

        /// <summary>
        /// Play fight music with intensity
        /// </summary>
        public void PlayFightMusic(float intensity = 0f)
        {
            currentMusicIntensity = intensity;
            AudioClip targetClip = intensity > 0.5f ? fightMusicHighIntensity : fightMusicLowIntensity;
            FadeToMusic(targetClip);
        }

        /// <summary>
        /// Play victory music
        /// </summary>
        public void PlayVictoryMusic()
        {
            FadeToMusic(victoryMusic);
        }

        /// <summary>
        /// Play defeat music
        /// </summary>
        public void PlayDefeatMusic()
        {
            FadeToMusic(defeatMusic);
        }

        /// <summary>
        /// Fade to new music track
        /// </summary>
        private void FadeToMusic(AudioClip newClip)
        {
            if (newClip == currentMusicClip) return;

            StopAllCoroutines();
            StartCoroutine(FadeMusicCoroutine(newClip));
        }

        private System.Collections.IEnumerator FadeMusicCoroutine(AudioClip newClip)
        {
            // Fade out current music
            float startVolume = musicSource.volume;
            float elapsed = 0f;

            while (elapsed < musicFadeTime / 2f)
            {
                elapsed += Time.deltaTime;
                musicSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / (musicFadeTime / 2f));
                yield return null;
            }

            // Switch track
            musicSource.clip = newClip;
            currentMusicClip = newClip;

            if (newClip != null)
            {
                musicSource.Play();
            }

            // Fade in new music
            elapsed = 0f;
            float targetVolume = musicVolume * masterVolume;

            while (elapsed < musicFadeTime / 2f)
            {
                elapsed += Time.deltaTime;
                musicSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / (musicFadeTime / 2f));
                yield return null;
            }

            musicSource.volume = targetVolume;
        }

        /// <summary>
        /// Set music intensity (for dynamic music)
        /// </summary>
        public void SetMusicIntensity(float intensity)
        {
            currentMusicIntensity = Mathf.Clamp01(intensity);

            // Switch between low and high intensity tracks
            AudioClip targetClip = currentMusicIntensity > 0.5f ?
                fightMusicHighIntensity : fightMusicLowIntensity;

            if (targetClip != currentMusicClip && targetClip != null)
            {
                FadeToMusic(targetClip);
            }
        }

        #endregion

        #region Ambience

        /// <summary>
        /// Play ambience loop
        /// </summary>
        public void PlayAmbience(AudioClip ambience)
        {
            if (ambienceSource.clip == ambience && ambienceSource.isPlaying) return;

            ambienceSource.clip = ambience;
            ambienceSource.Play();
        }

        /// <summary>
        /// Stop ambience
        /// </summary>
        public void StopAmbience()
        {
            ambienceSource.Stop();
        }

        #endregion

        #region Combat SFX

        /// <summary>
        /// Play light attack sound
        /// </summary>
        public void PlayLightAttackSound()
        {
            PlayRandomSound(lightPunchSounds);
        }

        /// <summary>
        /// Play heavy attack sound
        /// </summary>
        public void PlayHeavyAttackSound()
        {
            PlayRandomSound(heavyPunchSounds);
        }

        /// <summary>
        /// Play kick sound
        /// </summary>
        public void PlayKickSound()
        {
            PlayRandomSound(kickSounds);
        }

        /// <summary>
        /// Play block sound
        /// </summary>
        public void PlayBlockSound()
        {
            PlayRandomSound(blockSounds);
        }

        /// <summary>
        /// Play critical hit sound
        /// </summary>
        public void PlayCriticalHitSound()
        {
            PlaySound(criticalHitSound, 1.2f);
        }

        /// <summary>
        /// Play dodge sound
        /// </summary>
        public void PlayDodgeSound()
        {
            PlaySound(dodgeWhoosh, 0.8f);
        }

        /// <summary>
        /// Play body impact sound
        /// </summary>
        public void PlayBodyImpactSound()
        {
            PlayRandomSound(bodyImpactSounds);
        }

        /// <summary>
        /// Play head impact sound
        /// </summary>
        public void PlayHeadImpactSound()
        {
            PlayRandomSound(headImpactSounds, 1.1f);
        }

        /// <summary>
        /// Play knockdown sound
        /// </summary>
        public void PlayKnockdownSound()
        {
            PlaySound(knockdownSound, 1.3f);
        }

        /// <summary>
        /// Play KO sound
        /// </summary>
        public void PlayKOSound()
        {
            PlaySound(koSound, 1.5f);
        }

        #endregion

        #region UI SFX

        /// <summary>
        /// Play menu select sound
        /// </summary>
        public void PlayMenuSelect()
        {
            PlaySound(menuSelect, 0.6f);
        }

        /// <summary>
        /// Play menu confirm sound
        /// </summary>
        public void PlayMenuConfirm()
        {
            PlaySound(menuConfirm, 0.8f);
        }

        /// <summary>
        /// Play countdown beep
        /// </summary>
        public void PlayCountdownBeep()
        {
            PlaySound(countdownBeep);
        }

        /// <summary>
        /// Play fight announce
        /// </summary>
        public void PlayFightAnnounce()
        {
            PlaySound(fightAnnounce, 1.2f);
        }

        /// <summary>
        /// Play round end bell
        /// </summary>
        public void PlayRoundEndBell()
        {
            PlaySound(roundEndBell);
        }

        #endregion

        #region Crowd SFX

        /// <summary>
        /// Play crowd cheer
        /// </summary>
        public void PlayCrowdCheer()
        {
            PlaySound(crowdCheer, 0.7f);
        }

        /// <summary>
        /// Play crowd gasp
        /// </summary>
        public void PlayCrowdGasp()
        {
            PlaySound(crowdGasp, 0.7f);
        }

        /// <summary>
        /// Play crowd roar
        /// </summary>
        public void PlayCrowdRoar()
        {
            PlaySound(crowdRoar, 0.8f);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Play single sound effect
        /// </summary>
        private void PlaySound(AudioClip clip, float volumeMultiplier = 1f)
        {
            if (clip == null) return;

            AudioSource source = GetNextSFXSource();
            source.volume = sfxVolume * masterVolume * volumeMultiplier;
            source.PlayOneShot(clip);
        }

        /// <summary>
        /// Play random sound from array
        /// </summary>
        private void PlayRandomSound(AudioClip[] clips, float volumeMultiplier = 1f)
        {
            if (clips == null || clips.Length == 0) return;

            AudioClip randomClip = clips[Random.Range(0, clips.Length)];
            PlaySound(randomClip, volumeMultiplier);
        }

        /// <summary>
        /// Get next available audio source from pool
        /// </summary>
        private AudioSource GetNextSFXSource()
        {
            AudioSource source = sfxPool[currentSFXIndex];
            currentSFXIndex = (currentSFXIndex + 1) % sfxPool.Count;
            return source;
        }

        #endregion

        #region Volume Control

        /// <summary>
        /// Set master volume
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            UpdateAllVolumes();
        }

        /// <summary>
        /// Set music volume
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            musicSource.volume = musicVolume * masterVolume;
        }

        /// <summary>
        /// Set SFX volume
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            UpdateSFXVolumes();
        }

        /// <summary>
        /// Set ambience volume
        /// </summary>
        public void SetAmbienceVolume(float volume)
        {
            ambienceVolume = Mathf.Clamp01(volume);
            ambienceSource.volume = ambienceVolume * masterVolume;
        }

        /// <summary>
        /// Update all volume levels
        /// </summary>
        private void UpdateAllVolumes()
        {
            musicSource.volume = musicVolume * masterVolume;
            ambienceSource.volume = ambienceVolume * masterVolume;
            UpdateSFXVolumes();
        }

        /// <summary>
        /// Update SFX pool volumes
        /// </summary>
        private void UpdateSFXVolumes()
        {
            foreach (AudioSource source in sfxPool)
            {
                source.volume = sfxVolume * masterVolume;
            }
        }

        #endregion
    }
}
