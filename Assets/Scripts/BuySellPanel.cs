using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class BuySellPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    protected bool mouseOver = false;
    public GameObject childpanel;

    public Text number;
    public new Text name;

    public string Name;

    protected int Number;

    float moveTime = 0.1f;

    protected void Update()
    {
        if (mouseOver)
        {
            childpanel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(childpanel.GetComponent<RectTransform>().anchoredPosition, new Vector2(0, 75), UnityEngine.Time.deltaTime / moveTime);
        }
        else
        {
            childpanel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(childpanel.GetComponent<RectTransform>().anchoredPosition, new Vector2(0, 0), UnityEngine.Time.deltaTime / moveTime);
        }

        name.text = Name;
        number.text = ProPlotter.SIPrefix(Number,0);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
    }

    public abstract bool CanBuy(int number);

    public abstract bool CanSell(int number);

    public abstract void Buy(int number);

    public abstract void Sell(int number);
}
