using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "data_Entity_PlayableCharacter_", menuName = "Data/PlayableCharacter", order = 0)]
public class data_Entity_PlayableCharacters : data_Entity
{
    [Header("Speed")]
    public float MoveSpeed;
    public float DashSpeed;

    [Header("Description")]
    [TextArea(20,20)]
    public string Description;
}
