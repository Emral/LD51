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

    public Sprite GetRandom()
    {
        return images[Random.Range(0, images.Count)].texture;
    }
}
