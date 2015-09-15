using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public Rigidbody generatorHandle;
    public Light bulb;

    public Text supplyText;
    public Text demandText;
    public Text excessText;

    public Text lightButtonText;

    public int numCustomers;

    float supply = 0.0f; // in W
    float demand = 0.0f; // in W

    bool lighton = false;

    public ProPlotter plotter;
    public ProPlotter surplus;

    CustomerType customerDemand;
    List<Generator> generators = new List<Generator>();

    float sampleInterval;

    float prevSample = 0;
    float nextSample = 0;

    float timeSinceSample = float.PositiveInfinity;

    void Start()
    {
        plotter.NewPlot("supply", Color.green);
        plotter.NewPlot("demand", Color.red);

        surplus.NewPlot("surplus", Color.blue);

        customerDemand = new CustomerType();
    }

    bool shortened = false;

    void Update()
    {
        supply = 3*Mathf.Abs(generatorHandle.angularVelocity.x);

        if (lighton)
        {
            demand = 60;
        }
        else
        {
            demand = 0;
        }

        timeSinceSample += Time.instance.DeltaTime;

        sampleInterval = 0.05f / (customerDemand.TotalCustomers > 0 ? customerDemand.TotalCustomers : 1);
        //plotter.VerticalMax = 10000 * (customer0.TotalCustomers > 0 ? customer0.TotalCustomers : 1);
        plotter.VerticalGridStep = plotter.VerticalMax / 5;

        surplus.VerticalGridStep = surplus.VerticalRange / 5;

        if (timeSinceSample > sampleInterval)
        {
            prevSample = nextSample;

            float mean = customerDemand.TotalMean(Time.instance.WeekDay, Time.instance.CurrentSeason, Mathf.Repeat(Time.instance.DayFraction + sampleInterval, 1f), 0);
            float variance = customerDemand.TotalVariance(Time.instance.WeekDay, Time.instance.CurrentSeason, Mathf.Repeat(Time.instance.DayFraction + sampleInterval, 1f), 0);

            nextSample = Sample.Normal(mean, Mathf.Sqrt(variance), true);
            timeSinceSample = 0;
        }

        demand = Mathf.Lerp(prevSample, nextSample, timeSinceSample / sampleInterval);

        supply = 0;// gen.Output(Time.instance.DayFraction, Time.instance.CurrentSeason);

        foreach (var generator in generators)
        {
            supply += generator.Output(Time.instance.DayFraction, Time.instance.CurrentSeason);
        }

        float excess = supply - demand;

        float displaytime;

        if (Time.instance.secondsPerDay <= 3f)
        {
            displaytime = (int)Time.instance.WeekDay + Time.instance.DayFraction;

            if (!shortened) // do the first time we hit 3seconds
            {
                plotter.HorizontalAxisLabelSuffix = "";

                plotter.HorizontalGridStep = 1;

                plotter.HorizontalLabels = System.Enum.GetNames(typeof(Day));

                plotter.HorizontalRange = 7;
                plotter.ClearAll();

                surplus.HorizontalGridStep = 1;
                surplus.HorizontalRange = 7;
                surplus.ClearAll();

                shortened = true;
            }
        }
        else
        {
            displaytime = 24f * Time.instance.DayFraction;
        }

        if (excess>0)
        {
            float bulbPower = Mathf.Min(supply, 60);
            bulb.intensity = bulbPower;
        }
        else
        {
            bulb.intensity = 0;
        }

        supplyText.text = "Supply: " + supply + "W";
        demandText.text = "Demand: " + demand + "W";

        excessText.text = "Surplus: " + excess + "W";

        plotter.AddPoint("supply", displaytime, supply);
        plotter.AddPoint("demand", displaytime, demand);

        surplus.AddPoint("surplus", displaytime, excess);
    }

    public void TurnHandle()
    {
        generatorHandle.maxAngularVelocity = 50;
        generatorHandle.AddTorque(new Vector3(1, 0, 0), ForceMode.Impulse);
    }

    public void LightOn()
    {
        lighton = !lighton;

        if (lighton)
        {
            lightButtonText.text = "Light off";
        }
        else
        {
            lightButtonText.text = "Light on";
        }
    }

    public void AddRandom()
    {
        customerDemand.AddCustomer(Random.Range(0, 7));
    }

    public void Add100()
    {
        for (int i = 0; i < 80; i++)
        {
            customerDemand.AddCustomer(0);
        }
        for (int i = 80; i < 98; i++)
        {
            customerDemand.AddCustomer(1);
        }

        customerDemand.AddCustomer(2);
        customerDemand.AddCustomer(5);
    }

    public void AddSolar()
    {
        for (int i = 0; i < 10; i++ )
            generators.Add(new Solar());
    }

    public void AddNuclear()
    {
        generators.Add(new nuclear());
    }
}
