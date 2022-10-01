using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAnimation : MonoBehaviour
{
    public Image Head;
    public Image Pen;
    public Image Face;
    public Image Sweat;

    public Sprite DrawingNormal;
    public Sprite DrawingWeary;
    public Sprite DrawingWorried;
    public Sprite Happy;
    public Sprite VeryHappy;
    public Sprite HappyExhausted;
    public Sprite Relieved;

    // Update is called once per frame
    void Update()
    {
        if (DayManager.DayActive)
        {
            if (DayManager.TimerActive)
            {
                Head.transform.localPosition = Vector3.up * -8;
                Face.sprite = DrawingNormal;
                Pen.rectTransform.anchoredPosition = Vector3.right * 4 - Vector3.right * 20 * Input.mousePosition.x / Screen.width + Vector3.up * -2 * (Input.GetMouseButton(0) ? 4 : 0);
            }
            else
            {
                Head.transform.localPosition = Vector3.zero;
                Face.sprite = Happy;
            }
        } else
        {
            Head.transform.localPosition = Vector3.zero;
            Face.sprite = Relieved;
        }
    }
}
