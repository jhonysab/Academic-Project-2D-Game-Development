// Arquivo: Player_Health.cs (VERSÃO CORRIGIDA)
using UnityEngine;

public class Player_Health : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;

    public HealthUI healthUI; // UI da vida

    // --- ATRIBUTOS DE STATUS (Para Poções) ---
    public float baseDamage = 10f; 
    public float baseMoveSpeed = 5f; 

    private float tempDamageBonus = 0f;
    private float tempSpeedBonus = 0f;

    private float damageBonusExpireTime = 0f;
    private float speedBonusExpireTime = 0f;

    public float CurrentDamage { get { return baseDamage + tempDamageBonus; } }
    public float CurrentMoveSpeed { get { return baseMoveSpeed + tempSpeedBonus; } }
    // --- FIM ATRIBUTOS DE STATUS ---


    private void Start() 
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    private void Update() 
    {
        if (tempDamageBonus > 0 && Time.time >= damageBonusExpireTime)
        {
            tempDamageBonus = 0;
            Debug.Log("Bônus de dano expirou. Dano atual: " + CurrentDamage);
        }

        if (tempSpeedBonus > 0 && Time.time >= speedBonusExpireTime)
        {
            tempSpeedBonus = 0;
            Debug.Log("Bônus de velocidade expirou. Velocidade atual: " + CurrentMoveSpeed);
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

    public void ChangeHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log("Vida alterada em " + amount + ". Vida atual: " + currentHealth);
        UpdateUI();

        // --- ALTERAÇÃO PRINCIPAL AQUI ---
        // Agora, em vez de apenas desativar o objeto, nós chamamos a função Morrer()
        // que centraliza toda a lógica de morte.
        if (currentHealth <= 0)
        {
            Morrer();
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
    
    // Esta função agora será chamada corretamente.
    private void Morrer()
    {
        Debug.Log("O Player morreu! Chamando a tela de Game Over...");

        // Usando o singleton 'main', chamamos a função pública do LevelManager.
        LevelManager.main.TriggerGameOver();

        // Desativa o objeto do player para que ele não possa mais se mover
        // ou ser atingido após morrer.
        gameObject.SetActive(false); 
    }
}