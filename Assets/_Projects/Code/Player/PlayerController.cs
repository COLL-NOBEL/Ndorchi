using UnityEngine;
using UnityEngine.InputSystem; // Must have this for the new system!

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    
    private Rigidbody rb;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 1. Get a reference to the Input Action asset attached to this player
        var playerInput = GetComponent<PlayerInput>();

        // 2. Disable EVERYTHING first (The "Clean Slate")
        playerInput.actions.Disable();

        // 3. Enable ONLY the specific map we want to use for this player
        // Replace "Player" with whatever you named your Action Map in Task 1.1
        playerInput.actions.FindActionMap("Dodger").Enable();
    }

    // This function is called by the "Player Input" component automatically
    // Make sure the Message in Player Input is set to "Send Messages"
    void OnMove(InputValue value)
    {
        // Store the WASD input (Vector2 has X and Y)
        moveInput = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        // We use FixedUpdate for Physics/Rigidbody movement
        // We move on the X and Z axis (floor), not the Y (up)
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        
        // Apply velocity directly for snappy, athletic movement
        rb.linearVelocity = moveDirection * moveSpeed;
    }
}