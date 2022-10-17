using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ActivateOnRain : MonoBehaviour
{
    Image img;

    // Start is called before the first frame update
    void Start()
    {
        img = GetComponent<Image>();
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (SessionVariables.GetTodaysWeather().preScreenScrollingTexture == null)
            {
                gameObject.SetActive(false);
            } else
            {
                img.sprite = SessionVariables.GetTodaysWeather().preScreenScrollingTexture;
            }
        } else
        {
            if (DayManager.Globals.weather.scrollingWeatherTexture == null)
            {
                gameObject.SetActive(false);
            } else
            {
                img.sprite = DayManager.Globals.weather.scrollingWeatherTexture;
            }
        }
    }
}
