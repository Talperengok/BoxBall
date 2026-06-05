using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Creates smooth scrolling background particles or shapes for visual appeal.
/// Attach to a panel or canvas for ambient animation.
/// </summary>
public class BackgroundAnimator : MonoBehaviour
{
    [Header("Floating Shapes")]
    [SerializeField] private GameObject shapePrefab;
    [SerializeField] private int shapeCount = 10;
    [SerializeField] private float minSize = 20f;
    [SerializeField] private float maxSize = 80f;
    [SerializeField] private float minSpeed = 10f;
    [SerializeField] private float maxSpeed = 30f;
    [SerializeField] private float minAlpha = 0.1f;
    [SerializeField] private float maxAlpha = 0.3f;

    [Header("Gradient Background")]
    [SerializeField] private bool animateGradient = false;
    [SerializeField] private Image gradientImage;
    [SerializeField] private Color gradientColor1 = new Color(0.1f, 0.1f, 0.2f);
    [SerializeField] private Color gradientColor2 = new Color(0.2f, 0.1f, 0.3f);
    [SerializeField] private float gradientSpeed = 0.5f;

    private RectTransform rectTransform;
    private FloatingShape[] shapes;

    private class FloatingShape
    {
        public RectTransform transform;
        public Image image;
        public float speed;
        public float rotationSpeed;
        public Vector2 direction;
    }

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        
        if (shapePrefab != null)
        {
            CreateFloatingShapes();
        }
    }

    private void CreateFloatingShapes()
    {
        shapes = new FloatingShape[shapeCount];
        
        for (int i = 0; i < shapeCount; i++)
        {
            GameObject shapeObj = Instantiate(shapePrefab, transform);
            RectTransform shapeRect = shapeObj.GetComponent<RectTransform>();
            Image shapeImage = shapeObj.GetComponent<Image>();

            // Random size
            float size = Random.Range(minSize, maxSize);
            shapeRect.sizeDelta = new Vector2(size, size);

            // Random position
            float x = Random.Range(-rectTransform.rect.width / 2, rectTransform.rect.width / 2);
            float y = Random.Range(-rectTransform.rect.height / 2, rectTransform.rect.height / 2);
            shapeRect.anchoredPosition = new Vector2(x, y);

            // Random alpha
            if (shapeImage != null)
            {
                Color color = shapeImage.color;
                color.a = Random.Range(minAlpha, maxAlpha);
                shapeImage.color = color;
            }

            shapes[i] = new FloatingShape
            {
                transform = shapeRect,
                image = shapeImage,
                speed = Random.Range(minSpeed, maxSpeed),
                rotationSpeed = Random.Range(-30f, 30f),
                direction = new Vector2(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)
                ).normalized
            };
        }
    }

    private void Update()
    {
        // Animate floating shapes
        if (shapes != null)
        {
            foreach (var shape in shapes)
            {
                if (shape.transform == null) continue;

                // Move
                shape.transform.anchoredPosition += shape.direction * shape.speed * Time.deltaTime;

                // Rotate
                shape.transform.Rotate(0, 0, shape.rotationSpeed * Time.deltaTime);

                // Wrap around screen
                WrapPosition(shape);
            }
        }

        // Animate gradient
        if (animateGradient && gradientImage != null)
        {
            float t = (Mathf.Sin(Time.time * gradientSpeed) + 1f) / 2f;
            gradientImage.color = Color.Lerp(gradientColor1, gradientColor2, t);
        }
    }

    private void WrapPosition(FloatingShape shape)
    {
        Vector2 pos = shape.transform.anchoredPosition;
        float halfWidth = rectTransform.rect.width / 2 + shape.transform.sizeDelta.x;
        float halfHeight = rectTransform.rect.height / 2 + shape.transform.sizeDelta.y;

        if (pos.x > halfWidth) pos.x = -halfWidth;
        else if (pos.x < -halfWidth) pos.x = halfWidth;

        if (pos.y > halfHeight) pos.y = -halfHeight;
        else if (pos.y < -halfHeight) pos.y = halfHeight;

        shape.transform.anchoredPosition = pos;
    }
}
