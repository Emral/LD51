using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class Painter : MonoBehaviour
{
    public RenderTexture texture;
    public RenderTexture buffer;
    private RawImage _img;
    private RectTransform _rt;
    private Camera _cam;


    // Start is called before the first frame update
    void Start()
    {
        _cam = GetComponentInParent<Canvas>().worldCamera;
        _rt = GetComponent<RectTransform>();
        _img = GetComponent<RawImage>();
        _img.texture = texture;
        texture.Create();
        ClearTexture();
        Graphics.Blit(Texture2D.whiteTexture, buffer, _img.material);
    }

    // Update is called once per frame
    void Update()
    {
        var mouse = Mouse.current;
        var touch = Touchscreen.current;

        if (mouse.leftButton.ReadValue() == 1 || touch.primaryTouch.pressure.ReadValue() > 0)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_rt, Input.mousePosition, _cam, out Vector2 localPoint))
            {
                Vector2 normalizedPoint = Rect.PointToNormalized(_rt.rect, localPoint);

                RenderTexture.active = texture;
                GL.Clear(true, true, Color.black);
                _img.material.SetVector("_BrushPos", normalizedPoint);
                Graphics.Blit(texture, texture, _img.material);
                _img.texture = texture;
            } else
            {
                _img.material.SetVector("_BrushPos", -Vector2.one);
            }
        }
    }

    private void ClearTexture()
    {
        texture.DiscardContents();
        Graphics.Blit(Texture2D.whiteTexture, texture, _img.material);
    }
}
