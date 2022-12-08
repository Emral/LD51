using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DateText : MonoBehaviour
{
    private Text _text;

    // Start is called before the first frame update
    void Start()
    {
        _text = GetComponent<Text>();
        _text.text = SessionVariables.GetSeason().ToString() + ", " + "Day " + (((SessionVariables.Day.Value) % 8) + 1);
    }
}
