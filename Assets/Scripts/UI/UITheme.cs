using UnityEngine;

/// <summary>
/// ScriptableObject that defines a complete UI theme.
/// Create via: Assets > Create > UI > Theme
/// </summary>
[CreateAssetMenu(fileName = "UITheme", menuName = "UI/Theme", order = 1)]
public class UITheme : ScriptableObject
{
    [Header("Theme Info")]
    public string themeName = "Default Theme";
    
    [Header("Primary Colors")]
    [Tooltip("Main accent color - used for primary buttons and highlights")]
    public Color primaryColor = new Color(0.2f, 0.6f, 1f); // Blue
    
    [Tooltip("Secondary accent color - used for secondary elements")]
    public Color secondaryColor = new Color(0.5f, 0.3f, 0.9f); // Purple
    
    [Tooltip("Success color - used for positive feedback")]
    public Color successColor = new Color(0.2f, 0.8f, 0.4f); // Green
    
    [Tooltip("Warning color - used for warnings")]
    public Color warningColor = new Color(1f, 0.7f, 0.2f); // Orange
    
    [Tooltip("Danger color - used for errors and destructive actions")]
    public Color dangerColor = new Color(0.9f, 0.3f, 0.3f); // Red

    [Header("Background Colors")]
    [Tooltip("Main background color")]
    public Color backgroundColor = new Color(0.1f, 0.1f, 0.15f); // Dark
    
    [Tooltip("Panel/Card background color")]
    public Color panelColor = new Color(0.15f, 0.15f, 0.2f);
    
    [Tooltip("Overlay/Popup background color")]
    public Color overlayColor = new Color(0f, 0f, 0f, 0.7f);

    [Header("Text Colors")]
    [Tooltip("Primary text color")]
    public Color textPrimary = Color.white;
    
    [Tooltip("Secondary/muted text color")]
    public Color textSecondary = new Color(0.7f, 0.7f, 0.7f);
    
    [Tooltip("Disabled text color")]
    public Color textDisabled = new Color(0.4f, 0.4f, 0.4f);

    [Header("Button Styles")]
    public ButtonStyle primaryButton = new ButtonStyle
    {
        normalColor = new Color(0.29f, 0.56f, 0.85f),      // #4A90D9 - Mavi
        highlightedColor = new Color(0.36f, 0.64f, 0.93f), // #5BA3EC - Açık mavi
        pressedColor = new Color(0.23f, 0.48f, 0.78f),     // #3A7BC8 - Koyu mavi
        disabledColor = new Color(0.4f, 0.4f, 0.4f),       // Gri
        textColor = Color.white
    };
    
    public ButtonStyle secondaryButton = new ButtonStyle
    {
        normalColor = new Color(0.61f, 0.35f, 0.71f),      // #9B59B6 - Mor
        highlightedColor = new Color(0.69f, 0.43f, 0.79f), // #B06DC9 - Açık mor
        pressedColor = new Color(0.53f, 0.27f, 0.63f),     // #8845A0 - Koyu mor
        disabledColor = new Color(0.4f, 0.4f, 0.4f),
        textColor = Color.white
    };
    
    public ButtonStyle dangerButton = new ButtonStyle
    {
        normalColor = new Color(0.91f, 0.30f, 0.24f),      // #E74C3C - Kırmızı
        highlightedColor = new Color(0.96f, 0.38f, 0.32f), // #F56152 - Açık kırmızı
        pressedColor = new Color(0.78f, 0.22f, 0.16f),     // #C7382A - Koyu kırmızı
        disabledColor = new Color(0.4f, 0.4f, 0.4f),
        textColor = Color.white
    };
    
    public ButtonStyle successButton = new ButtonStyle
    {
        normalColor = new Color(0.18f, 0.80f, 0.44f),      // #2ECC71 - Yeşil
        highlightedColor = new Color(0.26f, 0.88f, 0.52f), // #43E085 - Açık yeşil
        pressedColor = new Color(0.15f, 0.68f, 0.38f),     // #27AE60 - Koyu yeşil
        disabledColor = new Color(0.4f, 0.4f, 0.4f),
        textColor = Color.white
    };

    [Header("Fonts")]
    [Tooltip("Main font for titles")]
    public TMPro.TMP_FontAsset titleFont;
    
    [Tooltip("Font for body text")]
    public TMPro.TMP_FontAsset bodyFont;

    [Header("Font Sizes")]
    public float titleFontSize = 48f;
    public float subtitleFontSize = 32f;
    public float bodyFontSize = 24f;
    public float smallFontSize = 18f;
    public float buttonFontSize = 28f;

    [Header("Spacing & Sizing")]
    public float buttonHeight = 60f;
    public float buttonCornerRadius = 10f;
    public float panelPadding = 20f;
    public float elementSpacing = 15f;

    [Header("Effects")]
    public Color shadowColor = new Color(0f, 0f, 0f, 0.5f);
    public Vector2 shadowOffset = new Vector2(2f, -2f);
    public Color glowColor = new Color(1f, 1f, 1f, 0.3f);

    [System.Serializable]
    public class ButtonStyle
    {
        public Color normalColor;
        public Color highlightedColor;
        public Color pressedColor;
        public Color disabledColor;
        public Color textColor;
        
        public ButtonStyle()
        {
            normalColor = new Color(0.29f, 0.56f, 0.85f);
            highlightedColor = new Color(0.36f, 0.64f, 0.93f);
            pressedColor = new Color(0.23f, 0.48f, 0.78f);
            disabledColor = new Color(0.4f, 0.4f, 0.4f);
            textColor = Color.white;
        }
        
        public UnityEngine.UI.ColorBlock ToColorBlock()
        {
            return new UnityEngine.UI.ColorBlock
            {
                normalColor = normalColor,
                highlightedColor = highlightedColor,
                pressedColor = pressedColor,
                disabledColor = disabledColor,
                colorMultiplier = 1f,
                fadeDuration = 0.1f
            };
        }
    }

    /// <summary>
    /// Get a gradient from primary to secondary color
    /// </summary>
    public Gradient GetPrimaryGradient()
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(primaryColor, 0f),
                new GradientColorKey(secondaryColor, 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            }
        );
        return gradient;
    }
}
