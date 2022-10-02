using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TotalSavingsText : MonoBehaviour
{
    private Text _text;

    public Color positiveColor;
    public Color negativeColor;

    // Start is called before the first frame update
    void Start()
    {
        _text = GetComponent<Text>();
        _text.text = "$" + SessionVariables.Savings;
        SessionVariables.OnSavingsChanged.AddListener(OnSubmit);
    }

    private void OnDestroy()
    {
        SessionVariables.OnSavingsChanged.RemoveListener(OnSubmit);
    }
    Tween t;
    private void OnSubmit(float a)
    {
        _text.text = "$" + Mathf.Floor(100 * a) / 100.0f;
        if (t != null)
        {
            t.Complete();
            t = null;
        }
        t = _text.rectTransform.DOPunchScale(Vector3.one + Vector3.right, 0.75f, 2, 0.3f);

        _text.color = a >= 0 ? positiveColor : negativeColor;
    }
}
