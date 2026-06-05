using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Waypoint'ler arasında hareket eden platform.
/// </summary>
public class MovingPlatform : LevelItem
{
    [Header("Movement Settings")]
    [Tooltip("Hareket hızı")]
    public float moveSpeed = 2f;
    
    [Tooltip("Waypoint'lerde bekleme süresi")]
    public float waitTime = 0.5f;
    
    [Tooltip("Döngü mü? (False = sona ulaşınca durur)")]
    public bool loop = true;
    
    [Tooltip("Ping-pong mu? (Geri gel)")]
    public bool pingPong = true;

    [Header("Waypoints")]
    [Tooltip("Hareket noktaları")]
    public Transform[] waypoints;
    
    [Tooltip("Local offset mi? (True = waypoint pozisyonları platform'a göre)")]
    public bool useLocalWaypoints = false;

    [Header("Ball Interaction")]
    [Tooltip("Ball'ı taşı (parent yap)")]
    public bool carryBall = true;

    [Header("Easing")]
    [Tooltip("Hareket eğrisi")]
    public AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // Internal
    private int currentWaypointIndex = 0;
    private int direction = 1;
    private Vector3[] globalWaypoints;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    private float journeyLength;
    private float journeyProgress;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Transform originalBallParent;
    private List<Transform> objectsOnPlatform = new List<Transform>();

    protected override void Awake()
    {
        base.Awake();
        
        // Global waypoint pozisyonlarını hesapla
        if (waypoints != null && waypoints.Length > 0)
        {
            globalWaypoints = new Vector3[waypoints.Length];
            
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] != null)
                {
                    globalWaypoints[i] = useLocalWaypoints 
                        ? transform.position + waypoints[i].localPosition 
                        : waypoints[i].position;
                }
                else
                {
                    globalWaypoints[i] = transform.position;
                }
            }
            
            SetupNextMovement();
        }
    }

    protected override void Update()
    {
        base.Update();
        
        if (!isActive || globalWaypoints == null || globalWaypoints.Length < 2) return;
        
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                isWaiting = false;
                MoveToNextWaypoint();
            }
            return;
        }
        
        // Hareketi hesapla
        journeyProgress += (moveSpeed / journeyLength) * Time.deltaTime;
        
        if (journeyProgress >= 1f)
        {
            // Hedefe ulaştı
            transform.position = targetPosition;
            OnReachedWaypoint();
        }
        else
        {
            // Hareket et
            float t = movementCurve.Evaluate(journeyProgress);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
        }
    }

    private void SetupNextMovement()
    {
        if (globalWaypoints.Length < 2) return;
        
        startPosition = transform.position;
        targetPosition = globalWaypoints[currentWaypointIndex];
        journeyLength = Vector3.Distance(startPosition, targetPosition);
        journeyProgress = 0f;
        
        if (journeyLength < 0.01f)
        {
            journeyLength = 0.01f; // Division by zero önle
        }
    }

    private void MoveToNextWaypoint()
    {
        currentWaypointIndex += direction;
        
        // Sınır kontrolü
        if (currentWaypointIndex >= globalWaypoints.Length)
        {
            if (pingPong)
            {
                direction = -1;
                currentWaypointIndex = globalWaypoints.Length - 2;
            }
            else if (loop)
            {
                currentWaypointIndex = 0;
            }
            else
            {
                currentWaypointIndex = globalWaypoints.Length - 1;
                isActive = false;
                return;
            }
        }
        else if (currentWaypointIndex < 0)
        {
            if (pingPong)
            {
                direction = 1;
                currentWaypointIndex = 1;
            }
            else if (loop)
            {
                currentWaypointIndex = globalWaypoints.Length - 1;
            }
            else
            {
                currentWaypointIndex = 0;
                isActive = false;
                return;
            }
        }
        
        SetupNextMovement();
    }

    private void OnReachedWaypoint()
    {
        if (waitTime > 0)
        {
            isWaiting = true;
            waitTimer = waitTime;
        }
        else
        {
            MoveToNextWaypoint();
        }
        
        PlayActivationFeedback();
    }

    protected override void OnBallEnter(Rigidbody2D ball)
    {
        if (carryBall)
        {
            originalBallParent = ball.transform.parent;
            ball.transform.SetParent(transform);
            
            if (!objectsOnPlatform.Contains(ball.transform))
            {
                objectsOnPlatform.Add(ball.transform);
            }
        }
    }

    protected override void OnBallExit(Rigidbody2D ball)
    {
        if (carryBall)
        {
            ball.transform.SetParent(originalBallParent);
            objectsOnPlatform.Remove(ball.transform);
        }
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length < 2) return;
        
        // Waypoint'leri çiz
        Gizmos.color = Color.cyan;
        
        Vector3 prevPos = transform.position;
        
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;
            
            Vector3 pos = useLocalWaypoints 
                ? transform.position + waypoints[i].localPosition 
                : waypoints[i].position;
            
            // Nokta
            Gizmos.DrawWireSphere(pos, 0.2f);
            
            // Numara
#if UNITY_EDITOR
            UnityEditor.Handles.Label(pos + Vector3.up * 0.3f, i.ToString());
#endif
            
            // Çizgi
            if (i > 0 || loop)
            {
                Gizmos.DrawLine(prevPos, pos);
            }
            
            prevPos = pos;
        }
        
        // Loop ise başa bağla
        if (loop && !pingPong && waypoints.Length > 0 && waypoints[0] != null)
        {
            Vector3 firstPos = useLocalWaypoints 
                ? transform.position + waypoints[0].localPosition 
                : waypoints[0].position;
            Gizmos.DrawLine(prevPos, firstPos);
        }
    }
}
