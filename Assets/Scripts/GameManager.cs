using UnityEngine;
using System.Collections;
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

    CustomerType customer0;

    float sampleInterval;

    float prevSample = 0;
    float nextSample = 0;

    float timeSinceSample = float.PositiveInfinity;

    void Start()
    {
        plotter.NewPlot("supply", Color.green);
        plotter.NewPlot("demand", Color.red);

        customer0 = new StandardEmployment();
    }

    void Update()
    {
        customer0.numCustomers = numCustomers;


        supply = 3*Mathf.Abs(generatorHandle.angularVelocity.x);

        if (lighton)
        {
            demand = 60;
        }
        else
        {
            demand = 0;
        }

        if (lighton)
        {
            float bulbPower = Mathf.Min(supply, 60);
            bulb.intensity = bulbPower / 10f;
        }
        else
        {
            bulb.intensity = 0;
        }

        timeSinceSample += Time.instance.DeltaTime;

        sampleInterval = 0.05f / (customer0.numCustomers>0?customer0.numCustomers:1);
        plotter.VerticalMax = 1000 * (customer0.numCustomers > 0 ? customer0.numCustomers : 1);
        plotter.VerticalGridStep = plotter.VerticalMax / 5;

        if (timeSinceSample > sampleInterval)
        {
            prevSample = nextSample;
            nextSample = Sample.Normal(customer0.TotalMean(Day.Monday, Mathf.Repeat(Time.instance.DayFraction + sampleInterval, 1f), 0), Mathf.Sqrt(customer0.TotalVariance(Day.Monday, Mathf.Repeat(Time.instance.DayFraction + sampleInterval, 1f), 0)), true);
            timeSinceSample = 0;
        }

        demand = Mathf.Lerp(prevSample, nextSample, timeSinceSample / sampleInterval);

        float excess = supply - demand;

        supplyText.text = "Supply: " + supply + "W";
        demandText.text = "Demand: " + demand + "W";

        excessText.text = "Surplus: " + excess + "W";

        plotter.AddPoint("supply", 24f * Time.instance.DayFraction, supply);
        plotter.AddPoint("demand", 24f * Time.instance.DayFraction, demand);
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
}
