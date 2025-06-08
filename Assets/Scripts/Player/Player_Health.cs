using UnityEngine;

public class Player_Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;

    public HealthUI healthUI;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void ChangeHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UpdateUI();

        if (currentHealth <= 0)
        {
            Debug.Log("Player morreu.");
            gameObject.SetActive(false);
        }
    }
    public class HealthUI : MonoBehaviour
{
    // Example method to update the health bar UI
    public void UpdateBar(int currentHealth, int maxHealth)
    {
        // Implement your UI update logic here
        Debug.Log($"Updating health bar: {currentHealth}/{maxHealth}");
    }
}

    private void UpdateUI()
    {
        if (healthUI != null)
        {
            healthUI.UpdateBar(currentHealth, maxHealth);
        }
    }
}
