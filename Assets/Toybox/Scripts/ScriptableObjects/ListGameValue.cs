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
public class ListGameValue<T> : GameValueBase, IEnumerable<T>
{
    [LabelText("Default value")]
    [SerializeField] protected List<T> _value = new List<T>();
    [LabelText("Debug value")]
    [SerializeField] protected List<T> _testValue = new List<T>();

    [AutoReset("_value", "_testValue")]
    protected List<T> v = new List<T>();

    public System.Action<List<T>> OnValueChanged;
    public System.Action<List<T>, T> OnItemAdded;
    public System.Action<List<T>, T> OnItemRemoved;

    public int Count => Value.Count;

    public List<T> Value
    {
        get { return v; }
        set
        {
            v = value;
            OnValueChanged?.Invoke(v);
        }
    }

    internal void Add(T item)
    {
        v.Add(item);
        OnItemAdded?.Invoke(v, item);
    }

    internal void AddUnique(T item)
    {
        if (!v.Contains(item))
        {
            Add(item);
        }
    }

    internal void RemoveAt(int idx)
    {
        if (idx < v.Count && idx >= 0)
        {
            var item = v[idx];
            v.RemoveAt(idx);
            OnItemRemoved?.Invoke(v, item);
        }
    }

    internal void Remove(T item)
    {
        if (v.Contains(item))
        {
            v.Remove(item);
            OnItemRemoved?.Invoke(v, item);
        }
    }
    public T this[int key]
    {
        get => Value[key];
        set => Value[key] = value;
    }

    internal override void Restore(string saveDataValue)
    {
        if (_persistent)
        {
            v = JsonConvert.DeserializeObject<List<T>>(saveDataValue);
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
            Value = new List<T>(_testValue);
        }
        else
        {
            Value = new List<T>(_value);
        }
#endif
    }

    public IEnumerator<T> GetEnumerator()
    {
        return Value.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Value.GetEnumerator();
    }
}