using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TotalSavingsText : MonoBehaviour
{
    private Text _text;

    [TextArea(minLines: 3, maxLines: 5)]
    public string Prefix;

    public Color positiveColor;
    public Color negativeColor;

    // Start is called before the first frame update
    void Start()
    {
        _text = GetComponent<Text>();
        _text.text = Prefix + "$" + SessionVariables.Savings.MakeDollarsString();
        SessionVariables.OnSavingsChanged.AddListener(OnSubmit);
    }

    private void OnDestroy()
    {
        SessionVariables.OnSavingsChanged.RemoveListener(OnSubmit);
    }
    Tween t;
    private void OnSubmit(float a)
    {
        _text.text = Prefix + "$" + a.MakeDollarsString();
        if (t != null)
        {
            t.Complete();
            t = null;
        }
        t = _text.rectTransform.DOPunchScale(Vector3.one + Vector3.right, 0.75f, 2, 0.3f);

        _text.color = a >= 0 ? positiveColor : negativeColor;
    }
}
