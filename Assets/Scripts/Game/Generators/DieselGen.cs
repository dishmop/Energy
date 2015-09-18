using UnityEngine;

public class DieselGen : Generator
{
    public static int number = 0;

    public override float Output(float time, Season season)
    {
        if(on){
            return 1000;
        }
        else
        {
            return 0;
        }
    }

    public override void Update()
    {
        GameManager.instance.totalCarbon += UnityEngine.Time.deltaTime * 40000.0f / Time.instance.secondsPerDay;
    }
}
