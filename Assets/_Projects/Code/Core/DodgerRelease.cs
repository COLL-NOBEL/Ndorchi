using UnityEngine;

public class DodgerRelease : MonoBehaviour
{
    [SerializeField] private Transform catchPoint;

    public void LaunchBack(float score, float force)
    {
        if (catchPoint.childCount == 0) return;

        // Récupérer la balle capturée
        Transform ballTransform = catchPoint.GetChild(0);
        ballTransform.SetParent(null); // Détacher la balle

        Rigidbody rb = ballTransform.GetComponent<Rigidbody>();
        rb.isKinematic = false; // Réactiver la physique

        // Réutiliser la logique de calcul d'angle de la tâche 3.3
        float normalizedDeviation = (score - 0.5f) * 2f;
        Vector3 throwDirection = Quaternion.Euler(0, normalizedDeviation * 30f, 0) * transform.forward;

        rb.AddForce(throwDirection * force, ForceMode.Impulse);
    }
}