using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Adds hover, click, and idle animations to UI buttons.
/// Attach this to any button for enhanced visual feedback.
/// </summary>
[RequireComponent(typeof(Button))]
public class ButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Scale Animation")]
    [SerializeField] private bool enableScaleAnimation = true;
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float clickScale = 0.95f;
    [SerializeField] private float scaleSpeed = 10f;

    [Header("Color Animation")]
    [SerializeField] private bool enableColorPulse = false;
    [SerializeField] private Color pulseColor = Color.white;
    [SerializeField] private float pulseSpeed = 2f;

    [Header("Bounce Animation")]
    [SerializeField] private bool enableIdleBounce = false;
    [SerializeField] private float bounceAmount = 0.05f;
    [SerializeField] private float bounceSpeed = 2f;

    [Header("Rotation Animation")]
    [SerializeField] private bool enableHoverRotation = false;
    [SerializeField] private float rotationAmount = 5f;

    [Header("Shadow/Glow Effect")]
    [SerializeField] private bool enableShadow = false;
    [SerializeField] private Shadow shadowComponent;
    [SerializeField] private Vector2 normalShadowOffset = new Vector2(2, -2);
    [SerializeField] private Vector2 hoverShadowOffset = new Vector2(4, -4);

    private Button button;
    private RectTransform rectTransform;
    private Image buttonImage;
    private Vector3 originalScale;
    private Quaternion originalRotation;
    private Color originalColor;
    
    private float targetScale = 1f;
    private bool isHovered = false;
    private bool isPressed = false;
    private Coroutine colorPulseCoroutine;

    private void Awake()
    {
        button = GetComponent<Button>();
        rectTransform = GetComponent<RectTransform>();
        buttonImage = GetComponent<Image>();
        
        originalScale = rectTransform.localScale;
        originalRotation = rectTransform.localRotation;
        
        if (buttonImage != null)
            originalColor = buttonImage.color;
    }

    private void OnEnable()
    {
        if (enableColorPulse && buttonImage != null)
        {
            colorPulseCoroutine = StartCoroutine(ColorPulseAnimation());
        }
    }

    private void OnDisable()
    {
        if (colorPulseCoroutine != null)
        {
            StopCoroutine(colorPulseCoroutine);
            if (buttonImage != null)
                buttonImage.color = originalColor;
        }
        
        // Reset state
        rectTransform.localScale = originalScale;
        rectTransform.localRotation = originalRotation;
    }

    private void Update()
    {
        if (!button.interactable) return;

        // Scale animation
        if (enableScaleAnimation)
        {
            UpdateScale();
        }

        // Idle bounce
        if (enableIdleBounce && !isHovered && !isPressed)
        {
            float bounce = 1f + Mathf.Sin(Time.time * bounceSpeed) * bounceAmount;
            rectTransform.localScale = originalScale * bounce;
        }

        // Shadow update
        if (enableShadow && shadowComponent != null)
        {
            Vector2 targetOffset = isHovered ? hoverShadowOffset : normalShadowOffset;
            shadowComponent.effectDistance = Vector2.Lerp(
                shadowComponent.effectDistance,
                targetOffset,
                Time.deltaTime * scaleSpeed
            );
        }
    }

    private void UpdateScale()
    {
        float currentTarget;
        
        if (isPressed)
            currentTarget = clickScale;
        else if (isHovered)
            currentTarget = hoverScale;
        else
            currentTarget = 1f;

        Vector3 targetScaleVector = originalScale * currentTarget;
        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale,
            targetScaleVector,
            Time.deltaTime * scaleSpeed
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable) return;
        
        isHovered = true;

        if (enableHoverRotation)
        {
            float randomRotation = Random.Range(-rotationAmount, rotationAmount);
            StartCoroutine(RotateTo(Quaternion.Euler(0, 0, randomRotation)));
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        isPressed = false;

        if (enableHoverRotation)
        {
            StartCoroutine(RotateTo(originalRotation));
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.interactable) return;
        
        isPressed = true;
        
        // Haptic feedback (mobile)
        #if UNITY_ANDROID || UNITY_IOS
        if (SettingsUI.IsVibrationEnabled())
        {
            Handheld.Vibrate();
        }
        #endif
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }

    private IEnumerator RotateTo(Quaternion targetRotation)
    {
        float elapsed = 0f;
        float duration = 0.15f;
        Quaternion startRotation = rectTransform.localRotation;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            rectTransform.localRotation = Quaternion.Lerp(startRotation, targetRotation, elapsed / duration);
            yield return null;
        }

        rectTransform.localRotation = targetRotation;
    }

    private IEnumerator ColorPulseAnimation()
    {
        while (true)
        {
            float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            buttonImage.color = Color.Lerp(originalColor, pulseColor, t * 0.3f);
            yield return null;
        }
    }

    /// <summary>
    /// Play a pop animation (useful for special buttons)
    /// </summary>
    public void PlayPopAnimation()
    {
        StartCoroutine(PopAnimation());
    }

    private IEnumerator PopAnimation()
    {
        float elapsed = 0f;
        float popDuration = 0.3f;
        
        // Scale up
        while (elapsed < popDuration / 2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (popDuration / 2f);
            rectTransform.localScale = originalScale * (1f + 0.2f * t);
            yield return null;
        }
        
        // Scale back
        elapsed = 0f;
        while (elapsed < popDuration / 2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (popDuration / 2f);
            rectTransform.localScale = originalScale * (1.2f - 0.2f * t);
            yield return null;
        }
        
        rectTransform.localScale = originalScale;
    }

    /// <summary>
    /// Shake animation for invalid actions
    /// </summary>
    public void PlayShakeAnimation()
    {
        StartCoroutine(ShakeAnimation());
    }

    private IEnumerator ShakeAnimation()
    {
        float elapsed = 0f;
        float shakeDuration = 0.4f;
        float shakeIntensity = 10f;
        Vector2 originalPos = rectTransform.anchoredPosition;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float x = Mathf.Sin(elapsed * 50f) * shakeIntensity * (1f - elapsed / shakeDuration);
            rectTransform.anchoredPosition = originalPos + new Vector2(x, 0);
            yield return null;
        }

        rectTransform.anchoredPosition = originalPos;
    }
}
