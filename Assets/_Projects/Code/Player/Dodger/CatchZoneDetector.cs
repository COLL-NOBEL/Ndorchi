using UnityEngine;

public class CatchZoneDetector : MonoBehaviour
{
    private bool isBallInZone = false;

    public bool IsBallInZone()
    {
        return isBallInZone;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            isBallInZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            isBallInZone = false;
        }
    }
}