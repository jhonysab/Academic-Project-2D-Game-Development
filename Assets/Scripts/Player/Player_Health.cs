using UnityEngine;

public class Player_Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;

    public HealthUI healthUI; // UI da vida

    // --- NOVO: ATRIBUTOS DE STATUS (Para Poções) ---
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
    // --- FIM NOVO: ATRIBUTOS DE STATUS ---


    private void Start() // Inicializa a vida no Start
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    private void Update() // NOVO: Método Update para gerenciar expiração de bônus
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
    
    // --- NOVO: Métodos para aplicar bônus de dano e velocidade (chamados pelos scripts de poção) ---
    // Estes métodos serão chamados por DamageBoostItem.cs e SpeedBoostItem.cs
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
    // --- FIM NOVO: MÉTODOS DE BÔNUS ---

    // --- HealthUI (Classe aninhada, como estava em sua base) ---
    // ATENÇÃO: Se esta classe aninhada causa problemas de atribuição no Inspector,
    // a solução é movê-la para um arquivo HealthUI.cs SEPARADO.
    public class HealthUI : MonoBehaviour
    {
        public void UpdateBar(int currentHealth, int maxHealth)
        {
            Debug.Log($"Updating health bar: {currentHealth}/{maxHealth}");
            // Implemente sua lógica de atualização de barra de vida aqui
        }
    }

    private void UpdateUI()
    {
        if (healthUI != null)
        {
            healthUI.UpdateBar(currentHealth, maxHealth);
        }
        else
        {
            Debug.LogWarning("HealthUI não atribuído no Inspector do Player_Health. A UI da vida não será atualizada.");
        }
    }
}