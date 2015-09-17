using UnityEngine;
using System.Collections;
using System.Linq;

public class SortCustomers : MonoBehaviour
{

    void Update()
    {
        sort();
    }


    // fuck knows how this works
    void sort()
    {
        var children = GetComponentsInChildren<CustomerPanel>(true);
        var sorted = from child in children
                     orderby child.Id ascending
                     select child;
        for (int i = 0; i < sorted.Count(); i++)
        {
            sorted.ElementAt(i).transform.SetSiblingIndex(i);
        }
    }
}