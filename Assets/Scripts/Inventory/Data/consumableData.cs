// ConsumableData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "My Game/Data/Consumable Data")]
public class ConsumableData : ItemData
{
    [Header("Efeitos do Consumível")]
    public int healAmount; // Para poção de vida

    [Header("Bônus Temporários")]
    public float tempDamageBonus;
    public float tempSpeedBonus;
    public float duration; // Duração dos bônus temporários

    // O método para aplicar os efeitos quando o item é coletado
    public void Use(GameObject user)
    {
        // Tenta encontrar o script de vida/status do jogador
        Player_Health playerHealth = user.GetComponent<Player_Health>();
        if (playerHealth == null) return;

        // Aplica os efeitos que o item tiver
        if (healAmount > 0)
        {
            playerHealth.ChangeHealth(healAmount);
        }
        if (tempDamageBonus > 0 && duration > 0)
        {
            playerHealth.ApplyDamageBoost(tempDamageBonus, duration);
        }
        if (tempSpeedBonus > 0 && duration > 0)
        {
            playerHealth.ApplySpeedBoost(tempSpeedBonus, duration);
        }
    }
}