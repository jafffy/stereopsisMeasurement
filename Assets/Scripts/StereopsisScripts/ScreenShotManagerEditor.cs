using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ScreenShotManager))]
[CanEditMultipleObjects]
public class ScreenShotManagerEditor : Editor {
    SerializedProperty autoCaptureInterval;

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        autoCaptureInterval = serializedObject.FindProperty("autoCaptureInterval");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(autoCaptureInterval);
        serializedObject.ApplyModifiedProperties();
		if(GUILayout.Button("Make Screen Shot"))
		{
			ScreenShotManager.MakeScreenShot();
		}
    }
}
