using UnityEngine;
using System.Collections;

/// <summary>
/// Lazer bariyeri - Topa değerse level'ı yeniden başlatır.
/// </summary>
public class LaserBarrier : LevelItem
{
    [Header("Laser Settings")]
    [Tooltip("Lazer aktif mi?")]
    public bool laserActive = true;
    
    [Tooltip("Yanıp sönme aralığı (0 = sürekli açık)")]
    public float blinkInterval = 0f;
    
    [Tooltip("Açık kalma süresi (blink mode)")]
    public float onDuration = 1f;
    
    [Tooltip("Kapalı kalma süresi (blink mode)")]
    public float offDuration = 0.5f;

    [Header("Visual")]
    [Tooltip("Lazer rengi")]
    public Color laserColor = Color.red;
    
    [Tooltip("Lazer kapalıyken renk")]
    public Color offColor = new Color(0.3f, 0f, 0f, 0.3f);
    
    [Tooltip("Lazer line renderer")]
    public LineRenderer lineRenderer;
    
    [Tooltip("Lazer genişliği")]
    public float laserWidth = 0.1f;
    
    [Tooltip("Titreşim efekti")]
    public bool flickerEffect = true;
    public float flickerIntensity = 0.1f;

    [Header("Endpoints")]
    [Tooltip("Lazer başlangıç noktası")]
    public Transform startPoint;
    
    [Tooltip("Lazer bitiş noktası")]
    public Transform endPoint;

    // Internal
    private SpriteRenderer spriteRenderer;
    private Collider2D laserCollider;
    private float flickerTimer = 0f;

    protected override void Awake()
    {
        base.Awake();
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        laserCollider = GetComponent<Collider2D>();
        
        // Line renderer ayarla
        if (lineRenderer != null)
        {
            lineRenderer.startWidth = laserWidth;
            lineRenderer.endWidth = laserWidth;
            lineRenderer.positionCount = 2;
        }
        
        UpdateLaserState();
    }

    private void Start()
    {
        // Blink mode başlat
        if (blinkInterval > 0)
        {
            StartCoroutine(BlinkLoop());
        }
    }

    protected override void Update()
    {
        base.Update();
        
        // Line renderer pozisyonlarını güncelle
        if (lineRenderer != null && startPoint != null && endPoint != null)
        {
            lineRenderer.SetPosition(0, startPoint.position);
            lineRenderer.SetPosition(1, endPoint.position);
        }
        
        // Titreşim efekti
        if (flickerEffect && laserActive)
        {
            flickerTimer += Time.deltaTime * 20f;
            float flicker = 1f + Mathf.Sin(flickerTimer) * flickerIntensity;
            
            if (lineRenderer != null)
            {
                lineRenderer.startWidth = laserWidth * flicker;
                lineRenderer.endWidth = laserWidth * flicker;
            }
        }
    }

    private IEnumerator BlinkLoop()
    {
        while (true)
        {
            // Açık dur
            laserActive = true;
            UpdateLaserState();
            yield return new WaitForSeconds(onDuration);
            
            // Kapalı dur
            laserActive = false;
            UpdateLaserState();
            yield return new WaitForSeconds(offDuration);
        }
    }

    private void UpdateLaserState()
    {
        // Collider
        if (laserCollider != null)
        {
            laserCollider.enabled = laserActive;
        }
        
        // Sprite rengi
        if (spriteRenderer != null)
        {
            spriteRenderer.color = laserActive ? laserColor : offColor;
        }
        
        // Line renderer
        if (lineRenderer != null)
        {
            lineRenderer.startColor = laserActive ? laserColor : offColor;
            lineRenderer.endColor = laserActive ? laserColor : offColor;
            lineRenderer.enabled = laserActive || offColor.a > 0;
        }
    }

    protected override void OnBallEnter(Rigidbody2D ball)
    {
        if (!laserActive) return;
        
        // Ölüm!
        Debug.Log("LAZER! Level yeniden başlatılıyor...");
        PlayActivationFeedback();
        
        // Kısa gecikme sonra restart
        StartCoroutine(RestartAfterDelay(0.5f));
    }

    private IEnumerator RestartAfterDelay(float delay)
    {
        // Ball'ı dondur
        if (ballRigidbody != null)
        {
            ballRigidbody.linearVelocity = Vector2.zero;
            ballRigidbody.simulated = false;
        }
        
        yield return new WaitForSeconds(delay);
        
        // Level'ı yeniden başlat
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ReloadLevel();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
            );
        }
    }

    /// <summary>
    /// Lazeri aç/kapa (buton ile tetiklemek için)
    /// </summary>
    public void SetLaserState(bool active)
    {
        laserActive = active;
        UpdateLaserState();
    }

    public void ToggleLaser()
    {
        laserActive = !laserActive;
        UpdateLaserState();
    }

    private void OnDrawGizmos()
    {
        Vector3 start = startPoint != null ? startPoint.position : transform.position - Vector3.right;
        Vector3 end = endPoint != null ? endPoint.position : transform.position + Vector3.right;
        
        Gizmos.color = laserActive ? laserColor : offColor;
        Gizmos.DrawLine(start, end);
        
        // Uç noktaları
        Gizmos.DrawWireSphere(start, 0.1f);
        Gizmos.DrawWireSphere(end, 0.1f);
    }
}
