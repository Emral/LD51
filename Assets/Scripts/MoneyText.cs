using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyText : MonoBehaviour
{
    private Text _text;

    // Start is called before the first frame update
    void Start()
    {
        _text = GetComponent<Text>();
        _text.text = "$" + SessionVariables.TodaysEarnings.MakeDollarsString();
        DayManager.OnSubmit.AddListener(OnSubmit);
    }

    private void OnDestroy()
    {
        DayManager.OnSubmit.RemoveListener(OnSubmit);
    }

    private void OnSubmit(float a)
    {
        _text.text = "$" + SessionVariables.TodaysEarnings.MakeDollarsString();
    }
}
