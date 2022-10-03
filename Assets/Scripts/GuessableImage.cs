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

    public Sprite GetRandom(float bias = 0, List<string> preferredTags = null)
    {
        var tagMask = new Dictionary<string, bool>();
        if (preferredTags != null)
        {
            foreach(var tag in preferredTags)
            {
                tagMask[tag] = true;

                if (DayManager.Globals.tagBiases.Contains(tag))
                {
                    tagMask.Clear();
                    tagMask[tag] = true;
                    break;
                }
            }
        }
        var upperLimit = Mathf.FloorToInt((SessionVariables.Experience + 0.5f - bias * 0.5f));
        var candidates = new List<Guessable>();

        while (candidates.Count == 0 && upperLimit < 8)
        {
            foreach (var image in images)
            {
                if ((SessionVariables.Colors & image.colors) == image.colors)
                {
                    if (Mathf.Clamp(upperLimit, 1, 7) >= image.strokeComplexity)
                    {
                        if (preferredTags == null || preferredTags.Count == 0)
                        {
                            candidates.Add(image);
                        }
                        else
                        {
                            foreach (var tag in image.tags)
                            {
                                if (tagMask.ContainsKey(tag))
                                {
                                    candidates.Add(image);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            upperLimit++;
        }

        if (candidates.Count > 0)
        {
            return candidates[Random.Range(0, candidates.Count)].texture;
        }
        else
        {
            Debug.Log("Unable to spawn image");
            return images[Random.Range(0, images.Count)].texture;
        }
    }
}
