using Fove.Managed;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CarBehaviourScript : MonoBehaviour {
    public FoveInterface foveInterface;

    public string patientID;

    public GameObject leftEye;
    public GameObject rightEye;

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

    // Use this for initialization
    void Start () {
		foveInterface.LeftFoveEye.ShouldDraw = true;
		foveInterface.RightFoveEye.ShouldDraw = true;

        foveInterface.leftEye = leftEye;
        foveInterface.rightEye = rightEye;
	}

	private bool _shouldStop = false;
	private bool _shouldLeftEyeDark = true;
	private float _shouldEyeDarkTimer = 0.0f;
	
	// Update is called once per frame
	void Update () {
        Vector3 pos = transform.position;

        SFVR_Quaternion orientation = FoveInterface.GetLastPose().orientation;
        Quaternion quat = new Quaternion(orientation.x, orientation.y, orientation.z, orientation.w);
        Vector3 forwad = quat * Vector3.forward;

		if (_shouldStop) {
			if (_shouldLeftEyeDark) {
				foveInterface.LeftFoveEye.ShouldDraw = false;
				foveInterface.RightFoveEye.ShouldDraw = true;
			} else {
				foveInterface.LeftFoveEye.ShouldDraw = true;
				foveInterface.RightFoveEye.ShouldDraw = false;
			}

			_shouldEyeDarkTimer += Time.deltaTime;

			if (_shouldEyeDarkTimer > 3.0f) {
				_shouldEyeDarkTimer = 0.0f;
				_shouldLeftEyeDark = !_shouldLeftEyeDark;

                if (_shouldLeftEyeDark)
                {
                    print("left flick");
                }
                else
                {
                    print("right flick");
                }
			}
		} else {
			float speed = 20;

			distanceThreshold += speed * Time.deltaTime;

			if (goForward && distanceThreshold > speed * 5) {
				distanceThreshold = 0.0f;
				goForward = false;
				_shouldStop = true;
			} else if (!goForward && distanceThreshold > speed * 5) {
				distanceThreshold = 0.0f;
				goForward = true;
			}

			if (goForward) {
				pos += forwad * speed * Time.deltaTime;
			} else {
				pos -= forwad * speed * Time.deltaTime;
			}
			transform.position = pos;
		}

        FoveInterfaceBase.EyeRays eyeRays = foveInterface.GetGazeRays();

        RaycastHit leftRaycastHit, rightRaycastHit;
        Physics.Raycast(eyeRays.left, out leftRaycastHit, Mathf.Infinity);
        if (leftRaycastHit.point != Vector3.zero)
        {
            leftEye.transform.position = leftRaycastHit.point;
        }
        else 
        {
            leftEye.transform.position = eyeRays.left.GetPoint(3.0f);
        }
        _leftRecords.Add(new EyeTrackingRecord(Timer, eyeRays.left.origin, eyeRays.left.direction, leftEye.transform.position));

        Physics.Raycast(eyeRays.right, out rightRaycastHit, Mathf.Infinity);
        if (rightRaycastHit.point != Vector3.zero)
        {
            rightEye.transform.position = rightRaycastHit.point;
        }
        else
        {
            rightEye.transform.position = eyeRays.right.GetPoint(3.0f);
        }
        _rightRecords.Add(new EyeTrackingRecord(Timer, eyeRays.right.origin, eyeRays.right.direction, rightEye.transform.position));

        if (SaveTimer > 1.0f)
        {
            string leftPath = "Assets/" + patientID + "_left.csv";

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

            string rightPath = "Assets/" + patientID + "_right.csv";
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
