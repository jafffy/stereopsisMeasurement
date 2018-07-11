using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

using UnityEngine;

public class ReplayManager : MonoBehaviour {

	public List<GameObject> serializedObjects;
    List<CustomTransform> frameData;

    public bool enableReplay = false;
    int cursor = 0;

	void Update()
	{
        if(enableReplay && frameData != null)
        {
            for (int i = 0 ; i < serializedObjects.Count; i ++)
            {
                CustomTransform currentFrameData = frameData[cursor++]; 
                serializedObjects[currentFrameData.index].transform.position = currentFrameData.position;
                serializedObjects[currentFrameData.index].transform.rotation = Quaternion.Euler(currentFrameData.rotation);
                serializedObjects[currentFrameData.index].transform.localScale = currentFrameData.scale;
            }
        }
        else
        {
            Flush();
        }
	}

	void Flush()
	{
        string filePath = "Assets/" + "replayData.csv";
		System.IO.FileInfo file = new System.IO.FileInfo(filePath);
        file.Directory.Create();
        using (var writer = new StreamWriter(filePath, append: true))
        {
            for(int i = 0; i < serializedObjects.Count; i++)
            {
                writer.WriteLine(SerializeTransfrom(i, serializedObjects[i].transform));
            }
            writer.Flush();
        }
	}
    
    string SerializeTransfrom(int index, Transform tr)
    {
        return String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
        index
        ,tr.position.x, tr.position.y, tr.position.z
        ,tr.rotation.x, tr.rotation.y, tr.rotation.z
        ,tr.localScale.x , tr.localScale.y, tr.localScale.z);
    }

    public void LoadReplayData()
    {
        string filePath = "Assets/" + "replayData.csv";
		System.IO.FileInfo file = new System.IO.FileInfo(filePath);
        file.Directory.Create();
        using (var reader = new StreamReader(filePath))
        {
            frameData = new List<CustomTransform>();
            string line = reader.ReadLine();
            while(line != null && line.Length != 0)
            {
                string[] splitArray = line.Split(',');
                Vector3 position = new Vector3(float.Parse(splitArray[1]), float.Parse(splitArray[2]), float.Parse(splitArray[3]));
                Vector3 rotation = new Vector3(float.Parse(splitArray[4]), float.Parse(splitArray[5]), float.Parse(splitArray[6]));
                Vector3 scale = new Vector3(float.Parse(splitArray[7]), float.Parse(splitArray[8]), float.Parse(splitArray[9]));
                
                CustomTransform tr = new CustomTransform();
                tr.index = int.Parse(splitArray[0]);
                tr.position = position;
                tr.rotation = rotation;
                tr.scale = scale;

                frameData.Add(tr);

                line = reader.ReadLine();
            }
        }
        StartReplay();
    }
    public void StartReplay()
    {
        enableReplay = true;
        cursor = 0;
    }

    class CustomTransform
    {
        public int index;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
    }
}
