using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    public Rigidbody generatorHandle;

    public Text lightButtonText;

    public float supply = 0.0f; // in W
    public float demand = 0.0f; // in W
    public float surplus;

    public ulong money = 0; // in £ 

    CustomerManager customerDemand;
    public List<Generator> generators = new List<Generator>();

    float sampleInterval;

    float prevSample = 0;
    float nextSample = 0;

    float timeSinceSample = float.PositiveInfinity;

    void Start()
    {
        instance = this;
        customerDemand = new CustomerManager();
    }

    void Update()
    {
        timeSinceSample += Time.instance.DeltaTime;

        sampleInterval = 0.05f / (customerDemand.TotalCustomers > 0 ? customerDemand.TotalCustomers : 1);

        if (timeSinceSample > sampleInterval)
        {
            prevSample = nextSample;

            float mean = customerDemand.TotalMean(Time.instance.WeekDay, Time.instance.CurrentSeason, Mathf.Repeat(Time.instance.DayFraction + sampleInterval, 1f), 0);
            float variance = customerDemand.TotalVariance(Time.instance.WeekDay, Time.instance.CurrentSeason, Mathf.Repeat(Time.instance.DayFraction + sampleInterval, 1f), 0);

            nextSample = Sample.Normal(mean, Mathf.Sqrt(variance), true);
            timeSinceSample = 0;
        }

        demand = Mathf.Lerp(prevSample, nextSample, timeSinceSample / sampleInterval);

        supply = 0;

        foreach (var generator in generators)
        {
            supply += generator.Output(Time.instance.DayFraction, Time.instance.CurrentSeason);
        }

        surplus = supply - demand;
    }

    public void TurnHandle()
    {
        generatorHandle.maxAngularVelocity = 50;
        generatorHandle.AddTorque(new Vector3(1, 0, 0), ForceMode.Impulse);
    }
}
