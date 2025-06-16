using UnityEngine;

public class Player_Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;

    public HealthUI healthUI; // UI da vida

    // --- ATRIBUTOS DE STATUS (Para Poções) ---
    public float baseDamage = 10f; // Dano base do player
    public float baseMoveSpeed = 5f; // Velocidade de movimento base do player

    // Bônus temporários que serão adicionados/removidos das bases
    private float tempDamageBonus = 0f;
    private float tempSpeedBonus = 0f;

    // Tempos de expiração para os bônus temporários
    private float damageBonusExpireTime = 0f;
    private float speedBonusExpireTime = 0f;

    // Propriedades somente leitura para acessar o dano e velocidade atuais
    public float CurrentDamage { get { return baseDamage + tempDamageBonus; } }
    public float CurrentMoveSpeed { get { return baseMoveSpeed + tempSpeedBonus; } }
    // --- FIM ATRIBUTOS DE STATUS ---


    private void Start() // Inicializa a vida no Start
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    private void Update() // Método Update para gerenciar expiração de bônus
    {
        // Gerenciar expiração do bônus de Dano
        if (tempDamageBonus > 0 && Time.time >= damageBonusExpireTime)
        {
            tempDamageBonus = 0;
            Debug.Log("Bônus de dano expirou. Dano atual: " + CurrentDamage);
        }

        // Gerenciar expiração do bônus de Velocidade
        if (tempSpeedBonus > 0 && Time.time >= speedBonusExpireTime)
        {
            tempSpeedBonus = 0;
            Debug.Log("Bônus de velocidade expirou. Velocidade atual: " + CurrentMoveSpeed);
        }
    } 

    // CORREÇÃO: O método UpdateUI foi movido para fora do método Update.
    // Ele agora está no nível correto, dentro da classe Player_Health.
    private void UpdateUI()
    {
        if (healthUI != null)
        {
            // A linha abaixo presume que seu script HealthUI.cs tem um método chamado SetHealth.
            // Se o nome for diferente (como UpdateBar ou UpdateHearts), ajuste aqui.
            healthUI.UpdateBar(currentHealth, maxHealth); // Na sua última versão, o método era UpdateBar(int, int)
        }
        else
        {
             Debug.LogWarning("HealthUI não atribuído no Inspector do Player_Health. A UI da vida não será atualizada.");
        }
    }

    public void ChangeHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log("Vida alterada em " + amount + ". Vida atual: " + currentHealth);
        UpdateUI();

        if (currentHealth <= 0)
        {
            Debug.Log("Player morreu.");
            gameObject.SetActive(false); // Desativa o jogador
        }
    }

    // --- MÉTODOS DE BÔNUS ---
    public void ApplyDamageBoost(float amount, float duration)
    {
        tempDamageBonus = amount;
        damageBonusExpireTime = Time.time + duration;
        Debug.Log($"Aplicado bônus de dano de {amount} por {duration}s. Dano atual: " + CurrentDamage);
    }

    public void ApplySpeedBoost(float amount, float duration)
    {
        tempSpeedBonus = amount;
        speedBonusExpireTime = Time.time + duration;
        Debug.Log($"Aplicado bônus de velocidade de {amount} por {duration}s. Velocidade atual: " + CurrentMoveSpeed);
    }
}