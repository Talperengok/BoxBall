using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Sıralı hedef - Belirli sırada vurulması gereken hedefler.
/// </summary>
public class SequentialTarget : LevelItem
{
    [Header("Sequence Settings")]
    [Tooltip("Bu hedefin sıra numarası (1'den başlar)")]
    public int sequenceOrder = 1;
    
    [Tooltip("Tüm hedefler (aynı manager için)")]
    public SequentialTarget[] allTargets;
    
    [Tooltip("Doğru sırada vuruldu mu?")]
    public bool isActivated = false;

    [Header("Events")]
    [Tooltip("Doğru sırada vurulduğunda")]
    public UnityEvent onCorrectHit;
    
    [Tooltip("Yanlış sırada vurulduğunda")]
    public UnityEvent onWrongHit;
    
    [Tooltip("Tüm hedefler tamamlandığında")]
    public UnityEvent onAllCompleted;

    [Header("Visual")]
    [Tooltip("Aktif olmayan renk")]
    public Color inactiveColor = Color.gray;
    
    [Tooltip("Sırası gelen renk")]
    public Color nextColor = Color.yellow;
    
    [Tooltip("Tamamlanmış renk")]
    public Color completedColor = Color.green;
    
    [Tooltip("Yanlış vuruş rengi")]
    public Color wrongColor = Color.red;

    // Internal
    private SpriteRenderer spriteRenderer;
    private static int currentSequenceIndex = 1;

    protected override void Awake()
    {
        base.Awake();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // Statik değeri sıfırla (her level başında)
        currentSequenceIndex = 1;
        UpdateVisual();
    }

    protected override void OnBallEnter(Rigidbody2D ball)
    {
        if (isActivated) return;
        
        CheckSequence();
    }

    protected override void OnBallCollision(Collision2D collision, Rigidbody2D ball)
    {
        if (isActivated) return;
        
        CheckSequence();
    }

    private void CheckSequence()
    {
        if (sequenceOrder == currentSequenceIndex)
        {
            // Doğru sırada!
            CorrectHit();
        }
        else
        {
            // Yanlış sırada!
            WrongHit();
        }
    }

    private void CorrectHit()
    {
        isActivated = true;
        currentSequenceIndex++;
        
        UpdateVisual();
        PlayActivationFeedback();
        
        onCorrectHit?.Invoke();
        
        // Tüm hedefler tamamlandı mı?
        if (CheckAllCompleted())
        {
            Debug.Log("Tüm sıralı hedefler tamamlandı!");
            onAllCompleted?.Invoke();
        }
        
        // Diğer hedeflerin görsellerini güncelle
        UpdateAllVisuals();
    }

    private void WrongHit()
    {
        // Yanlış vuruş efekti
        StartCoroutine(WrongHitEffect());
        
        onWrongHit?.Invoke();
        
        PlayActivationFeedback();
    }

    private System.Collections.IEnumerator WrongHitEffect()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = wrongColor;
            yield return new WaitForSeconds(0.3f);
            spriteRenderer.color = originalColor;
        }
    }

    private bool CheckAllCompleted()
    {
        if (allTargets == null || allTargets.Length == 0)
        {
            // Sadece bu hedefi kontrol et
            return isActivated;
        }
        
        foreach (var target in allTargets)
        {
            if (target != null && !target.isActivated)
            {
                return false;
            }
        }
        
        return true;
    }

    private void UpdateVisual()
    {
        if (spriteRenderer == null) return;
        
        if (isActivated)
        {
            spriteRenderer.color = completedColor;
        }
        else if (sequenceOrder == currentSequenceIndex)
        {
            spriteRenderer.color = nextColor;
        }
        else
        {
            spriteRenderer.color = inactiveColor;
        }
    }

    private void UpdateAllVisuals()
    {
        if (allTargets == null) return;
        
        foreach (var target in allTargets)
        {
            if (target != null)
            {
                target.UpdateVisual();
            }
        }
    }

    /// <summary>
    /// Tüm hedefleri sıfırla
    /// </summary>
    public static void ResetAllTargets(SequentialTarget[] targets)
    {
        currentSequenceIndex = 1;
        
        if (targets != null)
        {
            foreach (var target in targets)
            {
                if (target != null)
                {
                    target.isActivated = false;
                    target.UpdateVisual();
                }
            }
        }
    }

    public void ResetTarget()
    {
        isActivated = false;
        UpdateVisual();
    }

    private void OnDrawGizmos()
    {
        // Sıra numarasını göster
        Color color = isActivated ? completedColor : 
                      (sequenceOrder == currentSequenceIndex ? nextColor : inactiveColor);
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, 0.4f);
        
#if UNITY_EDITOR
        // Sıra numarası
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, 
            sequenceOrder.ToString(), 
            new GUIStyle() { fontSize = 20, fontStyle = FontStyle.Bold });
#endif
        
        // Sonraki hedefe bağlantı
        if (allTargets != null)
        {
            for (int i = 0; i < allTargets.Length; i++)
            {
                if (allTargets[i] != null && allTargets[i].sequenceOrder == sequenceOrder + 1)
                {
                    Gizmos.color = new Color(color.r, color.g, color.b, 0.3f);
                    Gizmos.DrawLine(transform.position, allTargets[i].transform.position);
                }
            }
        }
    }
}
