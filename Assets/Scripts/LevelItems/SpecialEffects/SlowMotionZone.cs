using UnityEngine;

/// <summary>
/// Slow motion zone - Belirli alanda zamanı yavaşlatır.
/// </summary>
public class SlowMotionZone : LevelItem
{
    [Header("Slow Motion Settings")]
    [Tooltip("Zaman ölçeği (0.5 = yarı hız)")]
    [Range(0.1f, 1f)]
    public float timeScale = 0.5f;
    
    [Tooltip("Sadece topu etkiler (true), tüm oyunu etkiler (false)")]
    public bool affectsOnlyBall = true;
    
    [Tooltip("Geçiş süresi")]
    public float transitionDuration = 0.2f;

    [Header("Visual")]
    [Tooltip("Zone rengi")]
    public Color zoneColor = new Color(0.5f, 0.5f, 1f, 0.3f);
    
    [Tooltip("Slow motion efekti")]
    public GameObject slowMotionEffect;

    // Internal
    private float originalTimeScale = 1f;
    private float originalVelocityMagnitude;
    private Vector2 originalVelocityDirection;
    private bool isBallInZone = false;

    protected override void OnBallEnter(Rigidbody2D ball)
    {
        isBallInZone = true;
        
        if (affectsOnlyBall)
        {
            // Sadece topun hızını azalt
            originalVelocityMagnitude = ball.linearVelocity.magnitude;
            originalVelocityDirection = ball.linearVelocity.normalized;
        }
        else
        {
            // Tüm oyun zamanını yavaşlat
            originalTimeScale = Time.timeScale;
            StartCoroutine(TransitionTimeScale(timeScale));
        }
        
        // Efekt
        if (slowMotionEffect != null)
        {
            Instantiate(slowMotionEffect, ball.position, Quaternion.identity);
        }
        
        PlayActivationFeedback();
    }

    protected override void OnBallStay(Rigidbody2D ball)
    {
        if (affectsOnlyBall)
        {
            // Topun hızını sürekli yavaşlat
            Vector2 currentVelocity = ball.linearVelocity;
            float targetSpeed = currentVelocity.magnitude * timeScale;
            
            // Çok hızlı yavaşlamaması için lerp kullan
            float newSpeed = Mathf.Lerp(currentVelocity.magnitude, targetSpeed, Time.deltaTime * 5f);
            ball.linearVelocity = currentVelocity.normalized * newSpeed;
        }
    }

    protected override void OnBallExit(Rigidbody2D ball)
    {
        isBallInZone = false;
        
        if (!affectsOnlyBall)
        {
            // Zamanı normale döndür
            StartCoroutine(TransitionTimeScale(originalTimeScale));
        }
    }

    private System.Collections.IEnumerator TransitionTimeScale(float targetScale)
    {
        float startScale = Time.timeScale;
        float elapsed = 0f;
        
        while (elapsed < transitionDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / transitionDuration;
            Time.timeScale = Mathf.Lerp(startScale, targetScale, t);
            Time.fixedDeltaTime = 0.02f * Time.timeScale; // Physics step'i ayarla
            yield return null;
        }
        
        Time.timeScale = targetScale;
        Time.fixedDeltaTime = 0.02f * targetScale;
    }

    private void OnDisable()
    {
        // Devre dışı bırakılırsa zamanı normale döndür
        if (!affectsOnlyBall)
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
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
            Gizmos.DrawCube(box.offset, box.size);
        }
        else if (circle != null)
        {
            Gizmos.DrawSphere(transform.position + (Vector3)circle.offset, circle.radius);
        }
        
        // Slow motion sembolü
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}
