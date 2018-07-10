using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

using UnityEngine;

public class ReplayLogger : MonoBehaviour {

	public List<GameObject> serializedObjects;

	void Start()
	{
        Flush();
	}

	void Update()
	{
		
	}

	void Flush()
	{
        string filePath = "Assets/" + "replayData.csv";
		System.IO.FileInfo file = new System.IO.FileInfo(filePath);
        file.Directory.Create();
        using (var writer = new StreamWriter(filePath, append: false))
        {
            foreach(GameObject g in serializedObjects)
            {
                writer.WriteLine(SerializeTransfrom(g.transform));
            }
            writer.Flush();
        }
	}
    
    string SerializeTransfrom(Transform tr)
    {
        return String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}"
        ,tr.position.x, tr.position.y, tr.position.z
        ,tr.rotation.x, tr.rotation.y, tr.rotation.z
        ,tr.localScale.x , tr.localScale.y, tr.localScale.z);
    }
}
