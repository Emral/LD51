using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeTextIfSaveGameExists : MonoBehaviour
{
    public string HasSaveDataText;

    void Start()
    {
        if (MetaManager.instance.HasSaveData())
        {
            GetComponent<Text>().text = string.Format(HasSaveDataText, MetaManager.instance.ReadDayFromSave() + 1);
        }
    }
}
