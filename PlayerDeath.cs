using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    public GameObject vfx;

    public bool isDead = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (vfx != null)
        {

            Instantiate(vfx, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);


    }

    private void OnTriggerEnter(Collider Dodger) {

        if (Dodger.CompareTag("Ball")) {
            Die();
        
        }
     }
}