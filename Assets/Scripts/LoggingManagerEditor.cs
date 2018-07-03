using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LoggingManager))] //? 이거 왜이럼
[CanEditMultipleObjects]
public class LoggingManagerEditor : Editor {

	SerializedProperty patientID;
	void OnEnable()
    {
        patientID = serializedObject.FindProperty("patientID");
    }

    public override void OnInspectorGUI()
    {
		serializedObject.Update();
		EditorGUILayout.PropertyField(patientID);
        serializedObject.ApplyModifiedProperties();
		if(GUILayout.Button("Make Data File"))
		{
			Debug.Log("Make Data File");
		}
    }

    public void OnSceneGUI()
    {

    }
}
