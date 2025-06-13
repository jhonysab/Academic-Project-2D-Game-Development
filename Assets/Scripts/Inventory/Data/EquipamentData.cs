// EquipmentData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "My Game/Data/Equipment Data")]
public class EquipmentData : ItemData
{
    [Header("Atributos de Equipamento")]
    public int tier; // Nível do item (0=Madeira, 1=Ferro, etc.)

    [Header("Bônus Permanentes")]
    public float damageBonus;
    public float speedBonus;
}