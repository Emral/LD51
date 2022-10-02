using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActivateOnRain : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (SessionVariables.UpcomingWeathers[1] != Weather.Rain)
            {
                gameObject.SetActive(false);
            }
        } else
        {
            if (!DayManager.Globals.IsRaining)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
