using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager main;

    [System.Serializable]
    public class SpawnRoute
    {
        public Transform spawnPoint;
        public Transform[] path;
    }

    public SpawnRoute[] spawnRoutes;

    private void Awake()
    {
        main = this;
    }

        [Header("Referências dos Gerentes")]
    public WaveManager waveManager; // Arraste seu objeto WaveManager aqui no Inspector

    // Você pode ter outras referências aqui, como o PlayerStatus, etc.

    void Start()
    {
        // Verifica se a referência ao WaveManager foi configurada para evitar erros.
        if (waveManager == null)
        {
            Debug.LogError("A referência ao WaveManager não foi configurada no LevelManager!");
            return;
        }

        // Inicia o nível
        StartLevel();
    }

    private void StartLevel()
    {
        Debug.Log("LevelManager: Iniciando o nível...");
        // Manda o WaveManager começar a primeira onda.
        waveManager.StartSpawning();
    }

    // Esta é a função que o WaveManager vai chamar quando todas as ondas terminarem.
    public void OnAllWavesCompleted()
    {
        Debug.Log("LEVEL MANAGER FOI AVISADO: TODAS AS ONDAS COMPLETAS! VITÓRIA!");

        // --- COLOQUE SUA LÓGICA DE VITÓRIA AQUI ---
        // Ex: Mostrar uma tela de vitória, parar o tempo, dar ouro ao jogador, etc.
        // GameObject painelDeVitoria = ...
        // painelDeVitoria.SetActive(true);
        // Time.timeScale = 0f;
    }
}