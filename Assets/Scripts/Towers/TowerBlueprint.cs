using UnityEngine;

// O atributo [CreateAssetMenu] permite criar "arquivos de planta" diretamente no editor do Unity.
[CreateAssetMenu(fileName = "Nova Planta de Torre", menuName = "Torre/Planta de Torre")]
public class TowerBlueprint : ScriptableObject
{
    [Header("Informações da Torre")]
    public GameObject prefabDaTorre; // O prefab da torre que será construído
    public int custo;              // Quanto custa para construir
    
    [Header("Informações da UI (Opcional)")]
    public string nomeDaTorre;
    [TextArea]
    public string descricao;
    public Sprite icone;
}