using UnityEngine;

// Este atributo mágico [CreateAssetMenu] é o que nos permitirá criar este tipo de asset no menu do Unity.
[CreateAssetMenu(fileName = "Nova Planta de Torre", menuName = "Torre/Planta de Torre")]
public class TowerBlueprint : ScriptableObject
{
    [Header("Configuração Principal")]
    public GameObject prefabDaTorre; // O prefab da BASE da torre que será construído
    public int custo;              // Quanto custa para construir a torre

    [Header("Informações para a Loja (UI)")]
    public string nomeDaTorre;
    [TextArea]
    public string descricao;
    public Sprite icone; // Ícone para mostrar na loja
}