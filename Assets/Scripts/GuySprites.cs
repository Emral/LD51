using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GuySprite
{
    public Sprite sprite;
    [Range(-2, 2)]
    public int bias = 0;
    public int weight = 10;
    public List<string> preferredTags = new List<string>();
}

[CreateAssetMenu(fileName = "GuysSprites", menuName = "Data/Guys")]
public class GuySprites : ScriptableObject
{
    public List<GuySprite> sprites;

    private List<GuySprite> weighted;

    public GuySprite GetRandom()
    {
        if (weighted == null || weighted.Count == 0)
        {
            weighted = new List<GuySprite>();

            for (int i = 0; i < sprites.Count; i++)
            {
                for (int j = 0; j < sprites[i].weight; j++)
                {
                    weighted.Add(sprites[i]);
                }
            }
        }

        return weighted[Random.Range(0, weighted.Count)];
    }
}
