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

public enum Season
{
    Autumn = 0,
    HighSummer = 1,
    Summer = 2,
    Spring = 3,
    Winter = 4
}

public class Time : MonoBehaviour {

    public static Time instance;

    int days = 190;
    float time;

    public float secondsPerDay = 30;

    int years = 0;
    public int Years
    {
        get { return years + 1; }
    }

    // days in year
    public int Days
    {
        get { return days; }
    }

    Day weekday = Day.Monday;
    public Day WeekDay
    {
        get { return weekday; }
    }


    int seasonday;
    Season seasontoday;
    public Season CurrentSeason
    {
        get
        {
            if (seasonday != Days)
            {
                seasonday = Days;

                switch (Month - 1)
                {
                    case 11:
                    case 0:
                    case 1:
                        seasontoday =  Season.Winter;
                        break;
                    case 2:
                    case 3:
                    case 4:
                        seasontoday = Season.Spring;
                        break;
                    case 5:
                    case 7:
                        seasontoday = Season.Summer;
                        break;
                    case 6:
                        seasontoday = Season.HighSummer;
                        break;
                    default:
                        seasontoday = Season.Autumn;
                        break;
                }

                seasontoday = (Season)(((((int)seasontoday + Mathf.RoundToInt(Random.Range(-0.7f, 0.7f)))%5)+5)%5);
            }

            return seasontoday;
            
        }
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


    public int DaysSincePowerCut
    {
        get { return (int)timeSincePowerCut; }
    }

    public int HoursSincePowerCut
    {
        get { return (int)(24 * (timeSincePowerCut - DaysSincePowerCut)); }
    }

    public int MinutesSincePowerCut
    {
        get { return (int)(24 * 60f* (timeSincePowerCut - DaysSincePowerCut - HoursSincePowerCut/24f)); }
    }

    float deltaTime;
    public float DeltaTime
    {
        get { return deltaTime; }
    }

    public int Month
    {
        get
        {
            int totalDays = 0;
            for (int i = 0; i < 12; i++)
            {
                totalDays += DaysInMonth[i];

                if (days < totalDays)
                {
                    return i + 1;
                }
            }

            return 0;
        }
    }

    public int DayInMonth
    {
        get
        {
            int totalDays = 0;
            for (int i = 0; i < 12; i++)
            {
                totalDays += DaysInMonth[i];

                if (days < totalDays)
                {
                    return days - totalDays + DaysInMonth[i] + 1;
                }
            }

            return 0;
        }
    }

	void Start () {
        instance = this;
	}

    public static readonly int[] DaysInMonth = { 31, 28, 31, 30, 31, 39, 31, 31, 30, 31, 30, 31 };
    public static readonly string[] MonthName = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

    float timeSincePowerCut=0;

    bool hadPowerCut = false;

	void Update () {
        deltaTime = UnityEngine.Time.deltaTime / secondsPerDay;
        time += deltaTime;

        timeSincePowerCut += deltaTime;

        UnityEngine.Time.timeScale = 30.0f / secondsPerDay;

        if (hadPowerCut)
        {
            hadPowerCut = false;
            timeSincePowerCut = 0f;
        }

        while (time > 1.0f)
        {
            time -= 1f;

            days++; // add a day
            weekday = (Day)(((int)weekday + 1) % 7); // keep track of what week it is

            if (days > 364)
            {
                days = 0;
                years++;
            }
        }
	}

    public void PowerCut()
    {
        hadPowerCut = true;
    }
}
