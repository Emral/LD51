using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpritesheetAnimation : MonoBehaviour
{
    public List<Sprite> sprites;
    public float animationSpeed;
    public Image spriteRenderer;

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.sprite = sprites[Mathf.FloorToInt(Time.time * animationSpeed) % sprites.Count];
    }
}