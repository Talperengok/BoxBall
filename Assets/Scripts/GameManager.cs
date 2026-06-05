using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Level Settings")]
    [Tooltip("Bu level'daki toplam yıldız sayısı")]
    public int totalStarsInLevel = 3;
    
    [Tooltip("Level tamamlandıktan sonra bekleme süresi (sadece autoAdvance açıksa)")]
    public float winDelay = 2.0f;
    
    [Tooltip("True ise otomatik sonraki level'a geçer, false ise UI butonunu bekler")]
    public bool autoAdvanceToNextLevel = false;
    
    [Tooltip("Main Menu scene adı")]
    public string mainMenuSceneName = "MainMenu";

    [Header("UI References")]
    [Tooltip("Level Complete UI paneli")]
    public GameObject levelCompleteUI;
    
    [Tooltip("Yıldız sayısı text'i (opsiyonel)")]
    public TMPro.TextMeshProUGUI starCountText;

    // Events
    public static event Action<int> OnStarCollected;
    public static event Action<int, int> OnLevelCompleted; // collectedStars, totalStars

    // State
    private bool levelComplete = false;
    private int collectedStars = 0;
    private int score = 0;
    private float levelStartTime;

    public int CollectedStars => collectedStars;
    public int Score => score;
    public float LevelTime => Time.time - levelStartTime;
    public bool IsLevelComplete => levelComplete;

    private void Awake()
    {
        // Simple Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        levelStartTime = Time.time;
        
        // UI'ı gizle
        if (levelCompleteUI != null)
        {
            levelCompleteUI.SetActive(false);
        }
        
        UpdateStarUI();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    /// <summary>
    /// Yıldız toplandığında çağrılır
    /// </summary>
    public void CollectStar(int pointValue = 100)
    {
        if (levelComplete) return;
        
        collectedStars++;
        score += pointValue;
        
        Debug.Log($"[GameManager] Star collected! {collectedStars}/{totalStarsInLevel}");
        
        UpdateStarUI();
        OnStarCollected?.Invoke(collectedStars);
    }

    /// <summary>
    /// Skora puan ekle
    /// </summary>
    public void AddScore(int points)
    {
        score += points;
        Debug.Log($"[GameManager] Score: {score}");
    }

    private void UpdateStarUI()
    {
        if (starCountText != null)
        {
            starCountText.text = $"{collectedStars}/{totalStarsInLevel}";
        }
    }

    /// <summary>
    /// Level tamamlandığında GoalBox tarafından çağrılır
    /// </summary>
    public void OnLevelComplete()
    {
        if (levelComplete) return;
        
        levelComplete = true;
        float completionTime = LevelTime;
        
        Debug.Log($"[GameManager] Level Complete! Stars: {collectedStars}/{totalStarsInLevel}, Time: {completionTime:F1}s, Score: {score}");
        
        // Level verilerini kaydet
        SaveLevelData();
        
        // UI göster
        if (levelCompleteUI != null)
        {
            levelCompleteUI.SetActive(true);
        }
        
        // Event
        OnLevelCompleted?.Invoke(collectedStars, totalStarsInLevel);
        
        // Otomatik geçiş açıksa sonraki level'a geç
        if (autoAdvanceToNextLevel)
        {
            Invoke(nameof(LoadNextLevel), winDelay);
        }
        // Değilse LevelUI butonlarını bekle (LevelCompletePanel gösterildi)
    }

    private void SaveLevelData()
    {
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        string levelKey = $"Level_{currentLevel}";
        
        // LevelObjectiveManager'dan kazanılan yıldız sayısını al
        int earnedStars = collectedStars; // Varsayılan: toplanan yıldız sayısı
        if (LevelObjectiveManager.Instance != null)
        {
            earnedStars = LevelObjectiveManager.Instance.CalculateStarsEarned();
        }
        
        Debug.Log($"[GameManager] Saving level data - Level: {currentLevel}, Earned Stars: {earnedStars}");
        
        // En yüksek yıldız sayısını kaydet
        int previousStars = PlayerPrefs.GetInt($"{levelKey}_Stars", 0);
        if (earnedStars > previousStars)
        {
            PlayerPrefs.SetInt($"{levelKey}_Stars", earnedStars);
            Debug.Log($"[GameManager] New high score! {earnedStars} stars saved for {levelKey}");
        }
        
        // Level tamamlandı olarak işaretle
        PlayerPrefs.SetInt($"{levelKey}_Completed", 1);
        
        // Sonraki level'ı aç
        int nextLevel = currentLevel + 1;
        int unlockedLevels = PlayerPrefs.GetInt("UnlockedLevels", 1);
        if (nextLevel > unlockedLevels)
        {
            PlayerPrefs.SetInt("UnlockedLevels", nextLevel);
        }
        
        // Ayrıca Level_{nextLevel}_Unlocked key'ini de ayarla (LevelSelectUI için)
        PlayerPrefs.SetInt($"Level_{nextLevel}_Unlocked", 1);
        
        PlayerPrefs.Save();
        Debug.Log($"[GameManager] Level data saved. Next level unlocked: {nextLevel}");
    }

    /// <summary>
    /// Sonraki level'a geç
    /// </summary>
    public void LoadNextLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;
        
        // Eğer sonraki scene varsa yükle, yoksa ana menüye dön
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.Log("[GameManager] All levels completed! Returning to main menu.");
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }

    /// <summary>
    /// Mevcut level'ı yeniden yükle
    /// </summary>
    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Ana menüye dön
    /// </summary>
    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    /// <summary>
    /// Belirli bir level'ı yükle
    /// </summary>
    public void LoadLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(levelIndex);
        }
        else
        {
            Debug.LogError($"[GameManager] Invalid level index: {levelIndex}");
        }
    }

    /// <summary>
    /// Level'ın kilidinin açık olup olmadığını kontrol et
    /// </summary>
    public static bool IsLevelUnlocked(int levelIndex)
    {
        int unlockedLevels = PlayerPrefs.GetInt("UnlockedLevels", 1);
        return levelIndex <= unlockedLevels;
    }

    /// <summary>
    /// Belirli bir level'da toplanan yıldız sayısını al
    /// </summary>
    public static int GetLevelStars(int levelIndex)
    {
        return PlayerPrefs.GetInt($"Level_{levelIndex}_Stars", 0);
    }
}

