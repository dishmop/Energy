using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Interface : MonoBehaviour {

    public Text supplyText;
    public Text demandText;
    public Text excessText;

    public ProPlotter plotter;
    public ProPlotter surplus;

    public Text money;


    // set up the parts of teh game's interface that are more easily done centrally
	void Start () {
        plotter.NewPlot("supply", Color.green);
        plotter.NewPlot("demand", Color.red);

        surplus.NewPlot("surplus", Color.blue);
	}

    bool weekview = false;
	void Update () {
        plotter.VerticalGridStep = plotter.VerticalMax / 5;
        surplus.VerticalGridStep = surplus.VerticalRange / 5;

        float displaytime;

        // when a game day is less than 3 seconds, switch to week view
        if (Time.instance.secondsPerDay <= 3f)
        {
            displaytime = (int)Time.instance.WeekDay + Time.instance.DayFraction;

            if (!weekview) // do the first time we hit 3seconds
            {
                plotter.HorizontalAxisLabelSuffix = "";

                plotter.HorizontalGridStep = 1;

                plotter.HorizontalLabels = System.Enum.GetNames(typeof(Day));

                plotter.HorizontalRange = 7;
                plotter.ClearAll();

                surplus.HorizontalGridStep = 1;
                surplus.HorizontalRange = 7;
                surplus.ClearAll();

                weekview = true;
            }
        }
        else
        {
            displaytime = 24f * Time.instance.DayFraction;
        }


        supplyText.text = "Supply: " + GameManager.instance.supply + "W";
        demandText.text = "Demand: " + GameManager.instance.demand + "W";

        excessText.text = "Surplus: " + GameManager.instance.surplus + "W";

        plotter.AddPoint("supply", displaytime, GameManager.instance.supply);
        plotter.AddPoint("demand", displaytime, GameManager.instance.demand);

        surplus.AddPoint("surplus", displaytime, GameManager.instance.surplus);

        money.text = "£" + GameManager.instance.money.ToString("#,0");
	}
}
