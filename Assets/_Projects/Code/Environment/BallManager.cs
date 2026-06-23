using UnityEngine;

public class BallManager : MonoBehaviour
{
    public static BallManager Instance { get; private set; }
    
    [Header("Ball Reference")]
    public GameObject ballObject;
    
    [Header("Shooter Turn Tracking")]
    public GameObject currentShooterWithBall; // Which shooter currently has/should have the ball
    public bool isLeftShooterTurn = true; // Alternates between left and right shooter
    
    [Header("Debug")]
    public bool showDebugMessages = true;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        // Find ball if not assigned
        if (ballObject == null)
        {
            ballObject = GameObject.FindWithTag("Ball");
        }
    }
    
    private void Start()
    {
        // Give ball to left shooter initially
        AssignBallToShooter(true);
    }
    
    // Call this after a shooter shoots the ball
    public void OnShooterShot()
    {
        // Switch turn to the other shooter
        isLeftShooterTurn = !isLeftShooterTurn;
        
        if (showDebugMessages)
            Debug.Log($"🔄 Ball turn switched to: {(isLeftShooterTurn ? "LEFT" : "RIGHT")} shooter");
    }
    
    // Call this when ball reaches the other shooter's zone
    public void OnBallReachOtherShooter(GameObject shooter)
    {
        // Give ball to this shooter
        if (shooter != null)
        {
            DodgerCatch catcher = shooter.GetComponent<DodgerCatch>();
            if (catcher != null && ballObject != null)
            {
                catcher.SecureBall(ballObject);
                currentShooterWithBall = shooter;
                
                if (showDebugMessages)
                    Debug.Log($"🏐 Ball secured by {shooter.name}");
            }
        }
    }
    
    // After elimination, ensure ball goes to correct shooter
    public void AssignBallToShooter(bool giveToLeftShooter)
    {
        if (SpawnManager.Instance == null)
        {
            Debug.LogError("❌ BallManager: SpawnManager.Instance is null!");
            return;
        }
        
        // Find shooters by tag - this is reliable
        GameObject[] shooters = GameObject.FindGameObjectsWithTag("Shooter");
        
        if (shooters.Length == 0)
        {
            Debug.LogError("❌ BallManager: No shooters found with tag 'Shooter'!");
            return;
        }
        
        // Sort by X position: leftmost first
        System.Array.Sort(shooters, (a, b) => a.transform.position.x.CompareTo(b.transform.position.x));
        
        // Pick left or right shooter
        GameObject targetShooter = giveToLeftShooter ? shooters[0] : shooters[shooters.Length - 1];
        
        if (targetShooter != null && ballObject != null)
        {
            DodgerCatch catcher = targetShooter.GetComponent<DodgerCatch>();
            if (catcher != null)
            {
                catcher.SecureBall(ballObject);
                currentShooterWithBall = targetShooter;
                Debug.Log($"🏐 Ball assigned to {targetShooter.name}");
            }
        }
    }
    
    // Reset ball to center after elimination
    public void ResetBallToCenter()
    {
        if (ballObject != null)
        {
            Rigidbody ballRb = ballObject.GetComponent<Rigidbody>();
            if (ballRb != null)
            {
                ballRb.linearVelocity = Vector3.zero;
                ballRb.angularVelocity = Vector3.zero;
            }
            
            BallCollision ballCol = ballObject.GetComponent<BallCollision>();
            if (ballCol != null)
            {
                ballCol.ballShotByShooter = false;
            }
            
            ballObject.transform.position = new Vector3(0, 1.0f, 0);
            
            if (!ballObject.activeSelf)
            {
                ballObject.SetActive(true);
            }
            
            // Assign to shooter
            AssignBallToShooter(isLeftShooterTurn);
        }
    }
}