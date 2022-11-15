using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class OpenInExplorer : MonoBehaviour
{
    public GameObject button;
    public Text text;
    // Add a "Screenshots" level to the path
    string screenshotFilePath;

    private void Start()
    {
        screenshotFilePath = Path.Combine(Application.persistentDataPath, "Drawings");
        if (!Directory.Exists(screenshotFilePath))
        {
            Directory.CreateDirectory(screenshotFilePath);
        }

        //if (Application.platform == RuntimePlatform.LinuxPlayer)
        //{
        //    button.SetActive(false);
        //    text.gameObject.SetActive(true);
        //    text.text = $"Images are saved to\n{screenshotFilePath}";
        //}
    }

    public void Click()
    {
        System.Diagnostics.Process.Start(screenshotFilePath);
    }
}
