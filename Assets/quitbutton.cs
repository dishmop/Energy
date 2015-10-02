using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class quitbutton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnPointerEnter(PointerEventData ped)
    {
        ToolTip.instance.ToolTipOn("Progress will not be saved", gameObject);
    }

    public void OnPointerExit(PointerEventData ped)
    {
        ToolTip.instance.ToolTipOff(gameObject);
    }
}
