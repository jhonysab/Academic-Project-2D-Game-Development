// Arquivo: WaveBarManage.cs
using System.Collections; // Necessário para Coroutines (pausas)
using UnityEngine;
using UnityEngine.UI;    // Necessário para o Slider
using TMPro;             // Necessário para o TextMeshPro

public class WaveBarManage : MonoBehaviour
{
    [Header("Referências de Gerenciamento")]
    [Tooltip("Arraste o objeto LevelManager da sua cena aqui.")]
    public LevelManager levelManager;
    public EnemySpawner enemySpawner;

    [Header("Configuração das Ondas")]
    [Tooltip("O número total de ondas para o jogador vencer este nível.")]
    public int totalWaves = 10;
    [Tooltip("O tempo em segundos que o jogador tem para respirar entre uma onda e outra.")]
    public float timeBetweenWaves = 5f;

    private int currentWave = 0;
    private bool isSpawningWaves = false; // Para evitar que as ondas comecem mais de uma vez

    [Header("Referências da UI")]
    [Tooltip("Arraste o componente Slider da sua UI aqui.")]
    public Slider waveProgressBar;
    [Tooltip("Arraste o componente TextMeshProUGUI da sua UI aqui.")]
    public TextMeshProUGUI waveText;

    /// <summary>
    /// Este é o ponto de entrada principal, chamado pelo LevelManager após o cronômetro inicial.
    /// </summary>
    public void StartSpawning()
    {
        // Se já estiver spawnando, não faz nada para evitar bugs.
        if (isSpawningWaves) return;

        isSpawningWaves = true;
        currentWave = 0; // Reseta a contagem para o início do nível
        StartNextWaveInternal(); // Chama a lógica interna para iniciar a primeira onda
    }

    /// <summary>
    /// Esta função pública deve ser chamada pelo seu sistema de jogo quando o último inimigo de uma onda for derrotado.
    /// </summary>
    public void OnWaveCleared()
    {
        Debug.Log("Onda " + currentWave + " foi finalizada! Preparando a próxima em " + timeBetweenWaves + " segundos.");
        // Inicia uma coroutine para esperar um pouco antes de chamar a próxima onda.
        StartCoroutine(NextWaveCountdown());
    }

    /// <summary>
    /// Coroutine que cria uma pausa entre as ondas.
    /// </summary>
    private IEnumerator NextWaveCountdown()
    {
        // Aqui você poderia ativar um texto na tela "Próxima onda em X segundos..."
        yield return new WaitForSeconds(timeBetweenWaves);
        StartNextWaveInternal();
    }

    /// <summary>
    /// Lógica central que avança para a próxima onda, verifica a condição de vitória e atualiza a UI.
    /// </summary>
private void StartNextWaveInternal()
{
    currentWave++;

    if (currentWave > totalWaves)
    {
        // ... (lógica de vitória continua a mesma) ...
        return;
    }

    UpdateWaveUI();

    // --- CHAMADA PARA O SPAWNER ---
    Debug.Log($"WaveBarManage: Dando ordem para o Spawner iniciar a Onda {currentWave}.");
    if (enemySpawner != null)
    {
        enemySpawner.SpawnWave(currentWave); // <--- ESTA É A LINHA IMPORTANTE
    }
    else
    {
        Debug.LogError("Referência ao EnemySpawner não configurada no WaveBarManage!");
    }
}
    /// <summary>
    /// Atualiza os elementos visuais da UI (barra e texto).
    /// </summary>
    private void UpdateWaveUI()
    {
        // Atualiza o texto, garantindo que não mostre "Onda 11 / 10" no final
        if (waveText != null)
        {
            waveText.text = $"Wave {Mathf.Min(currentWave, totalWaves)} / {totalWaves}";
        }

        // Atualiza o valor da barra de progresso
        if (waveProgressBar != null)
        {
            waveProgressBar.value = (float)currentWave / totalWaves;
        }
    }
}