using UnityEngine;

public class DamageTextSpawner : MonoBehaviour
{
    public static DamageTextSpawner Instance;

    public DamageText damageTextPrefab;

    void Awake()
    {
        Instance = this;
    }

    public void Spawn(int amount, Vector3 worldPos)
    {
        if (damageTextPrefab == null) return;

        // Small random offset for juice
        Vector3 offset = new Vector3(
            Random.Range(-0.2f, 0.2f),
            Random.Range(0.1f, 0.3f),
            0f
        );

        DamageText dt = Instantiate(
            damageTextPrefab,
            worldPos + offset,
            Quaternion.identity
        );

        dt.SetDamage(amount);
    }
}
