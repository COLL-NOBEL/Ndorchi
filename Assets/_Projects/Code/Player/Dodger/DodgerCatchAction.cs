using UnityEngine;

public class DodgerCatchAction : MonoBehaviour
{
    [Header("Catch Parameters")]
    public float catchRadius = 2.5f;
    public Transform holdPoint; 
    public LayerMask ballLayer;

    private GameObject caughtBall;
    private Rigidbody ballRb;
    private bool isHoldingBall = false;

    public void ExecuteCatch()
    {
        if (isHoldingBall) return; 

        // Search area around player for the Ball object
        Collider[] objectsFound = Physics.OverlapSphere(transform.position, catchRadius, ballLayer);
        foreach (var col in objectsFound)
        {
            if (col.CompareTag("Ball"))
            {
                GrabBall(col.gameObject);
                break;
            }
        }
    }

    private void GrabBall(GameObject ballObj)
    {
        caughtBall = ballObj;
        ballRb = caughtBall.GetComponent<Rigidbody>();

        if (ballRb != null)
        {
            ballRb.isKinematic = true; // Lock physics while holding
            ballRb.linearVelocity = Vector3.zero;
        }

        // De-arm projectile risk entirely on a successful pass/catch intercept
        BallCollision bc = caughtBall.GetComponent<BallCollision>();
        if (bc != null) bc.ballShotByShooter = false;

        // Snap to hold position reference
        if (holdPoint != null)
        {
            caughtBall.transform.position = holdPoint.position;
            caughtBall.transform.SetParent(holdPoint);
        }
        else
        {
            caughtBall.transform.position = transform.position + transform.forward * 1.2f;
            caughtBall.transform.SetParent(transform);
        }

        isHoldingBall = true;
        Debug.Log($"✨ {gameObject.name} CAUGHT the ball safely!");
    }

    public void ExecuteDrop()
    {
        if (!isHoldingBall || caughtBall == null) return;

        // Release the ball's hierarchy link
        caughtBall.transform.SetParent(null);

        if (ballRb != null)
        {
            ballRb.isKinematic = false;
            ballRb.linearVelocity = Vector3.zero;
        }

        // Push it slightly forward onto ground space
        caughtBall.transform.position = transform.position + transform.forward * 1.5f;

        isHoldingBall = false;
        caughtBall = null;
        Debug.Log($"Released ball: {gameObject.name} DROPPED it directly onto field.");
    }

    public bool IsHoldingBall() => isHoldingBall;

    // Call this if the Dodger successfully executes a DodgerThrow out of their hand space
    public void ForceReleaseOnThrow()
    {
        if (caughtBall != null) caughtBall.transform.SetParent(null);
        isHoldingBall = false;
        caughtBall = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, catchRadius);
    }
}