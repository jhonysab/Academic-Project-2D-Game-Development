using UnityEngine;

[CreateAssetMenu(fileName = "Nova Planta de Torre", menuName = "Torre/Planta de Torre")]
public class TowerBlueprint : ScriptableObject
{
    [Header("Configuração da Torre")]
    public GameObject prefabDaTorre;
    public int custo;
    
    // APAGUE OU COMENTE ESTA LINHA:
    // public Transform pontoDeSpawn; 

    [Header("Informações para a Loja (UI)")]
    public string nomeDaTorre;
    [TextArea]
    public string descricao;
    public Sprite icone;
}