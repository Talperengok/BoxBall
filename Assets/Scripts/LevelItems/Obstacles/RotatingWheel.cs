using UnityEngine;

/// <summary>
/// Sürekli dönen engel - Topa çarparak yön değiştirir.
/// </summary>
public class RotatingWheel : LevelItem
{
    [Header("Rotation Settings")]
    [Tooltip("Dönüş hızı (derece/saniye)")]
    public float rotationSpeed = 90f;
    
    [Tooltip("True: Saat yönünde, False: Saat yönünün tersine")]
    public bool clockwise = true;
    
    [Tooltip("Hız değişimi kullan (yavaşla/hızlan)")]
    public bool useSpeedVariation = false;
    
    [Tooltip("Minimum hız çarpanı")]
    public float minSpeedMultiplier = 0.5f;
    
    [Tooltip("Maksimum hız çarpanı")]
    public float maxSpeedMultiplier = 1.5f;
    
    [Tooltip("Hız değişim periyodu (saniye)")]
    public float speedVariationPeriod = 2f;

    [Header("Impact")]
    [Tooltip("Çarpma kuvvet çarpanı")]
    public float impactForceMultiplier = 1f;

    private float currentRotation = 0f;
    private float variationTimer = 0f;

    protected override void Update()
    {
        base.Update();
        
        if (!isActive) return;
        
        // Hız hesapla
        float speed = rotationSpeed;
        
        if (useSpeedVariation)
        {
            variationTimer += Time.deltaTime;
            float t = (Mathf.Sin(variationTimer * Mathf.PI * 2f / speedVariationPeriod) + 1f) / 2f;
            float multiplier = Mathf.Lerp(minSpeedMultiplier, maxSpeedMultiplier, t);
            speed *= multiplier;
        }
        
        // Yön
        float direction = clockwise ? -1f : 1f;
        
        // Döndür
        currentRotation = speed * direction * Time.deltaTime;
        transform.Rotate(0f, 0f, currentRotation);
    }

    protected override void OnBallCollision(Collision2D collision, Rigidbody2D ball)
    {
        // Dönen objenin hızına göre ekstra kuvvet uygula
        if (impactForceMultiplier > 0)
        {
            Vector2 contactNormal = collision.GetContact(0).normal;
            float impactForce = Mathf.Abs(rotationSpeed) * impactForceMultiplier * 0.1f;
            ball.AddForce(-contactNormal * impactForce, ForceMode2D.Impulse);
        }
        
        PlayActivationFeedback();
    }

    private void OnDrawGizmos()
    {
        // Dönüş yönünü göster
        Gizmos.color = Color.yellow;
        
        float direction = clockwise ? -1f : 1f;
        Vector3 center = transform.position;
        
        // Daire çiz
        int segments = 20;
        float radius = 0.5f;
        
        for (int i = 0; i < segments; i++)
        {
            float angle1 = (i / (float)segments) * Mathf.PI * 2f;
            float angle2 = ((i + 1) / (float)segments) * Mathf.PI * 2f;
            
            Vector3 p1 = center + new Vector3(Mathf.Cos(angle1), Mathf.Sin(angle1), 0) * radius;
            Vector3 p2 = center + new Vector3(Mathf.Cos(angle2), Mathf.Sin(angle2), 0) * radius;
            
            Gizmos.DrawLine(p1, p2);
        }
        
        // Yön oku
        float arrowAngle = clockwise ? 45f : -45f;
        Vector3 arrowStart = center + new Vector3(radius, 0, 0);
        Vector3 arrowDir = Quaternion.Euler(0, 0, arrowAngle) * Vector3.up * 0.3f * direction;
        Gizmos.DrawLine(arrowStart, arrowStart + arrowDir);
    }
}
