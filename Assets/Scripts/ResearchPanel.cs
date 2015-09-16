using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ResearchPanel : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler {
    bool mouseOver = false;

    float LerpTime = 0.1f;

    RectTransform rt;

    void Start()
    {
        rt = GetComponent<RectTransform>();
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
