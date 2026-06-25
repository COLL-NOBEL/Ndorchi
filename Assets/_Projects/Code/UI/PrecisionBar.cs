using UnityEngine;
using UnityEngine.UI;

public class PrecisionBar : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Slider precisionSlider;

    [Header("Settings")]
    [SerializeField] private float speed = 2f; // Vitesse d'oscillation

    private float timeValue;
    private bool isOscillating = false;

    void Start()
    {
        timeValue = Random.Range(-1, 1);
        StartOscillation();
    }

    void Update()
    {
        if (isOscillating)
        {
            // Utilisation d'une fonction sinusoïdale pour osciller proprement entre 0 et 1
            timeValue += Time.deltaTime * speed;
            precisionSlider.value = (Mathf.Sin(timeValue) + 1f) / 2f;
        }
        else
        {
            timeValue = Random.Range(-1, 1);
        }
    }

    public void StartOscillation()
    {
        isOscillating = true;
        timeValue = 0f; // Réinitialise pour démarrer au même endroit
    }

    public float StopOscillation()
    {
        isOscillating = false;
        return precisionSlider.value; // Renvoie la valeur actuelle (entre 0 et 1)
    }
}