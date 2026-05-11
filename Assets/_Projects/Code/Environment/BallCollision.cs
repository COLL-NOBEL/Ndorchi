using UnityEngine;

public class BallCollision : MonoBehaviour
{
    // This runs automatically whenever the ball hits something solid
    private void OnCollisionEnter(Collision collision)
    {
        // 1. Check if the object we hit has the "Dodger" tag
        if (collision.gameObject.CompareTag("Dodger"))
        {
            Debug.Log("HIT! The Dodger is out!");
            
            // For now, let's "eliminate" the player by hiding them
            // In Phase 4, we will replace this with a proper Game Manager
            collision.gameObject.SetActive(false);
            
            // Optional: Reset the ball's speed so it doesn't keep flying
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            // You can add logic here for when the ball hits a wall
            Debug.Log("Ball hit the wall.");
        }
    }
}