using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetColor : MonoBehaviour
{
    public Color color;
    public void Set()
    {
        DayManager.Globals.PenColor = color;
    }
}
