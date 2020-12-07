using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualDebuggerTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        NipaVisualDebugger.AddBallArrow_id("s", Vector3.zero, Vector3.forward, 5f, 0.5f, Color.blue);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
