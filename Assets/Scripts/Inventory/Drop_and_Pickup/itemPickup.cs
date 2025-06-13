using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData itemData; // Arraste o ScriptableObject do item aqui

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (itemData == null) return;

            // Lógica para decidir o que fazer com o item
            switch (itemData)
            {
                case EquipmentData equipment:
                    // Se for um equipamento, entrega para o manager e se destrói
                    PlayerProgressionManager.instance.EquipPermanentItem(equipment);
                    Destroy(gameObject);
                    break;
                case ConsumableData consumable:
                    // Se for um consumível, usa o efeito imediatamente e se destrói
                    consumable.Use(other.gameObject);
                    Destroy(gameObject);
                    break;
            }
        }
    }
}