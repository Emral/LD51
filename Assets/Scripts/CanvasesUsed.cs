using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CanvasesUsed : MonoBehaviour
{
    public string Prefix;
    private Text _text;
    private Expense expense;

    private void Start()
    {
        _text = GetComponent<Text>();
        expense = SessionVariables.Expenses.Value.Find(e => e.Name == "Utensils");
    }

    void Update()
    {
        _text.text =  expense.Multiplier + Prefix;
    }
}
