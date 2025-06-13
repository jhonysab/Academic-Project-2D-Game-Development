// EnemyHealth.cs (Versão Limpa e Focada)
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Atributos de Vida")]
    public float maxHealth = 10f;
    private float currentHealth;

    [Header("Referências")]
    public Animator anim;

    void Awake()
    {
        currentHealth = maxHealth;
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " morreu.");

        // ÚNICA RESPONSABILIDADE DE DROP: AVISAR O ENEMYLOOT
        EnemyLoot loot = GetComponent<EnemyLoot>();
        if (loot != null)
        {
            loot.TriggerDrops();
        }
        
        // Se você ainda precisar do evento para o spawner, mantenha-o.
        // if (EnemySpawner.onEnemyDestroy != null)
        // {
        //     EnemySpawner.onEnemyDestroy.Invoke();
        // }

        Destroy(gameObject);
    }
}