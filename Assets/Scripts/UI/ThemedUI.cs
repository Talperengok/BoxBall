using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Attach to any UI element to automatically apply theme styling.
/// Configure what aspects of the theme to apply in the inspector.
/// </summary>
public class ThemedUI : MonoBehaviour
{
    [Header("Element Type")]
    [SerializeField] private ElementType elementType = ElementType.Panel;

    [Header("Color Override")]
    [SerializeField] private bool useCustomColor = false;
    [SerializeField] private ThemeColor themeColor = ThemeColor.Primary;

    [Header("Text Settings")]
    [SerializeField] private bool applyFont = true;
    [SerializeField] private TextType textType = TextType.Body;
    [SerializeField] private bool applyFontSize = true;

    [Header("Button Settings")]
    [SerializeField] private ButtonType buttonType = ButtonType.Primary;

    public enum ElementType
    {
        Panel,
        Button,
        Text,
        Background,
        Overlay,
        Icon
    }

    public enum ThemeColor
    {
        Primary,
        Secondary,
        Success,
        Warning,
        Danger,
        Background,
        Panel,
        TextPrimary,
        TextSecondary
    }

    public enum TextType
    {
        Title,
        Subtitle,
        Body,
        Small,
        Button
    }

    public enum ButtonType
    {
        Primary,
        Secondary,
        Danger
    }

    private Image image;
    private Button button;
    private TextMeshProUGUI tmpText;

    private void Awake()
    {
        CacheComponents();
    }

    private void Start()
    {
        if (ThemeManager.Instance != null && ThemeManager.Instance.CurrentTheme != null)
        {
            ApplyTheme(ThemeManager.Instance.CurrentTheme);
        }
    }

    private void OnEnable()
    {
        if (ThemeManager.Instance != null)
        {
            ThemeManager.Instance.OnThemeChanged += ApplyTheme;
        }
    }

    private void OnDisable()
    {
        if (ThemeManager.Instance != null)
        {
            ThemeManager.Instance.OnThemeChanged -= ApplyTheme;
        }
    }

    private void CacheComponents()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        tmpText = GetComponent<TextMeshProUGUI>();
    }

    public void ApplyTheme(UITheme theme)
    {
        if (theme == null) return;

        if (image == null && button == null && tmpText == null)
            CacheComponents();

        switch (elementType)
        {
            case ElementType.Panel:
                ApplyPanelTheme(theme);
                break;
            case ElementType.Button:
                ApplyButtonTheme(theme);
                break;
            case ElementType.Text:
                ApplyTextTheme(theme);
                break;
            case ElementType.Background:
                if (image != null) image.color = theme.backgroundColor;
                break;
            case ElementType.Overlay:
                if (image != null) image.color = theme.overlayColor;
                break;
            case ElementType.Icon:
                ApplyIconTheme(theme);
                break;
        }
    }

    private void ApplyPanelTheme(UITheme theme)
    {
        if (image != null)
        {
            image.color = useCustomColor ? GetThemeColor(theme, themeColor) : theme.panelColor;
        }
    }

    private void ApplyButtonTheme(UITheme theme)
    {
        if (button == null) return;

        UITheme.ButtonStyle style;
        switch (buttonType)
        {
            case ButtonType.Secondary:
                style = theme.secondaryButton;
                break;
            case ButtonType.Danger:
                style = theme.dangerButton;
                break;
            default:
                style = theme.primaryButton;
                break;
        }

        if (style != null)
        {
            button.colors = style.ToColorBlock();

            // Apply text color to child text
            TextMeshProUGUI childText = GetComponentInChildren<TextMeshProUGUI>();
            if (childText != null)
            {
                childText.color = style.textColor;
                if (applyFont && theme.bodyFont != null)
                    childText.font = theme.bodyFont;
                if (applyFontSize)
                    childText.fontSize = theme.buttonFontSize;
            }
        }
    }

    private void ApplyTextTheme(UITheme theme)
    {
        if (tmpText == null) return;

        // Apply color
        tmpText.color = useCustomColor ? GetThemeColor(theme, themeColor) : theme.textPrimary;

        // Apply font
        if (applyFont)
        {
            tmpText.font = textType == TextType.Title ? theme.titleFont : theme.bodyFont;
            
            // Fallback if specific font is null
            if (tmpText.font == null)
                tmpText.font = theme.bodyFont ?? theme.titleFont;
        }

        // Apply font size
        if (applyFontSize)
        {
            tmpText.fontSize = GetFontSize(theme);
        }
    }

    private void ApplyIconTheme(UITheme theme)
    {
        if (image != null)
        {
            image.color = useCustomColor ? GetThemeColor(theme, themeColor) : theme.textPrimary;
        }
    }

    private float GetFontSize(UITheme theme)
    {
        switch (textType)
        {
            case TextType.Title: return theme.titleFontSize;
            case TextType.Subtitle: return theme.subtitleFontSize;
            case TextType.Body: return theme.bodyFontSize;
            case TextType.Small: return theme.smallFontSize;
            case TextType.Button: return theme.buttonFontSize;
            default: return theme.bodyFontSize;
        }
    }

    private Color GetThemeColor(UITheme theme, ThemeColor color)
    {
        switch (color)
        {
            case ThemeColor.Primary: return theme.primaryColor;
            case ThemeColor.Secondary: return theme.secondaryColor;
            case ThemeColor.Success: return theme.successColor;
            case ThemeColor.Warning: return theme.warningColor;
            case ThemeColor.Danger: return theme.dangerColor;
            case ThemeColor.Background: return theme.backgroundColor;
            case ThemeColor.Panel: return theme.panelColor;
            case ThemeColor.TextPrimary: return theme.textPrimary;
            case ThemeColor.TextSecondary: return theme.textSecondary;
            default: return theme.primaryColor;
        }
    }

    /// <summary>
    /// Force refresh theme
    /// </summary>
    [ContextMenu("Refresh Theme")]
    public void RefreshTheme()
    {
        if (ThemeManager.Instance != null && ThemeManager.Instance.CurrentTheme != null)
        {
            ApplyTheme(ThemeManager.Instance.CurrentTheme);
        }
    }
}
