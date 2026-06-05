using UnityEngine;
using TMPro; // TextMeshPro kullanmak için bu kütüphane şart!

public class TargetBox : MonoBehaviour
{
    [Header("Ayarlar")]
    public string targetTag = "Player";
    public float requiredTime = 3.0f;

    [Header("UI Bağlantısı")]
    public TextMeshProUGUI countdownText; // Ekrana koyduğumuz Text'i buraya sürükleyeceğiz

    [Header("Debug")]
    public float currentTimer = 0f;
    public bool isBallInside = false;

    private void Update()
    {
        if (isBallInside)
        {
            // Zamanı artır
            currentTimer += Time.deltaTime;

            // Geriye sayımı hesapla (Kalan Süre)
            float timeLeft = requiredTime - currentTimer;

            // UI GÜNCELLEME
            if (countdownText != null)
            {
                // Nesneyi görünür yap
                countdownText.gameObject.SetActive(true);

                // Kalan süreyi yukarı yuvarla (Örn: 2.1 saniye ise ekranda 3 yazar)
                // Mathf.CeilToInt: 0'dan büyükse bir üst sayıya yuvarlar
                countdownText.text = Mathf.CeilToInt(timeLeft).ToString();
            }

            // Süre doldu mu?
            if (currentTimer >= requiredTime)
            {
                WinGame();
            }
        }
        else
        {
            // Top dışarıdaysa sayacı sıfırla ve yazıyı gizle
            currentTimer = 0f;
            
            if (countdownText != null)
            {
                countdownText.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            isBallInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            isBallInside = false;
            // Çıktığı an yazıyı hemen gizle
            if (countdownText != null) countdownText.gameObject.SetActive(false);
        }
    }

    private void WinGame()
    {
        if (!this.enabled) return;

        // Kazanma anında "0" veya "Kazandın!" yazabilirsin
        if (countdownText != null) countdownText.text = "WIN!";

        Debug.Log("KAZANDIN!");
        GameManager.Instance.OnLevelComplete();
        this.enabled = false;
    }
}