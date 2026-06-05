using UnityEngine;
using System.Collections;

/// <summary>
/// Zamanlayıcıyla açılıp kapanan kapı/engel.
/// </summary>
public class ToggleDoor : LevelItem
{
    [Header("Toggle Settings")]
    [Tooltip("Açık kalma süresi (saniye)")]
    public float openDuration = 2f;
    
    [Tooltip("Kapalı kalma süresi (saniye)")]
    public float closedDuration = 2f;
    
    [Tooltip("Başlangıçta açık mı?")]
    public bool startsOpen = false;
    
    [Tooltip("Açılma/kapanma animasyon süresi")]
    public float transitionDuration = 0.3f;

    [Header("Movement")]
    [Tooltip("Açılma yönü")]
    public Vector2 openDirection = Vector2.up;
    
    [Tooltip("Açılma mesafesi")]
    public float openDistance = 2f;
    
    [Tooltip("Alternatif: Scale ile aç/kapa (true ise pozisyon değil scale değişir)")]
    public bool useScaleAnimation = false;

    [Header("Visual")]
    public Color openColor = Color.green;
    public Color closedColor = Color.red;
    public SpriteRenderer doorRenderer;

    // Internal
    private bool isOpen;
    private Vector3 closedPosition;
    private Vector3 openPosition;
    private Vector3 closedScale;
    private Vector3 openScale;
    private Collider2D doorCollider;
    private Coroutine toggleRoutine;

    protected override void Awake()
    {
        base.Awake();
        
        doorCollider = GetComponent<Collider2D>();
        closedPosition = transform.position;
        openPosition = closedPosition + (Vector3)(openDirection.normalized * openDistance);
        
        closedScale = transform.localScale;
        openScale = new Vector3(closedScale.x, 0.1f, closedScale.z);
        
        isOpen = startsOpen;
        
        // Başlangıç durumunu ayarla
        if (startsOpen)
        {
            if (useScaleAnimation)
                transform.localScale = openScale;
            else
                transform.position = openPosition;
                
            if (doorCollider != null)
                doorCollider.enabled = false;
        }
        
        UpdateVisual();
    }

    private void Start()
    {
        // Toggle döngüsünü başlat
        toggleRoutine = StartCoroutine(ToggleLoop());
    }

    private IEnumerator ToggleLoop()
    {
        while (true)
        {
            // Mevcut durumda bekle
            float waitTime = isOpen ? openDuration : closedDuration;
            yield return new WaitForSeconds(waitTime);
            
            // Durumu değiştir
            yield return StartCoroutine(ToggleState());
        }
    }

    private IEnumerator ToggleState()
    {
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 endPos = isOpen ? closedPosition : openPosition;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = isOpen ? closedScale : openScale;
        
        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;
            t = Mathf.SmoothStep(0, 1, t);
            
            if (useScaleAnimation)
            {
                transform.localScale = Vector3.Lerp(startScale, endScale, t);
            }
            else
            {
                transform.position = Vector3.Lerp(startPos, endPos, t);
            }
            
            yield return null;
        }
        
        // Son durumu ayarla
        if (useScaleAnimation)
            transform.localScale = endScale;
        else
            transform.position = endPos;
        
        isOpen = !isOpen;
        
        // Collider'ı güncelle
        if (doorCollider != null)
        {
            doorCollider.enabled = !isOpen;
        }
        
        UpdateVisual();
        PlayActivationFeedback();
    }

    private void UpdateVisual()
    {
        if (doorRenderer != null)
        {
            doorRenderer.color = isOpen ? openColor : closedColor;
        }
    }

    /// <summary>
    /// Manuel olarak toggle yapmak için
    /// </summary>
    public void ForceToggle()
    {
        if (toggleRoutine != null)
        {
            StopCoroutine(toggleRoutine);
        }
        StartCoroutine(ToggleState());
        toggleRoutine = StartCoroutine(ToggleLoop());
    }

    private void OnDrawGizmos()
    {
        Vector3 closed = Application.isPlaying ? closedPosition : transform.position;
        Vector3 open = closed + (Vector3)(openDirection.normalized * openDistance);
        
        // Kapalı pozisyon
        Gizmos.color = closedColor;
        Gizmos.DrawWireCube(closed, Vector3.one * 0.5f);
        
        // Açık pozisyon
        Gizmos.color = openColor;
        Gizmos.DrawWireCube(open, Vector3.one * 0.5f);
        
        // Hareket çizgisi
        Gizmos.color = Color.white;
        Gizmos.DrawLine(closed, open);
    }
}
