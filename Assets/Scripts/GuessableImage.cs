using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
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
[ShowOdinSerializedPropertiesInInspector]
public class GuessableMap
{
    [ReadOnly]
    [HorizontalGroup]
    [ListDrawerSettings(HideRemoveButton = true, HideAddButton = true, ShowPaging = false, DraggableItems = false, Expanded = true, IsReadOnly = true)]
    public List<ValidColors> Keys = new List<ValidColors>();
    [HorizontalGroup]
    [ListDrawerSettings(HideRemoveButton = true, HideAddButton = true, ShowPaging = false, DraggableItems = false, Expanded = true)]
    [SingleEnumFlagSelect(EnumType = typeof(ValidColors))]
    public List<ValidColors> Values = new List<ValidColors>();
}

[System.Serializable]
public class Guessable
{

    public static readonly Dictionary<Color, ValidColors> colorMap = new Dictionary<Color, ValidColors>()
    {
        [new Color(29 / 255.0f, 31 / 255.0f, 38 / 255.0f)] = ValidColors.Black,
        [new Color(104 / 255.0f, 92 / 255.0f, 100 / 255.0f)] = ValidColors.Grey,
        [new Color(173 / 255.0f, 34 / 255.0f, 46 / 255.0f)] = ValidColors.Red,
        [new Color(204 / 255.0f, 97 / 255.0f, 59 / 255.0f)] = ValidColors.Orange,
        [new Color(204 / 255.0f, 169 / 255.0f, 83 / 255.0f)] = ValidColors.Yellow,
        [new Color(97 / 255.0f, 183 / 255.0f, 97 / 255.0f)] = ValidColors.Green,
        [new Color(23 / 255.0f, 117 / 255.0f, 168 / 255.0f)] = ValidColors.Blue,
        [new Color(183 / 255.0f, 102 / 255.0f, 183 / 255.0f)] = ValidColors.Purple
    };

    public static readonly Dictionary<ValidColors, Color> colorReverseMap = new Dictionary<ValidColors, Color>()
    {
        [ValidColors.Black] = new Color(29 / 255.0f, 31 / 255.0f, 38 / 255.0f),
        [ValidColors.Grey] = new Color(104 / 255.0f, 92 / 255.0f, 100 / 255.0f),
        [ValidColors.Red] = new Color(173 / 255.0f, 34 / 255.0f, 46 / 255.0f),
        [ValidColors.Orange] = new Color(204 / 255.0f, 97 / 255.0f, 59 / 255.0f),
        [ValidColors.Yellow] = new Color(204 / 255.0f, 169 / 255.0f, 83 / 255.0f),
        [ValidColors.Green] = new Color(97 / 255.0f, 183 / 255.0f, 97 / 255.0f),
        [ValidColors.Blue] = new Color(23 / 255.0f, 117 / 255.0f, 168 / 255.0f),
        [ValidColors.Purple] = new Color(183 / 255.0f, 102 / 255.0f, 183 / 255.0f)
    };

    [HideLabel]
    [PreviewField(Height = 32, Alignment = ObjectFieldAlignment.Left)]
    [OnValueChanged("SetColors", InvokeOnUndoRedo = true)]
    public Sprite texture;
    [Range(1, 6)]
    public int strokeComplexity = 1;

    public bool allowHorizontalFlip = true;
    public bool allowVerticalFlip = false;

    public ValidColors colors = ValidColors.Black;

    public List<Tag> tag;

    [OnCollectionChanged(After = "AddNewVariation")]
    public List<GuessableMap> variations = new List<GuessableMap>();

#if UNITY_EDITOR
    public void AddNewVariation(CollectionChangeInfo info, object value)
    {
        if (info.ChangeType == CollectionChangeType.Add)
        {
            var d = (List<GuessableMap>)value;
            var dict = d[d.Count - 1];
            for (int i = 0; i <= 7; i++)
            {
                if ((((int)colors) & (int)Mathf.Pow(2, i)) != 0)
                {
                    dict.Keys.Add((ValidColors)Mathf.Pow(2, i));
                    dict.Values.Add(ValidColors.Black);
                }
            }
        }
    }
#endif

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

    private Dictionary<Tag, List<Guessable>> guessablesByTag = new Dictionary<Tag, List<Guessable>>();

    [Button]
    public void FixAll()
    {
        images.ForEach(k => k.SetColors());
    }

    public void Initialize()
    {
        guessablesByTag = new Dictionary<Tag, List<Guessable>>();

        foreach (var img in images)
        {
            foreach (var t in img.tag)
            {
                if (!guessablesByTag.ContainsKey(t))
                {
                    guessablesByTag[t] = new List<Guessable>();
                }

                guessablesByTag[t].Add(img);
            }
        }
    }

    public static bool CanDrawBase(Guessable sprite)
    {
        return (SessionVariables.Colors.Value & sprite.colors) == sprite.colors;
    }

