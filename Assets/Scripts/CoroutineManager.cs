using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoroutineManager : MonoBehaviour
{
    private static CoroutineManager instance;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
            SceneManager.sceneUnloaded += a => AbortAll();
        }
    }

    public static Coroutine Start(IEnumerator function)
    {
        return instance.StartCoroutine(function);
    }

    public static void Abort(Coroutine function)
    {
        instance.StopCoroutine(function);
    }

    public static void AbortAll()
    {
        instance.StopAllCoroutines();
    }
}