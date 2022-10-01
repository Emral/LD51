using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class Guessable
{
    [HorizontalGroup]
    [HideLabel]
    [PreviewField(Height = 32, Alignment = ObjectFieldAlignment.Left)]
    public Texture2D texture;
    [HorizontalGroup]
    [HideLabel]
    [Range(1, 5)]
    public int difficulty = 1;
    public List<string> tags;
}

[CreateAssetMenu(fileName = "Guessable", menuName = "Data/Guessables")]
public class GuessableImage : ScriptableObject
{
    public List<Guessable> images = new List<Guessable>();

    public Texture2D GetRandom()
    {
        images.Sort((a, b) => Random.Range(0, 2));
        return images[0].texture;
    }
}
