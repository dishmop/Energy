using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
//using System.Collections.Generic;
//using UnityEngine.Analytics;

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

    public GameObject custPanel;
    public GameObject genPanel;

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
            nametext.text = current.name;
            
            descriptiontext.text = current.LongDescription;

            buyButton.gameObject.SetActive(true);

            if (current.researched)
            {
                costtext.text = "Already researched";
                buyButton.interactable = false;
            }
            else
            {
                costtext.text = Utilities.MoneyToString((ulong)current.cost);
                buyButton.interactable = current.CanBuy();
            }
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

    public void AddCustomer(string name, int id, string description, Sprite image)
    {
//		Debug.Log ("researchCustomer - name: " + name + ", id :" + id + ", gameTime : " + UnityEngine.Time.timeSinceLevelLoad);
		GoogleAnalytics.Client.SendTimedEventHit("gameAction", "researchCustomer", name, UnityEngine.Time.timeSinceLevelLoad);
//		
//		Analytics.CustomEvent("researchCustomer", new Dictionary<string, object>
//		                      {
//			{ "name", name },
//			{ "id", id.ToString()  },
//			{ "gameTime", UnityEngine.Time.timeSinceLevelLoad},
//		});			
		
        var panel = (GameObject)Instantiate(Globals.instance.custPrefab);
        panel.transform.SetParent(custPanel.transform, false);

        panel.name = name;

        var cust = panel.GetComponent<CustomerPanel>();

        cust.Description = description;
        cust.Id = id;
        cust.sprite = image;
    }

    public void AddGenerator(string name, int cost, int saleCost, string type, string description, Sprite image)
    {
//		Debug.Log ("researchGenerator - name: " + name + ", type :" + type + ", gameTime : " + UnityEngine.Time.timeSinceLevelLoad);
		GoogleAnalytics.Client.SendTimedEventHit("gameAction", "researchGenerator", name, UnityEngine.Time.timeSinceLevelLoad);
//		Analytics.CustomEvent("researchGenerator", new Dictionary<string, object>
//		{
//			{ "name", name },
//			{ "type", type  },
//			{ "gameTime", UnityEngine.Time.timeSinceLevelLoad},
//		});			
        var panel = (GameObject)Instantiate(Globals.instance.genPrefab);
        panel.transform.SetParent(genPanel.transform, false);

        panel.name = name;

        var cust = panel.GetComponent<GeneratorPanel>();

        cust.Description = description;
        cust.Cost = cost;
        cust.SellCost = saleCost;
        cust.Type = type;
        cust.sprite = image;
    }
}
