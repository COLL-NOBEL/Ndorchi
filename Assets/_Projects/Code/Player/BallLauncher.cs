using UnityEngine;
using UnityEngine.InputSystem;

public class BallLauncher : MonoBehaviour
{
    [Header("Power Settings")]
    public float minThrowForce = 15f;
    public float maxThrowForce = 45f;
    public float chargeSpeed = 20f; 
    
    [Header("Accuracy Settings")]
    public PrecisionBarController precisionBar; 
    public float maxSpreadAngle = 30f; 

    [Header("Launch References")]
    public Transform launchPoint; 

    private GameObject ball;
    private Rigidbody ballRb;
    private BallCollision ballCollision; 
    
    private float currentPower;
    private bool isCharging = false;

    private PlayerController playerCtrl;
    private GameControls inputActions;

    void Awake()
    {
        playerCtrl = GetComponent<PlayerController>();
        inputActions = new GameControls();
    }

    void Start()
    {
        currentPower = minThrowForce;
    }

    void Update()
    {
        if (isCharging)
        {
            currentPower += chargeSpeed * Time.deltaTime;
            if (currentPower > maxThrowForce) currentPower = maxThrowForce;
        }
    }

    // Explicitly called by PlayerController to ensure the role is set BEFORE binding inputs
    public void RegisterInputs(PlayerController.MovementRole role)
    {
        if (inputActions == null) inputActions = new GameControls();
        
        // Clean up any existing bindings first
        UnregisterInputs();

        if (role == PlayerController.MovementRole.Shooter)
        {
            inputActions.Shooter.Throw.started += ctx => StartCharging();
            inputActions.Shooter.Throw.canceled += ctx => ReleaseCharge();
            inputActions.Shooter.Enable();
            Debug.Log($"🎯 {gameObject.name} BallLauncher bound to SHOOTER Throw.");
        }
        else if (role == PlayerController.MovementRole.Dodger)
        {
            inputActions.Dodger.DodgerThrow.started += ctx => StartCharging();
            inputActions.Dodger.DodgerThrow.canceled += ctx => ReleaseCharge();
            inputActions.Dodger.Enable();
            Debug.Log($"🥎 {gameObject.name} BallLauncher bound to DODGER DodgerThrow.");
        }
    }

    public void UnregisterInputs()
    {
        if (inputActions != null)
        {
            inputActions.Shooter.Throw.started -= ctx => StartCharging();
            inputActions.Shooter.Throw.canceled -= ctx => ReleaseCharge();
            inputActions.Dodger.DodgerThrow.started -= ctx => StartCharging();
            inputActions.Dodger.DodgerThrow.canceled -= ctx => ReleaseCharge();
            inputActions.Disable();
        }
    }

    void OnDisable()
    {
        UnregisterInputs();
    }

    private void StartCharging()
    {
        isCharging = true;
        currentPower = minThrowForce;
        Debug.Log($"{gameObject.name} is Charging Shot...");
    }

    private void ReleaseCharge()
    {
        if (isCharging) FireBall();
    }

    void FireBall()
    {
        isCharging = false;

        DodgerCatchAction catchCheck = GetComponent<DodgerCatchAction>();
        if (catchCheck != null && catchCheck.IsHoldingBall())
        {
            catchCheck.ForceReleaseOnThrow();
        }

        if (ball == null)
        {
            ball = GameObject.FindWithTag("Ball");
            if (ball != null)
            {
                ballRb = ball.GetComponent<Rigidbody>();
                ballCollision = ball.GetComponent<BallCollision>();
            }
        }
        
        if (precisionBar == null)
        {
            precisionBar = Object.FindAnyObjectByType<PrecisionBarController>();
        }

        if (ball == null || ballRb == null)
        {
            Debug.LogError("🚨 Ball Launcher Error: Can't find an object tagged 'Ball'!");
            return;
        }

        float accuracyOffset = (precisionBar != null) ? precisionBar.GetAccuracyOffset() : 0f; 
        float finalAngle = accuracyOffset * 2f * maxSpreadAngle;
        Quaternion spreadRotation = Quaternion.Euler(0, finalAngle, 0);
        Vector3 skewedDirection = spreadRotation * transform.forward;

        if (ballCollision != null)
        {
            if (playerCtrl != null && playerCtrl.playerRole == PlayerController.MovementRole.Shooter)
            {
                ballCollision.ballShotByShooter = true;
                Debug.Log("🎯 Ball armed by Shooter! Deadly trajectory active.");
            }
            else
            {
                ballCollision.ballShotByShooter = false;
                Debug.Log("🥎 Ball thrown safely by Dodger. Safe return trajectory.");
            }
        }

        // Temporary collision ignore to clear the player mesh safely
        Collider playerCollider = GetComponent<Collider>();
        Collider ballCollider = ball.GetComponent<Collider>();
        if (playerCollider != null && ballCollider != null)
        {
            StartCoroutine(TemporaryCollisionIgnore(playerCollider, ballCollider));
        }

        ballRb.linearVelocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
        ballRb.isKinematic = false;
        ballRb.WakeUp();

        if (launchPoint != null)
        {
            ball.transform.position = launchPoint.position;
        }
        else
        {
            ball.transform.position = transform.position + transform.forward * 1.5f;
        }

        ballRb.AddForce(skewedDirection * currentPower, ForceMode.Impulse);

        // Add this code right after the ball is launched in FireBall() method
        // Place it after: ballRb.AddForce(shootDirection * currentPower, ForceMode.Impulse);

        // Notify BallManager that a shot was made
        if (BallManager.Instance != null && playerCtrl != null && playerCtrl.playerRole == PlayerController.MovementRole.Shooter)
        {
            BallManager.Instance.OnShooterShot();
        }
        currentPower = minThrowForce;
    }

    private System.Collections.IEnumerator TemporaryCollisionIgnore(Collider player, Collider ball)
    {
        Physics.IgnoreCollision(player, ball, true);
        yield return new WaitForSeconds(0.2f);
        if (player != null && ball != null)
        {
            Physics.IgnoreCollision(player, ball, false);
        }
    }
}