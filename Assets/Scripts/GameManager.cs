﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public Rigidbody generatorHandle;
    public Light bulb;

    public Text supplyText;
    public Text demandText;
    public Text excessText;

    public Text lightButtonText;

    float supply = 0.0f; // in W
    float demand = 0.0f; // in W

    bool lighton = false;

    public PlotManager plotter;
    public PlotManager plotter2;

    void Start()
    {
        plotter.PlotCreate("Demand", Color.green);
        plotter.PlotCreate("Supply", Color.red);

        plotter2.PlotCreate("Surplus", Color.black);
    }

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

        plotter.PlotAdd("Supply", supply);
        plotter.PlotAdd("Demand", demand);

        plotter2.PlotAdd("Surplus", excess);
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
