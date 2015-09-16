using System;

public static class Utilities {
    public static string MoneyToString(ulong money)
    {
        if (money > 1e12)
        {
            return "£" + ((float)money / 1e12f).ToString("##0.0t");
        }else if (money > 1e9)
        {
            return "£" + ((float)money / 1e9f).ToString("##0.0b");
        }
        else if (money > 1e6)
        {
            return "£" + ((float)money / 1e6f).ToString("##0.0m");
        }
        else if (money > 1e3)
        {
            return "£" + ((float)money / 1e3f).ToString("##0.0k");
        }
        else
        {
            return "£" + money.ToString();
        }
    }
}
