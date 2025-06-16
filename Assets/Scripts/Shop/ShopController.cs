// Arquivo: ShopController.cs
using UnityEngine;

public class ShopController : MonoBehaviour
{
    public static ShopController main; // Singleton para acesso fácil

    [Header("Referências")]
    [Tooltip("Arraste o objeto do painel da loja aqui.")]
    public GameObject painelLoja;

    void Awake()
    {
        main = this;
    }

    void Start()
    {
        // Garante que a loja comece fechada
        if (painelLoja != null)
        {
            painelLoja.SetActive(false);
        }
    }
    
    // Método para abrir a loja
    public void AbrirLoja()
    {
        if (painelLoja != null)
        {
            painelLoja.SetActive(true);
            // Opcional: Pausar o jogo enquanto a loja está aberta
             Time.timeScale = 0f; 
        }
    }

    // Método para fechar a loja
    public void FecharLoja()
    {
        if (painelLoja != null)
        {
            painelLoja.SetActive(false);
            // Opcional: Despausar o jogo
            Time.timeScale = 1f;
        }
    }
}