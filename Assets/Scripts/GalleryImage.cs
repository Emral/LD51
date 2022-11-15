using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GalleryImage : MonoBehaviour
{
    public Image img;
    public Button button;
    public Image checkImage;
    public Sprite checkImageSprite;
    private Texture2D tex;
    private int index;
    private System.DateTime time;


    public void Initialize(Texture2D tex, int idx)
    {
        index = idx;
        this.tex = tex;
        img.sprite = Sprite.Create(tex, new Rect(0, 0, 256, 256), Vector2.one * 0.5f);
        time = System.DateTime.Now;
    }

    public void ClickButton()
    {
        button.interactable = false;
        checkImage.sprite = checkImageSprite;
        var path = Path.Combine(Application.persistentDataPath, "Drawings");
        Destroy(GetComponentInChildren<ButtonEffects>());

        var i = 0;
        foreach(Color c in tex.GetPixels())
        {
            if (c.a < 1)
            {
                tex.SetPixel(i % tex.width, Mathf.FloorToInt(i/tex.width), Color.white);
            }
            i++;
        }
        tex.Apply();
        var png = tex.EncodeToPNG();
        var file = File.Create(Path.Combine(path, $"{time.ToShortDateString()}_{time.ToShortTimeString().Replace(':', '.')}_Day_{SessionVariables.Day.Value + 1}_Image_{index+1}.png"));
        file.Write(png);
        file.Flush();
        file.Close();
        SFX.ButtonPress.Play();
    }
}
