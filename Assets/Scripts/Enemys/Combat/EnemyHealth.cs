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

    // Sua lógica de drop de item (continua exatamente igual)
    EnemyLoot loot = GetComponent<EnemyLoot>();
    if (loot != null)
    {
        loot.TriggerDrops();
    }

    // --- A LINHA DE CÓDIGO QUE ADICIONAMOS ---
    // Encontra o EnemySpawner na cena e avisa que este inimigo morreu.
    // Garante que o spawner existe para não dar erro.
    EnemySpawner spawner = FindFirstObjectByType<EnemySpawner>();
    if (spawner != null)
    {
        spawner.OnEnemyDied();
    }
    else
    {
        Debug.LogWarning("EnemySpawner não encontrado na cena. Não foi possível notificar a morte do inimigo.");
    }
    
    // O objeto só é destruído DEPOIS de avisar todo mundo.
    Destroy(gameObject);
}
}