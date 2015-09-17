using UnityEngine;

public class DieselGen : Generator
{
    public override float Output(float time, Season season)
    {
        return 1000;
    }
}
