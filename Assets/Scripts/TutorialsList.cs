using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TutorialElement
{
    public Mechanics mechanic;
    public string title;
    public string description;
}

[CreateAssetMenu(menuName = "Data/Tutorial", fileName = "Tutorial")]
public class TutorialsList : ScriptableObject
{
    public List<TutorialElement> entries;

    public TutorialElement Get(Mechanics mech)
    {
        return entries.Find(m => m.mechanic == mech);
    }
}
