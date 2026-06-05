using UnityEngine;

/// <summary>
/// Top yerleştirme ve başlatma scripti.
/// Slingshot mekaniği ile birlikte çalışır.
/// </summary>
public class BallPlacement : MonoBehaviour
{
    [Header("Ayarlar")]
    public BoxCollider2D spawnArea;
    public bool isGameStarted = false;
    
    [Header("Launch Mode")]
    [Tooltip("True: Slingshot modu, False: Legacy bat modu")]
    public bool useSlingshotMode = true;

    private Rigidbody2D rb;
    private bool isDragging = false;
    private Camera cam;
    private Vector2 initialPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        initialPosition = transform.position;

        // Başlangıçta top havada asılı (Kinematic)
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    void Update()
    {
        if (isGameStarted) return;
        
        // Slingshot modunda pozisyon ayarlama devre dışı
        // SlingshotController handle ediyor
        if (useSlingshotMode) return;

        if (isDragging)
        {
            DragBall();
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            if (GetComponent<Collider2D>().OverlapPoint(mousePos))
            {
                isDragging = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    void DragBall()
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        if (spawnArea != null)
        {
            Bounds bounds = spawnArea.bounds;
            float x = Mathf.Clamp(mousePos.x, bounds.min.x, bounds.max.x);
            float y = Mathf.Clamp(mousePos.y, bounds.min.y, bounds.max.y);
            rb.MovePosition(new Vector2(x, y));
        }
        else
        {
            rb.MovePosition(mousePos);
        }
    }

    /// <summary>
    /// Topu belirtilen hızla fırlat (Slingshot tarafından çağrılır)
    /// </summary>
    public void LaunchBall(Vector2 velocity)
    {
        if (isGameStarted) return;
        
        ActivateBall();
        rb.linearVelocity = velocity;
        
        Debug.Log($"[BallPlacement] Ball launched with velocity: {velocity}");
    }

    /// <summary>
    /// Topu aktif hale getir
    /// </summary>
    public void ActivateBall()
    {
        isGameStarted = true;
        isDragging = false;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    /// <summary>
    /// Topu sıfırla
    /// </summary>
    public void ResetBall()
    {
        isGameStarted = false;
        isDragging = false;
        transform.position = initialPosition;
        
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }

    /// <summary>
    /// Initial pozisyonu ayarla
    /// </summary>
    public void SetInitialPosition(Vector2 position)
    {
        initialPosition = position;
        transform.position = position;
    }
}
