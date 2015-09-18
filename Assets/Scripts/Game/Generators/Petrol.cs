using UnityEngine;

public class Petrol : Generator
{
    public override float Output(float time, Season season)
    {
        return 0;
    }

    static float timesinceclick = 0;
    static int num = 0;

    public Petrol()
    {
        num++;
    }

    float timebetweenclicks = 0.5f;

    public override void Update()
    {
        timesinceclick += UnityEngine.Time.deltaTime;
        //GameManager.instance.generatorHandle.AddTorque(new Vector3(1f, 0, 0), ForceMode.Force);

        while (timesinceclick >= timebetweenclicks/num)
        {
            GameManager.instance.TurnHandle();
            timesinceclick -= timebetweenclicks/num;
        }
    }
}
