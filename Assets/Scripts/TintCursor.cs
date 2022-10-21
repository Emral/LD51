using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TintCursor : MonoBehaviour
{
    public Sprite HoverA;
    public Sprite HoverB;
    public Sprite PressA;
    public Sprite PressB;

    public Image CursorImgA;
    public Image CursorImgB;

    // Start is called before the first frame update
    void Start()
    {
        DayManager.Globals.OnPenColorChanged.AddListener(ChangeTip);
        CursorImgB.color = DayManager.Globals.PenColor;
    }

    public void ChangeTip(Color col)
    {
        CursorImgB.color = col;
    }

    // Update is called once per frame
    void Update()
    {
        var mouse = Mouse.current;
        var touch = Touchscreen.current?.primaryTouch;
        var pos = Vector2.zero;

        if (touch != null)
        {
            pos = touch.position.ReadValue();
        }
        else
        {
            pos = mouse.position.ReadValue();
        }

        var isPressing = mouse.leftButton.ReadValue() > 0 || (touch != null && touch.pressure.ReadValue() > 0);
        bool mouseButton = isPressing;
        CursorImgA.sprite = mouseButton ? PressA : HoverA;
        CursorImgB.sprite = mouseButton ? PressB : HoverB;
        transform.position = pos;
    }
}
