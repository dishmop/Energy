using UnityEngine;
using System.Collections;
using System.Linq;

public class SortGenerators : MonoBehaviour
{

    void Update()
    {
        sort();
    }


    // fuck knows how this works
    void sort()
    {
        var children = GetComponentsInChildren<GeneratorPanel>(true);
        var sorted = from child in children
                     orderby child.Cost ascending
                     select child;
        for (int i = 0; i < sorted.Count(); i++)
        {
            sorted.ElementAt(i).transform.SetSiblingIndex(i);
        }
    }
}