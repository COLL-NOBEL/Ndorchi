using UnityEngine;
using UnityEngine.InputSystem;

public class BallLauncher : MonoBehaviour
{
    public float throwForce = 20f;
    public Transform launchPoint; // Drag your LaunchPoint object here
    
    private GameObject ball;
    private Rigidbody ballRb;

    void Start()
    {
        // Find the ball in the scene using the tag we made in Task 2.2
        ball = GameObject.FindWithTag("Ball");
        
        if (ball != null)
        {
            ballRb = ball.GetComponent<Rigidbody>();
        }
        else
        {
            Debug.LogError("No object with the tag 'Ball' found in the scene!");
        }
    }

    // This is called by the Player Input component (Message: OnThrow)
    void OnThrow()
    {
        if (ball == null || ballRb == null) return;

        // 1. Reset ball physics (stop it from rolling/moving)
        ballRb.linearVelocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;

        // 2. Teleport ball to the launch point
        ball.transform.position = launchPoint.position;

        // 3. Apply force forward relative to the player
        // We use Impulse for an immediate "kick"
        ballRb.AddForce(transform.forward * throwForce, ForceMode.Impulse);
        
        Debug.Log("Ball Launched!");
    }
}