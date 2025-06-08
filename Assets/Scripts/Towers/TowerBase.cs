using UnityEngine;

public class TowerBase : MonoBehaviour
{
    [Header("Configuração do NPC")]
    [SerializeField] private GameObject npcPrefab; // Arraste aqui o prefab do seu NPC Arqueiro
    [SerializeField] private Transform npcSpawnPoint; // Arraste aqui o objeto filho "PontoDeSpawnNPC"

    void Start()
    {
        if (npcPrefab != null && npcSpawnPoint != null)
        {
            // Instancia o NPC e o torna filho da base para organização
            GameObject npcInstance = Instantiate(npcPrefab, npcSpawnPoint.position, npcSpawnPoint.rotation);
            npcInstance.transform.SetParent(this.transform);
        }
        else
        {
            Debug.LogError("Prefab do NPC ou seu ponto de spawn não configurado na base da torre!");
        }
    }
}