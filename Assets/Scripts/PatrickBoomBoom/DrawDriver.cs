using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawDriver : MonoBehaviour
{
    public Draw drawComponent;
    public RectTransform canvasRect;
    public Color brushColor = Color.black;
    public Camera MyCamera;
    public int size = 100;
    private readonly Color[] myColor = new Color[] { Color.black, Color.red, Color.green, Color.yellow };

    private bool _canDraw = false;

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
            //??
            if (Input.GetMouseButtonDown(0))
            {
                DrawStart();
            }
            if (Input.GetMouseButton(0))
            {
                DrawLine();
            }
            if (Input.GetMouseButtonUp(0))
            {
                DrawEnd();
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
        drawComponent.StartWrite(Input.mousePosition);
    }

    private void DrawLine()
    {
        if (MyCamera == null) return;

        drawComponent.Writing(Input.mousePosition);
    }

    public Texture2D FinishDrawing()
    {
        var tex = drawComponent.GetTexture();
        _canDraw = false;

        return tex;
    }

    public void EnableDrawing()
    {
        _canDraw = true;
    }

    private void DrawEnd()
    {
        //Debug.Log("??");
    }
}