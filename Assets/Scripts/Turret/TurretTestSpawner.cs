// using UnityEngine;

// public class TurretTestSpawner : MonoBehaviour
// {
//     public TurretSlotManager slotManager;
//     public GameObject turretPrefab;

//     void Update()
//     {
//         // Press T to spawn turret for testing
//         if (Input.GetKeyDown(KeyCode.T))
//         {
//             Debug.Log("SpawnTurret called");
//             SpawnTurret();
//         }
//     }

//     void SpawnTurret()
//     {
//         Transform slot = slotManager.GetNextEmptySlot();

//         if (slot == null)
//         {
//             Debug.Log("All turret slots full");
//             return;
//         }

//         Instantiate(turretPrefab, slot.position, slot.rotation, slot);
//     }
// }
