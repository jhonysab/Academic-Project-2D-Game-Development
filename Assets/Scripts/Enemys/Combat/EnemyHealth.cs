using UnityEngine;
using UnityEngine.Events; // Para usar UnityEvent se precisar de eventos mais complexos depois

public class EnemyHealth : MonoBehaviour
{
    [Header("Atributos de Vida")]
    public float maxHealth = 10f;
    private float currentHealth;

    [Header("Referências (Opcional)")]
    public Animator anim; // animação de morte para o inimigo

    // evento específico para quando este inimigo morrer,
    // evento estático do EnemySpawner.
    // public UnityEvent onThisEnemyKilled;

    void Awake()
    {
        currentHealth = maxHealth;
        if (anim == null)
        {
            anim = GetComponent<Animator>(); // Tenta pegar o Animator automaticamente
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        // adicionar um feedback visual/sonoro de dano aqui
        // Ex: anim.SetTrigger("Hit");
        // Debug.Log(gameObject.name + " tomou " + damageAmount + " de dano. Vida restante: " + currentHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        // Debug.Log(gameObject.name + " morreu.");

        // Se tiver uma animação de morte:
        // if (anim != null)
        // {
        //     anim.SetTrigger("DeathTrigger"); // Crie um trigger "DeathTrigger" no seu Animator
        // }

        // Notifica o EnemySpawner que um inimigo foi destruído (usando o evento estático existente)
        if (EnemySpawner.onEnemyDestroy != null)
        {
            EnemySpawner.onEnemyDestroy.Invoke();
        }

        // onThisEnemyKilled?.Invoke(); // Se você usar o evento de instância

        Destroy(gameObject); // Destroi o GameObject do inimigo
    }
}