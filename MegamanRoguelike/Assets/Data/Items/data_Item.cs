using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "data_Item_", menuName = "Data/Item", order = 0)]
public class data_Item : ScriptableObject {
    [Header("Base")]
    public string Name;

    [Header("Description")]
    public string Description;
    
}
