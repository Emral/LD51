using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Tags", fileName = "Tags")]
public class Tags : ScriptableObject
{
    public List<string> tags;  //contain the names of the enum
}