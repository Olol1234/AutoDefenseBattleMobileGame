using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SpawnEntry
{
    [Header("Enemy Table")]
    public GameObject enemyPrefab;
    public float startTime;   // seconds into stage
    public float weight = 1f; // spawn chance weight
}

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    [Header("Spawn Settings")]
    public float spawnInterval = 2.5f;
    public float spawnYOffset = 0.5f;
    private float spawnTimer = 0f;
    public bool spawnActive = true;

    [Header("Tracking")]
    public int activeEnemyCount = 0;

    // [Header("Enemy Table")]
    // public SpawnEntry[] spawnTable;

    [Header("Stage Configs")]
    public StageConfig[] stageConfigs;

    // float nextSpawnTime;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (!spawnActive) return;
        if (LevelManager.Instance.IsLevelOver()) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= GetScaledSpawnInterval())
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }
    }

    StageConfig GetCurrentStageConfig()
    {
        int stage = LevelManager.Instance.currentStage;

        foreach (var config in stageConfigs)
        {
            if (config.stageNumber == stage)
                return config;
        }

        // Fallback: last config for endless scaling
        return stageConfigs[stageConfigs.Length - 1];
    }

    void SpawnEnemy()
    {
        if (Camera.main == null) return;

        GameObject prefab = PickEnemyPrefab();
        if (prefab == null) return;

        Camera cam = Camera.main;

        Vector3 topLeft = cam.ViewportToWorldPoint(new Vector3(0, 1, cam.nearClipPlane));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));

        float randomX = Random.Range(topLeft.x, topRight.x);
        float spawnY = topLeft.y + spawnYOffset;

        Vector3 spawnPos = new Vector3(randomX, spawnY, 0f);

        GameObject enemyGO = Instantiate(prefab, spawnPos, Quaternion.identity);
        ApplyStageScaling(enemyGO);

        activeEnemyCount++;

    }

    void ApplyStageScaling(GameObject enemyGO)
    {
        StageConfig config = GetCurrentStageConfig();

        var health = enemyGO.GetComponent<HealthEnemy>();
        if (health != null)
        {
            health.ApplyHealthMultiplier(config.stageHealthMultiplier);
        }

    }

    public void StopSpawning()
    {
        spawnActive = false;
        Debug.Log("Spawner stopped. Cleanup phase.");
    }

    public void OnEnemyDestroyed()
    {
        activeEnemyCount--;

        if (activeEnemyCount < 0)
            activeEnemyCount = 0;

        LevelManager.Instance.CheckForStageClear();
    }

    GameObject PickEnemyPrefab()
    {
        float t = LevelManager.Instance.ElapsedTime;

        StageConfig config = GetCurrentStageConfig();
        var table = config.spawnTable;

        if (table == null || table.Length == 0)
            return null;

        List<SpawnEntry> available = new List<SpawnEntry>();

        foreach (var entry in table)
        {
            if (t >= entry.startTime && entry.enemyPrefab != null)
                available.Add(entry);
        }

        if (available.Count == 0)
            return null;

        float totalWeight = 0f;
        foreach (var e in available)
            totalWeight += e.weight;

        float r = Random.Range(0, totalWeight);
        float acc = 0f;

        foreach (var e in available)
        {
            acc += e.weight;
            if (r <= acc)
                return e.enemyPrefab;
        }

        return available[available.Count - 1].enemyPrefab;
    }


    float GetScaledSpawnInterval()
    {
        float timeMult = LevelManager.Instance.GetDifficultyMultiplier();
        StageConfig config = GetCurrentStageConfig();

        float finalMult = timeMult * config.stageSpawnRateMultiplier;
        return spawnInterval / finalMult;
    }

}