    public static GuessableMap GetRandomValidVariation(Guessable sprite)
    {
        var sprites = new List<GuessableMap>();
        if (sprite.variations == null)
        {
            return null;
        }
        foreach (var variation in sprite.variations)
        {
            ValidColors color = (ValidColors)0;
            foreach (var col in Guessable.colorMap)
            {
                if (variation.Values.Contains(col.Value))
                {
                    color += (int)col.Value;
                }
            }

            if ((SessionVariables.Colors.Value & color) == color)
            {
                sprites.Add(variation);
            }
        }

        if (sprites.Count == 0)
        {
            return null;
        }

        return sprites[Random.Range(0, sprites.Count)];
    }

    private Guessable FindSprite(List<Guessable> sprites, float bias)
    {
        var upperLimit = Mathf.FloorToInt((SessionVariables.Experience.Value + 0.5f - bias * 0.5f));
        var candidates = new List<Guessable>();

        bool hasTested = false;

        while (!hasTested || (upperLimit < 9 && candidates.Count == 0))
        {
            hasTested = true;
            foreach (var image in sprites)
            {
                if (Mathf.Max(upperLimit, 1) >= image.strokeComplexity)
                {
                    if (CanDrawBase(image))
                    {
                        candidates.Add(image);
                        continue;
                    }

                    foreach (var variation in image.variations)
                    {
                        ValidColors color = (ValidColors)0;

                        if (GetRandomValidVariation(image) != null)
                        {
                            candidates.Add(image);
                            continue;
                        }
                    }
                }
            }

            upperLimit++;
        }

        if (candidates.Count == 0)
        {
            return null;
        }

        return candidates[Random.Range(0, candidates.Count)];
    }

    public Sprite GetRandom(Guy guy)
    {
        var joinedList = new List<Guessable>();
        Guessable result = null;
        if (guy.preferredTags.Count > 0)
        {
            if (Random.Range(0.0f, 1.0f) <= guy.DayTagSelectionChance)
            {
                foreach (var tag in DayManager.Globals.tagBiases)
                {
                    result = FindSprite(guessablesByTag[tag], guy.bias);

                    if (result != null)
                    {
                        break;
                    }
                }
            }

            foreach (var tag in guy.preferredTags)
            {

                if (DayManager.Globals.tagBiases.Contains(tag) && Random.Range(0, 1.0f) <= guy.PreferredTagSelectionChance)
                {
                    result = FindSprite(guessablesByTag[tag], guy.bias);

                    if (result != null)
                    {
                        break;
                    }
                }

                joinedList.AddRange(guessablesByTag[tag]);
            }
        }

        if (result == null)
        {
            result = FindSprite(joinedList, guy.bias);
        }

        if (result == null)
        {
            return null;
        }

        var variation = GetRandomValidVariation(result);
        var pixels = result.texture.texture.GetPixels((int)result.texture.rect.x, (int)result.texture.rect.y, 64, 64);

        var flipH = result.allowHorizontalFlip && Random.Range(0, 2) == 1;
        var flipV = result.allowVerticalFlip && Random.Range(0, 2) == 1;

        if (variation != null && (!CanDrawBase(result) || Random.Range(0, 1.0f) < result.variations.Count / (result.variations.Count + 1.0f)))
        {
            for (int x = 0; x < 64; x++)
            {
                for (int y = 0; y < 64; y++)
                {
                    var i = x + 64 * y;

                    var p = pixels[i];

                    var c = new Color(Mathf.Floor(p.r * 255) / 255.0f, Mathf.Floor(p.g * 255) / 255.0f, Mathf.Floor(p.b * 255) / 255.0f, 1);
                    if (Guessable.colorMap.ContainsKey(c) && variation.Keys.IndexOf(Guessable.colorMap[c]) is int idx && idx >= 0)
                    {
                        var newColor = Guessable.colorReverseMap[variation.Values[idx]];
                        pixels[i].r = newColor.r;
                        pixels[i].g = newColor.g;
                        pixels[i].b = newColor.b;
                    }
                }
            }
        }

        if (flipH && flipV)
        {
            System.Array.Reverse(pixels);
        }
        else if (flipH)
        {
            for (int x = 0; x < 32; x++)
            {
                for (int y = 0; y < 64; y++)
                {
                    var i1 = x + 64 * y;
                    var i2 = 63 - x + 64 * y;
                    var a = pixels[i1];
                    pixels[i1] = pixels[i2];
                    pixels[i2] = a;
                }
            }
        }
        else if (flipV)
        {
            for (int x = 0; x < 64; x++)
            {
                for (int y = 0; y < 32; y++)
                {
                    var i1 = x + 64 * y;
                    var i2 = x + 64 * (63 - y);
                    var a = pixels[i1];
                    pixels[i1] = pixels[i2];
                    pixels[i2] = a;
                }
            }
        }

        var texture = new Texture2D(64, 64, result.texture.texture.format, false);
        texture.filterMode = FilterMode.Point;
        texture.SetPixels(pixels);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, 64, 64), Vector2.one * 0.5f);
    }
}
