using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

using UnityEngine;
using UnityEngine.SceneManagement;

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
        else if (SceneManager.GetActiveScene().name != "ReplayScene")
        {
            SaveData();
        }
	}

	void SaveData()
	{
    string filePath = LoggingManager.GetPath("replayData.csv");
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
        while(cursor < frameData.Count && currentFrame == frameData[cursor].frame)
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
    string filePath = LoggingManager.GetPath("replayData.csv");
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
    
    public void IterateAllDirectory()
    {
        //5개 단위로 읽는다 (5개가 최소단위로 0을 놓치지 않는 단위임)
        const int minimumUnit = 5;
        const int indexBackwardOffset = 540;
        //File Path에 \와 /가 섞여서 존재함. split할 때 주의
        string dataDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "/data/";
        string[] allDates = Directory.GetDirectories(dataDirectory);
        foreach(string date in allDates)
        {
            string[] allPatientIDs = Directory.GetDirectories(date);
            foreach(string patientID in allPatientIDs)
            {
                if(patientID.Contains("test"))
                    continue;
                string path = patientID + "\\replayData.csv";
                if (File.Exists(path))
                {
                    using (StreamReader csv = new StreamReader(path))
                    {
                        string read = csv.ReadToEnd();
                        string[] rows = read.Split('\n');
                        int i = 1;
                        int j = 0;
                        int zeroCount = 0;
                        while (true)
                        {
                            for (i = 1; i < minimumUnit + 1; i++)
                            {
                                string[] elements = rows[rows.Length - (i + j * minimumUnit)].Split(',');
                                if (elements[0] == string.Empty)
                                {
                                    continue;
                                }
                                if (Int32.Parse(elements[0]) == 0)
                                {
                                    zeroCount++;
                                }
                                if (zeroCount >= indexBackwardOffset)
                                {
                                    //액셀에서는 아래의 Index값 +1이 맞음 (1부터 시작하므로)
                                    Debug.Log(path + " Index : " + (rows.Length - (i + j * minimumUnit)) + " " +
                                        "Contant : " + rows[rows.Length - (i + j * minimumUnit)]);
                                    break;
                                }
                            }
                            j++;
                            if (zeroCount >= indexBackwardOffset)
                                break;
                        }

                        string extractedFile = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) + "\\file.csv";
                        StreamWriter outStream;
                        if (File.Exists(extractedFile))
                        {
                            outStream = new StreamWriter(File.Open(extractedFile, FileMode.Append));
                        }
                        else
                        {
                            outStream = new StreamWriter(File.Create(extractedFile));
                        }

                        string replaced = path.Replace('/', '\\');
                        string[] splitedPath = replaced.Split('\\');
                        
                        string dateString = splitedPath[5];
                        string patientIdString = splitedPath[6];
                        //여기서 부터 rows.Length - (i+j*minimumUnit) 이 0에 해당한다. 
                        for (int k = 0; k < 4; k++)
                        {
                            int baseIndex = rows.Length - (i + j * minimumUnit);
                            Vector3 pos0, pos1;
                            Vector3 rot0, rot1;
                            ExtractPositionAndRotation(rows[baseIndex + k], out pos0, out rot0);
                            ExtractPositionAndRotation(rows[baseIndex + k + 1], out pos1, out rot1);
                            Vector3 deltaPos = pos1 - pos0;
                            Vector3 deltaRot = rot1 - rot0;
                            outStream.WriteLine(String.Format("{0},{1},{2},{3},{4},{5},{6},{7}",
                                dateString, patientIdString, baseIndex+k,
                                deltaPos.x, deltaPos.y, deltaPos.z,
                                Math.Abs(deltaPos.x), Math.Abs(deltaPos.y)));
                            //Debug.Log("Path : " + path + " Delta : " + deltaPos.ToString("F4"));
                        }
                        outStream.Close();
                    }
                }
                
            }
        }
    }

    private void ExtractPositionAndRotation(string source, out Vector3 pos, out Vector3 rot)
    {
        string[] elems = source.Split(',');
        pos = new Vector3(float.Parse(elems[1]), float.Parse(elems[2]), float.Parse(elems[3]));
        rot = new Vector3(float.Parse(elems[4]), float.Parse(elems[5]), float.Parse(elems[6]));
    }

    public void DataExtract()
    {
        //0, 1, 2, 3, 4 까지만 Box이다.
        int currentFrame = frameData[cursor].frame;
        CustomTransform currentData = frameData[cursor];
        while (cursor < frameData.Count && currentFrame == frameData[cursor].frame)
        {
            if (currentData.index > 3)
                return;
            //Debug.Log(currentData.position);
            Vector3 delta = frameData[cursor + 1].position - currentData.position;
            Debug.Log(" " + currentData.index + " " + delta.ToString("F4"));
            currentData = frameData[++cursor];
        }  
    }

    class CustomTransform
    {
        public int frame;
        public int index;
        public Vector3 position;
        public Vector3 rotation;
    }
}
