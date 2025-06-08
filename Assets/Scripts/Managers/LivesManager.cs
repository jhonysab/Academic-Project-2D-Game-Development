using UnityEngine;
using TMPro;

public class PlayerLivesManager : MonoBehaviour
{
    public int vidasIniciais = 15;
    private int vidasAtuais;

    public TextMeshProUGUI vidasTexto; // arraste o texto da UI aqui

    void Start()
    {
        vidasAtuais = vidasIniciais;
        AtualizarUI();
    }

    public void PerderVida()
    {
        vidasAtuais = Mathf.Max(vidasAtuais - 1, 0);
        AtualizarUI();

        if (vidasAtuais <= 0)
        {
            Debug.Log("GAME OVER");
            // Aqui você pode desativar o jogo, mostrar menu, etc.
        }
    }

    private void AtualizarUI()
    {
        vidasTexto.text = vidasAtuais.ToString();
    }
}
