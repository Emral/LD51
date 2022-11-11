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

    public string GetRandomDialogue(float price, Guy.ThanksOverride thanksOverride)
    {
        var list = new List<string>();
        if (price < 1)
        {
            list = thanksOverride.worst.Count > 0 ? thanksOverride.worst : worst;
        } else if (price < 0.3f * SessionVariables.IncomeMultiplier.Value * SessionVariables.MaxIncomeBase.Value)
        {
            list = thanksOverride.bad.Count > 0 ? thanksOverride.bad : bad;
        }
        else if (price < 0.6f * SessionVariables.IncomeMultiplier.Value * SessionVariables.MaxIncomeBase.Value)
        {
            list = thanksOverride.decent.Count > 0 ? thanksOverride.decent : decent;
        }
        else if (price < 0.85f * SessionVariables.IncomeMultiplier.Value * SessionVariables.MaxIncomeBase.Value)
        {
            list = thanksOverride.good.Count > 0 ? thanksOverride.good : good;
        }
        else if (price < 0.95f * SessionVariables.IncomeMultiplier.Value * SessionVariables.MaxIncomeBase.Value)
        {
            list = thanksOverride.great.Count > 0 ? thanksOverride.great : great;
        }
        else
        {
            list = thanksOverride.exceptional.Count > 0 ? thanksOverride.exceptional : exceptional;
        }

        return list[Random.Range(0, list.Count)];
    }
}
