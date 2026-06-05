using UnityEngine;

/// <summary>
/// Hız boost - Topun hızını artırır.
/// </summary>
public class SpeedBoost : LevelItem
{
    [Header("Boost Settings")]
    [Tooltip("Hız çarpanı")]
    public float boostMultiplier = 2f;
    
    [Tooltip("Mevcut yönü kullan (false ise boostDirection kullanır)")]
    public bool useCurrentDirection = true;
    
    [Tooltip("Boost yönü (useCurrentDirection false ise)")]
    public Vector2 boostDirection = Vector2.right;
    
    [Tooltip("Minimum hız (çok yavaş gelirse bile bu hıza çıkar)")]
    public float minSpeed = 5f;
    
    [Tooltip("Maksimum hız limiti")]
    public float maxSpeed = 50f;

    [Header("Visual")]
    [Tooltip("Boost efekti")]
    public GameObject boostEffect;
    
    [Tooltip("Boost rengi")]
    public Color boostColor = Color.yellow;

    protected override void Awake()
    {
        base.Awake();
        boostDirection = boostDirection.normalized;
    }

    protected override void OnBallEnter(Rigidbody2D ball)
    {
        ApplyBoost(ball);
    }

    private void ApplyBoost(Rigidbody2D ball)
    {
        Vector2 currentVelocity = ball.linearVelocity;
        float currentSpeed = currentVelocity.magnitude;
        
        Vector2 direction;
        
        if (useCurrentDirection && currentSpeed > 0.1f)
        {
            // Mevcut yönü kullan
            direction = currentVelocity.normalized;
        }
        else
        {
            // Tanımlı yönü kullan
            direction = boostDirection;
        }
        
        // Yeni hızı hesapla
        float newSpeed = Mathf.Max(currentSpeed * boostMultiplier, minSpeed);
        newSpeed = Mathf.Min(newSpeed, maxSpeed);
        
        // Hızı uygula
        ball.linearVelocity = direction * newSpeed;
        
        // Efekt
        if (boostEffect != null)
        {
            Instantiate(boostEffect, transform.position, Quaternion.identity);
        }
        
        PlayActivationFeedback();
        StartCooldown();
    }

    private void OnValidate()
    {
        if (boostDirection != Vector2.zero)
        {
            boostDirection = boostDirection.normalized;
        }
    }

    private void OnDrawGizmos()
    {
        // Boost alanı
        Gizmos.color = boostColor;
        
        // Yön oku
        Vector3 dir = useCurrentDirection 
            ? Vector3.right 
            : new Vector3(boostDirection.x, boostDirection.y, 0);
        
        Gizmos.DrawLine(transform.position - dir * 0.5f, transform.position + dir * 0.5f);
        
        // Ok ucu
        Vector3 arrowHead = transform.position + dir * 0.5f;
        Vector3 right = Quaternion.Euler(0, 0, 30) * -dir.normalized * 0.3f;
        Vector3 left = Quaternion.Euler(0, 0, -30) * -dir.normalized * 0.3f;
        Gizmos.DrawLine(arrowHead, arrowHead + right);
        Gizmos.DrawLine(arrowHead, arrowHead + left);
    }
}
