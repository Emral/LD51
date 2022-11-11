using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkipTutorialCheckbox : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Sprite Idle;
    public Sprite Press;
    public Sprite Hover;
    public Sprite IdleB;
    public Sprite PressB;
    public Sprite HoverB;

    public SFX HoverSound;
    public SFX UnhoverSound;

    public Image img;

    private bool hover = false;

    public void Toggle()
    {
        SFX.Toggle.Play();
        if (img.sprite == Press)
        {
            img.sprite = PressB;
        }
        else if (img.sprite == PressB)
        {
            img.sprite = Press;
        }
        else if (img.sprite == Hover)
        {
            img.sprite = HoverB;
        }
        else if (img.sprite == HoverB)
        {
            img.sprite = Hover;
        }
        else if (img.sprite == Idle)
        {
            img.sprite = IdleB;
        }
        else if (img.sprite == IdleB)
        {
            img.sprite = Idle;
        }
    }

    void Start()
    {
        if (!MetaManager.TutorialActive)
        {
            img.sprite = IdleB;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        img.sprite = !MetaManager.TutorialActive ? PressB : Press;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (img.sprite != Press && img.sprite != PressB)
        {
            img.sprite = !MetaManager.TutorialActive ? HoverB : Hover;
            HoverSound.Play();
        }
        hover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (img.sprite != Press && img.sprite != PressB)
        {
            img.sprite = !MetaManager.TutorialActive ? IdleB : Idle;
            UnhoverSound.Play();
        }
        hover = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (img.sprite == Press || img.sprite == PressB)
        {
            if (!MetaManager.TutorialActive)
            {
                img.sprite = hover ? HoverB : IdleB;
            } else
            {
                img.sprite = hover ? Hover : Idle;
            }
        }
    }
}
