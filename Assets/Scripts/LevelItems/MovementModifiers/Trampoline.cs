using UnityEngine;

/// <summary>
/// Topa çarptığında yukarı doğru (veya belirtilen yönde) zıplatan item.
/// Yaylı animasyon sistemi ile - yaylar sadece zıplama sırasında görünür.
/// </summary>
public class Trampoline : LevelItem
{
    [Header("Bounce Settings")]
    [Tooltip("Zıplama kuvveti")]
    public float bounceForce = 20f;
    
    [Tooltip("Zıplama yönü (normalize edilecek). Varsayılan: yukarı")]
    public Vector2 bounceDirection = Vector2.up;
    
    [Tooltip("True ise gelen hız zıplama kuvvetine eklenir")]
    public bool addIncomingVelocity = true;
    
    [Tooltip("Gelen hızın ne kadarı korunacak (0-1)")]
    [Range(0f, 1f)]
    public float velocityRetention = 0.5f;
    
    [Tooltip("Minimum zıplama kuvveti (çok yavaş gelirse bile bu kadar zıplar)")]
    public float minBounceForce = 10f;

    [Header("Spring Visual Settings")]
    [Tooltip("Trambolin yüzeyi (zıplama sırasında hareket edecek)")]
    public Transform trampolineSurface;
    
    [Tooltip("Sol yay GameObject")]
    public GameObject leftSpring;
    
    [Tooltip("Sağ yay GameObject")]
    public GameObject rightSpring;
    
    [Tooltip("Yüzeyin ne kadar aşağı ineceği")]
    public float compressDistance = 0.3f;
    
    [Tooltip("Sıkışma süresi (saniye)")]
    public float compressDuration = 0.08f;
    
    [Tooltip("Geri dönme süresi (saniye)")]
    public float releaseDuration = 0.15f;
    
    [Tooltip("Geri dönmede yukarı aşma miktarı")]
    public float overshootAmount = 0.1f;

    [Header("Legacy Animation (Opsiyonel)")]
    [Tooltip("Eski scale animasyonunu da kullan")]
    public bool useScaleAnimation = false;
    public float squashAmount = 0.2f;
    public float squashDuration = 0.1f;

    private Vector3 originalSurfacePosition;
    private Vector3 originalScale;
    private bool isAnimating = false;

    protected override void Awake()
    {
        base.Awake();
        bounceDirection = bounceDirection.normalized;
        originalScale = transform.localScale;
        
        // Yüzey pozisyonunu kaydet
        if (trampolineSurface != null)
        {
            originalSurfacePosition = trampolineSurface.localPosition;
        }
        
        // Başlangıçta yayları gizle
        SetSpringsVisible(false);
    }

    /// <summary>
    /// Trigger ile temas (IsTrigger = true olan collider'lar için)
    /// </summary>
    protected override void OnBallEnter(Rigidbody2D ball)
    {
        Debug.Log($"[Trampoline] OnBallEnter triggered! Ball velocity: {ball.linearVelocity}");
        ApplyBounce(ball, ball.linearVelocity.magnitude);
    }

    protected override void OnBallCollision(Collision2D collision, Rigidbody2D ball)
    {
        Debug.Log($"[Trampoline] OnBallCollision triggered! Relative velocity: {collision.relativeVelocity}");
        // Gelen hızı hesapla
        float incomingSpeed = collision.relativeVelocity.magnitude;
        ApplyBounce(ball, incomingSpeed);
    }

