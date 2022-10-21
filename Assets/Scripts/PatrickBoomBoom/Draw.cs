using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Draw : MonoBehaviour
{
    //??
    [SerializeField]
    private RawImage rawImage;
    [SerializeField]
    private Material brushMat;
    [SerializeField]
    private Material clearMat;

    //??
    private Camera m_uiCamera;
    private RenderMode m_renderMode;

    private float m_rawImageSizeX;
    private float m_rawImageSizeY;
    private Vector3 m_mousePos;
    private Vector3 m_lastMousePos;
    private RenderTexture m_renderTex;
    private RenderTexture m_lastRenderTex;
    private Canvas m_canvas;

    private float size = 0;

    public void Init(Canvas canvas, Camera uiCamera)
    {
        m_canvas = canvas;
        m_uiCamera = uiCamera;
        m_renderMode = canvas.renderMode;

        m_rawImageSizeX = rawImage.GetComponent<RectTransform>().sizeDelta.x;
        m_rawImageSizeY = rawImage.GetComponent<RectTransform>().sizeDelta.y;

        m_renderTex = RenderTexture.GetTemporary(256, 256);
        m_lastRenderTex = RenderTexture.GetTemporary(256, 256);
        rawImage.texture = m_renderTex;

        brushMat.SetColor("_Color", Color.black);
        brushMat.SetFloat("_Size", 300);
    }

    public void Release()
    {
        if (m_renderTex != null) m_renderTex.Release();
        if (m_lastRenderTex != null) m_lastRenderTex.Release();
    }

    public void SetProperty(Color brushColor, int size)
    {
        brushMat.SetColor("_Color", brushColor);
        brushMat.SetFloat("_Size", size);
        this.size = size;
    }
    public void SetProperty(Color brushColor)
    {
        brushMat.SetColor("_Color", brushColor);
    }
    public void SetProperty(int size)
    {
        brushMat.SetFloat("_Size", size);
    }

    public Texture2D GetTexture()
    {
        Texture2D tex = new Texture2D(m_renderTex.width, m_renderTex.height, TextureFormat.ARGB32, false);
        var old_rt = RenderTexture.active;
        RenderTexture.active = m_renderTex;

        tex.ReadPixels(new Rect(0, 0, m_renderTex.width, m_renderTex.height), 0, 0);
        tex.Apply();

        RenderTexture.active = old_rt;
        return tex;
    }

    public void Clear()
    {
        Graphics.Blit(null, m_renderTex, clearMat);
        Graphics.Blit(null, m_lastRenderTex, clearMat);
    }

    public void StartWrite(Vector3 pos, float scale)
    {
        m_mousePos = pos;
        m_lastMousePos = pos;
        brushMat.SetColor("_Color", DayManager.Globals.PenColor);
        brushMat.SetFloat("_Size", scale * size);
    }

    public void Writing(Vector3 pos, float scale)
    {
        m_mousePos = pos;
        brushMat.SetFloat("_Size", scale * size);
        Paint();
        m_lastMousePos = pos;
    }

    void Paint()
    {
        var uv = GetUV(m_mousePos);
        var last = GetUV(m_lastMousePos);

        brushMat.SetTexture("_Tex", m_renderTex);
        brushMat.SetVector("_UV", uv);
        brushMat.SetVector("_LastUV", last);

        Graphics.Blit(m_renderTex, m_renderTex, brushMat);
    }

    Vector2 GetUV(Vector2 brushPos)
    {
        //?????????????
        Vector2 rawImagePos = Vector2.zero;

        //???????????????????????????
        switch (m_renderMode)
        {
            case RenderMode.ScreenSpaceOverlay:
                rawImagePos = rawImage.rectTransform.position;
                break;
            default:
                rawImagePos = m_uiCamera.WorldToScreenPoint(rawImage.rectTransform.position);
                break;
        }
        var scale = m_canvas.scaleFactor; //new Vector2(Screen.width / 640, Screen.height / 360);
        //???????????????
        Vector2 pos = brushPos / scale - rawImagePos / scale;
        //????????UV??
        Vector2 uv = new Vector2(pos.x / (m_rawImageSizeX) + 0.5f, pos.y / (m_rawImageSizeY) + 0.5f);

        return uv;
    }
}