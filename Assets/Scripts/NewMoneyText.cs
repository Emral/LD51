using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class NewMoneyText : MonoBehaviour
{
    private Text _text;
    private Vector3 _position;
    private CanvasGroup _group;

    // Start is called before the first frame update
    void Start()
    {
        _position = transform.localPosition;
        _group = GetComponent<CanvasGroup>();
        _group.alpha = 0;
        _text = GetComponentInChildren<Text>();
        DayManager.OnSubmit.AddListener(OnSubmit);
    }

    private void OnDestroy()
    {
        DayManager.OnSubmit.RemoveListener(OnSubmit);
    }

    private void OnSubmit(float a)
    {
        _group.alpha = 1;
        transform.localPosition = _position;
        _text.text = "+ $" + a.MakeDollarsString();
        _group.DOFade(0, 0.5f).SetEase(Ease.InQuint);
        transform.DOLocalMoveY(transform.localPosition.y + 50, 0.6f).SetEase(Ease.OutQuint);
    }
}
