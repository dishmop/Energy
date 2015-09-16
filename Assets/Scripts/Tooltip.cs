using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ToolTip : MonoBehaviour
{
    public static ToolTip instance;

    public Text textField;
    public GameObject panel;

    float maxwidth = 180;

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        if (fromCurrent != null)
        {
            textField.text = text;
            panel.SetActive(true);

            Vector2 extents = textField.rectTransform.rect.size;
            var settings = textField.GetGenerationSettings(extents);

            float length = textField.cachedTextGenerator.GetPreferredWidth(text, settings) + 17;
            if (length > maxwidth)
            {
                length = maxwidth;
            }

            float height = textField.cachedTextGenerator.GetPreferredHeight(text, settings) + 10;

            GetComponent<RectTransform>().sizeDelta = new Vector2(length, height);
        }
        else
        {
            panel.SetActive(false);
        }

        Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        GetComponent<RectTransform>().anchoredPosition = new Vector3(mousePos.x + 2, mousePos.y + 2, 0);

        if (GetComponent<RectTransform>().anchoredPosition.x+GetComponent<RectTransform>().sizeDelta.x > ((RectTransform)transform.parent).sizeDelta.x)
        {
            GetComponent<RectTransform>().anchoredPosition -= new Vector2(maxwidth+10,0);
        }

        if (GetComponent<RectTransform>().anchoredPosition.y + GetComponent<RectTransform>().sizeDelta.y > ((RectTransform)transform.parent).sizeDelta.y)
        {
            GetComponent<RectTransform>().anchoredPosition -= new Vector2(0,GetComponent<RectTransform>().sizeDelta.y + 20);
        }
    }

    GameObject fromCurrent;
    string text;

    public void ToolTipOn(string Text, GameObject from)
    {
        fromCurrent = from;
        text = Text;
    }

    public void ToolTipOff(GameObject from)
    {
        if (from == fromCurrent)
        {
            fromCurrent = null;
        }
    }
}