    /// <summary>
    /// Topu zıplat - hem Trigger hem Collision için ortak metod
    /// </summary>
    private void ApplyBounce(Rigidbody2D ball, float incomingSpeed)
    {
        // Trambolinin rotasyonuna göre kuvvet yönünü hesapla
        Vector2 worldBounceDirection = transform.TransformDirection(bounceDirection.normalized);
        
        // Topun mevcut hızını al
        Vector2 currentVelocity = ball.linearVelocity;
        
        // Bounce yönüne dik olan ekseni hesapla (perpendicular)
        Vector2 perpendicularDirection = new Vector2(-worldBounceDirection.y, worldBounceDirection.x);
        
        // Topun mevcut hızının bounce yönüne dik bileşeni (yatay hareket)
        float lateralSpeed = Vector2.Dot(currentVelocity, perpendicularDirection);
        
        // Toplam zıplama kuvvetini hesapla
        float totalForce = bounceForce;
        
        if (addIncomingVelocity)
        {
            // Sadece bounce yönündeki hızı ekle
            float incomingBounceSpeed = Mathf.Abs(Vector2.Dot(currentVelocity, worldBounceDirection));
            totalForce += incomingBounceSpeed * velocityRetention;
        }
        
        // Minimum kuvveti uygula
        totalForce = Mathf.Max(totalForce, minBounceForce);
        
        Debug.Log($"[Trampoline] Bounce! Force: {totalForce}, Lateral: {lateralSpeed}, Direction: {worldBounceDirection}");
        
        // Yeni hız = bounce yönünde kuvvet + yatay hızı koru
        Vector2 newVelocity = (worldBounceDirection * totalForce) + (perpendicularDirection * lateralSpeed);
        ball.linearVelocity = newVelocity;
        
        // Feedback
        PlayActivationFeedback();
        StartCooldown();
        
        // Yaylı animasyon
        if (!isAnimating)
        {
            StartCoroutine(SpringBounceAnimation());
        }
        
        // Eski scale animasyonu (opsiyonel)
        if (useScaleAnimation)
        {
            StartCoroutine(SquashAnimation());
        }
    }

    /// <summary>
    /// Yaylı zıplama animasyonu - yüzey aşağı iner, yaylar görünür, sonra yukarı zıplar
    /// </summary>
    private System.Collections.IEnumerator SpringBounceAnimation()
    {
        if (trampolineSurface == null) yield break;
        
        isAnimating = true;
        
        // Yayları göster
        SetSpringsVisible(true);
        
        // Başlangıç yay yüksekliğini kaydet (surface ile taban arası mesafe)
        float originalSpringHeight = GetSpringHeight();
        
        // 1. SIKIŞTIRMA - Yüzey aşağı iner
        float elapsed = 0f;
        Vector3 compressedPosition = originalSurfacePosition - new Vector3(0, compressDistance, 0);
        
        while (elapsed < compressDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / compressDuration;
            // EaseOutQuad - hızlı başla, yavaş bitir
            t = 1f - (1f - t) * (1f - t);
            
            trampolineSurface.localPosition = Vector3.Lerp(originalSurfacePosition, compressedPosition, t);
            
            // Yayları surface ile senkronize et - alt kısım sabit, üst kısım surface'e bağlı
            UpdateSpringsToMatchSurface();
            
            yield return null;
        }
        
        trampolineSurface.localPosition = compressedPosition;
        UpdateSpringsToMatchSurface();
        
        // 2. SERBEST BIRAKMA - Yüzey yukarı zıplar (overshoot ile)
        elapsed = 0f;
        Vector3 overshootPosition = originalSurfacePosition + new Vector3(0, overshootAmount, 0);
        
        while (elapsed < releaseDuration * 0.6f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (releaseDuration * 0.6f);
            // EaseOutBack - hızlı çık, hafif geri gel
            t = 1f + 1.5f * Mathf.Pow(t - 1f, 3f) + Mathf.Pow(t - 1f, 2f);
            t = Mathf.Clamp01(t);
            
            trampolineSurface.localPosition = Vector3.Lerp(compressedPosition, overshootPosition, t);
            
            // Yayları surface ile senkronize et
            UpdateSpringsToMatchSurface();
            
            yield return null;
        }
        
        trampolineSurface.localPosition = overshootPosition;
        UpdateSpringsToMatchSurface();
        
        // 3. NORMALE DÖN
        elapsed = 0f;
        
        while (elapsed < releaseDuration * 0.4f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (releaseDuration * 0.4f);
            // EaseInOutQuad
            t = t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
            
            trampolineSurface.localPosition = Vector3.Lerp(overshootPosition, originalSurfacePosition, t);
            
            // Yayları surface ile senkronize et
            UpdateSpringsToMatchSurface();
            
            yield return null;
        }
        
        trampolineSurface.localPosition = originalSurfacePosition;
        UpdateSpringsToMatchSurface();
        
        // Kısa bir gecikme sonra yayları gizle
        yield return new WaitForSeconds(0.1f);
        
        // Yayları gizle
        SetSpringsVisible(false);
        
        isAnimating = false;
    }

