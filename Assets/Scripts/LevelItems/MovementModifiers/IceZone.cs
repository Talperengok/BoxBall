using UnityEngine;

/// <summary>
/// Buz zemin - Sürtünmeyi azaltarak topun kaymasını sağlar.
/// </summary>
public class IceZone : LevelItem
{
    [Header("Ice Settings")]
    [Tooltip("Sürtünme çarpanı (0 = tamamen kaygan, 1 = normal)")]
    [Range(0f, 1f)]
    public float frictionMultiplier = 0.05f;
    
    [Tooltip("Linear drag çarpanı")]
    [Range(0f, 1f)]
    public float dragMultiplier = 0.1f;

    [Header("Visual")]
    public Color iceColor = new Color(0.7f, 0.9f, 1f, 0.5f);

    // Internal - orijinal değerleri sakla
    private float originalDrag;
    private PhysicsMaterial2D originalMaterial;
    private PhysicsMaterial2D iceMaterial;

    protected override void Awake()
    {
        base.Awake();
        
        // Buz material'i oluştur
        iceMaterial = new PhysicsMaterial2D("IceMaterial");
        iceMaterial.friction = frictionMultiplier;
        iceMaterial.bounciness = 0.1f; // Hafif bouncy
    }

    protected override void OnBallEnter(Rigidbody2D ball)
    {
        // Orijinal değerleri sakla
        originalDrag = ball.linearDamping;
        
        Collider2D ballCollider = ball.GetComponent<Collider2D>();
        if (ballCollider != null)
        {
            originalMaterial = ballCollider.sharedMaterial;
            ballCollider.sharedMaterial = iceMaterial;
        }
        
        // Drag'i azalt
        ball.linearDamping = originalDrag * dragMultiplier;
        
        PlayActivationFeedback();
    }

    protected override void OnBallExit(Rigidbody2D ball)
    {
        // Orijinal değerlere dön
        ball.linearDamping = originalDrag;
        
        Collider2D ballCollider = ball.GetComponent<Collider2D>();
        if (ballCollider != null)
        {
            ballCollider.sharedMaterial = originalMaterial;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = iceColor;
        
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
