﻿using UnityEngine;
using System.Collections;

public class SetupResolution : MonoBehaviour {

	int count = 0;
	static bool firstTime = true;
	
	
	void Awake () {
		if (firstTime){
			Screen.SetResolution (960, 640, false);
		}
		
	}
	
	// Update is called once per frame
	void Update () {

		
	}
}
