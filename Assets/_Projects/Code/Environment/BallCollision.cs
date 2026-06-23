using UnityEngine;

public class BallCollision : MonoBehaviour
{
    [Header("Projectile State Tracker")]
    public bool ballShotByShooter = false;
    
    [Header("Debug")]
    public bool showDebugMessages = true;

    private void Start()
    {
        ballShotByShooter = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Ignore collisions with the shooter who just threw the ball if ball isn't armed
        if (collision.gameObject.CompareTag("Shooter") && !ballShotByShooter)
        {
            return;
        }

        if (showDebugMessages)
            Debug.Log($"🏐 Ball collided with: {collision.gameObject.name} | Tag: {collision.gameObject.tag} | Armed: {ballShotByShooter}");

        // Hit wall - disarm ball
        if (collision.gameObject.CompareTag("Wall"))
        {
            if (showDebugMessages) Debug.Log("🧱 Ball hit wall - disarmed");
            ballShotByShooter = false;
            return;
        }

        // Hit a dodger
        if (collision.gameObject.CompareTag("Dodger"))
        {
            HandleDodgerHit(collision.gameObject);
        }
        
        // Hit the OTHER shooter (ball crossed the field)
        if (collision.gameObject.CompareTag("Shooter") && ballShotByShooter)
        {
            if (showDebugMessages) Debug.Log("🎯 Armed ball reached OTHER shooter zone!");
            ballShotByShooter = false;
            
            // Notify BallManager that ball reached other shooter
            if (BallManager.Instance != null)
            {
                BallManager.Instance.OnBallReachOtherShooter(collision.gameObject);
            }
        }
    }

    private void HandleDodgerHit(GameObject dodgerObject)
    {
        // Get the dodger catch component
        DodgerCatch dodgerCatch = dodgerObject.GetComponent<DodgerCatch>();
        
        // If dodger has catch component
        if (dodgerCatch != null)
        {
            // Check immunity first
            if (dodgerCatch.IsImmune())
            {
                if (showDebugMessages) Debug.Log("🛡️ Dodger is immune - ignoring collision");
                return;
            }

            // Check if dodger is actively catching
            if (dodgerCatch.IsCurrentlyCatching())
            {
                if (showDebugMessages) Debug.Log("🤲 Dodger CAUGHT the ball!");
                ballShotByShooter = false;
                dodgerCatch.SecureBall(this.gameObject);
                return;
            }
        }

        // If ball is armed, eliminate dodger
        if (ballShotByShooter)
        {
            if (showDebugMessages) Debug.Log("💥 ELIMINATION! Dodger hit by active shot!");
            
            // Stop ball immediately
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            ballShotByShooter = false;

            // Process elimination
            if (GameManager.Instance != null)
            {
                GameManager.Instance.EliminateActiveDodger();
            }
            
            // Reset ball to center and give to correct shooter
            if (BallManager.Instance != null)
            {
                BallManager.Instance.ResetBallToCenter();
            }
        }
        else
        {
            if (showDebugMessages) Debug.Log("🥎 Ball touched dodger but not armed - no penalty");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dodger_Zone"))
        {
            if (showDebugMessages) Debug.Log("💨 Ball passed through DODGE ZONE!");
            
            if (ScoreManager.Instance != null && ballShotByShooter)
            {
                ScoreManager.Instance.AddScoreToActiveDodgers(1);
            }
        }
    }
}