using UnityEngine;

/// <summary>
/// Yapışkan zemin - Sürtünmeyi artırarak topu yavaşlatır.
/// </summary>
public class StickyZone : LevelItem
{
    [Header("Sticky Settings")]
    [Tooltip("Drag çarpanı (normal değerin kaç katı)")]
    public float dragMultiplier = 5f;
    
    [Tooltip("Hız yavaşlatma faktörü (her frame velocity'nin bu kadarı kalır)")]
    [Range(0f, 1f)]
    public float velocityDamping = 0.95f;
    
    [Tooltip("Sürtünme değeri")]
    [Range(0f, 1f)]
    public float friction = 0.9f;

    [Header("Visual")]
    public Color stickyColor = new Color(0.4f, 0.2f, 0f, 0.5f);

    // Internal
    private float originalDrag;
    private float originalAngularDrag;
    private PhysicsMaterial2D originalMaterial;
    private PhysicsMaterial2D stickyMaterial;

    protected override void Awake()
    {
        base.Awake();
        
        // Yapışkan material oluştur
        stickyMaterial = new PhysicsMaterial2D("StickyMaterial");
        stickyMaterial.friction = friction;
        stickyMaterial.bounciness = 0f; // Hiç zıplamaz
    }

    protected override void OnBallEnter(Rigidbody2D ball)
    {
        // Orijinal değerleri sakla
        originalDrag = ball.linearDamping;
        originalAngularDrag = ball.angularDamping;
        
        Collider2D ballCollider = ball.GetComponent<Collider2D>();
        if (ballCollider != null)
        {
            originalMaterial = ballCollider.sharedMaterial;
            ballCollider.sharedMaterial = stickyMaterial;
        }
        
        // Drag'i artır
        ball.linearDamping = originalDrag * dragMultiplier;
        ball.angularDamping = originalAngularDrag * dragMultiplier;
        
        PlayActivationFeedback();
    }

    protected override void OnBallStay(Rigidbody2D ball)
    {
        // Sürekli yavaşlat
        ball.linearVelocity *= velocityDamping;
    }

    protected override void OnBallExit(Rigidbody2D ball)
    {
        // Orijinal değerlere dön
        ball.linearDamping = originalDrag;
        ball.angularDamping = originalAngularDrag;
        
        Collider2D ballCollider = ball.GetComponent<Collider2D>();
        if (ballCollider != null)
        {
            ballCollider.sharedMaterial = originalMaterial;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = stickyColor;
        
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        CircleCollider2D circle = GetComponent<CircleCollider2D>();
        
        if (box != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.offset, box.size);
        }
        else if (circle != null)
        {
            Gizmos.DrawSphere(transform.position + (Vector3)circle.offset, circle.radius);
        }
    }
}
