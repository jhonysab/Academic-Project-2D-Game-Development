// Arquivo: NpcInteraction.cs
// Versão corrigida e mais segura

using UnityEngine;

public class NpcInteraction : MonoBehaviour
{
    [Header("Configuração de Interação")]
    [Tooltip("A tecla que o jogador deve pressionar para interagir.")]
    public KeyCode teclaDeInteracao = KeyCode.E;

    [Tooltip("Objeto de texto ou imagem para mostrar o prompt 'Pressione E'.")]
    public GameObject promptDeInteracao; // Ex: um texto "Pressione E" acima do NPC

    private bool jogadorEstaPerto = false;

    void Start()
    {
        // Garante que o prompt comece desligado
        if (promptDeInteracao != null)
        {
            promptDeInteracao.SetActive(false);
        }
    }

    void Update()
    {
        // Se o jogador está perto e pressionou a tecla de interação...
        if (jogadorEstaPerto && Input.GetKeyDown(teclaDeInteracao))
        {
            // ---- CORREÇÃO APLICADA AQUI ----
            // Primeiro, verificamos se a referência ao ShopController existe antes de usá-la.
            if (ShopController.main != null)
            {
                ShopController.main.AbrirLoja();
                
                // Opcional: esconder o prompt de interação ao abrir a loja
                if(promptDeInteracao != null) promptDeInteracao.SetActive(false);
            }
            else
            {
                // Se não existir, avisa o desenvolvedor sobre o problema no Console.
                Debug.LogError("ERRO: A referência para o ShopController.main não foi encontrada! Verifique se há um objeto com o script 'ShopController' ativo na cena.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorEstaPerto = true;
            if (promptDeInteracao != null)
            {
                promptDeInteracao.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jogadorEstaPerto = false;
            if (promptDeInteracao != null)
            {
                promptDeInteracao.SetActive(false);
            }
            
            // Opcional: Fechar a loja se o jogador se afastar
            if (ShopController.main != null)
            {
               ShopController.main.FecharLoja();
             }
        }
    }
}