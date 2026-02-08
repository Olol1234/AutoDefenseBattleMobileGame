using System.Collections.Generic;
using UnityEngine;

public class ShockwaveExpand : MonoBehaviour
{
    public float maxScale = 3f;
    public float expandSpeed = 4f;
    public float fadeSpeed = 2.5f;
    public float damage;

    private SpriteRenderer sr;
    private CircleCollider2D collider;
    private HashSet<HealthEnemy> affectedEnemies = new HashSet<HealthEnemy>();


    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        collider = GetComponent<CircleCollider2D>();

        collider.isTrigger = true;
    }

    void Update()
    {
        // Collider ring
        collider.radius += expandSpeed * Time.deltaTime;
        // Expand ring
        float visualDiameter = collider.radius * 2f;
        transform.localScale = Vector3.one * visualDiameter;

        // test
        // Debug.Log(collider.radius);

        // Fade out
        if (sr != null)
        {
            Color c = sr.color;
            c.a -= fadeSpeed * Time.deltaTime;
            sr.color = c;

            // if (c.a <= 0f)
            //     Destroy(gameObject);
        }

        if (collider.radius >= maxScale)
        {
            collider.enabled = false;
            Destroy(gameObject, 0.1f);
        }

        // Safety destroy
        if (transform.localScale.x >= maxScale)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;

        HealthEnemy enemyHealth = other.GetComponent<HealthEnemy>();
        if (enemyHealth == null) return;
        if (affectedEnemies.Contains(enemyHealth)) return;
        affectedEnemies.Add(enemyHealth);
        enemyHealth.TakeDamage(damage * 0.7f);
    }

}
