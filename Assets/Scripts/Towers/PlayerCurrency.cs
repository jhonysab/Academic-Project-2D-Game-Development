using UnityEngine;
using TMPro; // Adicione esta linha para usar TextMeshPro

public class PlayerCurrency : MonoBehaviour
{
    public static PlayerCurrency main;

    public int moedaInicial = 100;
    public int moedaAtual;

    [Header("Referências da UI")]
    public TMP_Text textoMoedaUI; // Arraste seu objeto "TextoMoeda" aqui no Inspector
    
    void Awake()
    {
        if (main == null) { main = this; }
        else { Destroy(gameObject); }
        moedaAtual = moedaInicial;
    }

    private void Start()
    {
        AtualizarUI();
    }

    public bool PodePagar(int custo)
    {
        return moedaAtual >= custo;
    }

    public void GastarMoeda(int valor)
    {
        moedaAtual -= valor;
        AtualizarUI();
    }

    public void AdicionarMoeda(int valor)
    {
        moedaAtual += valor;
        AtualizarUI();
    }

    // Método que atualiza o texto na tela
    void AtualizarUI()
    {
        if (textoMoedaUI != null)
        {
            textoMoedaUI.text = "Ouro: " + moedaAtual; // Formato do texto
        }
    }
}