using UnityEngine;

/// <summary>
/// Belirli hızla çarpınca kırılan duvar.
/// </summary>
public class BreakableWall : LevelItem
{
    [Header("Break Settings")]
    [Tooltip("Kırılmak için gereken minimum hız")]
    public float breakThreshold = 10f;
    
    [Tooltip("Kaç vuruş gerekli (1 = tek vuruşta kırılır)")]
    public int hitPoints = 1;
    
    [Tooltip("Kırılma efekti prefab'ı")]
    public GameObject breakEffect;
    
    [Tooltip("Kırılınca spawn olacak parçalar")]
    public GameObject[] debrisPrefabs;

    [Header("Visual Feedback")]
    [Tooltip("Hasar aldıkça renk değişimi")]
    public bool showDamage = true;
    
    [Tooltip("Tam sağlıkta renk")]
    public Color healthyColor = Color.white;
    
    [Tooltip("Hasarlı renk")]
    public Color damagedColor = Color.red;
    
    [Tooltip("Hasar sallanma şiddeti")]
    public float shakeIntensity = 0.1f;
    
    [Tooltip("Hasar sallanma süresi")]
    public float shakeDuration = 0.2f;

    // Internal
    private int currentHitPoints;
    private SpriteRenderer spriteRenderer;
    private Vector3 originalPosition;

    protected override void Awake()
    {
        base.Awake();
        currentHitPoints = hitPoints;
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalPosition = transform.position;
    }

    protected override void OnBallCollision(Collision2D collision, Rigidbody2D ball)
    {
        float impactSpeed = collision.relativeVelocity.magnitude;
        
        if (impactSpeed >= breakThreshold)
        {
            TakeDamage();
        }
        else
        {
            // Yeterince hızlı değil - geri tepme
            PlayActivationFeedback();
        }
    }

    private void TakeDamage()
    {
        currentHitPoints--;
        
        // Görsel geri bildirim
        if (showDamage)
        {
            UpdateDamageVisual();
            StartCoroutine(ShakeEffect());
        }
        
        PlayActivationFeedback();
        
        if (currentHitPoints <= 0)
        {
            Break();
        }
    }

    private void UpdateDamageVisual()
    {
        if (spriteRenderer != null)
        {
            float healthPercent = (float)currentHitPoints / hitPoints;
            spriteRenderer.color = Color.Lerp(damagedColor, healthyColor, healthPercent);
        }
    }

    private System.Collections.IEnumerator ShakeEffect()
    {
        float elapsed = 0f;
        
        while (elapsed < shakeDuration)
        {
            float x = originalPosition.x + Random.Range(-shakeIntensity, shakeIntensity);
            float y = originalPosition.y + Random.Range(-shakeIntensity, shakeIntensity);
            transform.position = new Vector3(x, y, originalPosition.z);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        transform.position = originalPosition;
    }

    private void Break()
    {
        // Objective tracking - duvar kırıldığını kaydet
        if (LevelObjectiveManager.Instance != null)
        {
            LevelObjectiveManager.Instance.RecordWallBroken();
        }
        
        // Kırılma efekti
        if (breakEffect != null)
        {
            Instantiate(breakEffect, transform.position, Quaternion.identity);
        }
        
        // Parçaları spawn et
        if (debrisPrefabs != null && debrisPrefabs.Length > 0)
        {
            foreach (GameObject debris in debrisPrefabs)
            {
                if (debris != null)
                {
                    Vector3 spawnPos = transform.position + (Vector3)Random.insideUnitCircle * 0.5f;
                    GameObject piece = Instantiate(debris, spawnPos, Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
                    
                    // Parçalara rastgele kuvvet ver
                    Rigidbody2D rb = piece.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        rb.AddForce(Random.insideUnitCircle * 5f, ForceMode2D.Impulse);
                        rb.AddTorque(Random.Range(-10f, 10f), ForceMode2D.Impulse);
                    }
                }
            }
        }
        
        // Yok et
        Destroy(gameObject);
    }

    /// <summary>
    /// Duvarı resetle (level restart için)
    /// </summary>
    public void ResetWall()
    {
        currentHitPoints = hitPoints;
        transform.position = originalPosition;
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = healthyColor;
        }
    }

    private void OnDrawGizmos()
    {
        // HP durumunu göster
        Gizmos.color = Color.Lerp(Color.red, Color.green, (float)currentHitPoints / Mathf.Max(1, hitPoints));
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.8f);
    }
}
