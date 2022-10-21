using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class DrawDriver : MonoBehaviour
{
    public Draw drawComponent;
    public RectTransform canvasRect;
    public Color brushColor = Color.black;
    public Camera MyCamera;
    public int size = 100;
    private readonly Color[] myColor = new Color[] { Color.black, Color.red, Color.green, Color.yellow };

    private bool _canDraw = false;

    private Mouse mouse;
    private Vector2 pointerPosition = Vector2.zero;
    private TouchControl touch;

    private bool isDrawing = false;

    private void Awake()
    {
        drawComponent.Init(canvasRect.GetComponent<Canvas>(), MyCamera);
        drawComponent.SetProperty(brushColor, size);
        _canDraw = true;

        Clear();
    }

    private void Update()
    {
        if (_canDraw)
        {
            mouse = Mouse.current;
            touch = Touchscreen.current?.primaryTouch;

            if (touch != null)
            {
                pointerPosition = touch.position.ReadValue();
            } else
            {
                pointerPosition = mouse.position.ReadValue();
            }

            //??
            if (!isDrawing && (mouse.leftButton.ReadValue() == 1 || touch?.pressure.ReadValue() > 0))
            {
                DrawStart();
            }
            if (isDrawing)
            {
                if ((mouse.leftButton.ReadValue() == 0 && (touch == null || touch?.pressure.ReadValue() == 0)))
                {
                    DrawEnd();
                } else
                {
                    DrawLine();
                }
            }
        }
    }

    public void ChangeColor(int colorIndex)
    {
        if (colorIndex >= 0 && colorIndex < myColor.Length)
            drawComponent.SetProperty(myColor[colorIndex]);
        else
            Debug.LogError("input color Error");
    }

    public void ChangeSize(int s)
    {
        drawComponent.SetProperty(s);
    }

    public void Clear()
    {
        drawComponent.Clear();
    }

    private void DrawStart()
    {
        if (MyCamera == null) return;
        //Debug.Log("??");
        isDrawing = true;
        drawComponent.StartWrite(pointerPosition, touch != null ? touch.pressure.ReadValue() : 1);
    }

    private void DrawLine()
    {
        if (MyCamera == null) return;

        drawComponent.Writing(pointerPosition, touch != null ? touch.pressure.ReadValue() : 1);
    }

    public void DisableDrawing()
    {
        _canDraw = false;
        isDrawing = false;
    }

    public Texture2D FinishDrawing()
    {
        var tex = drawComponent.GetTexture();
        _canDraw = false;
        isDrawing = false;

        return tex;
    }

    public void EnableDrawing()
    {
        _canDraw = true;
    }

    private void DrawEnd()
    {
        //Debug.Log("??");
        isDrawing = false;
    }
}