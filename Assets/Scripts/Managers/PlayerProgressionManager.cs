using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class PlayerProgressionManager : MonoBehaviour
{
    // --- Singleton Pattern ---
    public static PlayerProgressionManager instance;

    // --- NOSSA NOVA LINHA DE EVENTO ---
    public static event System.Action<Dictionary<ItemType, EquipmentData>> OnEquipmentChanged;
    // ------------------------------------


    [Header("Configuração da Progressão de Equip.")]
    [SerializeField] private LootProgressionData equipmentProgression; // A lista ordenada de drops permanentes

    // Dicionário que funciona como nosso "inventário" de melhores itens
    private Dictionary<ItemType, EquipmentData> equippedItems = new Dictionary<ItemType, EquipmentData>();

    // As propriedades públicas que outros scripts (como o PlayerController) vão ler
    public float TotalDamageBonus { get; private set; }
    public float TotalSpeedBonus { get; private set; }

    // Chave para salvar o progresso
    private const string PROGRESS_SAVE_KEY = "PlayerProgressionIndex";
    private int nextEquipmentIndex; // Rastreia qual o próximo item da lista a ser dropado

    void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }

        LoadProgress();
    }

    // NOVO: Método para a UI poder carregar o estado inicial
    private void Start()
    {
        // Força uma atualização inicial para a UI que acabou de "nascer"
        RecalculateAllBonuses(); 
    }


    // --- LÓGICA DE EQUIPAR ITENS PERMANENTES ---

    public void EquipPermanentItem(EquipmentData newItem)
    {
        if (newItem == null) return;

        // Verifica se já temos um item do mesmo TIPO equipado
        if (equippedItems.TryGetValue(newItem.itemType, out EquipmentData currentItem))
        {
            // Se o novo item for de um tier superior, ele substitui o antigo
            if (newItem.tier > currentItem.tier)
            {
                equippedItems[newItem.itemType] = newItem;
                Debug.Log($"UPGRADE! {newItem.itemName} (Tier {newItem.tier}) substituiu {currentItem.itemName} (Tier {currentItem.tier}).");
            }
        }
        else
        {
            // É o primeiro item deste tipo, equipa direto
            equippedItems.Add(newItem.itemType, newItem);
            Debug.Log($"NOVO EQUIPAMENTO! {newItem.itemName} equipado.");
        }
        RecalculateAllBonuses();
    }

    private void RecalculateAllBonuses()
    {
        TotalDamageBonus = 0f;
        TotalSpeedBonus = 0f;

        // Soma os bônus de todos os itens atualmente "equipados" (os de maior tier)
        foreach (EquipmentData item in equippedItems.Values)
        {
            TotalDamageBonus += item.damageBonus;
            TotalSpeedBonus += item.speedBonus;
        }

        Debug.Log($"Bônus Totais Atualizados: Dano +{TotalDamageBonus}, Velocidade +{TotalSpeedBonus}");
        // --- AVISANDO A UI SOBRE A MUDANÇA ---
        OnEquipmentChanged?.Invoke(equippedItems);
        // ---------------------------------------

    }

    // --- LÓGICA DE DROP PROGRESSIVO ---

    public void SpawnNextProgressionItem(Vector3 dropPosition)
    {
        // Verifica se ainda há itens para dropar na lista
        if (nextEquipmentIndex >= equipmentProgression.progressionItems.Count)
        {
            Debug.Log("Todos os itens de progressão já foram dropados.");
            return;
        }

        EquipmentData itemToDrop = equipmentProgression.progressionItems[nextEquipmentIndex];

        if (itemToDrop != null && itemToDrop.pickupPrefab != null)
        {
            Instantiate(itemToDrop.pickupPrefab, dropPosition, Quaternion.identity);
            Debug.Log($"DROP DE PROGRESSÃO: {itemToDrop.itemName} apareceu no mundo.");

            // Avança para o próximo item e salva o progresso
            nextEquipmentIndex++;
            SaveProgress();
        }
    }

    // --- PERSISTÊNCIA (SALVAR/CARREGAR) ---
    private void SaveProgress()
    {
        PlayerPrefs.SetInt(PROGRESS_SAVE_KEY, nextEquipmentIndex);
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        nextEquipmentIndex = PlayerPrefs.GetInt(PROGRESS_SAVE_KEY, 0);
    }

    // NOVO: Adicionei este getter para ajudar na inicialização da UI
    public Dictionary<ItemType, EquipmentData> GetCurrentEquippedItems()
    {
        return equippedItems;
    }
}

// Cole este bloco DEPOIS do fechamento da classe PlayerProgressionManager
#if UNITY_EDITOR

public class PlayerProgressionResetMenu
{
    [MenuItem("Debug/Reset Loot Progression")]
    public static void ResetLootProgression()
    {
        // Deleta a chave específica que salva o progresso do loot
        PlayerPrefs.DeleteKey("PlayerProgressionIndex");
        PlayerPrefs.Save();
        Debug.LogWarning("!!! PROGRESSÃO DE LOOT RESETADA PARA O COMEÇO !!!");
    }
}
#endif