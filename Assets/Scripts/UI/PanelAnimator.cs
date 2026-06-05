using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Handles smooth panel transitions with fade and scale animations.
/// Attach this to each panel that needs animations.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class PanelAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float scaleDuration = 0.25f;
    [SerializeField] private AnimationType animationType = AnimationType.FadeAndScale;
    
    [Header("Scale Settings")]
    [SerializeField] private Vector3 startScale = new Vector3(0.8f, 0.8f, 1f);
    [SerializeField] private Vector3 endScale = Vector3.one;
    
    [Header("Slide Settings")]
    [SerializeField] private SlideDirection slideDirection = SlideDirection.Right;
    [SerializeField] private float slideDistance = 100f;

    public enum AnimationType
    {
        Fade,
        Scale,
        FadeAndScale,
        SlideIn
    }

    public enum SlideDirection
    {
        Left,
        Right,
        Up,
        Down
    }

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Coroutine currentAnimation;
    private bool isInitialized = false;

    private void Awake()
    {
        EnsureInitialized();
    }

    private void EnsureInitialized()
    {
        if (isInitialized) return;
        
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
            originalPosition = rectTransform.anchoredPosition;
            
        isInitialized = true;
    }

    /// <summary>
    /// Show the panel with animation
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
        
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);
            
        currentAnimation = StartCoroutine(AnimateIn());
    }

    /// <summary>
    /// Hide the panel with animation
    /// </summary>
    public void Hide(System.Action onComplete = null)
    {
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);
            
        currentAnimation = StartCoroutine(AnimateOut(onComplete));
    }

    /// <summary>
    /// Instantly show without animation
    /// </summary>
    public void ShowInstant()
    {
        EnsureInitialized();
        gameObject.SetActive(true);
        if (canvasGroup != null) canvasGroup.alpha = 1f;
        if (rectTransform != null)
        {
            rectTransform.localScale = endScale;
            rectTransform.anchoredPosition = originalPosition;
        }
    }

    /// <summary>
    /// Instantly hide without animation
    /// </summary>
    public void HideInstant()
    {
        EnsureInitialized();
        if (canvasGroup != null) canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    private IEnumerator AnimateIn()
    {
        // Initial state
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        switch (animationType)
        {
            case AnimationType.Fade:
                yield return StartCoroutine(FadeIn());
                break;
            case AnimationType.Scale:
                rectTransform.localScale = startScale;
                canvasGroup.alpha = 1f;
                yield return StartCoroutine(ScaleIn());
                break;
            case AnimationType.FadeAndScale:
                rectTransform.localScale = startScale;
                yield return StartCoroutine(FadeAndScaleIn());
                break;
            case AnimationType.SlideIn:
                SetSlideStartPosition();
                canvasGroup.alpha = 1f;
                yield return StartCoroutine(SlideIn());
                break;
        }

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private IEnumerator AnimateOut(System.Action onComplete)
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        switch (animationType)
        {
            case AnimationType.Fade:
                yield return StartCoroutine(FadeOut());
                break;
            case AnimationType.Scale:
                yield return StartCoroutine(ScaleOut());
                break;
            case AnimationType.FadeAndScale:
                yield return StartCoroutine(FadeAndScaleOut());
                break;
            case AnimationType.SlideIn:
                yield return StartCoroutine(SlideOut());
                break;
        }

        gameObject.SetActive(false);
        onComplete?.Invoke();
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, EaseOutCubic(elapsed / fadeDuration));
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, EaseInCubic(elapsed / fadeDuration));
            yield return null;
        }
        canvasGroup.alpha = 0f;
    }

    private IEnumerator ScaleIn()
    {
        float elapsed = 0f;
        while (elapsed < scaleDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = EaseOutBack(elapsed / scaleDuration);
            rectTransform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }
        rectTransform.localScale = endScale;
    }

    private IEnumerator ScaleOut()
    {
        float elapsed = 0f;
        while (elapsed < scaleDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = EaseInCubic(elapsed / scaleDuration);
            rectTransform.localScale = Vector3.Lerp(endScale, startScale, t);
            yield return null;
        }
        rectTransform.localScale = startScale;
    }

    private IEnumerator FadeAndScaleIn()
    {
        float elapsed = 0f;
        float duration = Mathf.Max(fadeDuration, scaleDuration);
        
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            
            float fadeT = Mathf.Clamp01(elapsed / fadeDuration);
            float scaleT = Mathf.Clamp01(elapsed / scaleDuration);
            
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, EaseOutCubic(fadeT));
            rectTransform.localScale = Vector3.Lerp(startScale, endScale, EaseOutBack(scaleT));
            
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
        rectTransform.localScale = endScale;
    }

    private IEnumerator FadeAndScaleOut()
    {
        float elapsed = 0f;
        float duration = Mathf.Max(fadeDuration, scaleDuration);
        
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            
            float fadeT = Mathf.Clamp01(elapsed / fadeDuration);
            float scaleT = Mathf.Clamp01(elapsed / scaleDuration);
            
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, EaseInCubic(fadeT));
            rectTransform.localScale = Vector3.Lerp(endScale, startScale, EaseInCubic(scaleT));
            
            yield return null;
        }
        
        canvasGroup.alpha = 0f;
        rectTransform.localScale = startScale;
    }

    private void SetSlideStartPosition()
    {
        Vector2 offset = Vector2.zero;
        switch (slideDirection)
        {
            case SlideDirection.Left:
                offset = new Vector2(-slideDistance, 0);
                break;
            case SlideDirection.Right:
                offset = new Vector2(slideDistance, 0);
                break;
            case SlideDirection.Up:
                offset = new Vector2(0, slideDistance);
                break;
            case SlideDirection.Down:
                offset = new Vector2(0, -slideDistance);
                break;
        }
        rectTransform.anchoredPosition = originalPosition + offset;
    }

    private IEnumerator SlideIn()
    {
        Vector2 startPos = rectTransform.anchoredPosition;
        float elapsed = 0f;
        
        while (elapsed < scaleDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = EaseOutCubic(elapsed / scaleDuration);
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, originalPosition, t);
            yield return null;
        }
        
        rectTransform.anchoredPosition = originalPosition;
    }

    private IEnumerator SlideOut()
    {
        Vector2 endPos = originalPosition;
        switch (slideDirection)
        {
            case SlideDirection.Left:
                endPos += new Vector2(-slideDistance, 0);
                break;
            case SlideDirection.Right:
                endPos += new Vector2(slideDistance, 0);
                break;
            case SlideDirection.Up:
                endPos += new Vector2(0, slideDistance);
                break;
            case SlideDirection.Down:
                endPos += new Vector2(0, -slideDistance);
                break;
        }

        float elapsed = 0f;
        while (elapsed < scaleDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = EaseInCubic(elapsed / scaleDuration);
            rectTransform.anchoredPosition = Vector2.Lerp(originalPosition, endPos, t);
            yield return null;
        }
    }

    // Easing functions
    private float EaseOutCubic(float t) => 1f - Mathf.Pow(1f - t, 3f);
    private float EaseInCubic(float t) => t * t * t;
    private float EaseOutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }
}
