using System.Collections.Generic;
using UnityEngine;

public class TurretManager : MonoBehaviour
{
    public static TurretManager Instance;

    public TurretSlotManager slotManager;
    // public GameObject turretPrefab;
    public List<TurretData> turretDatas;
    public int activeTurretCount = 0;

    void Awake()
    {
        Instance = this;
    }

    // public bool SpawnTurret()
    // {
    //     Transform slot = slotManager.GetNextEmptySlot();

    //     if (slot == null)
    //     {
    //         Debug.Log("All turret slots full");
    //         return false;
    //     }

    //     Instantiate(turretPrefab, slot.position, slot.rotation, slot);
    //     activeTurretCount++;
    //     return true;
    // }

    public bool SpawnTurret(TurretType type)
    {
        var data = turretDatas.Find(t => t.type == type);

        if (data == null) return false;

        Transform slot = slotManager.GetNextEmptySlot();
        if (slot == null) return false;

        Instantiate(data.prefab, slot.position, slot.rotation, slot);
        activeTurretCount++;

        return true;
    }


}
