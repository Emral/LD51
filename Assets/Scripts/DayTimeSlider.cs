using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayTimeSlider : MonoBehaviour
{
    public Slider _slider;

    public RectTransform _rushHourSegment;

    // Start is called before the first frame update
    void Start()
    {
        DayManager.OnDayProgress.AddListener(OnProgress);
        _rushHourSegment.anchorMin = new Vector2(DayManager.Globals.RushHourStart, _rushHourSegment.anchorMin.y);
        _rushHourSegment.anchorMax = new Vector2(DayManager.Globals.RushHourEnd, _rushHourSegment.anchorMax.y);
    }
    private void OnDestroy()
    {
        DayManager.OnDayProgress.RemoveListener(OnProgress);
    }

    private void OnProgress(float a)
    {
        _slider.value = a;
    }
}
