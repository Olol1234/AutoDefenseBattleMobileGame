using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float floatSpeed = 1.5f;
    public float lifetime = 1.0f;
    public float fadeSpeed = 2.5f;

    public TMP_Text text;
    void Awake()
    {
        if (text == null)
            Debug.LogError("DamageText: Text reference not assigned!");
    }

    public void SetDamage(int amount)
    {
        if (text != null)
            text.text = amount.ToString("F1");
    }

    void Update()
    {
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        if (text != null)
        {
            Color c = text.color;
            c.a -= fadeSpeed * Time.deltaTime;
            text.color = c;
        }

        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
