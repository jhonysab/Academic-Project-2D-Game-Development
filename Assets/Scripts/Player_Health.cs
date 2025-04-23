using UnityEngine;

public class Player_Health : MonoBehaviour
{

    public int currentHealth = 100;
    public int maxHealth;

    public void ChangeHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
