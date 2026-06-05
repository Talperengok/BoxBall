using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Main Menu UI controller - handles button clicks on the main menu.
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button levelsButton;
    [SerializeField] private Button ballsButton;
    [SerializeField] private Button equipmentButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    [Header("First Level")]
    [SerializeField] private string firstLevelName = "SampleScene";

    private void Start()
    {
        SetupButtonListeners();
    }

    private void SetupButtonListeners()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayClicked);

        if (levelsButton != null)
            levelsButton.onClick.AddListener(OnLevelsClicked);

        if (ballsButton != null)
            ballsButton.onClick.AddListener(OnBallsClicked);

        if (equipmentButton != null)
            equipmentButton.onClick.AddListener(OnEquipmentClicked);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnPlayClicked()
    {
        // Son oynanan level'ı veya ilk level'ı göster
        int lastLevel = PlayerPrefs.GetInt("LastPlayedLevel", 1);
        string sceneName = $"Level_{lastLevel}";
        
        // Intro panel bul
        LevelIntroPanel introPanel = LevelIntroPanel.Instance;
        if (introPanel == null)
        {
            introPanel = FindObjectOfType<LevelIntroPanel>(true);
        }
        
        // Intro panel göster
        if (introPanel != null)
        {
            introPanel.ShowForLevel(lastLevel, sceneName);
        }
        else
        {
            // Panel yoksa direkt yükle
            UIManager.Instance.LoadLevel(firstLevelName);
        }
    }

    private void OnLevelsClicked()
    {
        UIManager.Instance.ShowLevelSelect();
    }

    private void OnBallsClicked()
    {
        UIManager.Instance.ShowBallSelect();
    }

    private void OnEquipmentClicked()
    {
        UIManager.Instance.ShowEquipment();
    }

    private void OnSettingsClicked()
    {
        UIManager.Instance.ShowSettings();
    }

    private void OnQuitClicked()
    {
        UIManager.Instance.QuitGame();
    }

    private void OnDestroy()
    {
        // Clean up listeners
        if (playButton != null) playButton.onClick.RemoveAllListeners();
        if (levelsButton != null) levelsButton.onClick.RemoveAllListeners();
        if (ballsButton != null) ballsButton.onClick.RemoveAllListeners();
        if (equipmentButton != null) equipmentButton.onClick.RemoveAllListeners();
        if (settingsButton != null) settingsButton.onClick.RemoveAllListeners();
        if (quitButton != null) quitButton.onClick.RemoveAllListeners();
    }
}