    /// <summary>
    /// Surface'in mevcut Y pozisyonuna göre yay yüksekliğini hesapla
    /// </summary>
    private float GetSpringHeight()
    {
        if (trampolineSurface == null) return 1f;
        // Surface'in alt kenarının Y pozisyonunu döndür (local space'de)
        return trampolineSurface.localPosition.y;
    }

    /// <summary>
    /// Yayların görünürlüğünü ayarla
    /// </summary>
    private void SetSpringsVisible(bool visible)
    {
        if (leftSpring != null)
        {
            leftSpring.SetActive(visible);
        }
        
        if (rightSpring != null)
        {
            rightSpring.SetActive(visible);
        }
    }

    /// <summary>
    /// Yayları surface pozisyonuna göre güncelle
    /// Alt kısım sabit kalır, üst kısım surface ile birlikte hareket eder
    /// </summary>
    private void UpdateSpringsToMatchSurface()
    {
        if (trampolineSurface == null) return;
        
        // Surface'in normal pozisyonuna göre ne kadar aşağıda/yukarıda olduğunu hesapla
        float surfaceOffset = trampolineSurface.localPosition.y - originalSurfacePosition.y;
        
        // Orijinal yay yüksekliğine göre scale oranını hesapla
        // Varsayılan yay yüksekliği 1 birim olarak kabul ediyoruz
        float baseSpringHeight = 1f; // Bu değeri Inspector'dan ayarlanabilir yapabiliriz
        float newHeight = baseSpringHeight + surfaceOffset;
        float scaleRatio = newHeight / baseSpringHeight;
        
        // Yayları güncelle - sadece Y scale değişir, pozisyon sabit (alt kısım tabanda)
        if (leftSpring != null)
        {
            Vector3 scale = leftSpring.transform.localScale;
            scale.y = Mathf.Max(0.1f, scaleRatio); // Minimum 0.1 scale
            leftSpring.transform.localScale = scale;
        }
        
        if (rightSpring != null)
        {
            Vector3 scale = rightSpring.transform.localScale;
            scale.y = Mathf.Max(0.1f, scaleRatio); // Minimum 0.1 scale
            rightSpring.transform.localScale = scale;
        }
    }

    private System.Collections.IEnumerator SquashAnimation()
    {
        // Squash (ezilme)
        Vector3 squashedScale = originalScale;
        squashedScale.y *= (1f - squashAmount);
        squashedScale.x *= (1f + squashAmount * 0.5f);
        
        transform.localScale = squashedScale;
        
        yield return new WaitForSeconds(squashDuration);
        
        // Stretch (uzama)
        Vector3 stretchedScale = originalScale;
        stretchedScale.y *= (1f + squashAmount * 0.5f);
        stretchedScale.x *= (1f - squashAmount * 0.25f);
        
        transform.localScale = stretchedScale;
        
        yield return new WaitForSeconds(squashDuration);
        
        // Normal
        transform.localScale = originalScale;
    }

    private void OnValidate()
    {
        if (bounceDirection != Vector2.zero)
        {
            bounceDirection = bounceDirection.normalized;
        }
    }

    private void OnDrawGizmos()
    {
        // Zıplama yönünü göster (world space - rotation'a göre)
        Gizmos.color = Color.green;
        Vector3 center = transform.position;
        
        // Local direction'ı world space'e çevir
        Vector3 localDir = new Vector3(bounceDirection.x, bounceDirection.y, 0);
        Vector3 worldDir = transform.TransformDirection(localDir.normalized) * 1.5f;
        
        Gizmos.DrawLine(center, center + worldDir);
        
        // Ok ucu
        Vector3 arrowHead = center + worldDir;
        Vector3 right = Quaternion.Euler(0, 0, 30) * -worldDir.normalized * 0.4f;
        Vector3 left = Quaternion.Euler(0, 0, -30) * -worldDir.normalized * 0.4f;
        Gizmos.DrawLine(arrowHead, arrowHead + right);
        Gizmos.DrawLine(arrowHead, arrowHead + left);
        
        // Sıkışma mesafesini göster (trambolinin local down yönünde)
        if (trampolineSurface != null)
        {
            Gizmos.color = Color.yellow;
            Vector3 surfacePos = trampolineSurface.position;
            Vector3 compressDir = transform.TransformDirection(Vector3.down) * compressDistance;
            Gizmos.DrawLine(surfacePos, surfacePos + compressDir);
        }
    }
}
