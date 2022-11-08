using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioOption : MonoBehaviour, IOptionsMenuOption
{
    public Slider slider;
    public AudioMixer mixer;
    public string audioMixerVariableName;
    public string PlayerPrefsIndex;
    public float defaultValue; // When merging this into the toybox, turn it into a self-resetting SO reference

    private bool isInitialized = false;

    private float GetVolumeInDB(float volume)
    {
        return Mathf.Max(-80f, Mathf.Log(Mathf.Clamp01(volume)) * 20);
    }

    private void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }

    void OnSliderValueChanged(float newValue)
    {
        mixer.SetFloat(audioMixerVariableName, GetVolumeInDB(newValue));
    }

    public void OnDiscard()
    {
        slider.SetValueWithoutNotify(PlayerPrefs.GetFloat(PlayerPrefsIndex, defaultValue));
        PlayerPrefs.SetFloat(PlayerPrefsIndex, slider.value);
    }

    public void OnSave()
    {

        PlayerPrefs.SetFloat(PlayerPrefsIndex, slider.value);
    }

    public void OnRestore()
    {
        if (!isInitialized)
        {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
            isInitialized = true;
        }

        float volume = PlayerPrefs.GetFloat(PlayerPrefsIndex, defaultValue);

        slider.SetValueWithoutNotify(volume);

        mixer.SetFloat(audioMixerVariableName, GetVolumeInDB(volume));
    }
}
