using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    private List<UpgradeData> allUpgrades = new List<UpgradeData>();

    private void Awake()
    {
        Instance = this;
        InitUpgrades();
    }

    void InitUpgrades()
    {
        allUpgrades.Add(new UpgradeData(
            UpgradeType.DamagePercent,
            "+50% Damage",
            "Increase damage by 50%"
        ));

        allUpgrades.Add(new UpgradeData(
            UpgradeType.ExtraBullet,
            "+1 Bullet",
            "Fire an additional bullet"
        ));

        allUpgrades.Add(new UpgradeData(
            UpgradeType.Penetration,
            "+1 Penetration",
            "Bullets pierce 1 more enemy"
        ));

        allUpgrades.Add(new UpgradeData(
            UpgradeType.AttackSpeed,
            "+50% Attack Speed",
            "Shoot faster"
        ));

        allUpgrades.Add(new UpgradeData(
            UpgradeType.HomingMissileTurret,
            "Homing Missile Turret",
            "Deploy a Homing Missile Turret to assist you in battle"
        ));

        allUpgrades.Add(new UpgradeData(
            UpgradeType.HomingMissileTurretCooldown,
            "-10% Turret Cooldown",
            "Reduce Homing Missile Turret's cooldown by 10%"
        ));

        allUpgrades.Add(new UpgradeData(
            UpgradeType.HomingMissileTurretDamagePercent,
            "+20% Turret Damage",
            "Increase Homing Missile Turret's damage by 20%"
        ));

        allUpgrades.Add(new UpgradeData(
            UpgradeType.HomingMissileTurretExtraMissile,
            "+1 Extra Missile",
            "Homing Missile Turret fires 1 additional missile"
        ));

    }

    // public List<UpgradeData> GetRandomUpgrades(int count)
    // {
    //     List<UpgradeData> pool = new List<UpgradeData>(allUpgrades);
    //     List<UpgradeData> result = new List<UpgradeData>();

    //     for (int i = 0; i < count && pool.Count > 0; i++)
    //     {
    //         int index = Random.Range(0, pool.Count);
    //         result.Add(pool[index]);
    //         pool.RemoveAt(index); // ensures unique
    //     }

    //     return result;
    // }

    public List<UpgradeData> GetRandomUpgrades(int count)
    {
        List<UpgradeData> pool = new List<UpgradeData>();

        bool turretUnlocked = PlayerProfile.Instance.homingMissileTurretUnlocked;
        bool turretSpawned = HomingMissileBrain.Instance != null;

        foreach (var upgrade in allUpgrades)
        {
            // ===== TURRET LOCKED =====
            if (!turretUnlocked)
            {
                if (upgrade.type == UpgradeType.HomingMissileTurret ||
                    upgrade.type == UpgradeType.HomingMissileTurretCooldown ||
                    upgrade.type == UpgradeType.HomingMissileTurretDamagePercent ||
                    upgrade.type == UpgradeType.HomingMissileTurretExtraMissile)
                    continue;
            }

            // ===== TURRET UNLOCKED BUT NOT SPAWNED =====
            if (turretUnlocked && !turretSpawned)
            {
                if (upgrade.type == UpgradeType.HomingMissileTurretCooldown ||
                    upgrade.type == UpgradeType.HomingMissileTurretDamagePercent ||
                    upgrade.type == UpgradeType.HomingMissileTurretExtraMissile)
                    continue;
            }

            // ===== TURRET SPAWNED =====
            if (turretSpawned)
            {
                if (upgrade.type == UpgradeType.HomingMissileTurret)
                    continue;
            }

            pool.Add(upgrade);
        }

        // ===== RANDOM PICK =====
        List<UpgradeData> result = new List<UpgradeData>();

        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int index = Random.Range(0, pool.Count);
            result.Add(pool[index]);
            pool.RemoveAt(index);
        }

        return result;
    }

}
