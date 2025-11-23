using UnityEngine;

public class EmmoPickUp : MonoBehaviour
{
    public int currentEmmo = 0;

    public void AddEmmo(int amount)
    {
        currentEmmo += amount;
        Debug.Log("Ammo: " + currentEmmo);
    }
}
