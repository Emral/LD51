using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GlobalVars
{
    private Color _penColor = new Color(29 / 255.0f, 31 / 255.0f, 38 / 255.0f);
    public float RushHourStart = 0;
    public float RushHourEnd = 0;
    public Color PenColor
    {
        get => _penColor;
        set
        {
            _penColor = value;
            OnPenColorChanged.Invoke(value);
        }
    }

    public UnityEvent<Color> OnPenColorChanged = new UnityEvent<Color>();
}
