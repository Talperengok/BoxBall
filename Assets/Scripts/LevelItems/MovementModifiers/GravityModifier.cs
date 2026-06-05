using UnityEngine;

/// <summary>
/// Belirli bir alanda yerçekimini değiştiren item.
/// </summary>
public class GravityModifier : LevelItem
{
    [Header("Gravity Settings")]
    [Tooltip("Yerçekimi çarpanı (1 = normal, 0 = sıfır yerçekimi, -1 = ters)")]
    public float gravityScale = 0.5f;
    
    [Tooltip("Özel yerçekimi yönü kullan")]
    public bool useCustomDirection = false;
    
    [Tooltip("Özel yerçekimi yönü ve şiddeti")]
    public Vector2 customGravity = new Vector2(0, -9.81f);
    
    [Tooltip("Geçiş yumuşaklığı (saniye)")]
    public float transitionDuration = 0.2f;

    [Header("Visual")]
    public Color zoneColor = new Color(0.5f, 0f, 0.5f, 0.3f);

    // Internal
    private float originalGravityScale;
    private bool ballInZone = false;
    private float transitionTimer = 0f;
    private float targetGravityScale;
    private Rigidbody2D trackedBall;

    protected override void Update()
    {
        base.Update();
        
        // Yumuşak geçiş
        if (trackedBall != null && ballInZone)
        {
            if (transitionTimer < transitionDuration)
            {
                transitionTimer += Time.deltaTime;
                float t = transitionTimer / transitionDuration;
                t = Mathf.SmoothStep(0, 1, t);
                
                if (!useCustomDirection)
                {
                    trackedBall.gravityScale = Mathf.Lerp(originalGravityScale, gravityScale, t);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        // Özel yerçekimi yönü uygula
        if (useCustomDirection && ballInZone && trackedBall != null)
        {
            // Normal yerçekimini devre dışı bırak
            trackedBall.gravityScale = 0f;
            
            // Özel yerçekimini uygula
            trackedBall.AddForce(customGravity * trackedBall.mass);
        }
    }

    protected override void OnBallEnter(Rigidbody2D ball)
    {
        trackedBall = ball;
        originalGravityScale = ball.gravityScale;
        ballInZone = true;
        transitionTimer = 0f;
        targetGravityScale = gravityScale;
        
        PlayActivationFeedback();
    }

    protected override void OnBallExit(Rigidbody2D ball)
    {
        if (trackedBall == ball)
        {
            // Orijinal yerçekimine dön
            ball.gravityScale = originalGravityScale;
            ballInZone = false;
            trackedBall = null;
        }
    }

    private void OnDrawGizmos()
    {
        // Zone'u göster
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
        
        // Yerçekimi yönünü göster
        Gizmos.color = Color.magenta;
        Vector3 gravDir;
        
        if (useCustomDirection)
        {
            gravDir = customGravity.normalized * 1.5f;
        }
        else
        {
            gravDir = Vector3.down * gravityScale;
        }
        
        Gizmos.DrawLine(transform.position, transform.position + gravDir);
        
        // Ok ucu
        if (gravDir.magnitude > 0.1f)
        {
            Vector3 arrowHead = transform.position + gravDir;
            Vector3 right = Quaternion.Euler(0, 0, 30) * -gravDir.normalized * 0.3f;
            Vector3 left = Quaternion.Euler(0, 0, -30) * -gravDir.normalized * 0.3f;
            Gizmos.DrawLine(arrowHead, arrowHead + right);
            Gizmos.DrawLine(arrowHead, arrowHead + left);
        }
    }
}
