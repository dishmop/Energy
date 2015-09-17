using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuySellButton : MonoBehaviour, IPointerClickHandler {
    public BuySellPanel parentPanel;

    Button button;

    public int multiplier;

    void Start()
    {
        button = GetComponent<Button>();
    }

	void Update () {
	    if(multiplier>0)
        {
            if (parentPanel.CanBuy(multiplier))
            {
                button.interactable = true;
            }
            else
            {
                button.interactable = false;
            }
        }
        else
        {
            if (parentPanel.CanSell(-multiplier))
            {
                button.interactable = true;
            }
            else
            {
                button.interactable = false;
            }
        }
	}

    public void OnPointerClick(PointerEventData ped){
        if (multiplier > 0)
        {
            parentPanel.Buy(multiplier);
        }
        else
        {
            parentPanel.Sell(-multiplier);
        }
    }
}
