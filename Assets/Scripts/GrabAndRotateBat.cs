using UnityEngine;

public class GrabAndRotateBat : MonoBehaviour
{
    private Rigidbody2D rb;
    private Camera cam;
    
    // Sopayı tutuyor muyuz?
    private bool isHolding = false;

    [Header("Ayarlar")]
    public float rotationSpeed = 30f; // Dönüş yumuşaklığı
    public float grabRadius = 1.5f;   // Sopayı ne kadar yakından tutabilirsin?
    public float angleOffset = -90f;  // Sopa resmin dikse -90, yataysa 0 yap.

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        
        // Fizik ayarları
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.useFullKinematicContacts = false;
    }

    void Update()
    {
        // 1. Mouse/Parmak Pozisyonunu Al
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // 2. Tıklama Kontrolü (SADECE TIKLAYINCA)
        if (Input.GetMouseButtonDown(0))
        {
            // Mouse, sopanın pivot noktasına yeterince yakın mı?
            float distance = Vector2.Distance(transform.position, mousePos);
            
            if (distance <= grabRadius)
            {
                isHolding = true;
                Debug.Log("Sopa Tutuldu!");
            }
        }

        // 3. Bırakma Kontrolü
        if (Input.GetMouseButtonUp(0))
        {
            isHolding = false;
            Debug.Log("Sopa Bırakıldı.");
        }
    }

    void FixedUpdate()
    {
        if (isHolding)
        {
            RotateBat();
        }
    }

    void RotateBat()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        
        // Pivot noktasından Mouse'a olan yönü bul
        Vector2 direction = mousePos - transform.position;

        // Yön vektöründen açıyı hesapla (Atan2 fonksiyonu)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Ofseti ekle (Sopa görselinin yönüne göre düzeltme)
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle + angleOffset);

        // Fiziksel olarak döndür (MoveRotation)
        // Lerp kullanarak biraz yumuşatıyoruz ki titreme yapmasın
        rb.MoveRotation(Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
    }

    // Sahne ekranında tutma alanını çiz (Gizmos)
    // Böylece oyun çalışmazken de ne kadar yakından tutman gerektiğini görürsün
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, grabRadius);
    }
}