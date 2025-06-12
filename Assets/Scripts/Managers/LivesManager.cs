using UnityEngine;
using TMPro;

// O nome da classe deve corresponder ao que está no seu arquivo
public class PlayerLivesManager : MonoBehaviour
{
    // NOVO: Singleton para acesso global fácil
    public static PlayerLivesManager main;

    public int vidasIniciais = 15;
    private int vidasAtuais;

    public TextMeshProUGUI vidasTexto; // arraste o texto da UI aqui

    private void Awake()
    {
        // Configuração do Singleton
        if (main == null)
        {
            main = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        vidasAtuais = vidasIniciais;
        AtualizarUI();
    }

    // MÉTODO MODIFICADO: Agora aceita a quantidade de vidas a perder
    // O '= 1' significa que se o método for chamado sem um número, ele perderá 1 vida por padrão.
    public void PerderVida(int quantidade = 1)
    {
        vidasAtuais = Mathf.Max(vidasAtuais - quantidade, 0);
        AtualizarUI();

        Debug.Log("Base levou " + quantidade + " de dano! Vidas restantes: " + vidasAtuais);

        if (vidasAtuais <= 0)
        {
            Debug.Log("GAME OVER - A base foi destruída.");
            // Aqui você pode desativar o jogo, mostrar menu, etc.
            Time.timeScale = 0f; // Pausa o jogo
        }
    }

    private void AtualizarUI()
    {
        if (vidasTexto != null)
        {
            vidasTexto.text = vidasAtuais.ToString();
        }
    }
}