using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpensesList : MonoBehaviour
{
    public Text expensePrefab;

    public Text totalExpenses;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = SessionVariables.Expenses.Count - 1; i >= 0; i--)
        {
            var exp = Instantiate(expensePrefab, transform);
            exp.transform.SetAsFirstSibling();
            exp.text = SessionVariables.Expenses[i].Name.ToString() + " - $" + SessionVariables.Expenses[i].Value.MakeDollarsString();
        }

        totalExpenses.text = "$" + SessionVariables.CalculateExpenses().MakeDollarsString();
    }
}
