using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Interface : MonoBehaviour {

    public Text supplyText;
    public Text demandText;
    public Text excessText;

    public Text timeText;
    public Text powerCutText;

    public ProPlotter plotter;
    public ProPlotter surplus;

    public Text money;


    // set up the parts of teh game's interface that are more easily done centrally
	void Start () {
        plotter.NewPlot("supply", Color.green);
        plotter.NewPlot("demand", Color.red);

        //surplus.NewPlot("surplus", Color.blue);
	}

    bool weekview = false;
	void Update () {
        plotter.VerticalGridStep = plotter.VerticalMax / 5;
        //surplus.VerticalGridStep = surplus.VerticalRange / 5;

        float displaytime;

        // when a game day is less than 5 seconds, switch to week view
        if (Time.instance.secondsPerDay <= 5f)
        {
            displaytime = (int)Time.instance.WeekDay + Time.instance.DayFraction;

            if (!weekview) // do the first time we hit 3seconds
            {
                plotter.HorizontalAxisLabelSuffix = "";

                plotter.HorizontalGridStep = 1;

                plotter.HorizontalLabels = System.Enum.GetNames(typeof(Day));

                plotter.HorizontalRange = 7;
                plotter.ClearAll();

                //surplus.HorizontalGridStep = 1;
                //surplus.HorizontalRange = 7;
                //surplus.ClearAll();

                weekview = true;
            }
        }
        else
        {
            displaytime = 24f * Time.instance.DayFraction;
        }


        supplyText.text = "Supply: " + ProPlotter.SIPrefix(GameManager.instance.supply) + "W";
        demandText.text = "Demand: " + ProPlotter.SIPrefix(GameManager.instance.demand) + "W";

        excessText.text = "Surplus: " + ProPlotter.SIPrefix(GameManager.instance.surplus) + "W";

        if (GameManager.instance.surplus > 0)
        {
            excessText.color = Color.green;

            GetComponent<AudioSource>().pitch = Mathf.Lerp(GetComponent<AudioSource>().pitch, 1, 10f*UnityEngine.Time.deltaTime);
        } else
        {
            excessText.color = Color.red;

            GetComponent<AudioSource>().pitch = Mathf.Lerp(GetComponent<AudioSource>().pitch, 0, 10f * UnityEngine.Time.deltaTime);
            
        }

        plotter.AddPoint("supply", displaytime, GameManager.instance.supply);
        plotter.AddPoint("demand", displaytime, GameManager.instance.demand);

        //surplus.AddPoint("surplus", displaytime, GameManager.instance.surplus);

        money.text = "£" + GameManager.instance.money.ToString("#,0");

        timeText.text = Time.instance.Hours.ToString("00") + ":" + Time.instance.Minutes.ToString("00") + " " + Time.instance.WeekDay + " " + Time.instance.DayInMonth + " " + Time.MonthName[Time.instance.Month - 1] + " Year #" + Time.instance.Years;


        string powercuttime = Time.instance.MinutesSincePowerCut+" minutes.";

        if (Time.instance.HoursSincePowerCut == 1)
        {
            powercuttime = "1 hour and " + powercuttime;
        }else 
        if (Time.instance.HoursSincePowerCut>1)
        {
            powercuttime = Time.instance.HoursSincePowerCut + " hours and " + powercuttime;
        }

        if(Time.instance.DaysSincePowerCut>0)
        {
            powercuttime = Time.instance.DaysSincePowerCut + " days, " + powercuttime;
        }

        powerCutText.text = "Time since power cut: " + powercuttime;
    }
}
