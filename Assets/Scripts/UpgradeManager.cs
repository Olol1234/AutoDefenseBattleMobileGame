using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    private List<UpgradeData> allUpgrades = new List<UpgradeData>();
    private HashSet<UpgradeType> uniqueUpgradesTaken = new HashSet<UpgradeType>();

    private void Awake()
    {
        Instance = this;
        InitUpgrades();
    }

    UpgradeRarity RollRarity()
    {
        float roll = Random.Range(0f, 1f);

        if (roll < 0.5f)
            return UpgradeRarity.Common;
        else if (roll < 0.8f)
            return UpgradeRarity.Uncommon;
        else if (roll < 0.95f)
            return UpgradeRarity.Rare;
        else if (roll < 0.99f)
            return UpgradeRarity.Epic;
        else
            return UpgradeRarity.Legendary;
    }

    void InitUpgrades()
    {
        allUpgrades.Add(new UpgradeData(
            UpgradeType.DamagePercent,
            "+50% Damage",
            "Increase damage by 50%",
            UpgradeRarity.Common
        ));

        allUpgrades.Add(new UpgradeData(
            UpgradeType.ExtraBullet,
            "+1 Bullet",
            "Fire an additional bullet",
            UpgradeRarity.Common
        ));

        allUpgrades.Add(new UpgradeData(
            UpgradeType.Penetration,
            "+1 Penetration",
            "Bullets pierce 1 more enemy",
            UpgradeRarity.Common
        ));

        allUpgrades.Add(new UpgradeData(
            UpgradeType.AttackSpeed,
            "+50% Attack Speed",
            "Shoot faster",
            UpgradeRarity.Uncommon
        ));

        allUpgrades.Add(new UpgradeData(
            UpgradeType.HomingMissileTurret,
            "Homing Missile Turret",
            "Deploy a Homing Missile Turret to assist you in battle",
            UpgradeRarity.Common
        ));

        allUpgrades.Add(new UpgradeData(
            UpgradeType.HomingMissileTurretCooldown,
            "-10% Turret Cooldown",
            "Reduce Homing Missile Turret's cooldown by 10%",
            UpgradeRarity.Common
        ));

        allUpgrades.Add(new UpgradeData(
            UpgradeType.HomingMissileTurretDamagePercent,
            "+20% Turret Damage",
            "Increase Homing Missile Turret's damage by 20%",
            UpgradeRarity.Common
        ));

        allUpgrades.Add(new UpgradeData(
            UpgradeType.HomingMissileTurretExtraMissile,
            "+1 Extra Missile",
            "Homing Missile Turret fires 1 additional missile",
            UpgradeRarity.Uncommon
        ));

        allUpgrades.Add(new UpgradeData(
            UpgradeType.HomingMissileTurretShockwaveOnImpact,
            "Shockwave on Impact",
            "Homing Missiles create a shockwave upon impact, damaging nearby enemies",
            UpgradeRarity.Rare
        ));

        allUpgrades.Add(new UpgradeData(
            UpgradeType.HomingMissileTurretMiniMissiles,
            "Mini Missiles",
            "Homing Missiles spawn smaller mini missiles upon impact",
            UpgradeRarity.Rare
        ));

        allUpgrades.Add(new UpgradeData(
            UpgradeType.LaserTurret,
            "Laser Turret",
            "Deploy a Laser Turret to assist you in battle",
            UpgradeRarity.Common
        ));

        allUpgrades.Add(new UpgradeData(
            UpgradeType.LaserTurretCooldown,
            "-10% Turret Cooldown",
            "Reduce Laser Turret's cooldown by 10%",
            UpgradeRarity.Common
        ));

        allUpgrades.Add(new UpgradeData(
            UpgradeType.LaserTurretDamagePercent,
            "+20% Turret Damage",
            "Increase Laser Turret's damage by 20%",
            UpgradeRarity.Common
        ));

        allUpgrades.Add(new UpgradeData(
            UpgradeType.LaserTurretDuration,
            "+0.5s Turret Duration",
            "Increase Laser Turret's active duration by 0.5 seconds",
            UpgradeRarity.Uncommon
        ));

        allUpgrades.Add(new UpgradeData(
            UpgradeType.LaserTurretSweepingLaser,
            "Sweeping Laser",
            "Laser Turret attack with an additional sweeping laser",
            UpgradeRarity.Rare
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
                type == UpgradeType.HomingMissileTurretExtraMissile ||
                type == UpgradeType.HomingMissileTurretShockwaveOnImpact ||
                type == UpgradeType.HomingMissileTurretMiniMissiles;
        }

        if (turretType == UpgradeType.LaserTurret)
        {
            return type == UpgradeType.LaserTurretCooldown ||
                type == UpgradeType.LaserTurretDamagePercent ||
                type == UpgradeType.LaserTurretDuration ||
                type == UpgradeType.LaserTurretSweepingLaser;
        }

        return false;
    }

    public void MarkUpgradeTaken(UpgradeType type)
    {
        uniqueUpgradesTaken.Add(type);
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

            if (uniqueUpgradesTaken.Contains(upgrade.type))
                continue;
            pool.Add(upgrade);
        }

        // ===== RANDOM PICK =====
        List<UpgradeData> result = new List<UpgradeData>();

        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            UpgradeRarity rarity = RollRarity();
            var rarityPool = pool.Where(u => u.rarity == rarity).ToList();

            if (rarityPool.Count == 0)
            {
                // If no upgrades of this rarity, pick any upgrade from the pool
                int index = Random.Range(0, pool.Count);
                result.Add(pool[index]);
                pool.RemoveAt(index);
            }
            else
            {
                int index = Random.Range(0, rarityPool.Count);
                result.Add(rarityPool[index]);
                pool.Remove(rarityPool[index]);
            }
        }

        return result;
    }

}
