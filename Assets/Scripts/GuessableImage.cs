using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif
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

    public static readonly Dictionary<Color, ValidColors> colorMap = new Dictionary<Color, ValidColors>()
    {
        [new Color(29 / 255.0f, 31 / 255.0f, 38 / 255.0f)] = ValidColors.Black,
        [new Color(111 / 255.0f, 109 / 255.0f, 142 / 255.0f)] = ValidColors.Grey,
        [new Color(153 / 255.0f, 18 / 255.0f, 54 / 255.0f)] = ValidColors.Red,
        [new Color(204 / 255.0f, 89 / 255.0f, 7 / 255.0f)] = ValidColors.Orange,
        [new Color(206 / 255.0f, 196 / 255.0f, 82 / 255.0f)] = ValidColors.Yellow,
        [new Color(0 / 255.0f, 195 / 255.0f, 110 / 255.0f)] = ValidColors.Green,
        [new Color(99 / 255.0f, 129 / 255.0f, 211 / 255.0f)] = ValidColors.Blue,
        [new Color(197 / 255.0f, 102 / 255.0f, 219 / 255.0f)] = ValidColors.Purple
    };

    [HideLabel]
    [PreviewField(Height = 32, Alignment = ObjectFieldAlignment.Left)]
    [OnValueChanged("SetColors", InvokeOnUndoRedo = true)]
    public Sprite texture;
    [Range(1, 4)]
    public int strokeComplexity = 1;

    public ValidColors colors = ValidColors.Black;


    public List<string> tags;

    public void SetColors()
    {
        var arr = texture.texture.GetPixels((int)texture.rect.x, (int)texture.rect.y, (int)texture.rect.width, (int)texture.rect.height);
        colors = (ValidColors)0;
        for (int i = 0; i < arr.Length; ++i)
        {
            if (arr[i].a > 0)
            {
                arr[i].r = Mathf.Floor(arr[i].r * 255) / 255.0f;
                arr[i].g = Mathf.Floor(arr[i].g * 255) / 255.0f;
                arr[i].b = Mathf.Floor(arr[i].b * 255) / 255.0f;
                if (colorMap.ContainsKey(arr[i]))
                {
                    var c = colorMap[arr[i]];
                    if ((colors & c) == 0)
                    {
                        colors = (ValidColors)((int)colors + (int)c);
                    }
                }
            }
        }
    }
}

[CreateAssetMenu(fileName = "Guessable", menuName = "Data/Guessables")]
public class GuessableImage : SerializedScriptableObject
{

    [Searchable]
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

        bool hasTested = false;

        while (!hasTested || (upperLimit < 9 && candidates.Count == 0))
        {
            hasTested = true;
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
