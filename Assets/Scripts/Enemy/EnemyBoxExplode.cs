using UnityEngine;

public class EnemyBoxExplode : MonoBehaviour
{
    public int baseDamage = 10;
    bool hasExploded = false;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasExploded) return;

        if (collision.gameObject.CompareTag("Fortress"))
        {
            hasExploded = true;

            FortressHealth fortress =
                collision.gameObject.GetComponent<FortressHealth>();

            if (fortress != null)
            {
                int damageToFortress = (int)GetFinalDamage();
                fortress.TakeDamage(damageToFortress);
            }

            // Optional: play explosion effect here

            Destroy(gameObject);
        }
    }

    float GetFinalDamage()
    {
        float mult = 1f;
        if (LevelManager.Instance != null)
            mult = LevelManager.Instance.GetDifficultyMultiplier();

        return baseDamage * mult;
    }

}