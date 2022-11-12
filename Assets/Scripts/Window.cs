using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Window : MonoBehaviour
{
    public Image DayBackground;
    public Image NightBackground;
    public Image WeatherBackground;
    public Rain rain;

    public enum WindowType
    {
        LivingRoom, Door, Main
    }

    public WindowType type;

    // Start is called before the first frame update
    void Start()
    {
        var weather = SessionVariables.GetTodaysWeather();
        switch (type)
        {
            case WindowType.LivingRoom:
                DayBackground.sprite = weather.preScreenExterior;
                WeatherBackground.sprite = weather.preScreenScrollingTexture;
                rain.speedMult = weather.preScreenScrollingTextureSpeed;
                rain.doSineWave = weather.preScreenScrollingWeatherTextureSine;
                break;
            case WindowType.Door:
                DayBackground.sprite = weather.preScreenWindowBackground;
                WeatherBackground.sprite = weather.preScreenWindowScrollingTexture;
                rain.speedMult = weather.preScreenWindowScrollingTextureSpeed;
                rain.doSineWave = weather.preScreenWindowScrollingTextureSine;
                break;
            case WindowType.Main:
                DayBackground.sprite = weather.exteriorImageDay;
                NightBackground.sprite = weather.exteriorImageNight;
                WeatherBackground.sprite = weather.scrollingWeatherTexture;
                rain.speedMult = weather.scrollingWeatherTextureSpeed;
                rain.doSineWave = weather.scrollingWeatherTextureSine;
                break;
        }
        if (WeatherBackground.sprite == null)
        {
            WeatherBackground.gameObject.SetActive(false);
        }
    }
}
