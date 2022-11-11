using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event", fileName = "SharedEvent")]
public class SharedEvent : ScriptableObject
{
    public UnityEvent Event;

    public void Invoke()
    {
        Event.Invoke();
    }
}
