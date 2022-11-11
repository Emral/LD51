using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using Sirenix.OdinInspector.Editor;
using System.Reflection;
using System;

[ExecuteAlways]
public class ResetScriptableObjects : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        Reset(true);
    }

    [RuntimeInitializeOnLoadMethod]
    private static void ResetVariables()
    {
        Reset(EditorPrefs.GetBool("UseProductionValues", true));
    }

    private static void Reset(bool production)
    {
        Dictionary<Type, object[]> loaded = new Dictionary<Type, object[]>();
        foreach (Type type in Assembly.GetAssembly(typeof(AutoResetAttribute)).GetTypes())
        {
            foreach (var field in type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var attr = field.GetCustomAttribute<AutoResetAttribute>();

                if (attr != null)
                {
                    if (!loaded.ContainsKey(type))
                    {
                        loaded[type] = GetAllInstances(type);
                    }

                    foreach (var obj in loaded[type])
                    {
                        var source = obj.GetType().GetField(production ? attr.copySourceName : attr.copySourceTestName, BindingFlags.NonPublic | BindingFlags.Instance);
                        var val = source.GetValue(obj);
                        var fieldname = obj.GetType().GetField(field.Name, BindingFlags.NonPublic | BindingFlags.Instance);
                        switch (val)
                        {
                            case int:
                            case float:
                            case Enum:
                            case bool:
                                fieldname.SetValue(obj, val);
                                break;
                            default:
                                fieldname.SetValue(obj, Activator.CreateInstance(val.GetType(), val));
                                break;
                        }
                    }
                }
            }
        }
    }

    private static object[] GetAllInstances(Type t)
    {
        string[] guids = AssetDatabase.FindAssets("t:" + t.Name);  //FindAssets uses tags check documentation for more info
        object[] a = new object[guids.Length];
        for (int i = 0; i < guids.Length; i++)         //probably could get optimized 
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a[i] = AssetDatabase.LoadAssetAtPath(path, typeof(object));
        }

        return a;
    }
}

public static class DebugMenu
{
    private const string MenuName = "Tools/UseProductionValues";
    private const string SettingName = "UseProductionValues";

    public static bool IsEnabled
    {
        get { return EditorPrefs.GetBool(SettingName, false); }
        set { EditorPrefs.SetBool(SettingName, value); }
    }

    [MenuItem(MenuName)]
    private static void ToggleAction()
    {
        IsEnabled = !IsEnabled;
    }

    [MenuItem(MenuName, true)]
    private static bool ToggleActionValidate()
    {
        Menu.SetChecked(MenuName, IsEnabled);
        return true;
    }
}