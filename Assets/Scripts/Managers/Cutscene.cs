using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public PlayableDirector director; // Arraste seu PlayableDirector aqui
    public string nextSceneName = "Level 1";    // Digite o nome da próxima cena aqui

    void OnEnable()
    {
        // Inscreve-se no evento 'stopped' do PlayableDirector
        if (director != null)
        {
            director.stopped += OnPlayableDirectorStopped;
        }
    }

    void OnDisable()
    {
        // Cancela a inscrição do evento para evitar erros
        if (director != null)
        {
            director.stopped -= OnPlayableDirectorStopped;
        }
    }

    void OnPlayableDirectorStopped(PlayableDirector aDirector)
    {
        // Verifica se é o diretor correto que parou
        if (director == aDirector)
        {
            // Carrega a próxima cena
            SceneManager.LoadScene(nextSceneName);
        }
    }
}