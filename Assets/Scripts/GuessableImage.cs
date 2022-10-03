using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Flags]
public enum ValidColors {
    Black = 1,
    Grey = 2,
    Red = 4,
    Orange = 8,
    Yellow = 16,
    Green = 32,
    Blue = 64,
    Purple = 128
}

[System.Serializable]
public class Guessable
{
    [HideLabel]
    [PreviewField(Height = 32, Alignment = ObjectFieldAlignment.Left)]
    public Sprite texture;
    [Range(1, 4)]
    public int strokeComplexity = 1;
    public ValidColors colors = ValidColors.Black;
    public List<string> tags;
}

[CreateAssetMenu(fileName = "Guessable", menuName = "Data/Guessables")]
public class GuessableImage : ScriptableObject
{
    public List<Guessable> images = new List<Guessable>();

    private Dictionary<string, List<Guessable>> guessablesByTag = new Dictionary<string, List<Guessable>>();

    public void Initialize()
    {
        guessablesByTag = new Dictionary<string, List<Guessable>>();

        foreach (var img in images)
        {
            foreach (var tag in img.tags)
            {
                if (!guessablesByTag.ContainsKey(tag))
                {
                    guessablesByTag[tag] = new List<Guessable>();
                }

                guessablesByTag[tag].Add(img);
            }
        }
    }

    private Sprite FindSprite(List<Guessable> sprites, float bias)
    {
        var upperLimit = Mathf.FloorToInt((SessionVariables.Experience + 0.5f - bias * 0.5f));
        var candidates = new List<Guessable>();

        while (upperLimit < 9 && candidates.Count == 0)
        {
            foreach (var image in sprites)
            {
                if ((SessionVariables.Colors & image.colors) == image.colors)
                {
                    if (Mathf.Max(upperLimit, 1) >= image.strokeComplexity)
                    {
                        candidates.Add(image);
                    }
                }
            }

            upperLimit++;
        }

        if (candidates.Count == 0)
        {
            return null;
        }

        return candidates[Random.Range(0, candidates.Count)].texture;
    }

    public Sprite GetRandom(float bias = 0, List<string> preferredTags = null)
    {
        var joinedList = new List<Guessable>();
        if (preferredTags != null)
        {
            foreach(var tag in preferredTags)
            {

                if (DayManager.Globals.tagBiases.Contains(tag))
                {
                    var result = FindSprite(guessablesByTag[tag], bias);

                    if (result)
                    {
                        return result;
                    }
                    break;
                }

                joinedList.AddRange(guessablesByTag[tag]);
            }
        }

        return FindSprite(joinedList, bias);
    }
}
