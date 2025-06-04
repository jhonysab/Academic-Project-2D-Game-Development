using UnityEngine;
using UnityEngine.SceneManagement;

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
            SceneManager.LoadScene(0); // volta pro menu
        }
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
