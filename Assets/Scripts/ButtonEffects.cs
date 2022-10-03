using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Sprite Idle;
    public Sprite Press;
    public Sprite Hover;

    public SFX HoverSound;
    public SFX UnhoverSound;

    public Image img;

    private bool hover = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        img.sprite = Press;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (img.sprite != Press)
        {
            img.sprite = Hover;
            HoverSound.Play();
        }
        hover = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (img.sprite != Press)
        {
            img.sprite = Idle;
            UnhoverSound.Play();
        }
        hover = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (img.sprite == Press)
        {
            img.sprite = hover ? Hover : Idle;
        }
    }
}
