using UnityEngine;
using System.Collections;

public class Music : Researchable
{

    protected override void OnResarch()
    {
        Globals.instance.musicPlayer.clip = Globals.instance.music;
        Globals.instance.musicPlayer.Play();
    }
}
