public abstract class CustomerType {
    public int numCustomers;

    protected abstract float mean(Day day, float time, float winter);
    protected abstract float standardDeviation(Day day, float time, float winter);

    public float TotalMean(Day day, float time, float winter)
    {
        return numCustomers*mean(day, time, winter);
    }

    public float TotalVariance(Day day, float time, float winter)
    {
        float sd = standardDeviation(day, time, winter);
        return numCustomers*sd*sd;
    }
}
