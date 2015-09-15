using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuyPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    bool mouseOver = false;

    public GameObject BuySellPanel;

    float moveTime = 0.1f;

    void Update()
    {
        if (mouseOver)
        {
            BuySellPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(BuySellPanel.GetComponent<RectTransform>().anchoredPosition, new Vector2(0, 75), UnityEngine.Time.deltaTime/moveTime);
        }
        else
        {
            BuySellPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(BuySellPanel.GetComponent<RectTransform>().anchoredPosition, new Vector2(0, 0), UnityEngine.Time.deltaTime / moveTime);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
    }
}
