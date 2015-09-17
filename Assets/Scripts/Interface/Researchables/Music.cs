using UnityEngine;
using System.Collections;

public class Music : Researchable
{

    protected override void OnResarch()
    {
        Globals.instance.GetComponent<AudioSource>().clip = Globals.instance.music;
        Globals.instance.GetComponent<AudioSource>().Play();
    }
}
