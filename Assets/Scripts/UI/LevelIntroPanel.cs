using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Level başlamadan önce gösterilen intro paneli.
/// MainMenu sahnesinde bulunur, level seçildiğinde gösterilir.
/// </summary>
public class LevelIntroPanel : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Level numarası text'i (örn: 'BÖLÜM 4')")]
    public TextMeshProUGUI levelNumberText;
    
    [Tooltip("Level adı text'i (örn: 'Işınlan')")]
    public TextMeshProUGUI levelNameText;
    
    [Tooltip("Level açıklaması text'i")]
    public TextMeshProUGUI levelDescriptionText;

    [Header("Star Objectives")]
    [Tooltip("1. yıldız görevi text'i")]
    public TextMeshProUGUI star1ObjectiveText;
    
    [Tooltip("2. yıldız görevi text'i")]
    public TextMeshProUGUI star2ObjectiveText;
    
    [Tooltip("3. yıldız görevi text'i")]
    public TextMeshProUGUI star3ObjectiveText;

    [Header("Star Icons")]
    public Image star1Icon;
    public Image star2Icon;
    public Image star3Icon;
    
    [Tooltip("Önceki performanstaki yıldız rengi")]
    public Color earnedStarColor = Color.yellow;
    
    [Tooltip("Kazanılmamış yıldız rengi")]
    public Color unearnedStarColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    [Header("Buttons")]
    public Button startButton;
    public Button backButton;

    [Header("Animation")]
    public Animator panelAnimator;
    public string showTrigger = "Show";
    public string hideTrigger = "Hide";

    [Header("Level Data")]
    [Tooltip("LevelData dosyalarının bulunduğu Resources klasörü yolu")]
    public string levelDataResourcePath = "LevelData";
    
    [Tooltip("Tüm level data'ları (Resources'tan yüklemek istemiyorsan buraya ekle)")]
    public LevelData[] allLevelData;

    [Header("Panel Content")]
    [Tooltip("Gizlenecek/Gösterilecek ana içerik paneli (bu objenin child'ı olmalı)")]
    public GameObject panelContent;

    // Singleton
    public static LevelIntroPanel Instance { get; private set; }

    // Current level info
    private int currentLevelIndex;
    private string currentSceneName;
    private LevelData currentLevelData;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        // Singleton - her zaman çalışmalı
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("[LevelIntroPanel] Instance set!");
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // CanvasGroup al veya oluştur (raycast kontrolü için)
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        // Başlangıçta gizle ve raycast'ları engelleme
        HidePanelImmediate();
    }

    private void Start()
    {
        // Button listeners
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartClicked);
        }
        
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackClicked);
        }
    }

    /// <summary>
    /// Level index ile paneli göster (LevelSelectUI tarafından çağrılır)
    /// </summary>
    public void ShowForLevel(int levelIndex, string sceneName)
    {
        Debug.Log($"[LevelIntroPanel] ShowForLevel called - Level: {levelIndex}, Scene: {sceneName}");
        
        currentLevelIndex = levelIndex;
        currentSceneName = sceneName;
        
        // LevelData'yı bul
        currentLevelData = FindLevelData(levelIndex);
        
        // CanvasGroup'u aktif et - görünür ve tıklanabilir yap
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }
        
        // Panel içeriğini göster
        if (panelContent != null)
        {
            panelContent.SetActive(true);
            Debug.Log("[LevelIntroPanel] Panel content activated!");
        }
        else
        {
            Debug.LogError("[LevelIntroPanel] panelContent is NULL! Inspector'da 'Panel Content' alanını doldurun.");
        }
        
        // UI'ı doldur
        PopulateUI();
        
        // Animasyon
        if (panelAnimator != null)
        {
            panelAnimator.SetTrigger(showTrigger);
        }
    }

    /// <summary>
    /// LevelData'yı bul
    /// </summary>
    private LevelData FindLevelData(int levelIndex)
    {
        // Önce array'den ara
        if (allLevelData != null && allLevelData.Length > 0)
        {
            foreach (var data in allLevelData)
            {
                if (data != null && data.levelNumber == levelIndex)
                {
                    return data;
                }
            }
        }
        
        // Resources'tan yükle
        string path = $"{levelDataResourcePath}/Level_{levelIndex}";
        LevelData loaded = Resources.Load<LevelData>(path);
        
        if (loaded != null)
        {
            return loaded;
        }
        
        Debug.LogWarning($"LevelData bulunamadı: {path}");
        return null;
    }

    /// <summary>
    /// UI elementlerini doldur
    /// </summary>
    private void PopulateUI()
    {
        // Level numarası
        if (levelNumberText != null)
        {
            levelNumberText.text = $"BÖLÜM {currentLevelIndex}";
        }
        
        // Level adı
        if (levelNameText != null)
        {
            if (currentLevelData != null)
            {
                levelNameText.text = currentLevelData.levelName;
            }
            else
            {
                levelNameText.text = $"Level {currentLevelIndex}";
            }
        }
        
        // Level açıklaması
        if (levelDescriptionText != null)
        {
            if (currentLevelData != null)
            {
                levelDescriptionText.text = currentLevelData.levelDescription;
            }
            else
            {
                levelDescriptionText.text = "Topu kutuya sok!";
            }
        }
        
        // Önceki yıldız performansını al
        int previousStars = PlayerPrefs.GetInt($"Level_{currentLevelIndex}_Stars", 0);
        
        // Yıldız görevleri
        PopulateStarObjective(1, star1ObjectiveText, star1Icon, previousStars);
        PopulateStarObjective(2, star2ObjectiveText, star2Icon, previousStars);
        PopulateStarObjective(3, star3ObjectiveText, star3Icon, previousStars);
    }

    /// <summary>
    /// Tek bir yıldız görevini doldur
    /// </summary>
    private void PopulateStarObjective(int starNumber, TextMeshProUGUI textField, Image starIcon, int previousStars)
    {
        if (textField == null) return;
        
        string objective = GetObjectiveDescription(starNumber);
        textField.text = $"★ {objective}";
        
        // Yıldız ikonunu güncelle
        if (starIcon != null)
        {
            bool isEarned = previousStars >= starNumber;
            starIcon.color = isEarned ? earnedStarColor : unearnedStarColor;
        }
    }

    /// <summary>
    /// Görev açıklamasını al
    /// </summary>
    private string GetObjectiveDescription(int starNumber)
    {
        if (currentLevelData == null)
        {
            if (starNumber == 1) return "Level'ı tamamla";
            return "???";
        }
        
        switch (starNumber)
        {
            case 1:
                return "Level'ı tamamla";
            case 2:
                return GetObjectiveText(currentLevelData.star2_ObjectiveType, currentLevelData.star2_Value);
            case 3:
                return GetObjectiveText(currentLevelData.star3_ObjectiveType, currentLevelData.star3_Value);
            default:
                return "";
        }
    }

    private string GetObjectiveText(ObjectiveType type, float value)
    {
        switch (type)
        {
            case ObjectiveType.None:
                return "Bonus!";
            case ObjectiveType.CollectAllStars:
                return "Tüm yıldızları topla";
            case ObjectiveType.CollectStars:
                return $"{(int)value} yıldız topla";
            case ObjectiveType.CompleteInTime:
                return $"{value:F0} saniyede tamamla";
            case ObjectiveType.CompleteWithMaxHits:
                return $"Maksimum {(int)value} vuruşta tamamla";
            case ObjectiveType.BounceCount:
                return $"{(int)value} kez zıpla";
            case ObjectiveType.BreakAllWalls:
                return "Tüm duvarları kır";
            case ObjectiveType.UseAllPortals:
                return "Tüm portalları kullan";
            case ObjectiveType.ScoreThreshold:
                return $"{(int)value} puan kazan";
            default:
                return "";
        }
    }

    /// <summary>
    /// Panel'i gizle
    /// </summary>
    public void HidePanel()
    {
        // Animasyon
        if (panelAnimator != null)
        {
            panelAnimator.SetTrigger(hideTrigger);
        }
        else
        {
            HidePanelImmediate();
        }
    }

    /// <summary>
    /// Panel'i hemen gizle (animasyonsuz)
    /// </summary>
    private void HidePanelImmediate()
    {
        if (panelContent != null)
        {
            panelContent.SetActive(false);
        }
        
        // Raycast'ları devre dışı bırak - arkadaki butonlar tıklanabilir olsun
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
    }

    /// <summary>
    /// Başla butonuna tıklandığında
    /// </summary>
    private void OnStartClicked()
    {
        HidePanel();
        
        // Level'ı yükle
        if (!string.IsNullOrEmpty(currentSceneName))
        {
            SceneManager.LoadScene(currentSceneName);
        }
    }

    /// <summary>
    /// Geri butonuna tıklandığında
    /// </summary>
    private void OnBackClicked()
    {
        HidePanel();
    }

    /// <summary>
    /// Animasyon event'i - panel gizlendiğinde
    /// </summary>
    public void OnHideAnimationComplete()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
        
        if (startButton != null) startButton.onClick.RemoveAllListeners();
        if (backButton != null) backButton.onClick.RemoveAllListeners();
    }
}
