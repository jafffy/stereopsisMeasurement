using System.Collections;
using System.Collections.Generic;

using Leap;
using Leap.Unity;
using UnityEngine;

public class HandSerializer : MonoBehaviour {

	CapsuleHand target;

	void Start()
	{
		target = gameObject.GetComponent<CapsuleHand>();
		if(target.Handedness == Chirality.Left)  {
			ReplayManager.instance.left_hand = this;
		}
		else  {
			ReplayManager.instance.right_hand = this; 
		}
	}

	public List<Vector3>[] GetHandPositions()
	{
		int arrayLength = target.GetLeapHand().Fingers.Count + 1;
		List<Vector3>[] indexedArray = new List<Vector3>[arrayLength];
		int i = 0;
		//Fingers
		foreach (var finger in target.GetLeapHand().Fingers) {
			List<Vector3> positions = new List<Vector3>();
			for (int j = 0; j < 4; j++) {
				int key = getFingerJointIndex((int)finger.Type, j);
				Vector3 position = finger.Bone((Bone.BoneType)j).NextJoint.ToVector3();
				positions.Add(position);
			}
			indexedArray[i++] = positions;
        }
		//Palm
		List<Vector3> palmPositions = new List<Vector3>();
		Vector3 palmPosition = target.GetLeapHand().PalmPosition.ToVector3();
      	palmPositions.Add(palmPosition);
		indexedArray[i] = palmPositions;
		return indexedArray;
	}

	private int getFingerJointIndex(int fingerIndex, int jointIndex) {
      return fingerIndex * 4 + jointIndex;
    }
}
