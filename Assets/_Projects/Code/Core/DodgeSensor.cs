using UnityEngine;

public class DodgeSensor : MonoBehaviour
{
    // This runs when an object passes through the invisible wall
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering is the Ball
        if (other.CompareTag("Ball"))
        {
            Debug.Log("💨 DODGE! +1 Point for the Dodger!");
            
            // Optional: You could add logic here to slow the ball down
            // or reset it for the next round.
        }
    }
}