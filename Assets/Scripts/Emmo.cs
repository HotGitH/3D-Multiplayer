using UnityEngine;

public class Emmo : MonoBehaviour
{
    public int emmoAmount = 100; // Set this in Inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EmmoPickUp emmo = other.GetComponent<EmmoPickUp>();
            if (emmo != null)
            {
                emmo.AddEmmo(emmoAmount);
                Destroy(gameObject);
            }
        }
    }
}
