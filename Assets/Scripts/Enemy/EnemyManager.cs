using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    // A list of all enemies currently alive in the scene
    public List<HealthEnemy> ActiveEnemies = new List<HealthEnemy>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterEnemy(HealthEnemy enemy)
    {
        if (!ActiveEnemies.Contains(enemy))
            ActiveEnemies.Add(enemy);
    }

    public void UnregisterEnemy(HealthEnemy enemy)
    {
        if (ActiveEnemies.Contains(enemy))
            ActiveEnemies.Remove(enemy);
    }
}