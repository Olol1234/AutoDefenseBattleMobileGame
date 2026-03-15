using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    // We map each Prefab to a Queue of its inactive instances
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public GameObject GetFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null) return null;

        if (!poolDictionary.ContainsKey(prefab))
        {
            poolDictionary.Add(prefab, new Queue<GameObject>());
        }

        GameObject obj;

        // If the queue is empty, instantiate a new one (Expanding the pool)
        if (poolDictionary[prefab].Count == 0)
        {
            obj = Instantiate(prefab);

            Bullet bulletScript = obj.GetComponent<Bullet>();
            if (bulletScript != null) {
                bulletScript.bulletPrefab = prefab;
            }

            EnemyBullet enemyBulletScript = obj.GetComponent<EnemyBullet>();
            if (enemyBulletScript != null) {
                enemyBulletScript.enemyBulletPrefab = prefab;
            }

            HomingMissile missileScript = obj.GetComponent<HomingMissile>();
            if (missileScript != null) {
                missileScript.myPrefab = prefab;
            }

            HealthEnemy enemyHealthScript = obj.GetComponent<HealthEnemy>();
            if (enemyHealthScript != null) {
                enemyHealthScript.myPrefab = prefab;
            }

            ShotgunPellet pelletScript = obj.GetComponent<ShotgunPellet>();
            if (pelletScript != null) {
                pelletScript.myPrefab = prefab;        
            }
        }
        else
        {
            obj = poolDictionary[prefab].Dequeue();
        }

        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);

        return obj;
    }

    public void ReturnToPool(GameObject prefab, GameObject instance)
    {
        instance.SetActive(false);
        poolDictionary[prefab].Enqueue(instance);
    }
}