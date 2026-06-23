using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 12f;
    public bool rotateToMovementDirection = true;
    
    [Header("Debug")]
    public bool showDebugMessages = true;
    
    private Rigidbody rb;
    private Vector3 lastMovementDirection;
    private bool isMoving = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        if (rb == null)
        {
            Debug.LogError($"❌ {gameObject.name}: PlayerRotation requires a Rigidbody component!");
            enabled = false;
            return;
        }
        
        // Set initial forward direction
        lastMovementDirection = transform.forward;
        
        if (showDebugMessages)
            Debug.Log($"🔄 {gameObject.name}: PlayerRotation initialized");
    }
    
    void Update()
    {
        if (!rotateToMovementDirection || rb == null) return;
        
        RotateTowardsMovement();
    }
    
    private void RotateTowardsMovement()
    {
        // Get horizontal velocity (ignore Y axis for rotation)
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        
        // Check if player is actually moving
        isMoving = horizontalVelocity.magnitude > 0.1f;
        
        if (isMoving)
        {
            // Player is moving - rotate towards movement direction
            lastMovementDirection = horizontalVelocity.normalized;
            
            // Calculate target rotation
            Quaternion targetRotation = Quaternion.LookRotation(lastMovementDirection);
            
            // Smoothly rotate
            transform.rotation = Quaternion.Slerp(
                transform.rotation, 
                targetRotation, 
                rotationSpeed * Time.deltaTime
            );
            
            if (showDebugMessages && Time.frameCount % 60 == 0) // Log every 60 frames
            {
                Debug.Log($"🔄 {gameObject.name}: Rotating towards movement - Direction: {lastMovementDirection}");
            }
        }
        else
        {
            // Player stopped - keep facing last movement direction
            if (lastMovementDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lastMovementDirection);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * 0.5f * Time.deltaTime // Slower when stopped
                );
            }
        }
    }
    
    // Visual debugging in Scene view
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        
        if (rb != null)
        {
            // Draw movement direction
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (horizontalVelocity.magnitude > 0.1f)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position, horizontalVelocity.normalized * 2f);
            }
            
            // Draw facing direction
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, transform.forward * 2f);
        }
    }
}