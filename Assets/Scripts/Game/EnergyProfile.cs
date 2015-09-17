using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CsvFile;

public struct EnergyProfile {
    public float[] data;

    public float interpolate(float time)
    {
        if (time < 0f || time > 1f)
        {
            throw new System.ArgumentOutOfRangeException("time must be between 0 and 1");
        }

        if (time < 1f / 48f)
        {
            return Mathf.Lerp(data[47], data[0], time * 48f);
        }
        else
        {
            return Mathf.Lerp(data[Mathf.FloorToInt(time * 48f)-1], data[Mathf.CeilToInt(time * 48f)-1], time*48f - Mathf.FloorToInt(time*48f));
        }
    }

    public EnergyProfile(int profileClass, int column)
    {
        data = new float[48];

        List<string> columns = new List<string>();

        using (var reader = new CsvFileReader("Assets\\profiles.csv"))
        {
            int profileClassNum = 0;
            int rowNum = -1; // first line of each profile headings, so ignore that

            while (reader.ReadRow(columns))
            {
                if (rowNum >= 0) // read exactly 48 rows
                {
                    if (rowNum >= 48)
                    {
                        break;
                    }

                    data[rowNum] = float.Parse(columns[column])*1000f; // kW to W
                }

                if (profileClassNum == profileClass)
                {
                    rowNum++;
                }
                
                if (columns.Count == 1)
                {
                    profileClassNum++;
                }
            }
        }
    }
}
