using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public Image barraDeVida;
    private float velocidade = 5f; // Quanto maior, mais rápida a transição
    private float alvoFill = 1f;

    void Update()
    {
        if (barraDeVida != null)
        {
            barraDeVida.fillAmount = Mathf.Lerp(barraDeVida.fillAmount, alvoFill, Time.deltaTime * velocidade);
        }
    }

    public void UpdateBar(int vidaAtual, int vidaMaxima)
    {
        alvoFill = (float)vidaAtual / vidaMaxima;
    }
}