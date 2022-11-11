using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerAnimation : MonoBehaviour
{
    public Image Head;
    public Image Pen;
    public Image Face;

    public Sprite BodyUp;
    public Sprite BodyDown;
    public Sprite DrawingNormal;
    public Sprite DrawingFast;
    public Sprite DrawingWeary;
    public Sprite DrawingWearyFast;
    public Sprite ShowingNormal;
    public Sprite ShowingHappy;
    public Sprite ShowingWearyNormal;
    public Sprite ShowingWearyHappy;
    public Sprite ShowingRelieved;

    private Vector2 lastMousePosition;

    private float cooldown = 0;

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
        var delta = pos - lastMousePosition;
        lastMousePosition = pos;
        if (delta.magnitude > 75)
        {
            cooldown = 0.35f;
        }

        cooldown -= Time.deltaTime;

        if (DayManager.DayActive)
        {
            if (DayManager.Globals.RushHour)
            {
                if (DayManager.TimerActive)
                {
                    Head.sprite = BodyDown;
                    Face.sprite = (cooldown > 0 && isPressing) ? DrawingWearyFast : DrawingWeary;
                    Pen.rectTransform.anchoredPosition = Vector3.right * 4 - Vector3.right * 20 * pos.x / Screen.width + Vector3.up * -2 * (isPressing ? 4 : 0);
                }
                else
                {
                    Head.sprite = BodyUp;
                    Face.sprite = DayManager.Globals.LastGain > 0.8f * SessionVariables.MaxIncomeBase.Value * SessionVariables.IncomeMultiplier.Value ? ShowingWearyHappy : ShowingWearyNormal;
                }
            } else
            {
                if (DayManager.TimerActive)
                {
                    Head.sprite = BodyDown;
                    Face.sprite = (cooldown > 0 && isPressing) ? DrawingFast : DrawingNormal;
                    Pen.rectTransform.anchoredPosition = Vector3.right * 4 - Vector3.right * 20 * pos.x / Screen.width + Vector3.up * -2 * (isPressing ? 4 : 0);
                }
                else
                {
                    Head.sprite = BodyUp;
                    Face.sprite = DayManager.Globals.LastGain > 0.8f * SessionVariables.MaxIncomeBase.Value * SessionVariables.IncomeMultiplier.Value ? ShowingHappy : ShowingNormal;
                }
            }
        } else
        {
            Head.sprite = BodyUp;
            Face.sprite = ShowingRelieved;
        }
    }
}
