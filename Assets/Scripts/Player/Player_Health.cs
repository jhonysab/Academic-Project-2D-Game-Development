using UnityEngine;

public class Player_Health : MonoBehaviour
{
    public float currentHealth; // vida atual do jogador
    public float maxHealth = 5f; // Vida máxima do jogador

    public HealthUI healthUI;

    private void Awake()
    {
        // Isso deve ser iniciado antes pra garantir que seja inciado antes do Start
        currentHealth = maxHealth;
    }

    private void Start()
    {
        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth);
        }
    }

    public void ChangeHealth(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (healthUI != null)
        {
            healthUI.UpdateHearts(currentHealth);
        }

        if (currentHealth <= 0.01f)
        {
            Debug.Log("Player morreu.");
            gameObject.SetActive(false);
        }
    }
}
