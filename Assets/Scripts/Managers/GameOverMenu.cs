// Arquivo: GameOverMenu.cs
using UnityEngine;
using UnityEngine.SceneManagement; // Essencial para gerenciar cenas!

public class GameOverMenu : MonoBehaviour
{
    [Header("Configuração de Cenas")]
    [Tooltip("O nome exato do arquivo da sua cena de jogo principal.")]
    public string nomeDaCenaDeJogo = "Level_01";

    [Tooltip("O nome exato do arquivo da sua cena de menu principal.")]
    public string nomeDaCenaDeMenu = "MenuPrincipal";

    /// <summary>
    /// Função para o botão "Reiniciar".
    /// </summary>
    public void ReiniciarJogo()
    {
        // Reseta a escala de tempo caso o jogo estivesse pausado
        Time.timeScale = 1f;
        SceneManager.LoadScene(nomeDaCenaDeJogo);
    }

    /// <summary>
    /// Função para o botão "Menu Principal".
    /// </summary>
    public void VoltarAoMenuPrincipal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nomeDaCenaDeMenu);
    }
}