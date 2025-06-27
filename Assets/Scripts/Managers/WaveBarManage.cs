// Arquivo: WaveManager.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [Header("Referências dos Gerentes")]
    public LevelManager levelManager; // Arraste seu objeto LevelManager aqui

    [Header("Configuração das Ondas")]
    public int totalWaves = 10;
    private int currentWave = 0; // Começa em 0 para que a primeira onda seja a 1

    [Header("Referências da UI")]
    public Slider waveProgressBar;
    public TextMeshProUGUI waveText;
    
    // Este é o novo método que o LevelManager vai chamar para iniciar tudo.
    public void StartSpawning()
    {
        currentWave = 0; // Garante que começa do zero
        StartNextWave();
    }

    // Esta função pode ser chamada quando o jogador derrota a onda atual.
    public void StartNextWave()
    {
        currentWave++;

        // --- LÓGICA DE VITÓRIA ---
        // Verifica se a nova onda a ser iniciada está além do total.
        if (currentWave > totalWaves)
        {
            // Se sim, todas as ondas foram completadas.
            levelManager.OnAllWavesCompleted(); // AVISA O CHEFE!
            return; // Para a execução para não tentar spawnar mais nada.
        }

        UpdateWaveUI();

        // --- COLOQUE SUA LÓGICA DE SPAWN DOS INIMIGOS AQUI ---
        Debug.Log($"WaveManager: Iniciando a onda {currentWave}");
        // Ex: StartCoroutine(SpawnWave(currentWave));
    }

    private void UpdateWaveUI()
    {
        if (waveText != null)
        {
            // Mostra a onda atual, mas limita o máximo ao total de ondas
            waveText.text = $"Onda {Mathf.Min(currentWave, totalWaves)} / {totalWaves}";
        }

        if (waveProgressBar != null)
        {
            waveProgressBar.value = (float)currentWave / totalWaves;
        }
    }
}