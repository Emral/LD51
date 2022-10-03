using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public List<GuySprite> sprites;

    private List<GuySprite> weighted;

    public void ResetAll()
    {
        weighted = new List<GuySprite>();

        for (int i = 0; i < sprites.Count; i++)
        {
            if ((sprites[i].RainOnly && DayManager.Globals.IsRaining) || (sprites[i].SunOnly && !DayManager.Globals.IsRaining) || (!sprites[i].RainOnly && !sprites[i].SunOnly))
                for (int j = 0; j < sprites[i].weight; j++)
                {
                    weighted.Add(sprites[i]);
                }
        }
    }

    public GuySprite GetRandom()
    {
        return weighted[Random.Range(0, weighted.Count)];
    }
}
