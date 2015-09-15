using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SolicitPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    bool mouseOver = false;

    public GameObject BuySellPanel;
    public Text number;
    public new Text name;

    public int Id;
    public string Name;

    float moveTime = 0.1f;

    void Update()
    {
        if (mouseOver)
        {
            BuySellPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(BuySellPanel.GetComponent<RectTransform>().anchoredPosition, new Vector2(0, 75), UnityEngine.Time.deltaTime / moveTime);
        }
        else
        {
            BuySellPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(BuySellPanel.GetComponent<RectTransform>().anchoredPosition, new Vector2(0, 0), UnityEngine.Time.deltaTime / moveTime);
        }

        name.text = Name;
        number.text = CustomerManager.instance.numCustomers[Id].ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
    }

    public void Buy(int number)
    {
        CustomerManager.instance.numCustomers[Id] += number;
    }

    public void Sell(int number)
    {
        if(CustomerManager.instance.numCustomers[Id] >= number)
        {
            CustomerManager.instance.numCustomers[Id] -= number;
        }
    }
}
