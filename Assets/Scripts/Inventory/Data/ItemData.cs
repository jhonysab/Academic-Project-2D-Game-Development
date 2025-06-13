// ItemData.cs
using UnityEngine;

// Um enum para todos os tipos de item possíveis
public enum ItemType { Arma, Botas, Armadura, Amuleto, Pocao }

// 'abstract' significa que esta classe não pode ser usada diretamente, apenas herdada.
public abstract class ItemData : ScriptableObject
{
    [Header("Informações Básicas do Item")]
    public string itemName;
    public Sprite icon;
    public ItemType itemType;

    [Header("Visual do Item no Chão")]
    public GameObject pickupPrefab; // O objeto que aparece no chão ao ser dropado
}