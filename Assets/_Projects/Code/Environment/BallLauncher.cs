using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class BallLauncher : MonoBehaviour
{
    public Transform launchPoint; // Drag your LaunchPoint object here
    public Slider slider;
    
    private GameObject ball;
    private Rigidbody ballRb;

    [SerializeField] private PrecisionBar precisionBar;
    [SerializeField] private float maxDeviationAngle = 30f; // Déviation max en degrés
    [SerializeField] private float throwForce = 15f;

    private float chargeTime = 0f;
    [SerializeField] private float maxChargeTime = 2f;
    [SerializeField] private float maxForceMultiplier = 2.5f;
    private bool isCharging = false;

    void Start()
    {
        // Find the ball in the scene using the tag we made in Task 2.2
        ball = GameObject.FindWithTag("Ball");
        
        if (ball != null)
        {
            ballRb = ball.GetComponent<Rigidbody>();
        }
        else
        {
            Debug.LogError("No object with the tag 'Ball' found in the scene!");
        }
    }

    // This is called by the Player Input component (Message: OnThrow)
    void OnThrow()
    {
        StartCharging();
        if (ball == null || ballRb == null) return;

        //float score = precisionBar.StopOscillation();

        // 1. Reset ball physics (stop it from rolling/moving)
        ballRb.linearVelocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;

        // 2. Teleport ball to the launch point
        ball.transform.position = launchPoint.position + Vector3.up;

        // 3. Apply force forward relative to the player
        ReleaseFire();


        Debug.Log("Ball Launched!");
    }

    // Appelé quand le bouton est enfoncé
    public void StartCharging()
    {
        isCharging = true;
        chargeTime = 0f;
        precisionBar.StartOscillation(); // Lance la barre en même temps !
    }

    void Update()
    {
        if (isCharging)
        {   
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Clamp(chargeTime, 0f, maxChargeTime);
            // Optionnel : Mettre à jour un indicateur visuel de puissance
        }
    }

    // Appelé quand le bouton est relâché
    public void ReleaseFire()
    {
        if (!isCharging) return;
        isCharging = false;

        float score = precisionBar.StopOscillation();

        // Calcul de la puissance
        float chargeRatio = chargeTime / maxChargeTime; // Entre 0 et 1
        float finalForce = throwForce * Mathf.Lerp(1f, maxForceMultiplier, chargeRatio);

        // Reprends la logique de la tâche 3.3 en remplaçant 'baseForce' par 'finalForce'
        ExecutePhysicalLaunch(score, finalForce);
    }

    void ExecutePhysicalLaunch(float score, float finalForce)
    {
        // Calculer la déviation(-1 pour la gauche, 0 pour le centre, 1 pour la droite)
        float normalizedDeviation = (score - 0.5f) * 2f;
        float finalAngle = normalizedDeviation * maxDeviationAngle;

        // Calculer la direction de tir (en supposant un tir vers l'avant sur l'axe Z)
        Vector3 fireDirection = Quaternion.Euler(0, finalAngle, 0) * Vector3.right;

        // We use Impulse for an immediate "kick"
        ballRb.AddForce(fireDirection * throwForce, ForceMode.Impulse);
    }

}