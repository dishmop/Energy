using UnityEngine;
using System.Collections;

public class GeneratorResearchable : Researchable
{
    public int generatorCost;
    public int saleCost;
    public string type;

    [TextArea(3,10)]
    public string generatortooltip;

    protected override void OnResarch()
    {
        ResearchPanel.instance.AddGenerator(name, generatorCost, saleCost, type, generatortooltip, sprite);
    }
}

