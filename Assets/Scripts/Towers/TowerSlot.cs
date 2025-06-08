using UnityEngine;

public class TowerSlot : MonoBehaviour
{
    private GameObject torreConstruida;
    private SpriteRenderer spriteRenderer;

    [Header("Cores de Feedback")]
    public Color corMouseEmCima = Color.yellow; // Cor para quando pode construir
    public Color corLocalOcupado = Color.red;    // Cor para quando já tem torre
    private Color corOriginal;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            corOriginal = spriteRenderer.color; // Salva a cor original
        }
    }

    private void OnMouseDown()
    {
        if (torreConstruida != null)
        {
            Debug.Log("Este local já está ocupado!");
            return;
        }
        BuildManager.main.ConstruirTorreNesteSlot(this);
    }

    // Chamado quando o mouse entra no collider
    private void OnMouseEnter()
    {
        if (spriteRenderer == null) return;

        // Se o jogador tem uma torre selecionada para construir
        if (BuildManager.main.TemTorreSelecionada())
        {
            if (torreConstruida == null) // E o local está livre
            {
                spriteRenderer.color = corMouseEmCima;
            }
            else // E o local está ocupado
            {
                spriteRenderer.color = corLocalOcupado;
            }
        }
    }

    // Chamado quando o mouse sai do collider
    private void OnMouseExit()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = corOriginal; // Volta para a cor original
        }
    }

    public void ConstruirTorre(TowerBlueprint planta)
    {
        torreConstruida = Instantiate(planta.prefabDaTorre, transform.position, Quaternion.identity);
        torreConstruida.transform.SetParent(this.transform);

        // Após construir, volta a cor para a de ocupado
        if (spriteRenderer != null)
        {
            spriteRenderer.color = corLocalOcupado;
        }
    }
}