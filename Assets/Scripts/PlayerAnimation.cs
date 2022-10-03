using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    private Vector3 lastMousePosition;

    private float cooldown = 0;

    // Update is called once per frame
    void Update()
    {
        var delta = Input.mousePosition - lastMousePosition;
        lastMousePosition = Input.mousePosition;
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
                    Face.sprite = (cooldown > 0 && Input.GetMouseButton(0)) ? DrawingWearyFast : DrawingWeary;
                    Pen.rectTransform.anchoredPosition = Vector3.right * 4 - Vector3.right * 20 * Input.mousePosition.x / Screen.width + Vector3.up * -2 * (Input.GetMouseButton(0) ? 4 : 0);
                }
                else
                {
                    Head.sprite = BodyUp;
                    Face.sprite = DayManager.Globals.LastGain > 0.8f * SessionVariables.MaxIncomeBase * SessionVariables.IncomeMultiplier ? ShowingWearyHappy : ShowingWearyNormal;
                }
            } else
            {
                if (DayManager.TimerActive)
                {
                    Head.sprite = BodyDown;
                    Face.sprite = (cooldown > 0 && Input.GetMouseButton(0)) ? DrawingFast : DrawingNormal;
                    Pen.rectTransform.anchoredPosition = Vector3.right * 4 - Vector3.right * 20 * Input.mousePosition.x / Screen.width + Vector3.up * -2 * (Input.GetMouseButton(0) ? 4 : 0);
                }
                else
                {
                    Head.sprite = BodyUp;
                    Face.sprite = DayManager.Globals.LastGain > 0.8f * SessionVariables.MaxIncomeBase * SessionVariables.IncomeMultiplier ? ShowingHappy : ShowingNormal;
                }
            }
        } else
        {
            Head.sprite = BodyUp;
            Face.sprite = ShowingRelieved;
        }
    }
}
