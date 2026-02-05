using System.Collections.Generic;
using UnityEngine;

public class TurretSlotManager : MonoBehaviour
{
    public Transform[] slots;
    private bool[] occupied;

    void Awake()
    {
        occupied = new bool[slots.Length];
    }

    public Transform GetNextEmptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (!occupied[i])
            {
                occupied[i] = true;
                return slots[i];
            }
        }

        return null;
    }


}
