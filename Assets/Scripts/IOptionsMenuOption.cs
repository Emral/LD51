using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOptionsMenuOption
{
    void OnSave();
    void OnDiscard();
    void OnRestore();
}
