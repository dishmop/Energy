using UnityEngine;

public class Sample {

    static float nextNormal = float.NaN;

    // polar Box-Muller
    public static float StandardNormal()
    {
        if (!float.IsNaN(nextNormal))
        {
            float temp = nextNormal;
            nextNormal = float.NaN;
            return temp;
        }

        float x1, x2, w;

        do
        {
            x1 = 2f * Random.value - 1f;
            x2 = 2f * Random.value - 1f;
            w = x1 * x1 + x2 * x2;
        } while (w >= 1f);

        w = Mathf.Sqrt(-2f * Mathf.Log(w) / w);

        nextNormal = x2 * w;

        return x1 * w;
    }

    public static float Normal(float mean, float standardDeviation, bool forcePositive)
    {
        float sample = standardDeviation * StandardNormal() + mean;
        if (forcePositive && sample < 0f)
        {
            sample = 0f;
        }
        return sample;
    }
}
