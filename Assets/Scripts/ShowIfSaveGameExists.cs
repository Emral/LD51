using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowIfSaveGameExists : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(MetaManager.instance.HasSaveData());
    }
}
