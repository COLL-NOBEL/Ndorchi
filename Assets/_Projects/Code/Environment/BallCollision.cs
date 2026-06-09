using UnityEngine;

public class BallCollision : MonoBehaviour
{
    [Header("Projectile State Tracker")]
    public bool ballShotByShooter = false;

    private void Start()
    {
        // Initially the ball is safe and harmless
        ballShotByShooter = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // If it hits a wall, it loses its lethal power
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Ball hit the wall.");
            ballShotByShooter = false; 
            return;
        }

        if (collision.gameObject.CompareTag("Dodger"))
        {
            DodgerCatch dodger = collision.gameObject.GetComponent<DodgerCatch>();

            if (dodger != null)
            {
                // SAFETY 1: If the Dodger just dropped/threw the ball, ignore collision tracking
                if (dodger.IsImmune()) return; 

                // SAFETY 2: If the Dodger times their button press perfectly
                if (dodger.IsCurrentlyCatching())
                {
                    // Catching the ball disarms it safely!
                    ballShotByShooter = false;
                    dodger.SecureBall(this.gameObject);
                    return; 
                }
            }

            // --- CRITICAL HIT DETERMINATION ---
            // The Dodger only gets eliminated if the ball was actively fired by a shooter!
            if (ballShotByShooter)
            {
                Debug.Log("💥 ELIMINATION HIT! The Dodger was struck by a live shot!");
                
                // Disarm the projectile immediately
                ballShotByShooter = false;
                GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
                GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

                if (GameManager.Instance != null)
                {
                    // 1. Process data list math tracking
                    GameManager.Instance.EliminateActiveDodger();
                    
                    // 2. Check if there are more players left to cycle into the match
                    if (GameManager.Instance.HasRemainingDodgers())
                    {
                        Debug.Log("🔄 Spawning next active Dodger from the bench lineup...");
                        if (SpawnManager.Instance != null)
                        {
                            // Call the isolated Dodger-only respawn system
                            SpawnManager.Instance.RespawnOnlyDodger();
                        }
                    }
                    else
                    {
                        Debug.Log("🚫 All 5 Dodgers eliminated! Team is wiped out. Triggering Swap...");
                        
                        // Task 4.5 Hook: Fire off the swap sequence automatically
                        GameManager.Instance.TriggerTeamSwap();
                    }
                }
            }
            else
            {
                Debug.Log("🥎 The ball bumped into the Dodger, but it wasn't shot by a Shooter. No penalty!");
            }
        }
    }
}