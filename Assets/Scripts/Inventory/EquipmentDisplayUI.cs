using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentDisplayUI : MonoBehaviour
{
    [Header("Referências dos Slots da UI")]
    public Image weaponSlotIcon;
    public Image bootsSlotIcon;
    // No futuro, você pode adicionar: public Image armorSlotIcon;

    // Ao ativar, se inscreve para ouvir o evento de mudança de equipamento
    private void OnEnable()
    {
        PlayerProgressionManager.OnEquipmentChanged += UpdateDisplay;
    }

    // Ao desativar, cancela a inscrição para evitar erros
    private void OnDisable()
    {
        PlayerProgressionManager.OnEquipmentChanged -= UpdateDisplay;
    }

    private void Start()
    {
        // Garante que a UI mostre os itens corretos ao iniciar o jogo
        if (PlayerProgressionManager.instance != null)
        {
            UpdateDisplay(PlayerProgressionManager.instance.GetCurrentEquippedItems());
        }
    }

    // O método que é chamado pelo evento do PlayerProgressionManager
    private void UpdateDisplay(Dictionary<ItemType, EquipmentData> currentEquipment)
    {
        // --- ATUALIZA O SLOT DA ARMA ---
        if (currentEquipment.TryGetValue(ItemType.Arma, out EquipmentData weapon))
        {
            weaponSlotIcon.sprite = weapon.icon;
            weaponSlotIcon.color = Color.white; // Torna visível
        }
        else
        {
            weaponSlotIcon.sprite = null;
            weaponSlotIcon.color = new Color(1, 1, 1, 0); // Torna transparente
        }

        // --- ATUALIZA O SLOT DAS BOTAS ---
        if (currentEquipment.TryGetValue(ItemType.Botas, out EquipmentData boots))
        {
            bootsSlotIcon.sprite = boots.icon;
            bootsSlotIcon.color = Color.white;
        }
        else
        {
            bootsSlotIcon.sprite = null;
            bootsSlotIcon.color = new Color(1, 1, 1, 0);
        }

        // No futuro, adicione aqui a lógica para outros slots...
    }
}