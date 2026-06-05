using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Settings panel controller.
/// Handles audio, vibration, language, and data reset settings.
/// </summary>
public class SettingsUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button backButton;
    
    [Header("Audio Settings")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TextMeshProUGUI musicValueText;
    [SerializeField] private TextMeshProUGUI sfxValueText;

    [Header("Other Settings")]
    [SerializeField] private Toggle vibrationToggle;
    [SerializeField] private TMP_Dropdown languageDropdown;

    [Header("Reset")]
    [SerializeField] private Button resetProgressButton;
    [SerializeField] private GameObject resetConfirmPanel;
    [SerializeField] private Button confirmResetButton;
    [SerializeField] private Button cancelResetButton;

    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";
    private const string VIBRATION_KEY = "Vibration";
    private const string LANGUAGE_KEY = "Language";

    private void Start()
    {
        SetupButtons();
        LoadSettings();
    }

    private void SetupButtons()
    {
        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);

        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        if (vibrationToggle != null)
            vibrationToggle.onValueChanged.AddListener(OnVibrationChanged);

        if (languageDropdown != null)
            languageDropdown.onValueChanged.AddListener(OnLanguageChanged);

        if (resetProgressButton != null)
            resetProgressButton.onClick.AddListener(ShowResetConfirmation);

        if (confirmResetButton != null)
            confirmResetButton.onClick.AddListener(OnConfirmReset);

        if (cancelResetButton != null)
            cancelResetButton.onClick.AddListener(HideResetConfirmation);
    }

    private void LoadSettings()
    {
        // Load music volume
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
        if (musicSlider != null)
        {
            musicSlider.value = musicVolume;
            UpdateMusicValueText(musicVolume);
        }

        // Load SFX volume
        float sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
        if (sfxSlider != null)
        {
            sfxSlider.value = sfxVolume;
            UpdateSFXValueText(sfxVolume);
        }

        // Load vibration setting
        bool vibrationEnabled = PlayerPrefs.GetInt(VIBRATION_KEY, 1) == 1;
        if (vibrationToggle != null)
        {
            vibrationToggle.isOn = vibrationEnabled;
        }

        // Load language setting
        int languageIndex = PlayerPrefs.GetInt(LANGUAGE_KEY, 0);
        if (languageDropdown != null)
        {
            languageDropdown.value = languageIndex;
        }

        // Hide reset confirmation panel
        if (resetConfirmPanel != null)
        {
            resetConfirmPanel.SetActive(false);
        }
    }

    private void OnMusicVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, value);
        PlayerPrefs.Save();
        UpdateMusicValueText(value);

        // Apply volume to AudioManager if you have one
        // AudioManager.Instance?.SetMusicVolume(value);
    }

    private void UpdateMusicValueText(float value)
    {
        if (musicValueText != null)
        {
            musicValueText.text = $"{Mathf.RoundToInt(value * 100)}%";
        }
    }

    private void OnSFXVolumeChanged(float value)
    {
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, value);
        PlayerPrefs.Save();
        UpdateSFXValueText(value);

        // Apply volume to AudioManager if you have one
        // AudioManager.Instance?.SetSFXVolume(value);
    }

    private void UpdateSFXValueText(float value)
    {
        if (sfxValueText != null)
        {
            sfxValueText.text = $"{Mathf.RoundToInt(value * 100)}%";
        }
    }

    private void OnVibrationChanged(bool isOn)
    {
        PlayerPrefs.SetInt(VIBRATION_KEY, isOn ? 1 : 0);
        PlayerPrefs.Save();

        // Test vibration when enabled
        if (isOn)
        {
            #if UNITY_ANDROID || UNITY_IOS
            Handheld.Vibrate();
            #endif
        }
    }

    private void OnLanguageChanged(int languageIndex)
    {
        PlayerPrefs.SetInt(LANGUAGE_KEY, languageIndex);
        PlayerPrefs.Save();

        // Apply language change
        // LocalizationManager.Instance?.SetLanguage(languageIndex);
        Debug.Log($"Language changed to index: {languageIndex}");
    }

    private void ShowResetConfirmation()
    {
        if (resetConfirmPanel != null)
        {
            resetConfirmPanel.SetActive(true);
        }
    }

    private void HideResetConfirmation()
    {
        if (resetConfirmPanel != null)
        {
            resetConfirmPanel.SetActive(false);
        }
    }

    private void OnConfirmReset()
    {
        // Reset all progress data
        ResetAllProgress();
        HideResetConfirmation();

        Debug.Log("All progress has been reset! Reloading scene...");
        
        // Sahneyi yeniden yükle - UI'ın güncellenmesi için
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

    private void ResetAllProgress()
    {
        Debug.Log("[Settings] Resetting all progress...");
        
        // Get all PlayerPrefs keys that store progress
        // Note: Unity doesn't provide a way to iterate all keys,
        // so we need to delete known keys

        // Reset level progress (both formats)
        for (int i = 0; i <= 100; i++) // Including 0 for buildIndex
        {
            PlayerPrefs.DeleteKey($"Level_{i}_Unlocked");
            PlayerPrefs.DeleteKey($"Level_{i}_Stars");
            PlayerPrefs.DeleteKey($"Level_{i}_Completed");
        }
        
        // Reset UnlockedLevels counter
        PlayerPrefs.DeleteKey("UnlockedLevels");

        // Reset ball unlocks
        for (int i = 0; i < 50; i++) // Assuming max 50 balls
        {
            PlayerPrefs.DeleteKey($"Ball_{i}_Unlocked");
        }
        PlayerPrefs.DeleteKey("EquippedBall");

        // Reset equipment
        PlayerPrefs.DeleteKey("EquippedBat");
        PlayerPrefs.DeleteKey("EquippedPowerUp");
        PlayerPrefs.DeleteKey("EquippedSkin");

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 50; j++)
            {
                PlayerPrefs.DeleteKey($"Equipment_{i}_{j}_Unlocked");
            }
        }

        // Keep settings (music, sfx, vibration, language)
        // These are intentionally NOT reset

        PlayerPrefs.Save();
        Debug.Log("[Settings] All progress has been reset!");
    }

    private void OnBackClicked()
    {
        // Save all settings before going back
        PlayerPrefs.Save();
        UIManager.Instance.ShowMainMenu();
    }

    private void OnDestroy()
    {
        if (backButton != null) backButton.onClick.RemoveAllListeners();
        if (musicSlider != null) musicSlider.onValueChanged.RemoveAllListeners();
        if (sfxSlider != null) sfxSlider.onValueChanged.RemoveAllListeners();
        if (vibrationToggle != null) vibrationToggle.onValueChanged.RemoveAllListeners();
        if (languageDropdown != null) languageDropdown.onValueChanged.RemoveAllListeners();
        if (resetProgressButton != null) resetProgressButton.onClick.RemoveAllListeners();
        if (confirmResetButton != null) confirmResetButton.onClick.RemoveAllListeners();
        if (cancelResetButton != null) cancelResetButton.onClick.RemoveAllListeners();
    }

    // Static methods for accessing settings from other scripts
    public static float GetMusicVolume() => PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
    public static float GetSFXVolume() => PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);
    public static bool IsVibrationEnabled() => PlayerPrefs.GetInt(VIBRATION_KEY, 1) == 1;
    public static int GetLanguageIndex() => PlayerPrefs.GetInt(LANGUAGE_KEY, 0);
}
