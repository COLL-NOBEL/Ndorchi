using UnityEngine;

public class DodgeSensor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            Debug.Log("💨 DODGE! +1 Point for the Dodger!");
            
            // Task 4.3 Hook: Award 1 point to the active dodging team
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScoreToActiveDodgers(1);
            }
        }
    }
}