using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Level oynarken gösterilen UI elementlerini yönetir.
/// Her level sahnesinde Canvas altında olmalı.
/// </summary>
public class LevelUI : MonoBehaviour
{
    [Header("HUD - Üst Panel")]
    [Tooltip("Toplanan yıldız sayısı")]
    public TextMeshProUGUI starCountText;
    
    [Tooltip("Yıldız ikonları (3 adet)")]
    public Image[] starIcons;
    
    [Tooltip("Boş yıldız rengi")]
    public Color emptyStarColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    
    [Tooltip("Dolu yıldız rengi")]
    public Color filledStarColor = Color.yellow;

    [Header("HUD - Butonlar")]
    [Tooltip("Pause butonu")]
    public Button pauseButton;
    
    [Tooltip("Restart butonu")]
    public Button restartButton;

    [Header("Pause Menü")]
    [Tooltip("Pause panel")]
    public GameObject pausePanel;
    
    [Tooltip("Resume butonu")]
    public Button resumeButton;
    
    [Tooltip("Main Menu butonu")]
    public Button mainMenuButton;

    [Header("Level Complete Panel")]
    [Tooltip("Level tamamlandığında gösterilen panel")]
    public GameObject levelCompletePanel;
    
    [Tooltip("Level Complete - Yıldız ikonları")]
    public Image[] levelCompleteStars;
    
    [Tooltip("Level Complete - Skor text")]
    public TextMeshProUGUI scoreText;
    
    [Tooltip("Level Complete - Süre text")]
    public TextMeshProUGUI timeText;
    
    [Tooltip("Level Complete - Next Level butonu")]
    public Button nextLevelButton;
    
    [Tooltip("Level Complete - Replay butonu")]
    public Button replayButton;
    
    [Tooltip("Level Complete - Menu butonu")]
    public Button levelCompleteMenuButton;

    [Header("Game Over Panel (Opsiyonel)")]
    public GameObject gameOverPanel;
    public Button retryButton;
    public Button gameOverMenuButton;

    private bool isPaused = false;
    private int collectedStars = 0;

    private void Start()
    {
        // Event'lere abone ol
        GameManager.OnStarCollected += OnStarCollected;
        GameManager.OnLevelCompleted += OnLevelCompleted;

        // Panelleri gizle
        if (pausePanel != null) pausePanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        // Buton listener'ları
        SetupButtons();

        // Başlangıç UI durumu
        UpdateStarDisplay(0);
    }

    private void OnDestroy()
    {
        // Event'lerden çık
        GameManager.OnStarCollected -= OnStarCollected;
        GameManager.OnLevelCompleted -= OnLevelCompleted;
    }

    private void Update()
    {
        // ESC ile pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    private void SetupButtons()
    {
        // HUD Butonları
        if (pauseButton != null)
            pauseButton.onClick.AddListener(PauseGame);
        
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartLevel);

        // Pause Menü Butonları
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);
        
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);

        // Level Complete Butonları
        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(NextLevel);
        
        if (replayButton != null)
            replayButton.onClick.AddListener(RestartLevel);
        
        if (levelCompleteMenuButton != null)
            levelCompleteMenuButton.onClick.AddListener(GoToMainMenu);

        // Game Over Butonları
        if (retryButton != null)
            retryButton.onClick.AddListener(RestartLevel);
        
        if (gameOverMenuButton != null)
            gameOverMenuButton.onClick.AddListener(GoToMainMenu);
    }

    #region Star Display

    private void OnStarCollected(int totalStars)
    {
        collectedStars = totalStars;
        UpdateStarDisplay(totalStars);
    }

    private void UpdateStarDisplay(int stars)
    {
        // Text güncelle
        if (starCountText != null)
        {
            int total = GameManager.Instance != null ? GameManager.Instance.totalStarsInLevel : 3;
            starCountText.text = $"{stars}/{total}";
        }

        // Yıldız ikonlarını güncelle
        if (starIcons != null)
        {
            for (int i = 0; i < starIcons.Length; i++)
            {
                if (starIcons[i] != null)
                {
                    starIcons[i].color = i < stars ? filledStarColor : emptyStarColor;
                }
            }
        }
    }

    #endregion

    #region Level Complete

    private void OnLevelCompleted(int stars, int totalStars)
    {
        // LevelObjectiveManager varsa ondan gerçek yıldız sayısını al
        int earnedStars = stars;
        if (LevelObjectiveManager.Instance != null)
        {
            earnedStars = LevelObjectiveManager.Instance.CalculateStarsEarned();
        }
        
        // Panel göster
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }

        // Yıldızları güncelle - kazanılan yıldızlara göre
        if (levelCompleteStars != null)
        {
            for (int i = 0; i < levelCompleteStars.Length; i++)
            {
                if (levelCompleteStars[i] != null)
                {
                    // Kazanılan yıldızlar sarı, kazanılmayanlar gri
                    levelCompleteStars[i].color = i < earnedStars ? filledStarColor : emptyStarColor;
                }
            }
        }

        // Görev açıklamalarını göster (opsiyonel)
        UpdateObjectiveTexts(earnedStars);

        // Skor
        if (scoreText != null && GameManager.Instance != null)
        {
            scoreText.text = $"Skor: {GameManager.Instance.Score}";
        }

        // Süre
        if (timeText != null && GameManager.Instance != null)
        {
            float time = GameManager.Instance.LevelTime;
            timeText.text = $"Süre: {time:F1}s";
        }

        // Time scale'i durdurma (panel animasyonu için)
        // Time.timeScale = 0f; // Opsiyonel - animasyon istiyorsan kaldır
    }

    private void UpdateObjectiveTexts(int earnedStars)
    {
        if (LevelObjectiveManager.Instance == null) return;
        
        // Eğer görev text'leri varsa güncelle
        // objectiveText1, objectiveText2, objectiveText3 gibi alanlar eklenebilir
        // Örnek:
        // if (objective1Text != null)
        // {
        //     objective1Text.text = LevelObjectiveManager.Instance.GetObjectiveDescription(1);
        //     objective1Text.color = earnedStars >= 1 ? Color.green : Color.gray;
        // }
    }

    #endregion

    #region Pause System

    public void PauseGame()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsLevelComplete) return;

        isPaused = true;
        Time.timeScale = 0f;
        
        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    #endregion

    #region Navigation

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReloadLevel();
        }
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadNextLevel();
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GoToMainMenu();
        }
    }

    #endregion

    #region Game Over (Opsiyonel)

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    #endregion
}
