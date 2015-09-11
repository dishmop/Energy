using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Time : MonoBehaviour {

    float time; // in days

    float secondsPerDay = 30;

    public Text timeText;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        time += UnityEngine.Time.deltaTime/secondsPerDay;

        float todaytime = time - (int)time;
        int hours = (int)(todaytime * 24f);
        int minutes = (int)((todaytime - hours / 24f) * 24f * 60f);


        timeText.text = hours.ToString("00")+":"+minutes.ToString("00");
	}
}
