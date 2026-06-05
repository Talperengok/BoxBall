using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Basınç butonu - Top ile tetiklendiğinde olayları çalıştırır.
/// </summary>
public class PressureButton : LevelItem
{
    [Header("Button Settings")]
    [Tooltip("Basılı kalması için topu bekleme zorunlu mu?")]
    public bool requireStay = false;
    
    [Tooltip("Bir kez basılınca kalıcı aktif mi?")]
    public bool stayPressed = false;
    
    [Tooltip("Basılı animasyon miktarı")]
    public float pressDepth = 0.1f;

    [Header("Events")]
    [Tooltip("Buton basıldığında")]
    public UnityEvent onPressed;
    
    [Tooltip("Buton bırakıldığında")]
    public UnityEvent onReleased;

    [Header("Visual")]
    [Tooltip("Basılı renk")]
    public Color pressedColor = Color.green;
    
    [Tooltip("Normal renk")]
    public Color normalColor = Color.red;

    // Internal
    private bool isPressed = false;
    private Vector3 originalPosition;
    private Vector3 pressedPosition;
    private SpriteRenderer spriteRenderer;

    protected override void Awake()
    {
        base.Awake();
        
        originalPosition = transform.position;
        pressedPosition = originalPosition - new Vector3(0, pressDepth, 0);
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        UpdateVisual();
    }

    protected override void OnBallEnter(Rigidbody2D ball)
    {
        if (isPressed && stayPressed) return;
        
        PressButton();
    }

    protected override void OnBallExit(Rigidbody2D ball)
    {
        if (stayPressed) return;
        if (!requireStay) return;
        
        ReleaseButton();
    }

    private void PressButton()
    {
        if (isPressed) return;
        
        isPressed = true;
        
        // Animasyon
        transform.position = pressedPosition;
        
        // Görsel
        UpdateVisual();
        
        // Event
        onPressed?.Invoke();
        
        PlayActivationFeedback();
        
        // Tek seferlik ise (stay değilse ve require stay değilse)
        if (!stayPressed && !requireStay)
        {
            // Kısa süre sonra serbest bırak
            Invoke(nameof(ReleaseButton), 0.2f);
        }
    }

    private void ReleaseButton()
    {
        if (!isPressed) return;
        if (stayPressed) return;
        
        isPressed = false;
        
        // Animasyon
        transform.position = originalPosition;
        
        // Görsel
        UpdateVisual();
        
        // Event
        onReleased?.Invoke();
    }

    private void UpdateVisual()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = isPressed ? pressedColor : normalColor;
        }
    }

    /// <summary>
    /// Butonu manuel sıfırla
    /// </summary>
    public void ResetButton()
    {
        isPressed = false;
        transform.position = originalPosition;
        UpdateVisual();
    }

    private void OnDrawGizmos()
    {
        // Buton durumunu göster
        Gizmos.color = isPressed ? pressedColor : normalColor;
        
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(box.offset, box.size);
        }
        else
        {
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.5f);
        }
    }
}
