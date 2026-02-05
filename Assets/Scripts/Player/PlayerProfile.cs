using UnityEngine;
using System.Collections;

public class PlayerProfile : MonoBehaviour
{
    public static PlayerProfile Instance;

    public int coins;
    public int highestStageCleared;
    // Base stats for player
    public float baseDamage;
    public float baseFortressMaxHP;
    public float baseFireRate;

    // Base stats for homing missile turret
    public bool homingMissileTurretUnlocked;
    public float homingMissileTurretBaseDamage;
    public int homingMissileTurretDamageLevel;
    public float homingMissileTurretCooldown;
    public int homingMissileTurretCooldownLevel;

    public bool IsLoaded { get; private set; } = false;
    // Event for UI + systems
    public static System.Action OnProfileLoaded;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(InitAfterSaveManager());
            // LoadFromSave();
        }
        else Destroy(gameObject);
    }

    IEnumerator InitAfterSaveManager()
    {
        while (SaveManager.Instance == null)
            yield return null;

        LoadFromSave();
    }

    public void LoadFromSave()
    {
        var data = SaveManager.Instance.Data;

        coins = data.coins;
        highestStageCleared = data.highestStageCleared;
        // PLAYER DATA
        baseDamage = data.baseDamage;
        baseFortressMaxHP = data.baseFortressMaxHP;
        baseFireRate = data.baseFireRate;
        // HOMING MISSILE TURRET DATA
        homingMissileTurretUnlocked = data.homingMissileTurretUnlocked;
        homingMissileTurretBaseDamage = data.homingMissileTurretBaseDamage;
        homingMissileTurretDamageLevel = data.homingMissileTurretDamageLevel;
        homingMissileTurretCooldown = data.homingMissileTurretCooldown;
        homingMissileTurretCooldownLevel = data.homingMissileTurretCooldownLevel;

        IsLoaded = true;

        Debug.Log("PlayerProfile loaded from save");
        // Notify listeners (UI, etc)
        OnProfileLoaded?.Invoke();
    }

    public void SaveToDisk()
    {
        var data = SaveManager.Instance.Data;

        data.coins = coins;
        data.highestStageCleared = highestStageCleared;
        // PLAYER DATA
        data.baseDamage = baseDamage;
        data.baseFortressMaxHP = baseFortressMaxHP;
        data.baseFireRate = baseFireRate;
        // HOMING MISSILE TURRET DATA
        data.homingMissileTurretUnlocked = homingMissileTurretUnlocked;
        data.homingMissileTurretBaseDamage = homingMissileTurretBaseDamage;
        data.homingMissileTurretDamageLevel = homingMissileTurretDamageLevel;
        data.homingMissileTurretCooldown = homingMissileTurretCooldown;
        data.homingMissileTurretCooldownLevel = homingMissileTurretCooldownLevel;

        SaveManager.Instance.Save();
    }
}
