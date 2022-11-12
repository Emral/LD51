using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Rain : MonoBehaviour
{

    private RectTransform _rt;

    public float speedMult = 1;

    public float doSineWave = 0;


    // Start is called before the first frame update
    void Start()
    {
        _rt = (RectTransform)transform;
        _rt.DOLocalMoveY(-_rt.sizeDelta.y * 0.75f, 0.45f * speedMult).SetLoops(-1).SetEase(Ease.Linear).OnComplete(() => _rt.localPosition = new Vector2(_rt.localPosition.x, 0));
    }

    private void Update()
    {
        if (doSineWave > 0)
        {
            var lp = _rt.localPosition;
            lp.x = Mathf.Sin(Time.time * 0.1f) * doSineWave;
        }
    }
}
