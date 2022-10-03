using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MainManager : MonoBehaviour
{

    public RectTransform Main;
    public RectTransform Credits;
    
    public void ShowCredits()
    {
        Main.DOLocalMoveY(-360, 1).SetEase(Ease.InOutQuad);
        Credits.DOLocalMoveY(-180, 1).SetEase(Ease.InOutQuad);
    }

    public void HideCredits()
    {
        Main.DOLocalMoveY(0, 1).SetEase(Ease.InOutQuad);
        Credits.DOLocalMoveY(180, 1).SetEase(Ease.InOutQuad);
    }
}
