using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public SFX sound;
    void Start()
    {
        sound.Play();
    }
}
