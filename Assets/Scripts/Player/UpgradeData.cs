using UnityEngine;

public class UpgradeData
{
    public UpgradeType type;
    public string name;
    public string description;
    public UpgradeRarity rarity;

    public UpgradeData(UpgradeType type, string name, string description, UpgradeRarity rarity)
    {
        this.type = type;
        this.name = name;
        this.description = description;
        this.rarity = rarity;
    }

    public void Apply()
    {
        var stats = PlayerStats.Instance;

        switch (type)
        {
            case UpgradeType.DamagePercent:
                // stats.damageMultiplier *= 1.5f;
                stats.damageAddition += stats.baseDamage * 0.5f;
                break;

            case UpgradeType.ExtraBullet:
                stats.extraBullets += 1;
                break;

            case UpgradeType.Penetration:
                stats.penetration += 1;
                break;

            case UpgradeType.AttackSpeed:
                stats.attackSpeedMultiplier *= 1.5f;
                break;

            case UpgradeType.HomingMissileTurret:
                TurretManager.Instance.SpawnTurret(TurretType.HomingMissileTurret);
                break;

            case UpgradeType.HomingMissileTurretCooldown:
                HomingMissileBrain.Instance.lowerCooldownFactor += 0.1f;
                break;
            
            case UpgradeType.HomingMissileTurretDamagePercent:
                HomingMissileBrain.Instance.damageMultiplier *= 1.2f;
                break;

            case UpgradeType.HomingMissileTurretExtraMissile:
                HomingMissileBrain.Instance.extraMissile += 1;
                break;

            case UpgradeType.HomingMissileTurretShockwaveOnImpact:
                HomingMissileBrain.Instance.hasShockwaveOnImpact = true;
                UpgradeManager.Instance.MarkUpgradeTaken(UpgradeType.HomingMissileTurretShockwaveOnImpact);
                break;

            case UpgradeType.HomingMissileTurretMiniMissiles:
                HomingMissileBrain.Instance.hasMiniMissiles = true;
                UpgradeManager.Instance.MarkUpgradeTaken(UpgradeType.HomingMissileTurretMiniMissiles);
                break;

            case UpgradeType.LaserTurret:
                TurretManager.Instance.SpawnTurret(TurretType.LaserTurret);
                break;
            
            case UpgradeType.LaserTurretCooldown:
                LaserTurretBrain.Instance.lowerCooldownFactor += 0.1f;
                break;

            case UpgradeType.LaserTurretDamagePercent:
                LaserTurretBrain.Instance.damageMultiplier *= 1.2f;
                break;

            case UpgradeType.LaserTurretDuration:
                LaserTurretBrain.Instance.extraDuration += 0.5f;
                break;

            case UpgradeType.LaserTurretSweepingLaser:
                LaserTurretBrain.Instance.hasSweepLaser = true;
                UpgradeManager.Instance.MarkUpgradeTaken(UpgradeType.LaserTurretSweepingLaser);
                break;

            case UpgradeType.LaserTurretSideLaser:
                LaserTurretBrain.Instance.hasSideLaser = true;
                UpgradeManager.Instance.MarkUpgradeTaken(UpgradeType.LaserTurretSideLaser);
                break;
        }
    }

}
