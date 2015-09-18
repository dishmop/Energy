using UnityEngine;
using System.Collections;

public class Globals : MonoBehaviour {

    public static Globals instance;

    public AudioClip clickhigh;
    public AudioClip clicklow;

    public AudioClip swipe;

    public AudioClip coin;

    public AudioClip hum;

    public AudioClip music;

    public AudioSource musicPlayer;
    public AudioSource clickPlayer;


    public GameObject genPrefab;
    public GameObject custPrefab;
    
    void Start () {
        instance = this;
	}
}
