using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Tags))]
public class EnumWriter : Editor
{
    Tags myScrip;
    string filePath = "Assets/Scripts/";
    string fileName = "Tag";

    private void OnEnable()
    {
        myScrip = (Tags)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        filePath = EditorGUILayout.TextField("Path", filePath);
        fileName = EditorGUILayout.TextField("Name", fileName);
        if (GUILayout.Button("Save"))
        {
            EdiorMethods.WriteToEnum(filePath, fileName, myScrip.tags);
        }
    }
}