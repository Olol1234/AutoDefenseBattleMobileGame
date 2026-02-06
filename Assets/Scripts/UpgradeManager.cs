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

        allUpgrades.Add(new UpgradeData(
            UpgradeType.LaserTurret,
            "Laser Turret",
            "Deploy a Laser Turret to assist you in battle"
        ));

        allUpgrades.Add(new UpgradeData(
            UpgradeType.LaserTurretCooldown,
            "-10% Turret Cooldown",
            "Reduce Laser Turret's cooldown by 10%"
        ));

        allUpgrades.Add(new UpgradeData(
            UpgradeType.LaserTurretDamagePercent,
            "+20% Turret Damage",
            "Increase Laser Turret's damage by 20%"
        ));

        allUpgrades.Add(new UpgradeData(
            UpgradeType.LaserTurretDuration,
            "+0.5s Turret Duration",
            "Increase Laser Turret's active duration by 0.5 seconds"
        ));

    }

    bool IsTurretUnlocked(UpgradeType type)
    {
        switch(type)
        {
            case UpgradeType.HomingMissileTurret:
                return PlayerProfile.Instance.homingMissileTurretUnlocked;

            case UpgradeType.LaserTurret:
                return PlayerProfile.Instance.laserTurretUnlocked;
        }

        return true;
    }

    bool IsTurretSpawned(UpgradeType type)
    {
        switch(type)
        {
            case UpgradeType.HomingMissileTurret:
                return HomingMissileBrain.Instance != null;

            case UpgradeType.LaserTurret:
                return LaserTurretBrain.Instance != null;
        }

        return false;
    }

    bool IsTurretUpgrade(UpgradeType type, UpgradeType turretType)
    {
        if (turretType == UpgradeType.HomingMissileTurret)
        {
            return type == UpgradeType.HomingMissileTurretCooldown ||
                type == UpgradeType.HomingMissileTurretDamagePercent ||
                type == UpgradeType.HomingMissileTurretExtraMissile;
        }

        if (turretType == UpgradeType.LaserTurret)
        {
            return type == UpgradeType.LaserTurretCooldown ||
                type == UpgradeType.LaserTurretDamagePercent ||
                type == UpgradeType.LaserTurretDuration;
        }

        return false;
    }

    public List<UpgradeData> GetRandomUpgrades(int count)
    {
        List<UpgradeData> pool = new List<UpgradeData>();

        // bool turretUnlocked = PlayerProfile.Instance.homingMissileTurretUnlocked;
        // bool turretSpawned = HomingMissileBrain.Instance != null;

        foreach (var upgrade in allUpgrades)
        {
            if (upgrade.type == UpgradeType.HomingMissileTurret ||
                IsTurretUpgrade(upgrade.type, UpgradeType.HomingMissileTurret))
            {
                bool unlocked = IsTurretUnlocked(UpgradeType.HomingMissileTurret);
                bool spawned = IsTurretSpawned(UpgradeType.HomingMissileTurret);

                if (!unlocked) continue;
                if (unlocked && !spawned && upgrade.type != UpgradeType.HomingMissileTurret) continue;
                if (spawned && upgrade.type == UpgradeType.HomingMissileTurret) continue;
            }

            if (upgrade.type == UpgradeType.LaserTurret ||
                IsTurretUpgrade(upgrade.type, UpgradeType.LaserTurret))
            {
                bool unlocked = IsTurretUnlocked(UpgradeType.LaserTurret);
                bool spawned = IsTurretSpawned(UpgradeType.LaserTurret);

                if (!unlocked) continue;
                if (unlocked && !spawned && upgrade.type != UpgradeType.LaserTurret) continue;
                if (spawned && upgrade.type == UpgradeType.LaserTurret) continue;
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
