using UnityEngine;

/// <summary>
/// Tüm level item'leri için temel abstract class.
/// Her item bu sınıftan türetilmeli.
/// </summary>
public abstract class LevelItem : MonoBehaviour
{
    [Header("Base Settings")]
    [Tooltip("Item aktif mi?")]
    public bool isActive = true;
    
    [Tooltip("Item'in tekrar tetiklenebilmesi için gereken süre")]
    public float cooldownTime = 0f;
    
    [Tooltip("Ball ile etkileşim için gereken tag")]
    public string ballTag = "Player";

    [Header("Visual Feedback")]
    [Tooltip("Tetiklendiğinde gösterilecek efekt")]
    public GameObject activationEffect;
    
    [Tooltip("Tetiklendiğinde çalınacak ses")]
    public AudioClip activationSound;

    // Internal state
    protected bool isOnCooldown = false;
    protected float cooldownTimer = 0f;
    protected Rigidbody2D ballRigidbody;
    protected AudioSource audioSource;

    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && activationSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    protected virtual void Update()
    {
        // Cooldown timer
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false;
                cooldownTimer = 0f;
            }
        }
    }

    /// <summary>
    /// Ball item'a girdiğinde çağrılır
    /// </summary>
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive || isOnCooldown) return;
        
        if (other.CompareTag(ballTag))
        {
            ballRigidbody = other.attachedRigidbody;
            if (ballRigidbody != null)
            {
                OnBallEnter(ballRigidbody);
            }
        }
    }

    /// <summary>
    /// Ball item içindeyken her frame çağrılır
    /// </summary>
    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        if (!isActive) return;
        
        if (other.CompareTag(ballTag))
        {
            if (ballRigidbody == null)
                ballRigidbody = other.attachedRigidbody;
                
            if (ballRigidbody != null)
            {
                OnBallStay(ballRigidbody);
            }
        }
    }

    /// <summary>
    /// Ball item'dan çıktığında çağrılır
    /// </summary>
    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(ballTag))
        {
            if (ballRigidbody != null)
            {
                OnBallExit(ballRigidbody);
            }
            ballRigidbody = null;
        }
    }

    /// <summary>
    /// Ball collision ile temas ettiğinde çağrılır
    /// </summary>
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isActive || isOnCooldown) return;
        
        if (collision.gameObject.CompareTag(ballTag))
        {
            ballRigidbody = collision.rigidbody;
            if (ballRigidbody != null)
            {
                OnBallCollision(collision, ballRigidbody);
            }
        }
    }

    // Override edilecek metodlar
    protected virtual void OnBallEnter(Rigidbody2D ball) { }
    protected virtual void OnBallStay(Rigidbody2D ball) { }
    protected virtual void OnBallExit(Rigidbody2D ball) { }
    protected virtual void OnBallCollision(Collision2D collision, Rigidbody2D ball) { }

    /// <summary>
    /// Cooldown başlatır
    /// </summary>
    protected void StartCooldown()
    {
        if (cooldownTime > 0)
        {
            isOnCooldown = true;
            cooldownTimer = cooldownTime;
        }
    }

    /// <summary>
    /// Aktivasyon efektlerini oynatır
    /// </summary>
    protected void PlayActivationFeedback()
    {
        if (activationEffect != null)
        {
            Instantiate(activationEffect, transform.position, Quaternion.identity);
        }

        if (activationSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(activationSound);
        }
    }
}
