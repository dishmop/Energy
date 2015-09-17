using UnityEngine;
using System.Collections;
using System.Linq;

public class SortResearchables : MonoBehaviour {

    void Update()
    {
        sort();
    }


    // fuck knows how this works
    void sort()
    {
        var children = GetComponentsInChildren<Researchable>(true);
        var sorted = from child in children
                     orderby child.cost ascending
                     select child;
        for (int i = 0; i < sorted.Count(); i++)
        {
            sorted.ElementAt(i).transform.SetSiblingIndex(i);
        }
    }
}
