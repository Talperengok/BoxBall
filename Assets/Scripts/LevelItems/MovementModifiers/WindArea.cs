using UnityEngine;

/// <summary>
/// Belirli bir alanda rüzgar etkisi oluşturarak topu iten item.
/// Rüzgar yönü objenin Z rotasyonundan otomatik hesaplanır.
/// Child sprite'ları hareket ettirerek rüzgar akışı gösterilir.
/// </summary>
public class WindArea : LevelItem
{
    [Header("Wind Settings")]
    [Tooltip("Rüzgar kuvveti")]
    public float windForce = 10f;
    
    [Tooltip("Sürekli kuvvet mi, pulse şeklinde mi?")]
    public bool isConstant = true;
    
    [Tooltip("Pulse modunda: Pulse aralığı (saniye)")]
    public float pulseInterval = 1f;
    
    [Tooltip("Pulse modunda: Pulse süresi (saniye)")]
    public float pulseDuration = 0.3f;

    [Header("Direction")]
    [Tooltip("Manuel yön kullan (false = Z rotation'dan hesapla)")]
    public bool useManualDirection = false;
    
    [Tooltip("Manuel rüzgar yönü (useManualDirection true ise)")]
    public Vector2 manualDirection = Vector2.right;

    [Header("Animation")]
    [Tooltip("Animasyon hızı")]
    public float animationSpeed = 2f;
    
    [Tooltip("Hareket mesafesi (local X)")]
    public float moveDistance = 2f;
    
    [Tooltip("Animasyonu aktif et")]
    public bool enableAnimation = true;
    
    [Tooltip("Hareket ettirilecek child objeler (boş bırakılırsa tüm child'lar kullanılır)")]
    public Transform[] animatedChildren;

    [Header("Visual")]
    [Tooltip("Rüzgar partikülleri (opsiyonel)")]
    public ParticleSystem windParticles;

    // Internal
    private float pulseTimer = 0f;
    private bool isPulseActive = false;
    private Vector3[] childStartPositions;
    private float animationProgress = 0f;

    /// <summary>
    /// Z rotation'dan hesaplanan rüzgar yönü
    /// </summary>
    public Vector2 WindDirection
    {
        get
        {
            if (useManualDirection)
            {
                return manualDirection.normalized;
            }
            
            // Z rotation'dan yön hesapla
            float angle = transform.eulerAngles.z * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }
    }

    protected override void Awake()
    {
        base.Awake();
        
        // Child objeleri bul
        if (animatedChildren == null || animatedChildren.Length == 0)
        {
            // Tüm child'ları al
            animatedChildren = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                animatedChildren[i] = transform.GetChild(i);
            }
        }
        
        // Başlangıç pozisyonlarını kaydet
        if (animatedChildren.Length > 0)
        {
            childStartPositions = new Vector3[animatedChildren.Length];
            for (int i = 0; i < animatedChildren.Length; i++)
            {
                if (animatedChildren[i] != null)
                {
                    childStartPositions[i] = animatedChildren[i].localPosition;
                }
            }
        }
    }

    protected override void Update()
    {
        base.Update();
        
        // Pulse mode timer
        if (!isConstant)
        {
            pulseTimer += Time.deltaTime;
            
            if (!isPulseActive && pulseTimer >= pulseInterval)
            {
                isPulseActive = true;
                pulseTimer = 0f;
            }
            else if (isPulseActive && pulseTimer >= pulseDuration)
            {
                isPulseActive = false;
                pulseTimer = 0f;
            }
        }
        
        // Child animasyonu - basit salınım
        if (enableAnimation && animatedChildren != null && animatedChildren.Length > 0)
        {
            animationProgress += animationSpeed * Time.deltaTime;
            
            for (int i = 0; i < animatedChildren.Length; i++)
            {
                if (animatedChildren[i] != null && childStartPositions != null)
                {
                    // Her child için farklı faz (dalgalı efekt)
                    float phase = animationProgress + i * 0.8f;
                    
                    // Sine wave ile yumuşak salınım
                    float wave = Mathf.Sin(phase) * moveDistance;
                    
                    // Local Y yönünde hafif salınım
                    Vector3 newPos = childStartPositions[i];
                    newPos.y += wave;
                    
                    animatedChildren[i].localPosition = newPos;
                }
            }
        }
    }

    protected override void OnBallStay(Rigidbody2D ball)
    {
        bool shouldApplyForce = isConstant || isPulseActive;
        
        if (shouldApplyForce)
        {
            ball.AddForce(WindDirection * windForce);
        }
    }

    protected override void OnBallEnter(Rigidbody2D ball)
    {
        PlayActivationFeedback();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = transform.position;
        Vector2 dir = WindDirection;
        Vector3 direction = new Vector3(dir.x, dir.y, 0) * 2f;
        Gizmos.DrawLine(center, center + direction);
        
        Vector3 arrowHead = center + direction;
        Vector3 right = Quaternion.Euler(0, 0, 30) * -direction.normalized * 0.5f;
        Vector3 left = Quaternion.Euler(0, 0, -30) * -direction.normalized * 0.5f;
        Gizmos.DrawLine(arrowHead, arrowHead + right);
        Gizmos.DrawLine(arrowHead, arrowHead + left);
    }
}

