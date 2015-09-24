using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Play();
        }
    }

    public void Play()
    {
        Application.LoadLevel(1); // load main game
    }
}
