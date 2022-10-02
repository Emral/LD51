using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TodaysExpensesText : MonoBehaviour
{
    public string Prefix;
    private Text _text;

    // Start is called before the first frame update
    void Update()
    {
        _text = _text == null ? GetComponent<Text>() : _text;
        _text.text = Prefix + "$" + SessionVariables.CalculateExpenses();
    }
}
