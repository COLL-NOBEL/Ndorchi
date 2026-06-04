using UnityEngine;

public class DodgerCatch : MonoBehaviour
{
    [SerializeField] private PrecisionBar precisionBar;
    [SerializeField] private Transform catchPoint; // Position où la balle s'arrête en cas de capture
    private GameObject ballInRange;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            ballInRange = other.gameObject;
            // Optionnel : Lancer automatiquement l'oscillation pour avertir le joueur
            precisionBar.StartOscillation();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            ballInRange = null;
            precisionBar.StopOscillation();
        }
    }

    void OnCatch()
    {
        if (ballInRange == null) return;

        float score = precisionBar.StopOscillation();

        // On définit le vert entre 0.4 et 0.6 (le centre exact)
        if (score >= 0.4f && score <= 0.6f)
        {
            CaptureBall();
        }
        else
        {
            TriggerHit();
        }
    }

    private void CaptureBall()
    {
        Rigidbody rb = ballInRange.GetComponent<Rigidbody>();
        rb.isKinematic = true; // Arrête la physique de la balle
        ballInRange.transform.position = catchPoint.position;
        ballInRange.transform.SetParent(catchPoint); // Attache la balle au joueur
    }

    private void TriggerHit()
    {
        // Logique de la Phase 4 (Dégâts / Élimination)
        Debug.Log("Touché ! Le Catch a échoué.");
    }
}