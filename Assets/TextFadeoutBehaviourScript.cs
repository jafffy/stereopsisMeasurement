using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextFadeoutBehaviourScript : MonoBehaviour {
  private float timer = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    if (gameObject.activeSelf)
    {
      timer += Time.deltaTime;

      if (timer > 10.0)
      {
        gameObject.SetActive(false);
      }
    }
	}
}
