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

    public FreePlotter plotter;
    public FreePlotter plotter2;

    CustomerType customer0;

    void Start()
    {
        plotter.NewPlot("supply", Color.green);
        plotter.NewPlot("demand", Color.red);

        plotter2.NewPlot("surplus", Color.blue);

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

        float excess = supply - demand;

        supplyText.text = "Supply: " + supply + "W";
        demandText.text = "Demand: " + demand + "W";

        excessText.text = "Surplus: " + excess + "W";

        //plotter.AddPoint("supply", Time.instance.Days, supply);
        plotter.AddPoint("demand", 24f*(Time.instance.Days + Time.instance.DayFraction), Sample.Normal(customer0.TotalMean(Day.Monday, Time.instance.DayFraction,0), Mathf.Sqrt(customer0.TotalVariance(Day.Monday, Time.instance.DayFraction,0)),true));

        //plotter2.AddPoint("surplus", Time.instance.Days, excess);

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
