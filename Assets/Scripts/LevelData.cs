using UnityEngine;

/// <summary>
/// Her level için görev/hedef bilgilerini tutan ScriptableObject.
/// Assets/LevelData/ klasöründe oluşturulur.
/// </summary>
[CreateAssetMenu(fileName = "Level_XX", menuName = "Box Ball/Level Data", order = 1)]
public class LevelData : ScriptableObject
{
    [Header("Level Info")]
    public string levelName = "Level 1";
    public int levelNumber = 1;
    
    [TextArea(2, 4)]
    public string levelDescription = "Topu kutuya sok!";

    [Header("Objectives - Yıldız Kazanma Koşulları")]
    [Tooltip("1 yıldız için: Her zaman - Level'ı tamamla")]
    public bool star1_CompleteLevel = true;
    
    [Tooltip("2 yıldız için koşul tipi")]
    public ObjectiveType star2_ObjectiveType = ObjectiveType.None;
    public float star2_Value = 0f;
    
    [Tooltip("3 yıldız için koşul tipi")]
    public ObjectiveType star3_ObjectiveType = ObjectiveType.None;
    public float star3_Value = 0f;

    [Header("Optional Constraints")]
    [Tooltip("Maksimum vuruş sayısı (0 = sınırsız)")]
    public int maxHits = 0;
    
    [Tooltip("Süre limiti saniye (0 = sınırsız)")]
    public float timeLimit = 0f;

    [Header("Ball Selection")]
    [Tooltip("Bu level için zorunlu top (null = oyuncu seçer)")]
    public GameObject requiredBall;
    
    [Tooltip("Oyuncunun top seçmesine izin ver")]
    public bool allowBallSelection = false;

    [Header("Scene Reference")]
    [Tooltip("Bu level'ın scene adı")]
    public string sceneName;
}

/// <summary>
/// Görev/Hedef tipleri
/// </summary>
public enum ObjectiveType
{
    None,                    // Hedef yok (otomatik kazanılır)
    CollectAllStars,         // Tüm yıldızları topla
    CollectStars,            // X adet yıldız topla (Value = sayı)
    CompleteInTime,          // X saniye içinde tamamla (Value = saniye)
    CompleteWithMaxHits,     // Maksimum X vuruşta tamamla (Value = vuruş sayısı)
    UseSpecificItem,         // Belirli item'ı kullan
    DontUseItem,             // Belirli item'ı kullanma
    BounceCount,             // X kez zıpla (Value = zıplama sayısı)
    BreakAllWalls,           // Tüm duvarları kır
    UseAllPortals,           // Tüm portalları kullan
    ScoreThreshold           // X puan kazan (Value = puan)
}
