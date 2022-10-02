using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallMetaManagerFunction : MonoBehaviour
{
    public void LoadPreGame()
    {
        MetaManager.instance.TransitionToPreGameScene();
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
}
