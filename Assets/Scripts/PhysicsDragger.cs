using UnityEngine;

public class PhysicsDragger : MonoBehaviour
{
    private TargetJoint2D targetJoint;
    private Camera cam;
    private Rigidbody2D rb;

    [Header("Tutma Ayarları")]
    public bool onlyGrabHandle = true; // Sadece saptan mı tutulsun?
    public Transform handlePosition;   // Sopanın sapının olduğu yer (Pivot)
    public float handleRadius = 1.0f;  // Sapı tutma hassasiyeti

    void Start()
    {
        targetJoint = GetComponent<TargetJoint2D>();
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;

        // Başlangıçta Joint kapalı olsun (Sopa serbest düşsün veya dursun)
        targetJoint.enabled = false;
    }

    void Update()
    {
        // Mouse Pozisyonunu Al
        Vector3 worldMousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        worldMousePos.z = 0;

        // TIKLAMA ANI (TUTMA)
        if (Input.GetMouseButtonDown(0))
        {
            // Eğer sadece saptan tutulsun istiyorsan mesafeyi kontrol et
            if (onlyGrabHandle && handlePosition != null)
            {
                float distance = Vector2.Distance(worldMousePos, handlePosition.position);
                if (distance > handleRadius) return; // Saptan uzaktaysa tutma
            }

            // Joint'i aktif et
            targetJoint.enabled = true;
            
            // Sopanın neresinden tuttuysak orayı çapa (anchor) yap
            // (Eğer hep saptan tutulsun istiyorsan burayı sabitleyebiliriz)
            targetJoint.anchor = transform.InverseTransformPoint(worldMousePos);
        }

        // TIKLAMA BIRAKMA (FIRLATMA/SALMA)
        else if (Input.GetMouseButtonUp(0))
        {
            targetJoint.enabled = false;
        }

        // SÜRÜKLEME (SAVURMA)
        if (targetJoint.enabled)
        {
            // Joint'in hedefini mouse pozisyonuna güncelle
            targetJoint.target = worldMousePos;
        }
    }

    // Sahne görünümünde tutma alanını çiz
    void OnDrawGizmosSelected()
    {
        if (handlePosition != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(handlePosition.position, handleRadius);
        }
    }
}