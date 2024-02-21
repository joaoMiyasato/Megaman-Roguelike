using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "data_Weapon_", menuName = "Data/Weapon", order = 0)]
public class data_Weapon : ScriptableObject {
    [Header("Base")]
    public string Name;
    public bool IsGun;
    public List<Sprite> Sprites;
    public GameObject Projectile;

    [Header("Attack")]
    public int Attack_Base;

    [Header("Deffense")]
    public int Deffense_Base;

    [Header("Attack Range")]
    public List<Transform> Positions;

    [Header("Attack Speed")]
    public float attackSpeed_Base;
    public float shotSpeed;

    [Header("Description")]
    [TextArea(10,10)]
    public string Description;
}
