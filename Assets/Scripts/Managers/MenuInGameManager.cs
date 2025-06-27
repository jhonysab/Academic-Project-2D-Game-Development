// Arquivo: InGameMenuManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenuManager : MonoBehaviour
{
    public static bool isGamePaused = false;

    [Header("Referências")]
    public GameObject pauseMenuUI; // O painel do menu de pause
    public GameObject optionsMenuUI; // O painel de opções de áudio

    void Update()
    {
        // Detecta se a tecla "Esc" foi pressionada
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
            {
                // Se o jogo já está pausado, despausa
                Resume();
            }
            else
            {
                // Se o jogo não está pausado, pausa
                Pause();
            }
        }
    }

    // --- Funções para serem chamadas pelos botões ---

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(false); // Garante que o menu de opções também feche
        Time.timeScale = 1f; // Volta o tempo ao normal
        isGamePaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // CONGELA O JOGO!
        isGamePaused = true;
    }

    public void LoadMainMenu()
    {
        // MUITO IMPORTANTE: Despausar o jogo antes de sair da cena
        Time.timeScale = 1f;
        isGamePaused = false;
        SceneManager.LoadScene("MenuPrincipal"); // Certifique-se que o nome da sua cena de menu está correto
    }

    public void QuitGame()
    {
        Debug.Log("Saindo do jogo...");
        Application.Quit();
    }

    public void OpenOptions()
    {
        // Esconde o menu de pause principal e mostra o de opções
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(true);
    }
    
    public void CloseOptions()
    {
        // Esconde o menu de opções e volta para o de pause
        optionsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }
}