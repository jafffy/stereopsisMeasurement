using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoggingManager : MonoBehaviour {

	public string patientID;

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}
	public void OnFlush()
	{
		if(IsValid(patientID))
		{

		}
		else
		{
			Debug.LogWarning("Patient ID가 정상적이지 않습니다. 확인해주세요");
		}
	}

	bool IsValid(string id)
	{
		return true;
	}
}
