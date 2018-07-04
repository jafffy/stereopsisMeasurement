using Fove.Managed;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EyeTrackingBehaviourScript : MonoBehaviour {
    public FoveInterface foveInterface;

    public string dataFileID;

    public Vector3 leftEye;
    public Vector3 rightEye;

    class EyeTrackingRecord
    {
        public readonly float Timer = 0.0f;
        public Vector3 Origin;
        public Vector3 Direction;
        public Vector3 TouchedPosition;

        public EyeTrackingRecord(float timer, Vector3 origin, Vector3 direction, Vector3 touchedPosition)
        {
            Timer = timer;
            Origin = origin;
            Direction = direction;
            TouchedPosition = touchedPosition;
        }
    }

    private readonly List<EyeTrackingRecord> _leftRecords = new List<EyeTrackingRecord>();
    private readonly List<EyeTrackingRecord> _rightRecords = new List<EyeTrackingRecord>();

    public float Timer = 0.0f;
    public float SaveTimer = 0.0f;

    private float distanceThreshold = 0.0f;
    private bool goForward = true;

    private bool _shouldStop = false;
    private bool _shouldLeftEyeDark = true;
    private float _shouldEyeDarkTimer = 0.0f;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        foveInterface = SceneLoader.instance.foveInterface.GetComponentInChildren<FoveInterface>();
    }
    // Update is called once per frame
    void Update()
    {
        FoveInterfaceBase.EyeRays eyeRays = foveInterface.GetGazeRays();

        RaycastHit leftRaycastHit, rightRaycastHit;
        Physics.Raycast(eyeRays.left, out leftRaycastHit, Mathf.Infinity);
        if (leftRaycastHit.point != Vector3.zero)
        {
            leftEye = leftRaycastHit.point;
        }
        else
        {
            leftEye = eyeRays.left.GetPoint(3.0f);
        }
        _leftRecords.Add(new EyeTrackingRecord(Timer, eyeRays.left.origin, eyeRays.left.direction, leftEye));

        Physics.Raycast(eyeRays.right, out rightRaycastHit, Mathf.Infinity);
        if (rightRaycastHit.point != Vector3.zero)
        {
            rightEye = rightRaycastHit.point;
        }
        else
        {
            rightEye = eyeRays.right.GetPoint(3.0f);
        }
        _rightRecords.Add(new EyeTrackingRecord(Timer, eyeRays.right.origin, eyeRays.right.direction, rightEye));

        if (SaveTimer > 1.0f)
        {
            string leftPath = "Assets/" + LoggingManager.instance.patientID+ "/" + dataFileID + "_left.csv";
            System.IO.FileInfo file = new System.IO.FileInfo(leftPath);
            file.Directory.Create();
            using (var writer = new StreamWriter(leftPath, append: true))
            {
                foreach (EyeTrackingRecord record in _leftRecords)
                {
                    writer.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                        record.Timer,
                        record.Origin.x,
                        record.Origin.y,
                        record.Origin.z,
                        record.Direction.x,
                        record.Direction.y,
                        record.Direction.z,
                        record.TouchedPosition.x,
                        record.TouchedPosition.y,
                        record.TouchedPosition.z
                    );
                }

                writer.Flush();

                _leftRecords.Clear();
            }

            string rightPath = "Assets/" + LoggingManager.instance.patientID+ "/" + dataFileID + "_right.csv";
            file = new System.IO.FileInfo(rightPath);
            file.Directory.Create();
            using (var writer = new StreamWriter(rightPath, append: true))
            {
                foreach (EyeTrackingRecord record in _rightRecords)
                {
                    writer.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                        record.Timer,
                        record.Origin.x,
                        record.Origin.y,
                        record.Origin.z,
                        record.Direction.x,
                        record.Direction.y,
                        record.Direction.z,
                        record.TouchedPosition.x,
                        record.TouchedPosition.y,
                        record.TouchedPosition.z
                    );
                }

                writer.Flush();

                _rightRecords.Clear();
            }

            SaveTimer = 0.0f;
        }

        SaveTimer += Time.deltaTime; 
        Timer += Time.deltaTime;
    }
}

/* 
1. ??? deltaTime을 더하는게 맞을까? Time.time을 쓰는게 아니라?
-> deltaTime을 중첩하면 어짜피 지난 시간이 되는거 아닌가? 
2. _leftRecords 가 2개 이상이 되면 어떤게 Left이고 어떤게 right인지 구별할 수 있나?
-> 이 경우가 없을 것 같음 Update는 한 프레임에 한 번 호출 (frame이 떨어질 경우 호출되지 않지만)
    따라서 _leftRecords에는 항상 하나의 record가 있을 것이다. 
3. 
*/