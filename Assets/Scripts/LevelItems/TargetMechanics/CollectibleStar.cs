using UnityEngine;

/// <summary>
/// Toplanabilir yıldız - Bonus puan verir.
/// </summary>
public class CollectibleStar : LevelItem
{
    [Header("Collectible Settings")]
    [Tooltip("Puan değeri")]
    public int pointValue = 100;
    
    [Tooltip("Toplama efekti")]
    public GameObject collectEffect;
    
    [Tooltip("Toplama sesi")]
    public AudioClip collectSound;

    [Header("Animation")]
    [Tooltip("Döndür")]
    public bool rotate = true;
    
    [Tooltip("Dönüş hızı (derece/saniye)")]
    public float rotationSpeed = 90f;
    
    [Tooltip("Yukarı aşağı hareket")]
    public bool bobUpDown = true;
    
    [Tooltip("Bob yüksekliği")]
    public float bobHeight = 0.2f;
    
    [Tooltip("Bob hızı")]
    public float bobSpeed = 2f;

    [Header("Visual")]
    [Tooltip("Yıldız rengi")]
    public Color starColor = Color.yellow;

    // Internal
    private Vector3 startPosition;
    private SpriteRenderer spriteRenderer;

    protected override void Awake()
    {
        base.Awake();
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = starColor;
        }
    }

    protected override void Update()
    {
        base.Update();
        
        if (!isActive) return;
        
        // Dönüş animasyonu
        if (rotate)
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }
        
        // Yukarı-aşağı animasyonu
        if (bobUpDown)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        }
    }

    protected override void OnBallEnter(Rigidbody2D ball)
    {
        Collect();
    }

    private void Collect()
    {
        // Puan ekle (GameManager'a bildir)
        Debug.Log($"Yıldız toplandı! +{pointValue} puan");
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CollectStar(pointValue);
        }
        
        // LevelObjectiveManager'a bildir (görev takibi için)
        if (LevelObjectiveManager.Instance != null)
        {
            LevelObjectiveManager.Instance.RecordStarCollected();
        }
        
        // Efekt
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }
        
        // Ses
        if (collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }
        
        PlayActivationFeedback();
        
        // Yok et
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        // Yıldız sembolü
        Gizmos.color = starColor;
        
        Vector3 center = transform.position;
        float size = 0.3f;
        
        // 5 köşeli yıldız çiz
        for (int i = 0; i < 5; i++)
        {
            float angle1 = (i * 72f - 90f) * Mathf.Deg2Rad;
            float angle2 = ((i + 2) * 72f - 90f) * Mathf.Deg2Rad;
            
            Vector3 p1 = center + new Vector3(Mathf.Cos(angle1), Mathf.Sin(angle1), 0) * size;
            Vector3 p2 = center + new Vector3(Mathf.Cos(angle2), Mathf.Sin(angle2), 0) * size;
            
            Gizmos.DrawLine(p1, p2);
        }
    }
}
