using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the current UI theme and applies it to UI elements.
/// Singleton that persists across scenes.
/// </summary>
public class ThemeManager : MonoBehaviour
{
    public static ThemeManager Instance { get; private set; }

    [Header("Current Theme")]
    [SerializeField] private UITheme currentTheme;

    [Header("Auto Apply")]
    [SerializeField] private bool applyOnStart = true;
    [SerializeField] private bool applyToChildren = true;

    public UITheme CurrentTheme => currentTheme;

    public delegate void ThemeChangedHandler(UITheme newTheme);
    public event ThemeChangedHandler OnThemeChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (applyOnStart && currentTheme != null)
        {
            ApplyTheme();
        }
    }

    /// <summary>
    /// Change to a new theme
    /// </summary>
    public void SetTheme(UITheme theme)
    {
        if (theme == null) return;
        
        currentTheme = theme;
        ApplyTheme();
        OnThemeChanged?.Invoke(theme);
    }

    /// <summary>
    /// Apply current theme to all ThemedUI components
    /// </summary>
    public void ApplyTheme()
    {
        if (currentTheme == null) return;

        ThemedUI[] themedElements = FindObjectsOfType<ThemedUI>(true);
        foreach (var element in themedElements)
        {
            element.ApplyTheme(currentTheme);
        }
    }

    /// <summary>
    /// Apply theme to a specific GameObject and its children
    /// </summary>
    public void ApplyThemeTo(GameObject target)
    {
        if (currentTheme == null || target == null) return;

        ThemedUI[] themedElements = target.GetComponentsInChildren<ThemedUI>(true);
        foreach (var element in themedElements)
        {
            element.ApplyTheme(currentTheme);
        }
    }

    // Quick access to theme colors
    public Color PrimaryColor => currentTheme?.primaryColor ?? Color.blue;
    public Color SecondaryColor => currentTheme?.secondaryColor ?? Color.purple;
    public Color BackgroundColor => currentTheme?.backgroundColor ?? Color.black;
    public Color TextPrimary => currentTheme?.textPrimary ?? Color.white;
}
