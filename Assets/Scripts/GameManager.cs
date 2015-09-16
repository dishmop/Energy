﻿using UnityEngine;
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

    enum PayLevel
    {
        none,
        threeh,
        sixh,
        twelveh,
        day
    }

    int dayssincepc = 0;

    PayLevel pay = PayLevel.none;

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
            generator.Update();
            supply += generator.Output(Time.instance.DayFraction, Time.instance.CurrentSeason);
        }

        supply += 40*generatorHandle.angularVelocity.magnitude;

        surplus = supply - demand;

        switch (pay)
        {
            case PayLevel.none:
                if (Time.instance.HoursSincePowerCut >= 3)
                {
                    pay = PayLevel.threeh;
                    Pay(1);
                }
                break;
            case PayLevel.threeh:
                if (Time.instance.HoursSincePowerCut >= 6)
                {
                    pay = PayLevel.sixh;
                    Pay(3);
                }
                break;
            case PayLevel.sixh:
                if (Time.instance.HoursSincePowerCut >= 12)
                {
                    pay = PayLevel.twelveh;
                    Pay(7);
                }
                break;
            case PayLevel.twelveh:
                if (Time.instance.DaysSincePowerCut >= 1)
                {
                    pay = PayLevel.day;
                    Pay(15);
                }
                break;
            case PayLevel.day:
                while (Time.instance.DaysSincePowerCut > dayssincepc)
                {
                    dayssincepc++;
                    Pay(25+dayssincepc);
                }
                break;
        }

        dayssincepc = Time.instance.DaysSincePowerCut;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TurnHandle();
        }

        if(surplus<0)
        {
            Time.instance.PowerCut();
            pay = PayLevel.none;
        }
    }

    public void TurnHandle()
    {
        generatorHandle.maxAngularVelocity = 50;
        generatorHandle.AddTorque(new Vector3(1, 0, 0), ForceMode.Impulse);
    }

    void Pay(int multiplier)
    {
        for (int i = 0; i < 8; i++)
        {
            money += (ulong)(multiplier*customerDemand.numCustomers[i]*CustomerManager.basepay[i]);
        }
    }
}
