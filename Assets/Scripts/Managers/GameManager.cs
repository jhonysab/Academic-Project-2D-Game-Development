using UnityEngine;
using UnityEngine.SceneManagement; // Essencial para gerenciar cenas!

public class GameManager : MonoBehaviour
{
    // Esta é a função pública que outros scripts vão chamar quando o jogo acabar.
    public void EndGame()
    {
        // Debug para confirmar que a função foi chamada.
        Debug.Log("GAME OVER!");

        // Carrega a cena de Game Over.
        // !!! MUITO IMPORTANTE !!!
        // Troque "GameOverScene" pelo nome EXATO do seu arquivo de cena.
        SceneManager.LoadScene("GameOver");
    }
}