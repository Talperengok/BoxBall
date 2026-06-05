using UnityEngine;

/// <summary>
/// Slingshot için yörünge preview çizen script.
/// Fizik simülasyonu ile tahmini yörüngeyi gösterir.
/// </summary>
public class TrajectoryRenderer : MonoBehaviour
{
    [Header("Trajectory Settings")]
    [Tooltip("Yörünge için kullanılacak LineRenderer")]
    public LineRenderer trajectoryLine;
    
    [Tooltip("Kaç nokta gösterilecek")]
    public int trajectoryPoints = 30;
    
    [Tooltip("Noktalar arası zaman farkı")]
    public float timeStep = 0.05f;
    
    [Tooltip("Maksimum simülasyon süresi")]
    public float maxSimulationTime = 1.5f;

    [Header("Visual Settings")]
    [Tooltip("Başlangıç rengi")]
    public Color startColor = new Color(1f, 1f, 1f, 0.8f);
    
    [Tooltip("Bitiş rengi")]
    public Color endColor = new Color(1f, 1f, 1f, 0.2f);
    
    [Tooltip("Çizgi kalınlığı")]
    public float lineWidth = 0.1f;

    [Header("Physics")]
    [Tooltip("Yerçekimi değeri (Physics2D.gravity kullanılır)")]
    public bool usePhysicsGravity = true;
    
    [Tooltip("Manuel yerçekimi (usePhysicsGravity false ise)")]
    public Vector2 customGravity = new Vector2(0, -9.81f);

    private bool isShowing = false;

    void Awake()
    {
        // LineRenderer yoksa oluştur
        if (trajectoryLine == null)
        {
            trajectoryLine = GetComponent<LineRenderer>();
            
            if (trajectoryLine == null)
            {
                trajectoryLine = gameObject.AddComponent<LineRenderer>();
            }
        }
        
        SetupLineRenderer();
        HideTrajectory();
    }

    /// <summary>
    /// LineRenderer ayarlarını yap
    /// </summary>
    void SetupLineRenderer()
    {
        trajectoryLine.startWidth = lineWidth;
        trajectoryLine.endWidth = lineWidth * 0.5f;
        trajectoryLine.startColor = startColor;
        trajectoryLine.endColor = endColor;
        trajectoryLine.useWorldSpace = true;
        trajectoryLine.positionCount = 0;
        
        // Material ayarla (basit unlit)
        if (trajectoryLine.material == null || trajectoryLine.material.name == "Default-Line")
        {
            // Sprites-Default material kullan
            trajectoryLine.material = new Material(Shader.Find("Sprites/Default"));
        }
    }

    /// <summary>
    /// Yörüngeyi hesapla ve göster
    /// </summary>
    /// <param name="startPosition">Başlangıç pozisyonu</param>
    /// <param name="initialVelocity">Başlangıç hızı</param>
    public void ShowTrajectory(Vector2 startPosition, Vector2 initialVelocity)
    {
        if (trajectoryLine == null) return;
        
        isShowing = true;
        trajectoryLine.enabled = true;
        
        Vector2 gravity = usePhysicsGravity ? Physics2D.gravity : customGravity;
        
        // Noktaları hesapla
        Vector3[] points = CalculateTrajectoryPoints(startPosition, initialVelocity, gravity);
        
        trajectoryLine.positionCount = points.Length;
        trajectoryLine.SetPositions(points);
    }

    /// <summary>
    /// Yörünge noktalarını hesapla
    /// Kinematik formül: p = p0 + v0*t + 0.5*g*t^2
    /// </summary>
    Vector3[] CalculateTrajectoryPoints(Vector2 startPos, Vector2 velocity, Vector2 gravity)
    {
        int pointCount = Mathf.Min(trajectoryPoints, Mathf.CeilToInt(maxSimulationTime / timeStep));
        Vector3[] points = new Vector3[pointCount];
        
        for (int i = 0; i < pointCount; i++)
        {
            float t = i * timeStep;
            
            // Kinematik formül
            float x = startPos.x + velocity.x * t;
            float y = startPos.y + velocity.y * t + 0.5f * gravity.y * t * t;
            
            points[i] = new Vector3(x, y, 0);
            
            // Ekran dışına çıktıysa erken bitir
            if (IsOutOfBounds(points[i]))
            {
                // Array'i kısalt
                Vector3[] trimmedPoints = new Vector3[i + 1];
                System.Array.Copy(points, trimmedPoints, i + 1);
                return trimmedPoints;
            }
        }
        
        return points;
    }

    /// <summary>
    /// Nokta ekran sınırları dışında mı?
    /// </summary>
    bool IsOutOfBounds(Vector3 point)
    {
        Camera cam = Camera.main;
        if (cam == null) return false;
        
        Vector3 viewportPoint = cam.WorldToViewportPoint(point);
        
        // Viewport sınırları dışında mı? (biraz tolerans ekle)
        return viewportPoint.x < -0.5f || viewportPoint.x > 1.5f ||
               viewportPoint.y < -0.5f || viewportPoint.y > 1.5f;
    }

    /// <summary>
    /// Yörüngeyi gizle
    /// </summary>
    public void HideTrajectory()
    {
        isShowing = false;
        
        if (trajectoryLine != null)
        {
            trajectoryLine.enabled = false;
            trajectoryLine.positionCount = 0;
        }
    }

    /// <summary>
    /// Yörünge gösteriliyor mu?
    /// </summary>
    public bool IsShowing => isShowing;

    /// <summary>
    /// Çizgi rengini güce göre güncelle
    /// </summary>
    public void UpdateColorByPower(float powerRatio)
    {
        if (trajectoryLine == null) return;
        
        // Güce göre renk: yeşil -> sarı -> kırmızı
        Color powerColor;
        if (powerRatio < 0.5f)
        {
            powerColor = Color.Lerp(Color.green, Color.yellow, powerRatio * 2f);
        }
        else
        {
            powerColor = Color.Lerp(Color.yellow, Color.red, (powerRatio - 0.5f) * 2f);
        }
        
        trajectoryLine.startColor = new Color(powerColor.r, powerColor.g, powerColor.b, startColor.a);
        trajectoryLine.endColor = new Color(powerColor.r, powerColor.g, powerColor.b, endColor.a);
    }

    // Editor'da preview için
    void OnValidate()
    {
        if (trajectoryLine != null)
        {
            SetupLineRenderer();
        }
    }
}
