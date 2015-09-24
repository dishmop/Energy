using UnityEngine;
using System.Collections;

public class ExampleScene : MonoBehaviour {

	public ProPlotter plotter;

	// Use this for initialization
	void Start () {
		plotter.NewPlot("Sine", Color.red);
	}

	int frames = 0;

	// Update is called once per frame
	void Update () {
		frames++;
		if (frames == 1)
		{
			frames = 0;
			//plotter.AddPoint("Sine", Time.time, Mathf.Sin(Time.time));
		}
	}
}
