using UnityEngine;
using UnityEngine.UI; // Essencial para acessar componentes de UI como Image

public class HealthUI : MonoBehaviour
{
    [Header("Referências da Barra de Vida")]
    [Tooltip("Arraste aqui a imagem da UI que servirá como o preenchimento da barra de vida.")]
    public Image healthBarFill; // A imagem que vai encher/esvaziar

    [Header("Configurações (Opcional)")]
    [Tooltip("Quão rápido a barra de vida se move. Deixe em 0 para uma transição instantânea.")]
    [SerializeField] private float smoothSpeed = 5f;

    private float targetFillAmount = 1f; // O valor alvo para o preenchimento (de 0.0 a 1.0)

    // O Update é usado para animar a barra de forma suave
    private void Update()
    {
        if (healthBarFill != null)
        {
            // Interpola suavemente o preenchimento atual em direção ao preenchimento alvo
            healthBarFill.fillAmount = Mathf.Lerp(healthBarFill.fillAmount, targetFillAmount, Time.deltaTime * smoothSpeed);
        }
    }

    // Este é o método público que o seu script Player_Health vai chamar
    public void UpdateBar(int currentHealth, int maxHealth)
    {
        // Garante que não haja divisão por zero se a vida máxima for 0
        if (maxHealth > 0)
        {
            // Calcula a porcentagem de vida (ex: 80 / 100 = 0.8)
            targetFillAmount = (float)currentHealth / maxHealth;
        }
        else
        {
            targetFillAmount = 0;
        }
    }
}