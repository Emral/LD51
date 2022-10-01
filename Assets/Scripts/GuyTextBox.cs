using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GuyTextBox : MonoBehaviour
{
    private Vector3 idlePosition;
    public GuyDialogue dialogue;
    public Text _text;
    // Start is called before the first frame update
    void Start()
    {
        idlePosition = transform.position;
        DayManager.OnSubmit.AddListener(OnSubmit);
        transform.position = idlePosition - Vector3.up * 75;
    }

    private void OnDestroy()
    {
        DayManager.OnSubmit.RemoveListener(OnSubmit);
    }

    private void OnSubmit(float a)
    {
        _text.text = dialogue.GetRandomDialogue(a);
        transform.position = idlePosition - Vector3.up * 75;
        transform.DOMoveY(idlePosition.y, 0.25f).SetEase(Ease.OutQuint).OnComplete(() =>
        {
            transform.DOMoveY(idlePosition.y - 75, 0.65f).SetEase(Ease.InQuad);
        });
    }
}
