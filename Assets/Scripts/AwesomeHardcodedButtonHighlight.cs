using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AwesomeHardcodedButtonHighlight : MonoBehaviour
{
    public ButtonEffects buttonEffects;

    public Image img;
    public Gradient gradient;

    // Update is called once per frame
    void Update()
    {
        if (buttonEffects.IsIdle())
        {
            img.enabled = true;
            img.color = gradient.Evaluate(Time.time * 0.5f % 1);
            transform.localScale = Vector3.one * (1.1f + (Mathf.Sin(Time.time * 2f)) * 0.1f);
        } else
        {
            img.enabled = false;
        }
    }
}
