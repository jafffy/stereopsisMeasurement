using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

using UnityEngine;

public class ReplayManager : MonoBehaviour {
    public static ReplayManager instance;
    //The order of Objects should be same on Replay Scene and Stereopsis Scene.
	public List<GameObject> serializedObjects;
    List<CustomTransform> frameData;
    Dictionary<int, GameObject> handGameObjects;

    [HideInInspector]
    public HandSerializer left_hand;
    [HideInInspector]
    public HandSerializer right_hand;

    bool enableReplay = false;
    int cursor = 0;
    float timeBucket = 0f;
    const float fixedUpdateTime = 0.0333334f; // Ensure 30 fps
    

    void Awake()
    {
        if(instance == null) {
            instance = this;
        }
        else {
            Destroy(this);
        }
    }
	void FixedUpdate()
	{
        if(Time.time - timeBucket < fixedUpdateTime) //Maximum 60 Records per Second
        {
            return;
        }
        timeBucket = Time.time;

        if(enableReplay && frameData != null)
        {
            Render();
        }
        else
        {
            SaveData();
        }
	}

	void SaveData()
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
            if(left_hand != null)  {
                List<Vector3>[] indexedPositions = left_hand.GetHandPositions();
                for(int i = 0 ; i < indexedPositions.Length; i++)
                {
                    List<Vector3> positions = indexedPositions[i];
                    for(int j = 0 ; j < positions.Count; j++)
                    {
                        writer.WriteLine(SerializeTransfrom(GetLeftHandIndex(i,j), positions[j]));
                    }
                }
            }
            if(right_hand != null)  {
                List<Vector3>[] indexedPositions = right_hand.GetHandPositions();
                for(int i = 0 ; i < indexedPositions.Length; i++)
                {
                    List<Vector3> positions = indexedPositions[i];
                    for(int j = 0 ; j < positions.Count; j++)
                    {
                        writer.WriteLine(SerializeTransfrom(GetRightHandIndex(i,j) , positions[j]));
                    }
                }
            }
            writer.Flush();
        }
	}

    void Render()
    {
        if(cursor >= frameData.Count)
        {
            return;
        }
        int currentFrame = frameData[cursor].frame;
        CustomTransform currentFrameData = frameData[cursor++]; 
        while(currentFrame == frameData[cursor].frame)
        {
            if(currentFrameData.index < serializedObjects.Count)
            {
                serializedObjects[currentFrameData.index].transform.position = currentFrameData.position;
                serializedObjects[currentFrameData.index].transform.rotation = Quaternion.Euler(currentFrameData.rotation);
            }
            else 
            {
                GameObject target;
                if(handGameObjects.TryGetValue(currentFrameData.index, out target))
                {
                    target.transform.position = currentFrameData.position; 
                }
                else
                {
                    target = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    target.transform.localScale = Vector3.one/100;
                    handGameObjects.Add(currentFrameData.index, target);
                }
            }
            currentFrameData = frameData[cursor++]; 
        }
        //Cylinder
        for(int i = 0 ; i < 5;  i++) //finger count
        {
            for(int j = 0 ; j < 2; j++) //finger joint count
            {
                GameObject target;
                GameObject leftStart = null;
                handGameObjects.TryGetValue(GetLeftHandIndex(i,j) , out leftStart);
                if(leftStart != null)
                {
                    if(handGameObjects.TryGetValue(GetLeftHandCylinderIndex(i,j), out target))
                    {
                        RepositionCylinderWithTwoPoints(leftStart.transform.position, handGameObjects[GetLeftHandIndex(i,j+1)].transform.position, target);
                    }
                    else
                    {
                        handGameObjects.Add(GetLeftHandCylinderIndex(i,j) , CreateCylinderWithTwoPoints(leftStart.transform.position, handGameObjects[GetLeftHandIndex(i,j+1)].transform.position, 0.003f)); 
                    }
                }
                GameObject rightStart = null;
                handGameObjects.TryGetValue(GetRightHandIndex(i,j) , out rightStart);
                if(rightStart != null)
                {
                    if(handGameObjects.TryGetValue(GetRightHandCylinderIndex(i,j), out target))
                    {
                        RepositionCylinderWithTwoPoints(rightStart.transform.position, handGameObjects[GetRightHandIndex(i,j+1)].transform.position, target);
                    }
                    else
                    {
                        handGameObjects.Add(GetRightHandCylinderIndex(i,j) , CreateCylinderWithTwoPoints(rightStart.transform.position, handGameObjects[GetRightHandIndex(i,j+1)].transform.position, 0.003f)); 
                    }
                }
            }
        }
    }
    int GetLeftHandIndex(int i , int j ) { return 10*(i+1)+j; }
    int GetRightHandIndex(int i , int j ) { return 100*(i+1)+j; }
    int GetLeftHandCylinderIndex(int i , int j ) { return 1000*(i+1)+j; }
    int GetRightHandCylinderIndex(int i , int j ) { return 10000*(i+1)+j; }

    GameObject CreateCylinderWithTwoPoints(Vector3 start, Vector3 end, float width)
    {
        Vector3 offset = end - start;
		var scale = new Vector3(width, offset.magnitude / 2.0f, width);
		var position = start + (offset / 2.0f);

        GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

		cylinder.transform.up = offset;
		cylinder.transform.localScale = scale;
		cylinder.transform.position = position;
        return cylinder;
    }

    void RepositionCylinderWithTwoPoints(Vector3 start, Vector3 end, GameObject target)
    {
        Vector3 offset = end - start;
        var scale = new Vector3(target.transform.localScale.x, offset.magnitude / 2.0f, target.transform.localScale.x);
		var position = start + (offset / 2.0f);
        

		target.transform.up = offset;
		target.transform.position = position;
        target.transform.localScale = scale;
    }

    string SerializeTransfrom(int index, Transform tr)
    {
        return String.Format("{0},{1},{2},{3},{4},{5},{6}",
        index
        ,tr.position.x, tr.position.y, tr.position.z
        ,tr.rotation.x, tr.rotation.y, tr.rotation.z);
    }
    string SerializeTransfrom(int index, Vector3 pos)
    {
        return String.Format("{0},{1},{2},{3},{4},{5},{6}",
        index
        ,pos.x, pos.y, pos.z
        ,0 , 0 , 0);
    }

    void StartReplay()
    {
        enableReplay = true;
        cursor = 0;
        handGameObjects = new Dictionary<int, GameObject>();
    }

    public void LoadReplayData()
    {
        string filePath = "Assets/" + "replayData.csv";
		System.IO.FileInfo file = new System.IO.FileInfo(filePath);
        using (var reader = new StreamReader(filePath))
        {
            frameData = new List<CustomTransform>();
            string line = reader.ReadLine();
            int frameCount = -1;
            while(line != null && line.Length != 0)
            {
                string[] splitArray = line.Split(',');
                Vector3 position = new Vector3(float.Parse(splitArray[1]), float.Parse(splitArray[2]), float.Parse(splitArray[3]));
                Vector3 rotation = new Vector3(float.Parse(splitArray[4]), float.Parse(splitArray[5]), float.Parse(splitArray[6]));
                
                CustomTransform tr = new CustomTransform();
                if(int.Parse(splitArray[0]) == 0) {
                    frameCount ++;
                }
                tr.index = int.Parse(splitArray[0]);
                tr.position = position;
                tr.rotation = rotation;
                tr.frame = frameCount;

                frameData.Add(tr);

                line = reader.ReadLine();
            }
        }
        StartReplay();
    }
    

    class CustomTransform
    {
        public int frame;
        public int index;
        public Vector3 position;
        public Vector3 rotation;
    }
}
