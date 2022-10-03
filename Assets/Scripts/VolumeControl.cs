using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using DG.Tweening;

public class VolumeControl : MonoBehaviour
{
    public AudioMixer mixer;
    public CanvasGroup VolumeGroup;
    public Slider VolumeSlider;

    private float volume = 80;

    Tweener t;

    // Start is called before the first frame update
    void Start()
    {
        mixer.SetFloat("masterVol", Mathf.Max(-80f, Mathf.Log(Mathf.Clamp01(volume * 0.01f)) * 20));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            volume = Mathf.Clamp(volume + Input.mouseScrollDelta.y * 2, 0, 100);
            mixer.SetFloat("masterVol", Mathf.Max(-80f, Mathf.Log(Mathf.Clamp01(volume * 0.01f)) * 20));
            VolumeGroup.alpha = 2;

            if (t != null)
            {
                t.Kill();
                t = null;
            }

            t = VolumeSlider.DOValue(volume / 100, 0.3f).SetEase(Ease.OutQuad);
        }

        VolumeGroup.alpha = VolumeGroup.alpha - Time.deltaTime;
    }
}
