using UnityEngine;

public class CatchZoneDetector : MonoBehaviour
{
    [Header("Debug")]
    public bool showDebugMessages = true;
    
    private bool isBallInZone = false;
    private GameObject ballInZone;

    public bool IsBallInZone()
    {
        // Verify ball still exists
        if (ballInZone == null)
        {
            isBallInZone = false;
        }
        return isBallInZone;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            isBallInZone = true;
            ballInZone = other.gameObject;
            if (showDebugMessages)
                Debug.Log($"🎯 Ball ENTERED catch zone of {transform.parent.name}");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            if (!isBallInZone)
            {
                isBallInZone = true;
                ballInZone = other.gameObject;
                if (showDebugMessages)
                    Debug.Log($"🎯 Ball in catch zone of {transform.parent.name}");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            isBallInZone = false;
            ballInZone = null;
            if (showDebugMessages)
                Debug.Log($"🚫 Ball LEFT catch zone of {transform.parent.name}");
        }
    }

    // Visual feedback in Scene view
    private void OnDrawGizmos()
    {
        if (isBallInZone)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }
    }
}