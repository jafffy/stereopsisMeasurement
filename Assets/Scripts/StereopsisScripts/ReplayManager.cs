using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

using UnityEngine;

public class ReplayManager : MonoBehaviour {
    //The order of Objects should be same on Replay Scene and Stereopsis Scene.
    
    public static ReplayManager instance;
	public List<GameObject> serializedObjects;
    public Mesh _sphereMesh;
    public Material _material;
    List<CustomTransform> frameData;

    [HideInInspector]
    public HandSerializer left_hand;
    [HideInInspector]
    public HandSerializer right_hand;

    bool enableReplay = false;
    int cursor = 0;

    void Awake()
    {
        if(instance == null) {
            instance = this;
        }
        else {
            Destroy(this);
        }

        if(!enableReplay)
        {

        }
    }
	void Update()
	{
        if(enableReplay && frameData != null)
        {
            while(true)
            {
                int currentFrame = frameData[cursor].frame;
                CustomTransform currentFrameData = frameData[cursor++]; 
                if(currentFrameData.index < serializedObjects.Count)
                {
                    serializedObjects[currentFrameData.index].transform.position = currentFrameData.position;
                    serializedObjects[currentFrameData.index].transform.rotation = Quaternion.Euler(currentFrameData.rotation);
                }
                else
                {
                    Graphics.DrawMesh(_sphereMesh, Matrix4x4.TRS(currentFrameData.position,
                    Quaternion.identity, Vector3.one/100), _material, 0);
                }

                if(currentFrame != frameData[cursor].frame)
                    return;
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
            int i;
            for(i = 0; i < serializedObjects.Count; i++)
            {
                writer.WriteLine(SerializeTransfrom(i, serializedObjects[i].transform));
            }
            if(left_hand != null)  {
                List<Vector3> positions = left_hand.GetHandPositions();
                foreach(Vector3 pos in positions) { 
                    writer.WriteLine(SerializeTransfrom(i++, pos));
                }
            }
            if(right_hand != null)  {
                List<Vector3> positions = right_hand.GetHandPositions();
                foreach(Vector3 pos in positions) { 
                    writer.WriteLine(SerializeTransfrom(i++, pos));
                }
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
    string SerializeTransfrom(int index, Vector3 pos)
    {
        return String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
        index
        ,pos.x, pos.y, pos.z
        ,0 , 0 , 0
        ,1, 1, 1);
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
            int frameCount = 0;
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

                if(tr.index == 0)
                    frameCount ++;
                tr.frame = frameCount;

                frameData.Add(tr);

                line = reader.ReadLine();
            }
        }
        StartReplay();
    }
    void StartReplay()
    {
        enableReplay = true;
        cursor = 0;
    }

    class CustomTransform
    {
        public int frame;
        public int index;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;
    }
}
