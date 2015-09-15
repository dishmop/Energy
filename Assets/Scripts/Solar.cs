public class Solar : Generator {
    public float size = 1f; //in m^2

    public override float Output(float time, Season season)
    {
        float seasonfactor = 0f;
        switch(season)
        {
            case Season.Autumn:
                seasonfactor = 750f;
                break;
            case Season.HighSummer:
                seasonfactor = 1000f;
                break;
            case Season.Summer:
                seasonfactor = 1000f;
                break;
            case Season.Winter:
                seasonfactor = 600f;
                break;
            case Season.Spring:
                seasonfactor = 900f;
                break;
        }

        float output = seasonfactor - 6000f*(time - 0.58f) * (time - 0.58f);

        output = output > 0 ? output : 0;

        return output*size;
    }
}
