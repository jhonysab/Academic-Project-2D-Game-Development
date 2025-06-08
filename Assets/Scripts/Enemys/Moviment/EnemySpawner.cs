using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemyGroup
    {
        public GameObject enemyPrefab;
        public int quantity = 1;
        public int routeIndex = 0;
    }

    [System.Serializable]
    public class Wave
    {
        public List<EnemyGroup> enemyGroups = new List<EnemyGroup>();
        public float spawnRate = 1f;
    }

    [Header("Waves Personalizadas")]
    [SerializeField] private List<Wave> waves = new List<Wave>();
    [SerializeField] private float timeBetweenWaves = 5f;

    [Header("Events")]
    public static UnityEvent onEnemyDestroy = new UnityEvent();

    private int currentWave = 0;
    private float timeSinceLastSpawn;
    private bool isSpawning = false;

    private Queue<GameObject> spawnQueue = new Queue<GameObject>();
    private int enemiesAlive;

    private void Awake()
    {
        onEnemyDestroy.AddListener(EnemyDestroyed);
    }

    private void Start()
    {
    // Agora não começamos automaticamente
    // Esperamos o StartFirstWave() ser chamado
    }

    public GameObject botaoWave;

    public void StartFirstWave()
    {
        Debug.Log("Iniciando manualmente a primeira onda.");
        StartCoroutine(StartWave());

        if (botaoWave != null)
            botaoWave.SetActive(false); // Desativa o botão após clicar
    }


    private void Update()
    {
        if (isSpawning)
        {
            timeSinceLastSpawn += Time.deltaTime;

            if (spawnQueue.Count > 0)
            {
                Wave wave = waves[currentWave];

                if (timeSinceLastSpawn >= (1f / wave.spawnRate))
                {
                    GameObject enemyToSpawn = spawnQueue.Dequeue();
                    SpawnEnemy(enemyToSpawn);
                    enemiesAlive++;
                    timeSinceLastSpawn = 0f;
                }
            }

            // ✅ Verifica fim da wave separadamente
            if (spawnQueue.Count == 0 && enemiesAlive == 0)
            {
                EndWave();
            }
        }
    }


    private void EnemyDestroyed()
    {
        enemiesAlive--;
    }

    private IEnumerator StartWave()
    {
        if (currentWave >= waves.Count)
        {
            Debug.Log("Todas as ondas finalizadas!");
            yield break;
        }

        Debug.Log($"Iniciando Onda {currentWave + 1}");

        Wave wave = waves[currentWave];
        foreach (var group in wave.enemyGroups)
        {
            for (int i = 0; i < group.quantity; i++)
            {
                GameObject wrapped = WrapEnemyPrefab(group.enemyPrefab, group.routeIndex);
                spawnQueue.Enqueue(wrapped);
            }
        }

        yield return new WaitForSeconds(timeBetweenWaves);
        isSpawning = true;
    }

    private void EndWave()
    {
        isSpawning = false;
        timeSinceLastSpawn = 0f;
        currentWave++;
        StartCoroutine(StartWave());
    }

    private void SpawnEnemy(GameObject wrappedEnemy)
    {
        // Desempacota o inimigo e a rota
        EnemySpawnWrapper wrapper = wrappedEnemy.GetComponent<EnemySpawnWrapper>();
        if (wrapper == null) return;

        LevelManager.SpawnRoute route = LevelManager.main.spawnRoutes[wrapper.routeIndex];
        GameObject enemy = Instantiate(wrapper.prefab, route.spawnPoint.position, Quaternion.identity);

        EnemyCombinedMovement movement = enemy.GetComponent<EnemyCombinedMovement>();
        if (movement != null)
        {
            movement.SetPath(route.path);
        }
    }

    // Cria um "pacote" com prefab + rota (pra Queue funcionar)
    private GameObject WrapEnemyPrefab(GameObject prefab, int routeIndex)
    {
        GameObject wrapper = new GameObject("EnemyWrapper");
        var comp = wrapper.AddComponent<EnemySpawnWrapper>();
        comp.prefab = prefab;
        comp.routeIndex = routeIndex;
        return wrapper;
    }

    // Componente auxiliar pra empacotar info na fila
    private class EnemySpawnWrapper : MonoBehaviour
    {
        public GameObject prefab;
        public int routeIndex;
    }
}
