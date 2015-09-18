using UnityEngine;

public class Petrol : Generator
{
    public override float Output(float time, Season season)
    {
        return 0;
    }

    float timesinceclick = 0;
    
    public static int number = 0;


    float timebetweenclicks = 0.5f;

    public override void Update()
    {
        timesinceclick += UnityEngine.Time.deltaTime;

        if (on)
        {
            while (timesinceclick >= timebetweenclicks)
            {
                GameManager.instance.TurnHandle();
                timesinceclick -= timebetweenclicks;
                GameManager.instance.totalCarbon += 700.0f;
            }
        }
    }
}
