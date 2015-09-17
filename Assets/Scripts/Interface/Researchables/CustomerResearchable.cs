using UnityEngine;
using System.Collections;

public class CustomerResearchable : Researchable
{
    public int id;

    [TextArea(3, 10)]
    public string generatortooltip;


    protected override void OnResarch()
    {
        ResearchPanel.instance.AddCustomer(name, id, generatortooltip, sprite);
    }
}

