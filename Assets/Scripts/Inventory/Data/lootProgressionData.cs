using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LootProgression", menuName = "My Game/Data/Loot Progression Data")]
public class LootProgressionData : ScriptableObject
{
    [Header("Ordem de Drop dos Equipamentos")]
    [Tooltip("A lista de equipamentos permanentes na ordem em que devem ser dropados.")]
    public List<EquipmentData> progressionItems;
}