using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayTimeBlend : MonoBehaviour
{

    public CanvasGroup _canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        DayManager.OnDayProgress.AddListener(OnProgress);
        _canvasGroup.alpha = 0;
    }
    private void OnDestroy()
    {
        DayManager.OnDayProgress.RemoveListener(OnProgress);
    }

    private void OnProgress(float a)
    {
        _canvasGroup.alpha = a;
    }
}
