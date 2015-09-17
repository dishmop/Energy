using UnityEngine;
using System.Collections;

public class TimeBoost : Researchable {

    protected override void OnResarch()
    {
        Time.instance.secondsPerDay /= 7f;
    }
}
