using UnityEngine;

[System.Serializable]
public class StageConfig
{
    public int stageNumber;

    [Header("Spawn Table")]
    public SpawnEntry[] spawnTable;

    [Header("Difficulty")]
    public float stageHealthMultiplier = 1f;
    public float stageSpeedMultiplier = 1f;
    public float stageSpawnRateMultiplier = 1f;
}
