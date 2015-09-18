using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class BuySellPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    protected bool mouseOver = false;
    public GameObject childpanel;

    public Text number;
    public Text Name;

    [TextArea(3, 10)]
    public string Description;

    public int Number;

    public Sprite sprite;

    public Image image;

    protected float moveTime = 0.1f;

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

        Name.text = name;
        number.text = ProPlotter.SIPrefix(Number,0);
        image.sprite = sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
        ToolTip.instance.ToolTipOn(Description, gameObject);
        childpanel.GetComponent<AudioSource>().Play();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
        ToolTip.instance.ToolTipOff(gameObject);
    }

    public abstract bool CanBuy(int number);

    public abstract bool CanSell(int number);

    public abstract void Buy(int number);

    public abstract void Sell(int number);
}
