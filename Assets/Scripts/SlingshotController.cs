using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Slingshot (Sapan) mekaniği kontrol scripti.
/// Topu geri çekip bırakarak fırlatmayı sağlar.
/// </summary>
public class SlingshotController : MonoBehaviour
{
    [Header("Slingshot Settings")]
    [Tooltip("Maximum çekme mesafesi")]
    public float maxPullDistance = 3f;
    
    [Tooltip("Minimum fırlatma gücü")]
    public float minLaunchForce = 5f;
    
    [Tooltip("Maximum fırlatma gücü")]
    public float maxLaunchForce = 25f;
    
    [Tooltip("Topu çekmeye başlamak için gereken mesafe")]
    public float grabRadius = 0.5f;

    [Header("Visual Feedback")]
    [Tooltip("Çekme çizgisi için LineRenderer")]
    public LineRenderer pullLine;
    
    [Tooltip("Yörünge preview için TrajectoryRenderer")]
    public TrajectoryRenderer trajectoryRenderer;

    [Header("Events")]
    public UnityEvent OnPullStart;
    public UnityEvent OnPullEnd;
    public UnityEvent<Vector2> OnLaunch;

    // Internal state
    private Camera cam;
    private Rigidbody2D ballRb;
    private BallPlacement ballPlacement;
    private Vector2 startPosition;
    private bool isPulling = false;
    private bool isLaunched = false;

    // Current pull data
    private Vector2 pullDirection;
    private float pullDistance;
    private float launchForce;

    public bool IsPulling => isPulling;
    public bool IsLaunched => isLaunched;
    public float CurrentPullDistance => pullDistance;
    public float CurrentLaunchForce => launchForce;
    public Vector2 LaunchVelocity => pullDirection * launchForce;

    void Start()
    {
        cam = Camera.main;
        ballRb = GetComponent<Rigidbody2D>();
        ballPlacement = GetComponent<BallPlacement>();
        
        startPosition = transform.position;
        
        // Başlangıçta top kinematic
        if (ballRb != null)
        {
            ballRb.bodyType = RigidbodyType2D.Kinematic;
            ballRb.linearVelocity = Vector2.zero;
        }
        
        // Pull line'ı gizle
        if (pullLine != null)
        {
            pullLine.enabled = false;
        }
    }

    void Update()
    {
        if (isLaunched) return;
        
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        
        // Mouse basıldığında
        if (Input.GetMouseButtonDown(0))
        {
            TryStartPull(mousePos);
        }
        
        // Mouse tutuluyorken
        if (isPulling && Input.GetMouseButton(0))
        {
            UpdatePull(mousePos);
        }
        
        // Mouse bırakıldığında
        if (Input.GetMouseButtonUp(0) && isPulling)
        {
            ReleasePull();
        }
    }

    /// <summary>
    /// Çekme işlemini başlatmaya çalış
    /// </summary>
    void TryStartPull(Vector2 mousePos)
    {
        // Top pozisyon ayarlanıyorsa (BallPlacement dragging) çekme başlatma
        if (ballPlacement != null && !ballPlacement.isGameStarted)
        {
            float distanceToMouse = Vector2.Distance(transform.position, mousePos);
            
            if (distanceToMouse <= grabRadius)
            {
                isPulling = true;
                startPosition = transform.position;
                
                if (pullLine != null)
                {
                    pullLine.enabled = true;
                }
                
                OnPullStart?.Invoke();
                Debug.Log("[Slingshot] Pull started!");
            }
        }
    }

    /// <summary>
    /// Çekme sırasında güncelleme
    /// </summary>
    void UpdatePull(Vector2 mousePos)
    {
        // Çekme vektörü: başlangıç noktasından mouse pozisyonuna
        Vector2 pullVector = mousePos - startPosition;
        
        // Mesafeyi sınırla
        pullDistance = Mathf.Min(pullVector.magnitude, maxPullDistance);
        
        // Fırlatma yönü = çekmenin tersi
        if (pullVector.magnitude > 0.01f)
        {
            pullDirection = -pullVector.normalized;
        }
        
        // Güç hesapla (mesafeye orantılı)
        float pullRatio = pullDistance / maxPullDistance;
        launchForce = Mathf.Lerp(minLaunchForce, maxLaunchForce, pullRatio);
        
        // Topu çekme pozisyonuna taşı
        Vector2 clampedPullPos = startPosition + (-pullDirection * pullDistance);
        ballRb.MovePosition(clampedPullPos);
        
        // Pull line güncelle
        UpdatePullLine();
        
        // Trajectory güncelle
        if (trajectoryRenderer != null)
        {
            trajectoryRenderer.ShowTrajectory(clampedPullPos, LaunchVelocity);
        }
    }

    /// <summary>
    /// Çekme çizgisini güncelle
    /// </summary>
    void UpdatePullLine()
    {
        if (pullLine == null) return;
        
        pullLine.positionCount = 2;
        pullLine.SetPosition(0, startPosition);
        pullLine.SetPosition(1, transform.position);
        
        // Renk: güce göre yeşil -> sarı -> kırmızı
        float ratio = pullDistance / maxPullDistance;
        Color lineColor = Color.Lerp(Color.green, Color.red, ratio);
        pullLine.startColor = lineColor;
        pullLine.endColor = lineColor;
    }

    /// <summary>
    /// Çekme bırakıldığında topu fırlat
    /// </summary>
    void ReleasePull()
    {
        isPulling = false;
        
        // Pull line'ı gizle
        if (pullLine != null)
        {
            pullLine.enabled = false;
        }
        
        // Trajectory'i gizle
        if (trajectoryRenderer != null)
        {
            trajectoryRenderer.HideTrajectory();
        }
        
        // Minimum güç kontrolü
        if (pullDistance < 0.2f)
        {
            // Çok az çekilmiş, fırlatma
            Debug.Log("[Slingshot] Pull too short, not launching.");
            OnPullEnd?.Invoke();
            return;
        }
        
        // Topu fırlat!
        LaunchBall();
    }

    /// <summary>
    /// Topu fırlat
    /// </summary>
    void LaunchBall()
    {
        isLaunched = true;
        
        // Topu dynamic yap
        ballRb.bodyType = RigidbodyType2D.Dynamic;
        
        // Hız uygula
        Vector2 velocity = LaunchVelocity;
        ballRb.linearVelocity = velocity;
        
        // BallPlacement'a bildir
        if (ballPlacement != null)
        {
            ballPlacement.isGameStarted = true;
        }
        
        Debug.Log($"[Slingshot] Launched! Velocity: {velocity}, Force: {launchForce:F1}");
        
        OnLaunch?.Invoke(velocity);
        OnPullEnd?.Invoke();
    }

    /// <summary>
    /// Topun pozisyonunu sıfırla (restart için)
    /// </summary>
    public void ResetSlingshot(Vector2 newPosition)
    {
        isLaunched = false;
        isPulling = false;
        pullDistance = 0;
        launchForce = 0;
        startPosition = newPosition;
        
        transform.position = newPosition;
        
        if (ballRb != null)
        {
            ballRb.bodyType = RigidbodyType2D.Kinematic;
            ballRb.linearVelocity = Vector2.zero;
            ballRb.angularVelocity = 0f;
        }
        
        if (pullLine != null)
        {
            pullLine.enabled = false;
        }
        
        if (trajectoryRenderer != null)
        {
            trajectoryRenderer.HideTrajectory();
        }
    }

    // Editor'da grab radius'u göster
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, grabRadius);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(Application.isPlaying ? startPosition : transform.position, maxPullDistance);
    }
}
