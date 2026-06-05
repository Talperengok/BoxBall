using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Creates a gradient effect on UI Image using vertex colors.
/// Supports horizontal and vertical gradients.
/// </summary>
[RequireComponent(typeof(Image))]
[AddComponentMenu("UI/Effects/UI Gradient")]
public class UIGradient : BaseMeshEffect
{
    [Header("Gradient Settings")]
    [SerializeField] private Color topColor = Color.white;
    [SerializeField] private Color bottomColor = Color.black;
    [SerializeField] private GradientDirection direction = GradientDirection.Vertical;
    
    [Header("Theme Integration")]
    [SerializeField] private bool useThemeColors = false;
    [SerializeField] private ThemedUI.ThemeColor themeTopColor = ThemedUI.ThemeColor.Primary;
    [SerializeField] private ThemedUI.ThemeColor themeBottomColor = ThemedUI.ThemeColor.Secondary;

    public enum GradientDirection
    {
        Vertical,
        Horizontal,
        DiagonalLeftToRight,
        DiagonalRightToLeft
    }

    public Color TopColor
    {
        get => topColor;
        set { topColor = value; graphic.SetVerticesDirty(); }
    }

    public Color BottomColor
    {
        get => bottomColor;
        set { bottomColor = value; graphic.SetVerticesDirty(); }
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive()) return;

        Color top = topColor;
        Color bottom = bottomColor;

        // Get colors from theme if enabled
        if (useThemeColors && ThemeManager.Instance != null && ThemeManager.Instance.CurrentTheme != null)
        {
            var theme = ThemeManager.Instance.CurrentTheme;
            top = GetThemeColor(theme, themeTopColor);
            bottom = GetThemeColor(theme, themeBottomColor);
        }

        UIVertex vertex = default;
        
        for (int i = 0; i < vh.currentVertCount; i++)
        {
            vh.PopulateUIVertex(ref vertex, i);
            
            float t = 0f;
            
            switch (direction)
            {
                case GradientDirection.Vertical:
                    // Normalize Y position (0 = bottom, 1 = top)
                    t = Mathf.InverseLerp(GetMinY(vh), GetMaxY(vh), vertex.position.y);
                    break;
                case GradientDirection.Horizontal:
                    t = Mathf.InverseLerp(GetMinX(vh), GetMaxX(vh), vertex.position.x);
                    break;
                case GradientDirection.DiagonalLeftToRight:
                    float normalizedX = Mathf.InverseLerp(GetMinX(vh), GetMaxX(vh), vertex.position.x);
                    float normalizedY = Mathf.InverseLerp(GetMinY(vh), GetMaxY(vh), vertex.position.y);
                    t = (normalizedX + normalizedY) / 2f;
                    break;
                case GradientDirection.DiagonalRightToLeft:
                    normalizedX = Mathf.InverseLerp(GetMinX(vh), GetMaxX(vh), vertex.position.x);
                    normalizedY = Mathf.InverseLerp(GetMinY(vh), GetMaxY(vh), vertex.position.y);
                    t = ((1f - normalizedX) + normalizedY) / 2f;
                    break;
            }
            
            vertex.color *= Color.Lerp(bottom, top, t);
            vh.SetUIVertex(vertex, i);
        }
    }

    private float GetMinY(VertexHelper vh)
    {
        float min = float.MaxValue;
        UIVertex vertex = default;
        for (int i = 0; i < vh.currentVertCount; i++)
        {
            vh.PopulateUIVertex(ref vertex, i);
            if (vertex.position.y < min) min = vertex.position.y;
        }
        return min;
    }

    private float GetMaxY(VertexHelper vh)
    {
        float max = float.MinValue;
        UIVertex vertex = default;
        for (int i = 0; i < vh.currentVertCount; i++)
        {
            vh.PopulateUIVertex(ref vertex, i);
            if (vertex.position.y > max) max = vertex.position.y;
        }
        return max;
    }

    private float GetMinX(VertexHelper vh)
    {
        float min = float.MaxValue;
        UIVertex vertex = default;
        for (int i = 0; i < vh.currentVertCount; i++)
        {
            vh.PopulateUIVertex(ref vertex, i);
            if (vertex.position.x < min) min = vertex.position.x;
        }
        return min;
    }

    private float GetMaxX(VertexHelper vh)
    {
        float max = float.MinValue;
        UIVertex vertex = default;
        for (int i = 0; i < vh.currentVertCount; i++)
        {
            vh.PopulateUIVertex(ref vertex, i);
            if (vertex.position.x > max) max = vertex.position.x;
        }
        return max;
    }

    private Color GetThemeColor(UITheme theme, ThemedUI.ThemeColor color)
    {
        switch (color)
        {
            case ThemedUI.ThemeColor.Primary: return theme.primaryColor;
            case ThemedUI.ThemeColor.Secondary: return theme.secondaryColor;
            case ThemedUI.ThemeColor.Success: return theme.successColor;
            case ThemedUI.ThemeColor.Warning: return theme.warningColor;
            case ThemedUI.ThemeColor.Danger: return theme.dangerColor;
            case ThemedUI.ThemeColor.Background: return theme.backgroundColor;
            case ThemedUI.ThemeColor.Panel: return theme.panelColor;
            case ThemedUI.ThemeColor.TextPrimary: return theme.textPrimary;
            case ThemedUI.ThemeColor.TextSecondary: return theme.textSecondary;
            default: return theme.primaryColor;
        }
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        graphic?.SetVerticesDirty();
    }
#endif
}
