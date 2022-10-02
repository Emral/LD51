using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static string MakeDollarsString(this float s)
    {
        return (Mathf.Floor(s * 100) / 100.0f).ToString("0.00");
    }

    public static float MakeDollars(this float s)
    {
        return Mathf.Floor(s * 100) / 100.0f;
    }

    public static float MakeDollars(this int s)
    {
        return Mathf.Floor(s * 100) / 100.0f;
    }
}
