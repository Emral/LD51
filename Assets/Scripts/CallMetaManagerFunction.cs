using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallMetaManagerFunction : MonoBehaviour
{
    public void LoadPreGame()
    {
        MetaManager.instance.TransitionToPreGameScene();
    }

    public void StartNewGame()
    {
        MetaManager.instance.StartNewGame();
    }

    public void LoadGame()
    {
        MetaManager.instance.TransitionToGameScene();
    }

    public void LoadPostGame()
    {
        MetaManager.instance.TransitionToPostGameScene();
    }

    public void LoadTitle()
    {
        MetaManager.instance.TransitionToTitleScene();
    }

    public void SuspendGame()
    {
        MetaManager.instance.TransitionToTitleScene();
    }

    public void OpenOptionsMenu()
    {
        MetaManager.instance.options.Open();
    }
}
