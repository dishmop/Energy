using UnityEngine;

public class Diesel : Generator
{
    public override float Output(float time, Season season)
    {
        return 0;
    }

    public override void Update()
    {
        GameManager.instance.generatorHandle.AddTorque(new Vector3(1f, 0, 0), ForceMode.Force);
    }
}
