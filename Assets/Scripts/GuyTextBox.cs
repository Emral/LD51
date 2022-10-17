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
        idlePosition = transform.localPosition;
        DayManager.OnSubmit.AddListener(OnSubmit);
        transform.position = idlePosition - Vector3.up * 75;
    }

    private void OnDestroy()
    {
        DayManager.OnSubmit.RemoveListener(OnSubmit);
    }

    private void OnSubmit(float a)
    {
        _text.text = dialogue.GetRandomDialogue(a, DayManager.Globals.currentGuy.thanksMessages);
        transform.localPosition = idlePosition - Vector3.up * 50;
        transform.DOLocalMoveY(idlePosition.y, 0.25f).SetEase(Ease.OutQuint).OnComplete(() =>
        {
            transform.DOLocalMoveY(idlePosition.y - 50, 0.65f).SetEase(Ease.InQuad);
        });
    }
}
