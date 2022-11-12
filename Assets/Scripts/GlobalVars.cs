using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GlobalVars
{
    private Color _penColor = new Color(29 / 255.0f, 31 / 255.0f, 38 / 255.0f);
    public float RushHourStart = 0;
    public float RushHourEnd = 0;
    public bool RushHour = false;
    public WeatherConfig weather;
    public float LastGain = 0;

    public Guy currentGuy;
    public List<Tag> tagBiases = new List<Tag>();
    public Color PenColor
    {
        get => _penColor;
        set
        {
            _penColor = value;
            OnPenColorChanged.Invoke(value);
        }
    }

    public Vector2 CustomerDelay;

    public UnityEvent<Color> OnPenColorChanged = new UnityEvent<Color>();
}
