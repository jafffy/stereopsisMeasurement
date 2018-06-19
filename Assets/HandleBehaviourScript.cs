﻿using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleBehaviourScript : MonoBehaviour {
  protected InteractionBehaviour _intObj;

  // Use this for initialization
  void Start () {
    _intObj = GetComponent<InteractionBehaviour>();
    _intObj.OnGraspBegin += onGraspBegin;
    _intObj.OnGraspEnd += onGraspEnd;

    PhysicsCallbacks.OnPostPhysics += onPostPhysics;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

  private void onPostPhysics()
  {
    _intObj.rigidbody.position = this.transform.position;
    _intObj.rigidbody.rotation = this.transform.rotation;
  }


  private void onGraspBegin()
  {
  }

  private void onGraspEnd()
  {
  }
}
