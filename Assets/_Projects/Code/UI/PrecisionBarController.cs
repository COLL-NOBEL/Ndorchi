using UnityEngine;
using UnityEngine.UI;

public class PrecisionBarController : MonoBehaviour
{
    [Header("UI References")]
    public Slider precisionSlider;

    [Header("Settings")]
    public float oscillationSpeed = 2f; 
    // Set this to true so it starts moving immediately
    public bool isActive = true; 

    // Removed the unused 'timer' variable to fix the warning

    public float GetAccuracyOffset()
    {
        return precisionSlider.value - 0.5f; 
    }

    void Update()
    {
        if (isActive)
        {
            UpdateNeedlePosition();
        }
    }

    void UpdateNeedlePosition()
    {
        // Using Time.time works perfectly for continuous movement
        float newValue = Mathf.PingPong(Time.time * oscillationSpeed, 1f);
        precisionSlider.value = newValue;
    }

    public void StartOscillating(float speed)
    {
        oscillationSpeed = speed;
        isActive = true;
    }

    public void StopOscillating()
    {
        // For your current testing, we will keep this function 
        // but we won't call it from the BallLauncher yet.
        isActive = false;
    }
}