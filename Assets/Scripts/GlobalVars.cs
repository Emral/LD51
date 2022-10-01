using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GlobalVars
{
    public float Money = 50;
    public int DailyCost = 50;
    public float Reputation = 0.5f;
    public float Popularity = 0f;
    public int MaxIncome = 10;
    private Color _penColor = new Color(29 / 255, 31 / 255, 38 / 255);
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

    public int Day = 1;
    public int ExtraColors = 0;

    public string GetDayString()
    {
        var s = "";
        var d = Day%365;
        switch(d)
        {
            case < 30:
                s += "June ";
                break;
            case < 61:
                s += "July ";
                d -= 30;
                break;
            case < 92:
                s += "August ";
                d -= 61;
                break;
            case < 122:
                s += "September ";
                d -= 92;
                break;
            case < 153:
                s += "October ";
                d -= 122;
                break;
            case < 183:
                s += "November ";
                d -= 153;
                break;
            case < 214:
                s += "December ";
                d -= 183;
                break;
            case < 245:
                s += "January ";
                d -= 214;
                break;
            case < 273:
                s += "February ";
                d -= 245;
                break;
            case < 304:
                s += "March ";
                d -= 273;
                break;
            case < 334:
                s += "April ";
                d -= 304;
                break;
            case < 365:
                s += "May ";
                d -= 334;
                break;
        }

        return s += d;
    }
}
