using UnityEngine;

public class battery : Generator
{
    public static int number = 0;

    float charge = 0.0f; // in kWh
    float maxcharge = 10.0f; // in kWh

    float maxoutput = 2000; // in W
    float maxinput = 2000;

    float currentoutput = 0.0f; // in W

    float buffer = 200.0f;//desired surplus

    public override float Output(float time, Season season)
    {
        if (on)
        {
            float targetoutput; // in W
            float diff = (GameManager.instance.surplus - buffer * panel.activeSlider.value) / panel.activeSlider.value;

            if (diff < 0.0f)
            {
                targetoutput = Mathf.Min(-diff, maxoutput);

                targetoutput = Mathf.Min((1000.0f * charge) / (Time.instance.DeltaTime * 24.0f), targetoutput);
            }
            else
            {
                targetoutput = Mathf.Min(maxinput, diff);

                targetoutput = -Mathf.Min(targetoutput, (1000.0f * (maxcharge - charge)) / (Time.instance.DeltaTime * 24.0f));
            }


            currentoutput = Mathf.Lerp(currentoutput, targetoutput, 35.0f*Time.instance.DeltaTime);


            charge -= 24.0f * Time.instance.DeltaTime * currentoutput / 1000.0f;

            if (currentoutput > 0.0f)
            {
                return currentoutput;
            }
            else
            {
                GameManager.instance.demand -= currentoutput;
                return 0.0f;
            }
        }
        else
        {
            return 0;
        }
    }
}
