using UnityEngine;
using System.Collections;

/// <summary>
/// Topun boyutunu değiştiren item.
/// </summary>
public class SizeChanger : LevelItem
{
    [Header("Size Settings")]
    [Tooltip("Boyut çarpanı (>1 büyütür, <1 küçültür)")]
    public float sizeMultiplier = 1.5f;
    
    [Tooltip("Efekt süresi (0 = kalıcı)")]
    public float duration = 5f;
    
    [Tooltip("Kalıcı değişiklik")]
    public bool isPermanent = false;
    
    [Tooltip("Boyut değişim animasyon süresi")]
    public float transitionDuration = 0.3f;

    [Header("Visual")]
    [Tooltip("Boyut değişim efekti")]
    public GameObject sizeChangeEffect;
    
    [Tooltip("Zone rengi")]
    public Color zoneColor = new Color(1f, 0.5f, 0f, 0.3f);

    // Internal
    private static bool isSizeChanged = false;
    private static Coroutine sizeResetCoroutine;

    protected override void OnBallEnter(Rigidbody2D ball)
    {
        if (isSizeChanged && !isPermanent)
        {
            // Zaten boyut değişmiş, reset için bekle
            return;
        }
        
        // Boyutu değiştir
        StartCoroutine(ChangeBallSize(ball.transform));
        
        PlayActivationFeedback();
        StartCooldown();
    }

    private IEnumerator ChangeBallSize(Transform ballTransform)
    {
        Vector3 originalScale = ballTransform.localScale;
        Vector3 targetScale = originalScale * sizeMultiplier;
        
        // Efekt
        if (sizeChangeEffect != null)
        {
            Instantiate(sizeChangeEffect, ballTransform.position, Quaternion.identity);
        }
        
        // Animasyonlu geçiş
        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;
            t = Mathf.SmoothStep(0, 1, t);
            ballTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }
        
        ballTransform.localScale = targetScale;
        isSizeChanged = true;
        
        // Geçici efekt ise geri al
        if (!isPermanent && duration > 0)
        {
            if (sizeResetCoroutine != null)
            {
                StopCoroutine(sizeResetCoroutine);
            }
            sizeResetCoroutine = StartCoroutine(ResetBallSize(ballTransform, originalScale));
        }
    }

    private IEnumerator ResetBallSize(Transform ballTransform, Vector3 originalScale)
    {
        yield return new WaitForSeconds(duration);
        
        if (ballTransform == null) yield break;
        
        Vector3 currentScale = ballTransform.localScale;
        
        // Efekt
        if (sizeChangeEffect != null)
        {
            Instantiate(sizeChangeEffect, ballTransform.position, Quaternion.identity);
        }
        
        // Animasyonlu geçiş
        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;
            t = Mathf.SmoothStep(0, 1, t);
            ballTransform.localScale = Vector3.Lerp(currentScale, originalScale, t);
            yield return null;
        }
        
        ballTransform.localScale = originalScale;
        isSizeChanged = false;
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
        
        // Boyut sembolü
        Gizmos.color = sizeMultiplier > 1f ? Color.green : Color.red;
        float symbolSize = sizeMultiplier > 1f ? 0.4f : 0.2f;
        Gizmos.DrawWireSphere(transform.position, symbolSize);
    }
}
