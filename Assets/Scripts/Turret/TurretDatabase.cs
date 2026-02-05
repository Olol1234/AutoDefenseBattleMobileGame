using UnityEngine;
using System.Collections.Generic;

public class TurretDatabase : MonoBehaviour
{
    public static TurretDatabase Instance;

    public List<TurretDefinition> turretDefinitions;

    void Awake()
    {
        Instance = this;
    }

    public TurretDefinition GetDefinition(TurretType type)
    {
        return turretDefinitions.Find(t => t.turretType == type);
    }
}
