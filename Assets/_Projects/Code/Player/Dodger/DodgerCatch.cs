using UnityEngine;
using UnityEngine.InputSystem;

public class DodgerCatch : MonoBehaviour
{
    [Header("Catch Detection")]
    public CatchZoneDetector catchZone;
    public Transform handHoldPoint;

    [Header("Timing Windows")]
    public float catchDuration = 0.5f;
    public float dropImmunityDuration = 0.5f;

    [Header("Debug")]
    public bool showDebugMessages = true;

    private bool hasBall = false;
    private GameObject caughtBall;
    private bool isTryingToCatch = false;
    private float catchTimer = 0f;

    private bool isImmuneToOwnBall = false;
    private float immunityTimer = 0f;

    void Update()
    {
        // Catch window timer
        if (isTryingToCatch)
        {
            catchTimer -= Time.deltaTime;
            if (catchTimer <= 0f)
            {
                isTryingToCatch = false;
                if (showDebugMessages) Debug.Log($"❌ {gameObject.name}: Catch window expired");
            }
        }

        // Immunity timer
        if (isImmuneToOwnBall)
        {
            immunityTimer -= Time.deltaTime;
            if (immunityTimer <= 0f)
            {
                isImmuneToOwnBall = false;
                if (showDebugMessages) Debug.Log($"🛡️ {gameObject.name}: Immunity ended");
            }
        }
    }

    // This method can be called by other scripts
    public void TriggerCatch()
    {
        if (hasBall)
        {
            if (showDebugMessages) Debug.Log($"⚠️ {gameObject.name}: Already holding ball!");
            return;
        }
        
        if (isTryingToCatch)
        {
            if (showDebugMessages) Debug.Log($"⚠️ {gameObject.name}: Already attempting catch!");
            return;
        }

        // Check if ball is in catch zone
        if (catchZone != null && catchZone.IsBallInZone())
        {
            isTryingToCatch = true;
            catchTimer = catchDuration;
            if (showDebugMessages) Debug.Log($"🥋 {gameObject.name}: CATCH ATTEMPT! Window: {catchDuration}s");
        }
        else
        {
            if (showDebugMessages) Debug.Log($"❌ {gameObject.name}: No ball in catch zone");
        }
    }

    // Input System callback
    public void OnCatch(InputValue value)
    {
        if (!value.isPressed) return;
        TriggerCatch();
    }

    public void OnDodgerThrow(InputValue value)
    {
        if (!value.isPressed || !hasBall || caughtBall == null) return;

        if (showDebugMessages) Debug.Log($"🤾 {gameObject.name}: Throwing ball!");
        
        GameObject ballToThrow = caughtBall;
        ReleaseBall();

        Rigidbody ballRb = ballToThrow.GetComponent<Rigidbody>();
        if (ballRb != null)
        {
            ballRb.AddForce(transform.forward * 25f, ForceMode.Impulse);
        }
    }

    public void OnDodgerDrop(InputValue value)
    {
        if (!hasBall || caughtBall == null) return;

        if (showDebugMessages) Debug.Log($"👇 {gameObject.name}: Dropping ball");
        ReleaseBall();
    }

    public bool IsCurrentlyCatching()
    {
        return isTryingToCatch;
    }

    public bool IsImmune()
    {
        return isImmuneToOwnBall;
    }
    
    public bool HasBall()
    {
        return hasBall;
    }

    public void SecureBall(GameObject ball)
    {
        if (ball == null)
        {
            Debug.LogError($"❌ {gameObject.name}: Tried to secure null ball!");
            return;
        }

        // Reset catch state
        isTryingToCatch = false;
        hasBall = true;
        caughtBall = ball;

        if (showDebugMessages) Debug.Log($"✨ {gameObject.name}: SECURING BALL!");

        // Determine attachment point
        Transform attachPoint = handHoldPoint;
        if (attachPoint == null)
        {
            // Create runtime hand point if not assigned
            GameObject handObj = new GameObject("RuntimeHandPoint");
            handObj.transform.SetParent(transform);
            handObj.transform.localPosition = new Vector3(0, 1.5f, 1f);
            attachPoint = handObj.transform;
            if (showDebugMessages) Debug.Log($"⚠️ {gameObject.name}: Created runtime hand point - assign handHoldPoint in inspector!");
        }

        // Parent the ball
        ball.transform.SetParent(attachPoint);
        ball.transform.localPosition = Vector3.zero;
        ball.transform.localRotation = Quaternion.identity;

        // Freeze physics
        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        if (ballRb != null)
        {
            ballRb.linearVelocity = Vector3.zero;
            ballRb.angularVelocity = Vector3.zero;
            ballRb.isKinematic = true;
        }

        // Disable collider to prevent unwanted collisions
        Collider ballCollider = ball.GetComponent<Collider>();
        if (ballCollider != null)
        {
            ballCollider.enabled = false;
        }

        // Disarm ball
        BallCollision ballCol = ball.GetComponent<BallCollision>();
        if (ballCol != null)
        {
            ballCol.ballShotByShooter = false;
        }

        if (showDebugMessages) Debug.Log($"✅ {gameObject.name}: Ball secured successfully");

        // Award points
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScoreToActiveDodgers(10);
            if (showDebugMessages) Debug.Log("+10 points for catch!");
        }
    }

    public void ForceReleaseBall()
    {
        if (caughtBall != null)
        {
            // Set immunity
            isImmuneToOwnBall = true;
            immunityTimer = dropImmunityDuration;

            // Unparent
            caughtBall.transform.SetParent(null);

            // Re-enable physics
            Rigidbody ballRb = caughtBall.GetComponent<Rigidbody>();
            if (ballRb != null)
            {
                ballRb.isKinematic = false;
                ballRb.linearVelocity = Vector3.zero;
            }

            // Re-enable collider
            Collider ballCollider = caughtBall.GetComponent<Collider>();
            if (ballCollider != null)
            {
                ballCollider.enabled = true;
            }

            if (showDebugMessages) Debug.Log($"🏐 {gameObject.name}: Force released ball");

            caughtBall = null;
            hasBall = false;
        }
    }

    private void ReleaseBall()
    {
        if (caughtBall == null) return;

        // Set immunity
        isImmuneToOwnBall = true;
        immunityTimer = dropImmunityDuration;

        // Unparent
        caughtBall.transform.SetParent(null);

        // Re-enable physics
        Rigidbody ballRb = caughtBall.GetComponent<Rigidbody>();
        if (ballRb != null)
        {
            ballRb.isKinematic = false;
            ballRb.linearVelocity = Vector3.zero;
        }

        // Re-enable collider
        Collider ballCollider = caughtBall.GetComponent<Collider>();
        if (ballCollider != null)
        {
            ballCollider.enabled = true;
        }

        // Drop slightly in front of player
        caughtBall.transform.position = transform.position + transform.forward * 1.5f + Vector3.up * 0.5f;

        if (showDebugMessages) Debug.Log($"🏐 {gameObject.name}: Ball released");

        caughtBall = null;
        hasBall = false;
    }
}