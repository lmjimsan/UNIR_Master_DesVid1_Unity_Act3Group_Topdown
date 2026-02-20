using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveSpawner : MonoBehaviour
{
    [Header("Spawner Identity")]
    [SerializeField] private string spawnerId = "DoorA";

    [Header("Wave Timing")]
    [SerializeField] private int waveCount = 3;
    [SerializeField] private float timeBetweenWaves = 1f;
    [SerializeField] private float timeBetweenSpawns = 2f;

    [Header("Enemy Slot A")]
    [SerializeField] private GameObject enemyPrefabA;
    [SerializeField] private int enemyCountA = 5;

    [Header("Enemy Slot B")]
    [SerializeField] private GameObject enemyPrefabB;
    [SerializeField] private int enemyCountB = 1;

    [Header("Spawn Limits")]
    [SerializeField] private int maxAlive = 6;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Control")]
    [SerializeField] private bool startWhenPlayerNear = true;
    [SerializeField] private float startRadius = 7f;
    [SerializeField] private string playerTag = "Player";

    private readonly List<GameObject> spawned = new List<GameObject>();
    private Coroutine spawnRoutine;
    private Transform player;
    private bool hasStarted;

    public string SpawnerId => spawnerId;

    private void Awake()
    {
        if (maxAlive <= 0)
        {
            maxAlive = Mathf.Max(1, enemyCountA + enemyCountB);
        }
    }

    private void OnEnable()
    {
        TryCachePlayer();
        if (!startWhenPlayerNear)
        {
            StartWaves();
            hasStarted = true;
        }
    }

    private void Update()
    {
        if (hasStarted || spawnRoutine != null || !startWhenPlayerNear) return;

        if (player == null)
        {
            TryCachePlayer();
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= startRadius)
        {
            StartWaves();
            hasStarted = true;
        }
    }

    public void StartWaves()
    {
        if (spawnRoutine != null) return;
        spawnRoutine = StartCoroutine(SpawnWaves());
    }

    public void StopWaves()
    {
        if (spawnRoutine == null) return;
        StopCoroutine(spawnRoutine);
        spawnRoutine = null;
    }

    private void TryCachePlayer()
    {
        if (string.IsNullOrWhiteSpace(playerTag)) return;
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    private IEnumerator SpawnWaves()
    {
        for (int wave = 0; wave < waveCount; wave++)
        {
            int remainingA = enemyCountA;
            int remainingB = enemyCountB;

            // Spawnea enemigos sin superar el maximo en escena.
            while (remainingA > 0 || remainingB > 0)
            {
                CleanupSpawned();

                if (spawned.Count >= maxAlive)
                {
                    yield return new WaitForSeconds(timeBetweenSpawns);
                    continue;
                }

                bool canSpawnA = remainingA > 0 && enemyPrefabA != null;
                bool canSpawnB = remainingB > 0 && enemyPrefabB != null;

                if (canSpawnA && canSpawnB)
                {
                    if (Random.value < 0.5f)
                    {
                        Spawn(enemyPrefabA);
                        remainingA--;
                    }
                    else
                    {
                        Spawn(enemyPrefabB);
                        remainingB--;
                    }
                }
                else if (canSpawnA)
                {
                    Spawn(enemyPrefabA);
                    remainingA--;
                }
                else if (canSpawnB)
                {
                    Spawn(enemyPrefabB);
                    remainingB--;
                }
                else
                {
                    break;
                }

                yield return new WaitForSeconds(timeBetweenSpawns);
            }

            // Espera a que mueran los enemigos de la oleada antes de continuar.
            while (spawned.Count > 0)
            {
                CleanupSpawned();
                yield return new WaitForSeconds(timeBetweenSpawns);
            }

            if (timeBetweenWaves > 0f)
            {
                yield return new WaitForSeconds(timeBetweenWaves);
            }
        }

        spawnRoutine = null;
    }

    private void Spawn(GameObject prefab)
    {
        if (prefab == null) return;

        Transform point = GetSpawnPoint();
        Vector3 position = point != null ? point.position : transform.position;

        GameObject instance = Instantiate(prefab, position, Quaternion.identity);
        if (instance != null)
        {
            spawned.Add(instance);
        }
    }

    private Transform GetSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return null;
        int index = Random.Range(0, spawnPoints.Length);
        return spawnPoints[index];
    }

    private void CleanupSpawned()
    {
        for (int i = spawned.Count - 1; i >= 0; i--)
        {
            if (spawned[i] == null)
            {
                spawned.RemoveAt(i);
            }
        }
    }
}
