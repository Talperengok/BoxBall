using UnityEngine;
using System.Collections;

/// <summary>
/// Topun zıplama özelliğini değiştiren item.
/// </summary>
public class BouncinessModifier : LevelItem
{
    [Header("Bounciness Settings")]
    [Tooltip("Yeni bounciness değeri (0-1 aralığı normal, >1 süper zıplama)")]
    [Range(0f, 2f)]
    public float bouncinessValue = 1f;
    
    [Tooltip("Efekt süresi (0 = kalıcı)")]
    public float duration = 5f;
    
    [Tooltip("Kalıcı değişiklik")]
    public bool isPermanent = false;

    [Header("Visual")]
    [Tooltip("Efekt")]
    public GameObject modifyEffect;
    
    [Tooltip("Zone rengi")]
    public Color zoneColor = new Color(0f, 1f, 0.5f, 0.3f);

    // Internal
    private PhysicsMaterial2D originalMaterial;
    private PhysicsMaterial2D modifiedMaterial;
    private static bool isModified = false;
    private static Coroutine resetCoroutine;

    protected override void Awake()
    {
        base.Awake();
        
        // Özel material oluştur
        modifiedMaterial = new PhysicsMaterial2D("BouncyMaterial");
        modifiedMaterial.bounciness = bouncinessValue;
        modifiedMaterial.friction = 0.4f;
    }

    protected override void OnBallEnter(Rigidbody2D ball)
    {
        if (isModified && !isPermanent)
        {
            return;
        }
        
        Collider2D ballCollider = ball.GetComponent<Collider2D>();
        if (ballCollider == null) return;
        
        // Orijinal material'i sakla
        originalMaterial = ballCollider.sharedMaterial;
        
        // Yeni material'i uygula
        ballCollider.sharedMaterial = modifiedMaterial;
        
        // Efekt
        if (modifyEffect != null)
        {
            Instantiate(modifyEffect, ball.position, Quaternion.identity);
        }
        
        isModified = true;
        PlayActivationFeedback();
        StartCooldown();
        
        // Geçici efekt ise geri al
        if (!isPermanent && duration > 0)
        {
            if (resetCoroutine != null)
            {
                StopCoroutine(resetCoroutine);
            }
            resetCoroutine = StartCoroutine(ResetBounciness(ballCollider));
        }
    }

    private IEnumerator ResetBounciness(Collider2D ballCollider)
    {
        yield return new WaitForSeconds(duration);
        
        if (ballCollider != null)
        {
            ballCollider.sharedMaterial = originalMaterial;
            
            // Efekt
            if (modifyEffect != null)
            {
                Instantiate(modifyEffect, ballCollider.transform.position, Quaternion.identity);
            }
        }
        
        isModified = false;
    }

    private void OnValidate()
    {
        if (modifiedMaterial != null)
        {
            modifiedMaterial.bounciness = bouncinessValue;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = zoneColor;
        
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        CircleCollider2D circle = GetComponent<CircleCollider2D>();
        
        if (box != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(box.offset, box.size);
        }
        else if (circle != null)
        {
            Gizmos.DrawWireSphere(transform.position + (Vector3)circle.offset, circle.radius);
        }
        
        // Zıplama sembolü
        Gizmos.color = bouncinessValue > 0.5f ? Color.green : Color.gray;
        
        // Yay çiz
        Vector3 center = transform.position;
        for (int i = 0; i < 3; i++)
        {
            float y = i * 0.15f;
            float width = 0.3f - (i * 0.1f);
            Gizmos.DrawLine(center + new Vector3(-width, y, 0), center + new Vector3(width, y, 0));
        }
    }
}
