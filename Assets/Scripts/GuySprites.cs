using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

[System.Serializable]
public class GuySprite
{
    public Sprite sprite;
    [Range(-2, 2)]
    public float bias = 0;
    public int weight = 10;
    public bool RainOnly = false;
    public bool SunOnly = false;
    public SFX ArrivalSound = SFX.None;
    public List<string> preferredTags = new List<string>();
}

[CreateAssetMenu(fileName = "GuysSprites", menuName = "Data/Guys")]
public class GuySprites : ScriptableObject
{
    [AssetList(AssetNamePrefix = "Guy_", AutoPopulate = true)]
    public List<Guy> guys;

    private List<Guy> weighted;

    public void ResetAll()
    {
        weighted = new List<Guy>();

        for (int i = 0; i < guys.Count; i++)
        {
            var guy = guys[i];

            if (guy.OnlyAppearIfDayPrefersTag)
            {
                var stays = false;
                foreach (var tag in guy.preferredTags)
                {
                    if (DayManager.Globals.tagBiases.Contains(tag))
                    {
                        stays = true;
                        break;
                    }
                }

                if (!stays)
                {
                    continue;
                }
            }

            if (guy.appearanceWeights.Keys.Contains(SessionVariables.GetTodaysWeather().weather) && guy.seasonAppearanceMultipliers.Keys.Contains(SessionVariables.GetSeason()))
            {
                for (int j = 0; j < Mathf.FloorToInt(guy.appearanceWeights[SessionVariables.GetTodaysWeather().weather].WeightValue * guy.seasonAppearanceMultipliers[SessionVariables.GetSeason()].Multiplier); j++)
                {
                    weighted.Add(guys[i]);
                }
            }
        }
    }

    public Guy GetRandom()
    {
        return weighted[Random.Range(0, weighted.Count)];
    }
}
