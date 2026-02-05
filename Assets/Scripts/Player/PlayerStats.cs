using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("Base Stats (from PlayerProfile)")]
    [SerializeField] public float baseDamage;
    [SerializeField] public float baseAttackSpeed;

    [Header("Run Stats")]
    public float damageMultiplier = 1f;
    public float damageAddition = 0f;
    public float damagePenalty = 0f;
    public int extraBullets = 0;
    public int penetration = 0;
    public float attackSpeedMultiplier = 1f;

    private void Awake()
    {
        Instance = this;
    }

    public void InitFromProfile()
    {
        baseDamage = PlayerProfile.Instance.baseDamage;
        baseAttackSpeed = PlayerProfile.Instance.baseFireRate;

        ResetRunStats();
    }

    public void ResetRunStats()
    {
        damageMultiplier = 1f;
        damageAddition = 0f;
        damagePenalty = 0f;
        extraBullets = 0;
        penetration = 0;
        attackSpeedMultiplier = 1f;
    }

    public float GetDamage()
    {
        // return baseDamage * damageMultiplier;
        float basePlusAdd = baseDamage + damageAddition;
        float penalty = baseDamage * 0.25f * Mathf.Pow(extraBullets, 0.55f);

        return Mathf.Max(1f, basePlusAdd - penalty);
    }

    public float GetAttackSpeed()
    {
        return baseAttackSpeed / attackSpeedMultiplier;
    }

    public float GetExtraBullets()
    {
        return extraBullets;
    }

    public float GetPenetration()
    {
        return penetration;
    }
}
