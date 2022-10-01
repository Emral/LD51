using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/GuyDialogue", fileName = "GuyDialogue")]
public class GuyDialogue : ScriptableObject
{
    public List<string> worst;
    public List<string> bad;
    public List<string> decent;
    public List<string> good;
    public List<string> great;
    public List<string> exceptional;

    public string GetRandomDialogue(float price)
    {
        var list = new List<string>();
        if (price < 1)
        {
            list = worst;
        } else if (price < 0.2f * DayManager.Globals.MaxIncome)
        {
            list = bad;
        }
        else if (price < 0.5f * DayManager.Globals.MaxIncome)
        {
            list = decent;
        }
        else if (price < 0.75f * DayManager.Globals.MaxIncome)
        {
            list = good;
        }
        else if (price < 0.9f * DayManager.Globals.MaxIncome)
        {
            list = great;
        }
        else
        {
            list = exceptional;
        }

        return list[Random.Range(0, list.Count)];
    }
}
