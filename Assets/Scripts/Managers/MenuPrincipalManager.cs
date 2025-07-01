using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipalManager : MonoBehaviour
{
    // Carrega a cena principal do jogo
    public void IniciarJogo()
    {
        // Lembre-se de substituir "NomeDaSuaCenaDeJogo" pelo nome real!
        SceneManager.LoadScene("Cutscene 1");
    }

    // Ação para o botão de Opções
public GameObject painelOpcoes; // Arraste o PainelOpcoes aqui no Inspector

public void AbrirOpcoes()
{
    Debug.Log("Abrindo o painel de opções...");
    if (painelOpcoes != null)
    {
        painelOpcoes.SetActive(true);
    }
}

// Adicione esta nova função para o botão "Voltar"
public void FecharOpcoes()
{
    if (painelOpcoes != null)
    {
        painelOpcoes.SetActive(false);
    }
}

    // Ação para o botão de Sair
    public void SairDoJogo()
    {
        Debug.Log("Fechando o jogo...");
        Application.Quit();
    }

}
