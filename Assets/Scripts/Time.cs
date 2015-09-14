using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum Day
{
    Monday,
    Tuesday,
    Wendesday,
    Thursday,
    Friday,
    Saturday,
    Sunday
}

public class Time : MonoBehaviour {

    public static Time instance;

    int days;
    float time;

    float secondsPerDay = 30;

    public Text timeText;

    public int Days
    {
        get { return days; }
    }

    public float DayFraction
    {
        get { return time; }
    }

    public int Hours
    {
        get { return (int)(DayFraction * 24f); }
    }

    public int Minutes
    {
        get { return (int)((DayFraction - Hours / 24f) * 24f * 60f); }
    }

	void Start () {
        instance = this;
	}
	
	void Update () {
        time += UnityEngine.Time.deltaTime/secondsPerDay;

        while (time > 1.0f)
        {
            time -= 1f;
            days++;
        }

        timeText.text = Hours.ToString("00")+":"+Minutes.ToString("00");
	}
}
