using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Level selection panel controller.
/// Displays available levels with their unlock status and star ratings.
/// </summary>
public class LevelSelectUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button backButton;
    [SerializeField] private Transform levelGridContainer;
    [SerializeField] private GameObject levelButtonPrefab;

    [Header("Level Settings")]
    [SerializeField] private int totalLevels = 20;
    [SerializeField] private string levelScenePrefix = "Level_";

    private void Start()
    {
        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);

        GenerateLevelButtons();
    }

    private void GenerateLevelButtons()
    {
        if (levelGridContainer == null || levelButtonPrefab == null)
        {
            Debug.LogWarning("LevelSelectUI: Missing levelGridContainer or levelButtonPrefab!");
            return;
        }

        // Clear existing buttons
        foreach (Transform child in levelGridContainer)
        {
            Destroy(child.gameObject);
        }

        // Generate level buttons
        for (int i = 1; i <= totalLevels; i++)
        {
            GameObject buttonObj = Instantiate(levelButtonPrefab, levelGridContainer);
            buttonObj.name = $"Level_{i}_Button";

            // Setup button
            Button button = buttonObj.GetComponent<Button>();
            int levelIndex = i; // Capture for closure

            if (button != null)
            {
                button.onClick.AddListener(() => OnLevelSelected(levelIndex));
            }

            // Setup text
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = i.ToString();
            }

            // Check if level is unlocked
            bool isUnlocked = IsLevelUnlocked(levelIndex);
            button.interactable = isUnlocked;

            // Visual feedback for locked levels
            Image buttonImage = buttonObj.GetComponent<Image>();
            if (buttonImage != null && !isUnlocked)
            {
                buttonImage.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
            }

            // Show stars if level is completed
            SetupStars(buttonObj, levelIndex);
        }
    }

    private bool IsLevelUnlocked(int levelIndex)
    {
        // First level is always unlocked
        if (levelIndex == 1) return true;

        // Check PlayerPrefs for unlock status
        return PlayerPrefs.GetInt($"Level_{levelIndex}_Unlocked", 0) == 1;
    }

    private int GetLevelStars(int levelIndex)
    {
        return PlayerPrefs.GetInt($"Level_{levelIndex}_Stars", 0);
    }

    private void SetupStars(GameObject buttonObj, int levelIndex)
    {
        // Find star container if it exists
        Transform starContainer = buttonObj.transform.Find("StarContainer");
        if (starContainer == null) return;

        int stars = GetLevelStars(levelIndex);
        
        for (int i = 0; i < starContainer.childCount && i < 3; i++)
        {
            Image starImage = starContainer.GetChild(i).GetComponent<Image>();
            if (starImage != null)
            {
                starImage.color = i < stars ? Color.yellow : Color.gray;
            }
        }
    }

    private void OnLevelSelected(int levelIndex)
    {
        string sceneName = $"{levelScenePrefix}{levelIndex}";
        Debug.Log($"Level selected: {sceneName}");
        
        // Intro panel bul
        LevelIntroPanel introPanel = LevelIntroPanel.Instance;
        
        // Instance null ise sahneden bul
        if (introPanel == null)
        {
            introPanel = FindObjectOfType<LevelIntroPanel>(true); // includeInactive = true
            Debug.Log($"LevelIntroPanel FindObjectOfType result: {(introPanel != null ? "Found!" : "Not found")}");
        }
        
        // Intro panel göster
        if (introPanel != null)
        {
            introPanel.ShowForLevel(levelIndex, sceneName);
        }
        else
        {
            // Panel yoksa direkt yükle
            Debug.LogWarning("LevelIntroPanel bulunamadı, level direkt yükleniyor.");
            UIManager.Instance.LoadLevel(sceneName);
        }
    }

    private void OnBackClicked()
    {
        UIManager.Instance.ShowMainMenu();
    }

    private void OnDestroy()
    {
        if (backButton != null)
            backButton.onClick.RemoveAllListeners();
    }

    /// <summary>
    /// Call this to unlock a level (usually after completing the previous one)
    /// </summary>
    public static void UnlockLevel(int levelIndex)
    {
        PlayerPrefs.SetInt($"Level_{levelIndex}_Unlocked", 1);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Call this to set stars for a completed level
    /// </summary>
    public static void SetLevelStars(int levelIndex, int stars)
    {
        int currentStars = PlayerPrefs.GetInt($"Level_{levelIndex}_Stars", 0);
        if (stars > currentStars)
        {
            PlayerPrefs.SetInt($"Level_{levelIndex}_Stars", stars);
            PlayerPrefs.Save();
        }
    }
}
