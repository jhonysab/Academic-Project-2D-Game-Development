// Arquivo: EnemySpawner.cs (COM A CORREÇÃO DE SETPATH)
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public GameObject enemyPrefab;
        public int enemyCount;
        public float spawnRate;
    }

    [Header("Referências dos Gerentes")]
    public WaveBarManage waveBarManage;
    public LevelManager levelManager;

    [Header("Configuração das Ondas")]
    public Wave[] waves;

    private int enemiesAlive = 0;

    public void SpawnWave(int waveIndex)
    {
        if (waveIndex - 1 < waves.Length)
        {
            Wave currentWave = waves[waveIndex - 1];
            enemiesAlive = currentWave.enemyCount;
            StartCoroutine(SpawnWaveCoroutine(currentWave));
        }
        else
        {
            Debug.LogError("Tentativa de iniciar uma onda que não existe!");
        }
    }

    private IEnumerator SpawnWaveCoroutine(Wave wave)
    {
        Debug.Log("Spawner: Spawnando " + wave.enemyCount + " inimigos.");
        Transform spawnPoint = levelManager.spawnRoutes[0].spawnPoint;

        for (int i = 0; i < wave.enemyCount; i++)
        {
            // --- BLOCO CORRIGIDO ---
            GameObject newEnemy = Instantiate(wave.enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            EnemyCombinedMovement movement = newEnemy.GetComponent<EnemyCombinedMovement>();
            Transform[] path = levelManager.spawnRoutes[0].path;

            if (movement != null && path != null && path.Length > 0)
            {
                movement.SetPath(path);
            }
            else
            {
                Debug.LogError("Não foi possível definir o caminho do inimigo!");
            }
            // --- FIM DO BLOCO CORRIGIDO ---
            
            yield return new WaitForSeconds(wave.spawnRate);
        }
    }

    public void OnEnemyDied()
    {
        enemiesAlive--;
        if (enemiesAlive <= 0)
        {
            waveBarManage.OnWaveCleared();
        }
    }
}