using UnityEngine;
using UnityEngine.InputSystem;

public class Tireur : MonoBehaviour
{
    [Header("Configuration du Tir")]
    [SerializeField] private Rigidbody ballePrefab;
    [SerializeField] private Transform pointDeLancer;
    
    [Header("Paramètres de Puissance")]
    [SerializeField] private float forceMin = 5f;
    [SerializeField] private float forceMax = 20f;
    [SerializeField] private float tempsChargeMax = 1.5f; // Temps pour atteindre la force max

    private float tempsDebutCharge;
    private bool estEnTrainDeCharger = false;
    private float forceActuelle;

    // Référence à ton script d'UI développé à l'étape précédente
    [SerializeField] private PrecisionBar precisionBar; 
    private float precisionAngleDeviation = 0f;

    // Cette fonction doit être liée à l'événement .started de ton Input Action
    public void OnFireStarted(InputAction.CallbackContext context)
    {
        if (estEnTrainDeCharger) return;

        estEnTrainDeCharger = true;
        tempsDebutCharge = Time.time;
        forceActuelle = forceMin;

        // On lance simultanément la barre de précision visuelle
        if (precisionBar != null)
        {
            precisionBar.StartOscillation();
        }
    }

    // Cette fonction doit être liée à l'événement .canceled (touche relâchée)
    public void OnFireCanceled(InputAction.CallbackContext context)
    {
        if (!estEnTrainDeCharger) return;

        estEnTrainDeCharger = false;

        // On stoppe d'abord l'UI pour figer la valeur de précision
        if (precisionBar != null)
        {
            precisionBar.StopOscillation();
        }

        // On procède au lancer physique de la balle
        LancerBalle();
    }

    private void Update()
    {
        if (estEnTrainDeCharger)
        {
            // Calcul du temps écoulé depuis le début de la pression
            float tempsEcoule = Time.time - tempsDebutCharge;
            
            // Ratio entre 0 et 1
            float ratio = Mathf.Clamp01(tempsEcoule / tempsChargeMax); 
            
            // Interpolation linéaire pour augmenter la force
            forceActuelle = Mathf.Lerp(forceMin, forceMax, ratio);

            // OPTIONNEL : Mettre à jour un visuel de jauge de puissance ici si nécessaire
        }
    }

    private void CalculerDeviationPrecision(float valeurPrecision)
    {
        // Logique de la tâche 3.3 : vert (0.5) = droit, rouge (0 ou 1) = dévié 
        // Calcule l'écart par rapport au centre parfait (0.5)
        float ecart = Mathf.Abs(valeurPrecision - 0.5f); // Donne une valeur entre 0 et 0.5
        
        // Si l'écart est maximal (0.5), on dévie de 25 degrés max par exemple
        float angleMaxDeviation = 25f;
        precisionAngleDeviation = (ecart / 0.5f) * angleMaxDeviation;

        // Donne une direction aléatoire à la déviation (gauche ou droite)
        if (valeurPrecision < 0.5f) precisionAngleDeviation *= -1f;
    }

    private void LancerBalle()
    {
        // 1. Instanciation de la balle [cite: 5]
        Rigidbody nouvelleBalle = Instantiate(ballePrefab, pointDeLancer.position, pointDeLancer.rotation);

        // 2. Calcul de la direction de base déviée par la précision 
        Vector3 directionDeBase = transform.forward;
        Vector3 directionDeviee = Quaternion.Euler(0, precisionAngleDeviation, 0) * directionDeBase;

        // 3. Application de la force physique calculée au Rigidbody 
        nouvelleBalle.AddForce(directionDeviee * forceActuelle, ForceMode.Impulse);
    }
}