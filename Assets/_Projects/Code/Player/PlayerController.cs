using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public enum MovementRole { Unassigned, Dodger, Shooter }
    
    [Header("Role Binding")]
    public MovementRole playerRole = MovementRole.Unassigned;

    [Header("Movement Settings")]
    public float moveSpeed = 8f;

    [Header("Debug")]
    public bool showDebugMessages = true;

    private Rigidbody rb;
    private GameControls inputActions;
    private Vector2 movementInput;
    
    private BallLauncher ballLauncher;
    private DodgerCatchAction catchComponent;
    private DodgerCatch dodgerCatch;
    private PlayerRotation playerRotation;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ballLauncher = GetComponent<BallLauncher>();
        catchComponent = GetComponent<DodgerCatchAction>();
        dodgerCatch = GetComponent<DodgerCatch>();
        inputActions = new GameControls();
        
        if (rb == null)
        {
            Debug.LogError($"❌ {gameObject.name}: No Rigidbody found! Player needs a Rigidbody component.");
            return;
        }
        
        // Get or add PlayerRotation component
        playerRotation = GetComponent<PlayerRotation>();
        if (playerRotation == null)
        {
            playerRotation = gameObject.AddComponent<PlayerRotation>();
            if (showDebugMessages) 
                Debug.Log($"➕ {gameObject.name}: PlayerRotation component added automatically");
        }
        
        // Configure Rigidbody for rotation
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        // Don't freeze Y rotation - we want to rotate on Y axis
    }
    

    private void OnEnable()
    {
        ConfigureInputHandling();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
        if (inputActions != null) inputActions.Disable();
    }

    public void AssignRole(MovementRole newRole)
    {
        if (showDebugMessages) 
            Debug.Log($"👤 {gameObject.name} assigned role: {newRole}");
        
        UnsubscribeFromEvents();
        playerRole = newRole;
        ConfigureInputHandling();
    }

    private void ConfigureInputHandling()
    {
        if (inputActions == null) return;
        inputActions.Disable();

        if (playerRole == MovementRole.Dodger)
        {
            // WASD keys for dodger movement
            inputActions.Dodger.Move.performed += OnDodgerMovePerformed;
            inputActions.Dodger.Move.canceled += OnDodgerMoveCanceled;
            
            // Dodger actions
            inputActions.Dodger.Catch.started += OnCatchInputTriggered;
            inputActions.Dodger.Drop.started += OnDropInputTriggered;

            inputActions.Dodger.Enable();
            
            if (showDebugMessages) 
                Debug.Log($"🎮 {gameObject.name}: DODGER controls - WASD to move, E to catch, Q to drop");
        }
        else if (playerRole == MovementRole.Shooter)
        {
            // Arrow keys for shooter movement
            inputActions.Shooter.Move.performed += OnShooterMovePerformed;
            inputActions.Shooter.Move.canceled += OnShooterMoveCanceled;
            
            inputActions.Shooter.Enable();
            
            if (showDebugMessages) 
                Debug.Log($"🎮 {gameObject.name}: SHOOTER controls - Arrow keys to move, SPACE to charge/shoot");
        }

        // Register ball launcher inputs
        if (ballLauncher != null)
        {
            ballLauncher.RegisterInputs(playerRole);
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (inputActions == null) return;

        // Unsubscribe Dodger events
        inputActions.Dodger.Move.performed -= OnDodgerMovePerformed;
        inputActions.Dodger.Move.canceled -= OnDodgerMoveCanceled;
        inputActions.Dodger.Catch.started -= OnCatchInputTriggered;
        inputActions.Dodger.Drop.started -= OnDropInputTriggered;

        // Unsubscribe Shooter events
        inputActions.Shooter.Move.performed -= OnShooterMovePerformed;
        inputActions.Shooter.Move.canceled -= OnShooterMoveCanceled;
        
        // Unregister ball launcher
        if (ballLauncher != null) 
            ballLauncher.UnregisterInputs();
    }

    // Catch input handler
    private void OnCatchInputTriggered(InputAction.CallbackContext context)
    {
        if (playerRole != MovementRole.Dodger) return;
        
        if (dodgerCatch != null)
        {
            if (showDebugMessages) Debug.Log($"🤲 {gameObject.name}: Attempting catch...");
            dodgerCatch.TriggerCatch();
        }
        else if (catchComponent != null)
        {
            if (showDebugMessages) Debug.Log($"🤲 {gameObject.name}: Attempting catch via CatchAction...");
            catchComponent.ExecuteCatch();
        }
        else
        {
            if (showDebugMessages) Debug.LogWarning($"⚠️ {gameObject.name}: No catch component found!");
        }
    }

    // Drop input handler
    private void OnDropInputTriggered(InputAction.CallbackContext context)
    {
        if (playerRole != MovementRole.Dodger) return;
        
        if (dodgerCatch != null)
        {
            if (showDebugMessages) Debug.Log($"👇 {gameObject.name}: Dropping ball...");
            // Create a null InputValue to satisfy the method signature
            dodgerCatch.OnDodgerDrop(new InputValue());
        }
        else if (catchComponent != null)
        {
            if (showDebugMessages) Debug.Log($"👇 {gameObject.name}: Dropping ball via CatchAction...");
            catchComponent.ExecuteDrop();
        }
    }

    // Movement handlers for Dodger (WASD)
    private void OnDodgerMovePerformed(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        if (showDebugMessages && movementInput.magnitude > 0.1f)
        {
            Debug.Log($"🎯 Dodger moving: {movementInput}");
        }
    }

    private void OnDodgerMoveCanceled(InputAction.CallbackContext context)
    {
        movementInput = Vector2.zero;
    }

    // Movement handlers for Shooter (Arrow Keys)
    private void OnShooterMovePerformed(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        if (showDebugMessages && movementInput.magnitude > 0.1f)
        {
            Debug.Log($"🎯 Shooter moving: {movementInput}");
        }
    }

    private void OnShooterMoveCanceled(InputAction.CallbackContext context)
    {
        movementInput = Vector2.zero;
    }

    private void FixedUpdate()
    {
        if (rb == null) return;
        
        // Apply movement
        Vector3 moveVector = new Vector3(movementInput.x, 0f, movementInput.y) * moveSpeed;
        rb.linearVelocity = new Vector3(moveVector.x, rb.linearVelocity.y, moveVector.z);
    }
}