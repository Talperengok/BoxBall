using UnityEngine;

/// <summary>
/// Portal - Topu başka bir portala ışınlar.
/// </summary>
public class Portal : LevelItem
{
    [Header("Portal Settings")]
    [Tooltip("Çıkış portalı")]
    public Portal exitPortal;
    
    [Tooltip("Hızı koru")]
    public bool preserveVelocity = true;
    
    [Tooltip("Manuel çıkış yönü kullan (false = topun mevcut yönü korunur)")]
    public bool useManualExitDirection = false;
    
    [Tooltip("Manuel çıkış yönü (useManualExitDirection true ise kullanılır)")]
    public Vector2 exitDirection = Vector2.up;
    
    [Tooltip("Çift yönlü portal (her iki taraftan da geçilebilir)")]
    public bool bidirectional = true;

    [Header("Visual")]
    [Tooltip("Portal rengi")]
    public Color portalColor = Color.blue;
    
    [Tooltip("Işınlanma efekti")]
    public GameObject teleportEffect;

    // Internal
    private bool canTeleport = true;

    protected override void Awake()
    {
        base.Awake();
        exitDirection = exitDirection.normalized;
    }

    protected override void OnBallEnter(Rigidbody2D ball)
    {
        if (!canTeleport) return;
        if (exitPortal == null)
        {
            Debug.LogWarning("Portal: Çıkış portalı atanmamış!");
            return;
        }
        
        TeleportBall(ball);
    }

    private void TeleportBall(Rigidbody2D ball)
    {
        // Çıkış portalını geçici olarak devre dışı bırak (sonsuz döngü önle)
        exitPortal.DisableTeleport();
        
        // Giriş efekti
        if (teleportEffect != null)
        {
            Instantiate(teleportEffect, transform.position, Quaternion.identity);
        }
        
        // Mevcut hızı kaydet
        Vector2 currentVelocity = ball.linearVelocity;
        float speed = currentVelocity.magnitude;
        
        // Topu ışınla
        ball.position = exitPortal.transform.position;
        
        // Hızı ayarla
        if (preserveVelocity)
        {
            if (exitPortal.useManualExitDirection)
            {
                // Manuel çıkış yönü kullan
                ball.linearVelocity = exitPortal.exitDirection.normalized * speed;
            }
            else
            {
                // Mevcut hareket yönünü koru (varsayılan davranış)
                ball.linearVelocity = currentVelocity;
            }
        }
        else
        {
            // Hızı sıfırla
            ball.linearVelocity = Vector2.zero;
        }
        
        // Çıkış efekti
        if (exitPortal.teleportEffect != null)
        {
            Instantiate(exitPortal.teleportEffect, exitPortal.transform.position, Quaternion.identity);
        }
        
        PlayActivationFeedback();
        
        // Kısa süre sonra tekrar aktif et
        Invoke(nameof(EnableTeleport), cooldownTime > 0 ? cooldownTime : 0.5f);
        exitPortal.Invoke(nameof(EnableTeleport), cooldownTime > 0 ? cooldownTime : 0.5f);
    }

    public void DisableTeleport()
    {
        canTeleport = false;
    }

    public void EnableTeleport()
    {
        canTeleport = true;
    }

    private void OnValidate()
    {
        if (exitDirection != Vector2.zero)
        {
            exitDirection = exitDirection.normalized;
        }
    }

    private void OnDrawGizmos()
    {
        // Portal'ı göster
        Gizmos.color = portalColor;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
        // Çıkış yönü
        Vector3 dir = new Vector3(exitDirection.x, exitDirection.y, 0);
        Gizmos.DrawLine(transform.position, transform.position + dir);
        
        // Çıkış portalına bağlantı
        if (exitPortal != null)
        {
            Gizmos.color = new Color(portalColor.r, portalColor.g, portalColor.b, 0.3f);
            Gizmos.DrawLine(transform.position, exitPortal.transform.position);
        }
    }
}
