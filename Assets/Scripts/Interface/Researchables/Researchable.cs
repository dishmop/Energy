using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Researchable : MonoBehaviour, IPointerDownHandler {
    public int cost;

    public Sprite sprite;

    [System.NonSerialized]
    public bool researched = false;

    public UnityEngine.UI.Image icon;

    protected void Start()
    {
        icon.sprite = sprite;
        GetComponentInChildren<UnityEngine.UI.Text>().text = name;
    }

    [TextArea(3,10)]
    public string LongDescription;

    public bool CanBuy()
    {
        return GameManager.instance.money >= (ulong)cost;
    }

    public void Buy()
    {
        if (CanBuy())
        {
            researched = true;
            AudioSource.PlayClipAtPoint(Globals.instance.clickhigh, transform.position);
            GameManager.instance.money -= (ulong)cost;
            OnResarch();
        }
    }

    protected abstract void OnResarch();

    public void OnPointerDown(PointerEventData ped)
    {
        AudioSource.PlayClipAtPoint(Globals.instance.clicklow, transform.position);
        ResearchPanel.instance.current = this;
    }
}
