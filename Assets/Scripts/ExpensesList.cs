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
            var prefix = SessionVariables.Expenses[i].PerWhat == "%" ? " - " : " - $";
            exp.text = SessionVariables.Expenses[i].Name.ToString() + prefix + SessionVariables.Expenses[i].Value.MakeDollarsString() + SessionVariables.Expenses[i].PerWhat;
        }

        totalExpenses.text = "$" + SessionVariables.CalculateExpenses().MakeDollarsString() + " or more";
    }
}
