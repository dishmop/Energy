using UnityEngine;
using System.Collections;

public class TimeBoost : Researchable {

    public float spd;

    protected override void OnResarch()
    {
        Time.instance.secondsPerDay = spd;
    }
}
