using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetColor : MonoBehaviour
{
    public Color color;
    public ValidColors ownColor;
    private void Start()
    {
        if (ownColor != (ValidColors)0 && (ownColor & SessionVariables.Colors) == 0)
        {
            gameObject.SetActive(false);
        }
    }
    public void Set()
    {
        SFX.SelectColor.Play();
        DayManager.Globals.PenColor = color;
    }
}
