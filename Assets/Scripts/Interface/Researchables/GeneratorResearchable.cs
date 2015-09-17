using UnityEngine;
using System.Collections;

public class GeneratorResearchable : Researchable
{
    public int generatorCost;
    public string type;

    [TextArea(3,10)]
    public string generatortooltip;

    public MonoBehaviour thisisatest;

    protected override void OnResarch()
    {
        ResearchPanel.instance.AddGenerator(name, generatorCost, type, generatortooltip, sprite);
    }
}

