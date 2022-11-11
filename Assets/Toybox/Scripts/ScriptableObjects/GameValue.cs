using GenericUnityObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Newtonsoft.Json;
#if UNITY_EDITOR
using UnityEditor;
using Sirenix.OdinInspector.Editor;
#endif

[System.Serializable]
[CreateGenericAssetMenu]
public class GameValue<T> : GameValueBase
{
    [LabelText("Default value")]
    [SerializeField] protected T _value;
    [LabelText("Debug value")]
    [SerializeField] protected T _testValue;

    [AutoReset("_value", "_testValue")]
    protected T v;

    public System.Action<T> OnValueChanged;

    public T Value
    {
        get { return v; }
        set
        {
            v = value;
            OnValueChanged?.Invoke(v);
        }
    }

    internal override void Restore(string saveDataValue)
    {
        if (_persistent)
        {
            v = JsonConvert.DeserializeObject<T>(saveDataValue);
        }
    }

    internal override object Save()
    {
        if (_persistent)
        {
            return v;
        }
        return null;
    }

    internal override void ResetToDefault()
    {
#if !UNITY_EDITOR
        Value = _value;
#else
        if (!EditorPrefs.GetBool("UseProductionValues", true))
        {
            Value = _testValue;
        } else
        {
            Value = _value;
        }
#endif
    }

    public override bool Equals(object b)
    {
        return Value.Equals(b);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator == (GameValue<T> a, T b)
    {
        return a.Value.Equals(b);
    }

    public static bool operator !=(GameValue<T> a, T b)
    {
        return !(a.Value.Equals(b));
    }
}

#if UNITY_EDITOR

public class MyProcessedClassAttributeProcessor : OdinAttributeProcessor<GameValueBase>
{
    public override void ProcessChildMemberAttributes(
    InspectorProperty parentProperty,
    System.Reflection.MemberInfo member,
    List<System.Attribute> attributes)
    {
        // These attributes will be added to all of the child elements.
        attributes.Add(new HideLabelAttribute());
        attributes.Add(new BoxGroupAttribute("Box", showLabel: false));

        // Here we add attributes to child properties respectively.
        if ((member.Name == "_value" || member.Name == "_testValue" ) && (member.GetType() == typeof(UnityEngine.Object) || member.GetType() == typeof(List<UnityEngine.Object>)))
        {
            attributes.Add(new InlineEditorAttribute());
        }
    }
}

#endif

public abstract class GameValueBase : GenericScriptableObject
{
    [LabelText("Store in SaveData")]
    [SerializeField] internal bool _persistent;

    internal virtual void Restore(string saveDataValue)
    {
        if (_persistent)
        {
            
        }
    }

    internal virtual object Save()
    {
        if (_persistent)
        {

        }
        return null;
    }


    internal virtual void ResetToDefault() { 
        
    }
}