using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class data_Entity : ScriptableObject {
    [Header("Base")]
    public string Name;

    [Header("Health")]
    public int Health_Base;
    public int Health_Max;
    public int Health_Current;
    
    [Header("Attack")]
    public int Attack_Base;

    [Header("Deffense")]
    public int Deffense_Base;
}
