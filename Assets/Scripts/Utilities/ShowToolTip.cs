using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ShowToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [TextArea(3, 10)]
    public string ToolTipText  = "Missing tooltip!";

    public void OnPointerEnter(PointerEventData eventData)
    {
        ToolTip.instance.ToolTipOn(ToolTipText, gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTip.instance.ToolTipOff(gameObject);
    }

}
