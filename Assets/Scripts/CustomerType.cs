public class CustomerType {
    int[] numCustomers = new int[8];
    EnergyProfile[,] profile = new EnergyProfile[8, 15];

    public int TotalCustomers
    {
        get
        {
            int total = 0;
            for (int i = 0; i < numCustomers.Length; i++)
            {
                total += numCustomers[i];
            }

            return total;
        }
    }

    public CustomerType()
    {
        for (int i = 0; i < 8; i++)
        {
            numCustomers[i] = 0;

            foreach(int val in System.Enum.GetValues(typeof(Season))) {
                profile[i, val * 3] = new EnergyProfile(i + 1, val * 3 + 1);
                profile[i, val * 3+1] = new EnergyProfile(i + 1, val * 3 + 2);
                profile[i, val * 3+2] = new EnergyProfile(i + 1, val * 3 + 3);
            }
        }
    }

    float mean(Day day, Season season, float time, float winter, int prof )
    {
        int seasonNum = (int)season;

        switch (day)
        {
            case Day.Saturday:
                return profile[prof, 3 * seasonNum + 1].interpolate(time);

            case Day.Sunday:
                return profile[prof, 3 * seasonNum + 2].interpolate(time);

            default:
                return profile[prof, 3 * seasonNum].interpolate(time);
        }
    }

    float standardDeviation(Day day, Season season, float time, float winter, int profile )
    {
        return 30f;
    }

    public float TotalMean(Day day, Season season, float time, float winter)
    {
        float meantotal = 0;
        for (int i = 0; i < numCustomers.Length; i++ ) {
            meantotal += numCustomers[i] * mean(day, season, time, winter, i);
        }

        return meantotal;
    }

    public float TotalVariance(Day day, Season season, float time, float winter)
    {
        float meantotal = 0;
        for (int i = 0; i < numCustomers.Length; i++)
        {
            float sd = standardDeviation(day, season, time, winter, i);
            meantotal += numCustomers[i] * sd * sd;
        }

        return meantotal;
    }

    public void AddCustomer(int profileNum)
    {
        numCustomers[profileNum]++;
    }
}
