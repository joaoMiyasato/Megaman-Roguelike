using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "data_Entity_Enemy_", menuName = "Data/Enemy", order = 0)]
public class data_Entity_Enemies : data_Entity
{
    [Header("Description")]
    [TextArea(20, 20)]
    public string Description;
}
