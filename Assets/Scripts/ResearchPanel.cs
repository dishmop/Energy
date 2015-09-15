using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ResearchPanel : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler {
    bool mouseOver = false;

    float LerpTime = 10f;

    RectTransform rt;

    void Start()
    {
        rt = GetComponent<RectTransform>();
    }

	void Update () {
        if (mouseOver)
        {
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchorMin = rt.anchorMax;

            rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, new Vector2(0.0f, 0.0f), UnityEngine.Time.deltaTime/LerpTime);
        }
        else
        {
            rt.pivot = new Vector2(0.5f, 1.0f);
            rt.anchorMax = new Vector2(0.5f, 0.1f);
            rt.anchorMin = rt.anchorMax;

            rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, new Vector2(0.0f, 0.1f), UnityEngine.Time.deltaTime / LerpTime);
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
