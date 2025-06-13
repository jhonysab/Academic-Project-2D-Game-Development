using UnityEngine;

public class EnemyLoot : MonoBehaviour
{
    [Header("Drop de Progressão")]
    [Tooltip("Chance de dropar o PRÓXIMO equipamento da lista.")]
    [Range(0, 100)] public float progressionDropChance = 10f;

    [Header("Drop de Consumível")]
    [Tooltip("Opcional: prefab de um consumível (ex: poção de vida).")]
    public GameObject consumablePrefab;
    [Range(0, 100)] public float consumableDropChance = 25f;


    // Esta função será chamada pelo EnemyHealth na morte
    public void TriggerDrops()
    {
        // Tenta dropar o item de progressão
        if (Random.Range(0f, 100f) <= progressionDropChance)
        {
            PlayerProgressionManager.instance.SpawnNextProgressionItem(transform.position);
        }

        // Tenta dropar o consumível
        if (consumablePrefab != null && Random.Range(0f, 100f) <= consumableDropChance)
        {
            Instantiate(consumablePrefab, transform.position, Quaternion.identity);
        }
    }
}