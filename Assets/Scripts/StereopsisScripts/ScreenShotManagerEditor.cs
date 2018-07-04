using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ScreenShotManager))]
[CanEditMultipleObjects]
public class ScreenShotManagerEditor : Editor {

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();
		if(GUILayout.Button("Make Screen Shot"))
		{
			ScreenShotManager.MakeScreenShot();
		}
    }
}
