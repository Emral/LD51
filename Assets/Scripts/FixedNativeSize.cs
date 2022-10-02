using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FixedNativeSize : MonoBehaviour
{
    private Image _img;

    // Start is called before the first frame update
    void Start()
    {
        _img = GetComponent<Image>();    
    }

    // Update is called once per frame
    void LateUpdate()
    {
        _img.SetNativeSize();
    }
}
