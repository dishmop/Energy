public class nuclear : Generator {
    public static int number = 0;

    public override float Output(float time, Season season)
    {
        if (on)
        {
            return 100000000;
        }
        else
        {
            return 0;
        }
    }
}
