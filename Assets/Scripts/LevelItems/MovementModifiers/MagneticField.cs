using UnityEngine;

/// <summary>
/// Topu kendine çeken veya iten manyetik alan.
/// </summary>
public class MagneticField : LevelItem
{
    [Header("Magnetic Settings")]
    [Tooltip("Manyetik kuvvet şiddeti")]
    public float magnetStrength = 15f;
    
    [Tooltip("True: Çeker, False: İter")]
    public bool isAttracting = true;
    
    [Tooltip("Etki alanı yarıçapı")]
    public float effectRadius = 5f;
    
    [Tooltip("Mesafeye göre kuvvet azalması (true ise uzaklaştıkça azalır)")]
    public bool distanceFalloff = true;
    
    [Tooltip("Falloff eğrisi (1 = lineer, 2 = kare)")]
    public float falloffPower = 1f;

    [Header("Visual")]
    [Tooltip("Manyetik alan rengi")]
    public Color fieldColor = new Color(0.5f, 0f, 1f, 0.3f);

    private void FixedUpdate()
    {
        if (!isActive) return;
        
        // Ball'ı bul (tag ile)
        GameObject ballObj = GameObject.FindGameObjectWithTag(ballTag);
        if (ballObj == null) return;
        
        Rigidbody2D ball = ballObj.GetComponent<Rigidbody2D>();
        if (ball == null) return;
        
        // Mesafeyi hesapla
        Vector2 direction = (Vector2)transform.position - ball.position;
        float distance = direction.magnitude;
        
        // Etki alanı içinde mi?
        if (distance > effectRadius || distance < 0.1f) return;
        
        // Kuvvet hesapla
        float force = magnetStrength;
        
        if (distanceFalloff)
        {
            // Mesafe arttıkça kuvvet azalır
            float normalizedDistance = distance / effectRadius;
            force *= Mathf.Pow(1f - normalizedDistance, falloffPower);
        }
        
        // Yön
        Vector2 forceDirection = direction.normalized;
        if (!isAttracting)
        {
            forceDirection = -forceDirection; // İtme
        }
        
        // Kuvveti uygula
        ball.AddForce(forceDirection * force);
    }

    private void OnDrawGizmos()
    {
        // Etki alanını göster
        Gizmos.color = fieldColor;
        Gizmos.DrawWireSphere(transform.position, effectRadius);
        
        // İç daire (güçlü alan)
        Gizmos.color = new Color(fieldColor.r, fieldColor.g, fieldColor.b, fieldColor.a * 2f);
        Gizmos.DrawWireSphere(transform.position, effectRadius * 0.3f);
        
        // Çekme/itme göstergesi
        Gizmos.color = isAttracting ? Color.blue : Color.red;
        float arrowSize = effectRadius * 0.2f;
        
        for (int i = 0; i < 4; i++)
        {
            float angle = i * 90f * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            Vector3 start = transform.position + dir * effectRadius * 0.5f;
            Vector3 end = transform.position + dir * (isAttracting ? effectRadius * 0.2f : effectRadius * 0.8f);
            Gizmos.DrawLine(start, end);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Seçildiğinde daha detaylı göster
        Gizmos.color = new Color(fieldColor.r, fieldColor.g, fieldColor.b, 0.1f);
        Gizmos.DrawSphere(transform.position, effectRadius);
    }
}
