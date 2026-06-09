using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public enum MovementRole { Unassigned, Dodger, Shooter }
    
    [Header("Role Binding")]
    public MovementRole playerRole = MovementRole.Unassigned;

    [Header("Movement Settings")]
    public float moveSpeed = 8f;

    private Rigidbody rb;
    private GameControls inputActions;
    private Vector2 movementInput;
    
    private BallLauncher ballLauncher;
    private DodgerCatchAction catchComponent;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ballLauncher = GetComponent<BallLauncher>();
        catchComponent = GetComponent<DodgerCatchAction>();
        inputActions = new GameControls();
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
            inputActions.Dodger.Move.performed += OnDodgerMovePerformed;
            inputActions.Dodger.Move.canceled += OnDodgerMoveCanceled;
            
            // Task 4.5.3 Dodger Extra Action Mapping
            inputActions.Dodger.Catch.started += OnCatchInputTriggered;
            inputActions.Dodger.Drop.started += OnDropInputTriggered;

            inputActions.Dodger.Enable();
        }
        else if (playerRole == MovementRole.Shooter)
        {
            inputActions.Shooter.Move.performed += OnShooterMovePerformed;
            inputActions.Shooter.Move.canceled += OnShooterMoveCanceled;
            inputActions.Shooter.Enable();
        }

        if (ballLauncher != null) ballLauncher.RegisterInputs(playerRole);
    }

    private void UnsubscribeFromEvents()
    {
        if (inputActions == null) return;

        inputActions.Dodger.Move.performed -= OnDodgerMovePerformed;
        inputActions.Dodger.Move.canceled -= OnDodgerMoveCanceled;
        inputActions.Dodger.Catch.started -= OnCatchInputTriggered;
        inputActions.Dodger.Drop.started -= OnDropInputTriggered;

        inputActions.Shooter.Move.performed -= OnShooterMovePerformed;
        inputActions.Shooter.Move.canceled -= OnShooterMoveCanceled;
        
        if (ballLauncher != null) ballLauncher.UnregisterInputs();
    }

    private void OnCatchInputTriggered(InputAction.CallbackContext context)
    {
        if (catchComponent != null && playerRole == MovementRole.Dodger) catchComponent.ExecuteCatch();
    }

    private void OnDropInputTriggered(InputAction.CallbackContext context)
    {
        if (catchComponent != null && playerRole == MovementRole.Dodger) catchComponent.ExecuteDrop();
    }

    private void OnDodgerMovePerformed(InputAction.CallbackContext context) => movementInput = context.ReadValue<Vector2>();
    private void OnDodgerMoveCanceled(InputAction.CallbackContext context) => movementInput = Vector2.zero;
    private void OnShooterMovePerformed(InputAction.CallbackContext context) => movementInput = context.ReadValue<Vector2>();
    private void OnShooterMoveCanceled(InputAction.CallbackContext context) => movementInput = Vector2.zero;

    private void FixedUpdate()
    {
        Vector3 moveVector = new Vector3(movementInput.x, 0f, movementInput.y) * moveSpeed;
        rb.linearVelocity = new Vector3(moveVector.x, rb.linearVelocity.y, moveVector.z);
    }
}