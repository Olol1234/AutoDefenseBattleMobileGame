using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Turret/Turret Definition")]
public class TurretDefinition : ScriptableObject
{
    public TurretType turretType;

    [Header("UI")]
    public string displayName;
    public Sprite icon;
    [TextArea] public string description;

    [Header("Base Stats (UI Only)")]
    public float baseDamage;
    public float baseCooldown;
    public float baseDuration; // laser only (ok if unused)
}
