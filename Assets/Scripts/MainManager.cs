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
        Main.DOMoveY(-180, 1).SetEase(Ease.InOutQuad);
        Credits.DOMoveY(0, 1).SetEase(Ease.InOutQuad);
    }

    public void HideCredits()
    {
        Main.DOMoveY(180, 1).SetEase(Ease.InOutQuad);
        Credits.DOMoveY(360, 1).SetEase(Ease.InOutQuad);
    }
}
