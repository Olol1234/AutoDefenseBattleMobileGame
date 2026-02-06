using UnityEngine;

public static class TurretUpgradeHelper
{
    public static int GetDamageUpgradeCost(TurretType type)
    {
        var p = PlayerProfile.Instance;

        int level = 0;
        int baseCost = 100;

        switch (type)
        {
            case TurretType.HomingMissileTurret:
                level = p.homingMissileTurretDamageLevel;
                break;

            case TurretType.LaserTurret:
                level = p.laserTurretDamageLevel;
                break;
        }

        return Mathf.RoundToInt(baseCost * Mathf.Pow(1.3f, level));
    }

    public static int GetCooldownUpgradeCost(TurretType type)
    {
        var p = PlayerProfile.Instance;

        int level = 0;
        int baseCost = 120; // Usually cooldown should cost more than damage

        switch (type)
        {
            case TurretType.HomingMissileTurret:
                level = p.homingMissileTurretCooldownLevel;
                break;

            case TurretType.LaserTurret:
                level = p.laserTurretCooldownLevel;
                break;
        }

        return Mathf.RoundToInt(baseCost * Mathf.Pow(1.3f, level));
    }

    public static bool UpgradeDamage(TurretType type)
    {
        var p = PlayerProfile.Instance;

        int cost = GetDamageUpgradeCost(type);

        if (p.coins < cost)
            return false;

        p.coins -= cost;

        switch (type)
        {
            case TurretType.HomingMissileTurret:
                p.homingMissileTurretDamageLevel++;
                p.homingMissileTurretBaseDamage += 50f;
                break;

            case TurretType.LaserTurret:
                p.laserTurretDamageLevel++;
                p.laserTurretBaseDPS += 30f;
                break;
        }

        p.SaveToDisk();
        return true;
    }

    public static bool UpgradeCooldown(TurretType type)
    {
        var p = PlayerProfile.Instance;

        int cost = GetCooldownUpgradeCost(type);

        if (p.coins < cost)
            return false;

        p.coins -= cost;

        switch (type)
        {
            case TurretType.HomingMissileTurret:
                p.homingMissileTurretCooldownLevel++;
                p.homingMissileTurretCooldown *= 0.95f; // 5% faster
                break;

            case TurretType.LaserTurret:
                p.laserTurretCooldownLevel++;
                p.laserTurretCooldown *= 0.95f; // 5% faster
                break;
        }

        p.SaveToDisk();
        return true;
    }

    public static float GetNextDamageValue(TurretType type)
    {
        var p = PlayerProfile.Instance;

        switch (type)
        {
            case TurretType.HomingMissileTurret:
                return p.homingMissileTurretBaseDamage + 50f;

            case TurretType.LaserTurret:
                return p.laserTurretBaseDPS + 30f;
                
        }

        return 0;
    }

    public static float GetNextCooldownValue(TurretType type)
    {
        var p = PlayerProfile.Instance;

        switch (type)
        {
            case TurretType.HomingMissileTurret:
                return p.homingMissileTurretCooldown * 0.95f;

            case TurretType.LaserTurret:
                return p.laserTurretCooldown * 0.95f;
        }

        return 0;
    }

}
