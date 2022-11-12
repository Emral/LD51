using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class WeatherConfig
{
    public float dayLength = 100;

    public string name;

    public float drawTimerMultiplier = 1.0f;

    [MinMaxSlider(minValue: 0, maxValue: 5, ShowFields = true)]
    public Vector2 customerDelay = Vector2.zero;

    public float rushHourMultiplier = 1;
    [MinMaxSlider(minValue: 0, maxValue: 1, ShowFields = true)]
    public Vector2 rushHourMinMax = Vector2.up;

    [HorizontalGroup(GroupID = "A")]
    [PreviewField]
    [HideLabel]
    public Sprite exteriorImageDay;
    [HorizontalGroup(GroupID = "A")]
    [PreviewField]
    [HideLabel]
    public Sprite exteriorImageNight;
    [PreviewField]
    [HideLabel]
    public Sprite scrollingWeatherTexture;
    public float scrollingWeatherTextureSpeed;
    public float scrollingWeatherTextureSine;
    [HorizontalGroup(GroupID = "B")]
    [PreviewField]
    [HideLabel]
    public Sprite preScreenExterior;
    [HorizontalGroup(GroupID = "B")]
    [PreviewField]
    [HideLabel]
    public Sprite preScreenScrollingTexture;
    public float preScreenScrollingTextureSpeed;
    public float preScreenScrollingWeatherTextureSine;
    [HorizontalGroup(GroupID = "C")]
    [PreviewField]
    [HideLabel]
    public Sprite preScreenWindowBackground;
    [HorizontalGroup(GroupID = "C")]
    [PreviewField]
    [HideLabel]
    public Sprite preScreenWindowScrollingTexture;
    public float preScreenWindowScrollingTextureSpeed;
    public float preScreenWindowScrollingTextureSine;
    [PreviewField]
    [HorizontalGroup(GroupID = "D")]
    [HideLabel]
    public Sprite weatherIconSpring;
    [PreviewField]
    [HorizontalGroup(GroupID = "D")]
    [HideLabel]
    public Sprite weatherIconSummer;
    [PreviewField]
    [HorizontalGroup(GroupID = "D")]
    [HideLabel]
    public Sprite weatherIconAutumn;
    [PreviewField]
    [HorizontalGroup(GroupID = "D")]
    [HideLabel]
    public Sprite weatherIconWinter;

    public List<string> sallyDialogue;

    public BGM backgroundMusic;

    [HideInInspector]
    public Weather weather;

    public List<Tag> inherentTags = new List<Tag>();
}

[CreateAssetMenu(fileName = "Weather", menuName = "Data/Weather")]
public class WeatherSettings : SerializedScriptableObject
{
    public Dictionary<Weather, WeatherConfig> weathers = new Dictionary<Weather, WeatherConfig>();

    public void Init()
    {
        foreach (var kvp in weathers)
        {
            kvp.Value.weather = kvp.Key;
        }
    }
}
