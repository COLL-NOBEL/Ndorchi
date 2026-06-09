using UnityEngine;
using UnityEngine.InputSystem;

public class DodgerCatch : MonoBehaviour
{
    [Header("Catch Detection")]
    public CatchZoneDetector catchZone;
    public Transform handHoldPoint; 

    [Header("Timing Windows")]
    public float catchDuration = 0.4f; 
    public float dropImmunityDuration = 0.5f; // Time in seconds the Dodger is immune to the ball after letting go

    [Header("Dodger Throw Settings")]
    public float throwForce = 25f; 

    private bool hasBall = false;
    private GameObject caughtBall;
    private bool isTryingToCatch = false;
    private float catchTimer = 0f;

    // Safety state to track immunity windows
    private bool isImmuneToOwnBall = false;
    private float immunityTimer = 0f;

    void Update()
    {
        if (isTryingToCatch)
        {
            catchTimer -= Time.deltaTime;
            if (catchTimer <= 0f) isTryingToCatch = false;
        }

        if (isImmuneToOwnBall)
        {
            immunityTimer -= Time.deltaTime;
            if (immunityTimer <= 0f) isImmuneToOwnBall = false;
        }
    }

    public void OnCatch(InputValue value)
    {
        if (!value.isPressed || hasBall || isTryingToCatch) return;

        if (catchZone != null && catchZone.IsBallInZone())
        {
            isTryingToCatch = true;
            catchTimer = catchDuration;
            Debug.Log("🥋 Dodger triggers Catch Animation!");
        }
    }

    public void OnDodgerThrow(InputValue value)
    {
        if (!value.isPressed || !hasBall || caughtBall == null) return;

        Debug.Log("🤾 Dodger Throws the ball back!");
        
        GameObject ballToThrow = caughtBall;
        ReleaseBall(); 

        Rigidbody ballRb = ballToThrow.GetComponent<Rigidbody>();
        if (ballRb != null)
        {
            ballRb.AddForce(transform.forward * throwForce, ForceMode.Impulse);
        }
    }

    public void OnDodgerDrop(InputValue value)
    {
        if (!value.isPressed || !hasBall || caughtBall == null) return;

        Debug.Log("👇 Dodger drops the ball at their feet.");
        ReleaseBall(); 
    }

    public bool IsCurrentlyCatching()
    {
        return isTryingToCatch;
    }

    // Public getter so BallCollision script can respect the drop safety window
    public bool IsImmune()
    {
        return isImmuneToOwnBall;
    }

    public void SecureBall(GameObject ball)
    {
        isTryingToCatch = false;
        hasBall = true;
        caughtBall = ball;

        ball.transform.SetParent(handHoldPoint);
        ball.transform.localPosition = Vector3.zero;

        Rigidbody ballRb = ball.GetComponent<Rigidbody>();
        if (ballRb != null)
        {
            ballRb.linearVelocity = Vector3.zero;
            ballRb.angularVelocity = Vector3.zero;
            ballRb.isKinematic = true;
        }

        Collider ballCollider = ball.GetComponent<Collider>();
        if (ballCollider != null) ballCollider.enabled = false;

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScoreToActiveDodgers(10);
        }
    }

    private void ReleaseBall()
    {
        if (caughtBall == null) return;

        // Turn on safety immunity right before resetting physics
        isImmuneToOwnBall = true;
        immunityTimer = dropImmunityDuration;

        caughtBall.transform.SetParent(null);

        Rigidbody ballRb = caughtBall.GetComponent<Rigidbody>();
        if (ballRb != null) ballRb.isKinematic = false;

        Collider ballCollider = caughtBall.GetComponent<Collider>();
        if (ballCollider != null) ballCollider.enabled = true;

        caughtBall = null;
        hasBall = false;
    }
}