using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        bool mouseButton = Input.GetMouseButton(0);
        CursorImgA.sprite = mouseButton ? PressA : HoverA;
        CursorImgB.sprite = mouseButton ? PressB : HoverB;
        transform.position = Input.mousePosition;
    }
}
