[System.Serializable]
public class PlayerData
{
    public int coins = 0;
    public int highestStageCleared = 0;

    // PLAYER DATA
    public float baseDamage = 20f;
    public float baseFortressMaxHP = 100f;
    public float baseFireRate = 1f;

    // HOMING MISSILE TURRET DATA
    public bool homingMissileTurretUnlocked = false;
    public float homingMissileTurretBaseDamage = 50f;
    public int homingMissileTurretDamageLevel = 1;
    public float homingMissileTurretCooldown = 5f;
    public int homingMissileTurretCooldownLevel = 1;

    // Future:
    // public int fortressLevel;
    // public List<string> unlockedWeapons;
}
