using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class ResearchPanel : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler {
    public static ResearchPanel instance;

    bool mouseOver = false;

    float LerpTime = 0.1f;

    RectTransform rt;

    public Researchable current = null;

    public Text nametext;
    public Text costtext;
    public Text descriptiontext;

    public Button buyButton;

    void Start()
    {
        rt = GetComponent<RectTransform>();
        instance = this;
    }

	void Update () {
        if (mouseOver)
        {
            rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, new Vector2(0.0f, rt.rect.height), UnityEngine.Time.deltaTime/LerpTime);
        }
        else
        {
            rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, new Vector2(0.0f, 70f), UnityEngine.Time.deltaTime / LerpTime);
        }

        if (current != null)
        {
            nametext.text = current.Name;
            costtext.text = Utilities.MoneyToString((ulong)current.cost);
            descriptiontext.text = current.LongDescription;

            buyButton.gameObject.SetActive(true);

            buyButton.interactable = current.CanBuy();
        }
        else
        {
            nametext.text = "";
            descriptiontext.text = "";
            costtext.text = "";

            buyButton.gameObject.SetActive(false);
        }
	}

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioSource.PlayClipAtPoint(Globals.instance.swipe,transform.position);
        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOver = false;
        current = null;
    }

    public void Buy()
    {
        if(current!=null)
        {
            current.Buy();
        }
    }
}
