public class StandardEmployment : CustomerType {
    EnergyProfile prof = new EnergyProfile(1, 1);

    protected override float mean(Day day, float time, float winter)
    {
        return prof.interpolate(time);
    }

    protected override float standardDeviation(Day day, float time, float winter)
    {
        return 10f;
    }
}
