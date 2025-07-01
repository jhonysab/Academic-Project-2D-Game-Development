// Arquivo: LevelManager.cs (VERSÃO FINAL COM GAME OVER)
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Essencial para carregar cenas
using TMPro;                   // Essencial para TextMeshPro

public class LevelManager : MonoBehaviour
{
    // --- Sua Lógica de Singleton e Rotas (Mantida) ---
    public static LevelManager main;

    [System.Serializable]
    public class SpawnRoute
    {
        public Transform spawnPoint;
        public Transform[] path;
    }

    [Header("Configuração de Rotas de Spawn")]
    public SpawnRoute[] spawnRoutes;

    // --- Configurações do Nível e Referências (Unificadas) ---
    [Header("Referências dos Gerentes e UI")]
    public WaveBarManage waveBarManage;
    public TextMeshProUGUI countdownText;

    [Header("Configuração do Nível")]
    public float countdownTime = 30f;
    
    // --- NOVAS ADIÇÕES PARA O GAME OVER ---
    [Header("Configuração de Fim de Jogo")]
    [Tooltip("O nome exato do seu arquivo de cena de Game Over.")]
    public string GameOver = "GameOver";
    private bool isGameOver = false; // Trava para garantir que o Game Over só seja chamado uma vez

    // --- Métodos da Unity ---

    private void Awake()
    {
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
        isGameOver = false; // Garante que o estado de jogo é resetado no início
        Time.timeScale = 1f; // Garante que o tempo está correndo normalmente no início do nível

        if (waveBarManage == null || countdownText == null)
        {
            Debug.LogError("Referências para WaveBarManage ou CountdownText não foram configuradas no LevelManager!");
            return;
        }

        StartCoroutine(StartLevelCountdown());
    }

    // --- Lógica do Jogo ---

    private IEnumerator StartLevelCountdown()
    {
        Debug.Log("LevelManager: Iniciando cronômetro de preparação...");
        countdownText.gameObject.SetActive(true);
        float currentTime = countdownTime;
        while (currentTime > 0)
        {
            countdownText.text = Mathf.Ceil(currentTime).ToString();
            currentTime -= Time.deltaTime;
            yield return null;
        }
        countdownText.text = "INVADINDO!";
        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false);
        
        waveBarManage.StartSpawning();
    }

    public void OnAllWavesCompleted()
    {
        Debug.Log("LEVEL MANAGER FOI AVISADO: TODAS AS ONDAS COMPLETAS! VITÓRIA!");
        // --- COLOQUE SUA LÓGICA DE VITÓRIA AQUI ---
    }

    // --- NOVO MÉTODO PÚBLICO PARA O GAME OVER ---
    /// <summary>
    /// Esta função é o "botão de pânico". Qualquer script pode chamá-la para acabar o jogo.
    /// </summary>
    public void TriggerGameOver()
    {
        // A trava de segurança para não executar o Game Over múltiplas vezes.
        if (isGameOver) return;
        
        isGameOver = true;
        Debug.Log("GAME OVER! Carregando a cena de Game Over...");

        // Carrega a sua cena de Game Over definida no Inspector.
        SceneManager.LoadScene(GameOver);
    }
}