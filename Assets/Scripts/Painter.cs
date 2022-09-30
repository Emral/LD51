using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Painter : MonoBehaviour
{
    public RenderTexture texture;
    private RawImage _img;

    // Start is called before the first frame update
    void Start()
    {
        _img = GetComponent<RawImage>();
        _img.texture = texture;
        texture.Create();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            _img.material.SetVector("_BrushPos", Input.mousePosition);
        }
    }

    private void ClearTexture()
    {
    }
}
