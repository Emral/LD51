using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GuyDisplay : MonoBehaviour
{
    public Image guyImage;

    private Vector3 guyPosition;

    // Start is called before the first frame update
    void Start()
    {
        guyPosition = guyImage.transform.localPosition;
        DayManager.OnSubmit.AddListener(OnSubmit);
        DayManager.OnNewGuy.AddListener(OnNewGuy);
    }

    private void OnDestroy()
    {
        DayManager.OnSubmit.RemoveListener(OnSubmit);
        DayManager.OnNewGuy.RemoveListener(OnNewGuy);
    }

    private void OnSubmit(float a)
    {
        guyImage.transform.localPosition = guyPosition;
        guyImage.transform.DOLocalMoveX(guyPosition.x - 160, 0.4f).SetEase(Ease.InBack);
    }

    private void OnNewGuy(GuySprite a)
    {
        guyImage.sprite = a.sprite;
        guyImage.transform.localPosition = guyPosition + Vector3.right * 160;
        guyImage.transform.DOLocalMoveX(guyPosition.x, 0.4f).SetEase(Ease.OutBack);
    }
}
