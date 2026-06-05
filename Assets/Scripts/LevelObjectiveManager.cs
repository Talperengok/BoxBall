using UnityEngine;

/// <summary>
/// Level'daki görevleri takip eder ve yıldız kazanma durumunu kontrol eder.
/// Her level sahnesinde GameManager ile birlikte olmalı.
/// </summary>
public class LevelObjectiveManager : MonoBehaviour
{
    [Header("Level Data")]
    [Tooltip("Bu level'ın görev bilgileri")]
    public LevelData levelData;

    [Header("Runtime Stats")]
    [SerializeField] private int hitCount = 0;
    [SerializeField] private int collectedStars = 0;
    [SerializeField] private int bounceCount = 0;
    [SerializeField] private int wallsBroken = 0;
    [SerializeField] private int portalsUsed = 0;

    // Singleton
    public static LevelObjectiveManager Instance { get; private set; }

    // Properties
    public int HitCount => hitCount;
    public int CollectedStars => collectedStars;
    public int BounceCount => bounceCount;
    public float ElapsedTime => Time.time - startTime;

    private float startTime;
    private bool levelCompleted = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        startTime = Time.time;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    #region Event Tracking

    /// <summary>
    /// Vuruş yapıldığında çağır
    /// </summary>
    public void RecordHit()
    {
        hitCount++;
        Debug.Log($"[Objectives] Hit count: {hitCount}");
    }

    /// <summary>
    /// Yıldız toplandığında çağır
    /// </summary>
    public void RecordStarCollected()
    {
        collectedStars++;
        Debug.Log($"[Objectives] Stars collected: {collectedStars}");
    }

    /// <summary>
    /// Top zıpladığında çağır
    /// </summary>
    public void RecordBounce()
    {
        bounceCount++;
    }

    /// <summary>
    /// Duvar kırıldığında çağır
    /// </summary>
    public void RecordWallBroken()
    {
        wallsBroken++;
        Debug.Log($"[Objectives] Walls broken: {wallsBroken}");
    }

    /// <summary>
    /// Portal kullanıldığında çağır
    /// </summary>
    public void RecordPortalUsed()
    {
        portalsUsed++;
        Debug.Log($"[Objectives] Portals used: {portalsUsed}");
    }

    #endregion

    #region Star Calculation

    /// <summary>
    /// Level tamamlandığında kazanılan yıldız sayısını hesapla
    /// </summary>
    public int CalculateStarsEarned()
    {
        if (levelData == null) return 1; // Default 1 yıldız

        int stars = 0;

        // Yıldız 1: Level tamamlandı
        if (levelData.star1_CompleteLevel)
        {
            stars = 1;
        }

        // Yıldız 2
        if (CheckObjective(levelData.star2_ObjectiveType, levelData.star2_Value))
        {
            stars = 2;
        }

        // Yıldız 3
        if (stars >= 2 && CheckObjective(levelData.star3_ObjectiveType, levelData.star3_Value))
        {
            stars = 3;
        }

        Debug.Log($"[Objectives] Stars earned: {stars}/3");
        return stars;
    }

    private bool CheckObjective(ObjectiveType type, float value)
    {
        switch (type)
        {
            case ObjectiveType.None:
                return true;

            case ObjectiveType.CollectAllStars:
                return collectedStars >= (levelData != null ? GameManager.Instance?.totalStarsInLevel ?? 3 : 3);

            case ObjectiveType.CollectStars:
                return collectedStars >= (int)value;

            case ObjectiveType.CompleteInTime:
                return ElapsedTime <= value;

            case ObjectiveType.CompleteWithMaxHits:
                return hitCount <= (int)value;

            case ObjectiveType.BounceCount:
                return bounceCount >= (int)value;

            case ObjectiveType.BreakAllWalls:
                // Tüm duvarlar kırıldı mı kontrolü
                return wallsBroken >= (int)value;

            case ObjectiveType.UseAllPortals:
                return portalsUsed >= (int)value;

            case ObjectiveType.ScoreThreshold:
                return GameManager.Instance?.Score >= (int)value;

            default:
                return false;
        }
    }

    #endregion

    #region UI Display

    /// <summary>
    /// Görev açıklamasını döndür (UI için)
    /// </summary>
    public string GetObjectiveDescription(int starNumber)
    {
        if (levelData == null) return "";

        ObjectiveType type;
        float value;

        switch (starNumber)
        {
            case 1:
                return "Level'ı tamamla";
            case 2:
                type = levelData.star2_ObjectiveType;
                value = levelData.star2_Value;
                break;
            case 3:
                type = levelData.star3_ObjectiveType;
                value = levelData.star3_Value;
                break;
            default:
                return "";
        }

        return GetObjectiveText(type, value);
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

    #endregion
}
