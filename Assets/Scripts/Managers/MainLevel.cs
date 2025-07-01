using UnityEngine;
using UnityEngine.SceneManagement; // <-- LINHA ESSENCIAL

public class LevelManagerSimple : MonoBehaviour
{
    public void LoadNextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("Último nível! Voltando ao menu...");
            SceneManager.LoadScene(0); // Volta para a cena de índice 0 (menu)
        }
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu"); // Carrega a cena pelo nome "Menu"
    }
}