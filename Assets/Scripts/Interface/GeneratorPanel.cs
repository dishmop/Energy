using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;

public class GeneratorPanel : BuySellPanel {

    public int Cost;
    public int SellCost;

    public Text cost;

    public string Type;

    System.Type type;

    public GameObject mask;

    public Slider activeSlider;

    void Start()
    {
        type = System.Type.GetType(Type);
        if (type==null)
        {
            throw new System.Exception("Type " + Type+ " not recognised");
        }
        else if(!type.IsSubclassOf(typeof(Generator))){
            throw new System.Exception("Type must be a subclass of Generator");
        }
    }

    bool up;

    new void Update()
    {
        Number = GameManager.instance.generators.FindAll(delegate(Generator gen) { return gen.GetType() == type; }).Count;
        cost.text = Utilities.MoneyToString((ulong)Cost);

        Name.text = name;
        number.text = ProPlotter.SIPrefix(Number, 0);
        image.sprite = sprite;

        activeSlider.maxValue = Number;

        if (mouseOver)
        {
            //mask.GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(mask.GetComponent<RectTransform>().sizeDelta, new Vector2(250, mask.GetComponent<RectTransform>().sizeDelta.y), UnityEngine.Time.deltaTime);
            mask.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(mask.GetComponent<RectTransform>().sizeDelta.x, 230, 5.0f*UnityEngine.Time.deltaTime));

            if (Input.GetMouseButtonDown(0))
            {
                up = true;
            }
        }
        else
        {
            up = false;
            mask.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Lerp(mask.GetComponent<RectTransform>().sizeDelta.x, 200, 5.0f * UnityEngine.Time.deltaTime));
        }

        if (up)
        {
            childpanel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(childpanel.GetComponent<RectTransform>().anchoredPosition, new Vector2(0, 75), UnityEngine.Time.deltaTime / moveTime);
        }
        else
        {
            childpanel.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(childpanel.GetComponent<RectTransform>().anchoredPosition, new Vector2(0, 0), UnityEngine.Time.deltaTime / moveTime);
        }

        //base.Update();
    }

    public override bool CanBuy(int number)
    {
        return GameManager.instance.money >= (ulong)(number * Cost);
    }

    public override bool CanSell(int number)
    {
        return Number >= number;
    }

    public override void Buy(int number)
    {
        if (CanBuy(number))
        {
            AudioSource.PlayClipAtPoint(Globals.instance.clicklow,transform.position);
            for (int i = 0; i < number; i++)
            {
                Generator newgen = (Generator)System.Activator.CreateInstance(type);
                newgen.panel = this;
                GameManager.instance.generators.Add(newgen);
                GameManager.instance.money -= (ulong)Cost;

                activeSlider.maxValue++;
                activeSlider.value++;
                Number++;
            }
            
//			Debug.Log ("buyGenerators - numToBuy: " + number + "type = " + this.Type + ", totalNumGenerators: " + Number + ", gameTime : " + UnityEngine.Time.timeSinceLevelLoad);
			Analytics.CustomEvent("buyGenerators", new Dictionary<string, object>
			{
				{ "numToBuy", number },
				{ "type", this.Type  },
				{ "totalNumGenerators", Number},
				{ "gameTime", UnityEngine.Time.timeSinceLevelLoad},
			});				
        }
    }

    public override void Sell(int number)
    {
        if (CanSell(number))
        {
            AudioSource.PlayClipAtPoint(Globals.instance.clicklow, transform.position);
            for (int i = 0; i < number; i++)
            {
                int index = GameManager.instance.generators.FindLastIndex(delegate(Generator gen) { return gen.GetType() == type; });
                GameManager.instance.generators.RemoveAt(index);
                GameManager.instance.money += (ulong)SellCost;
            }
//			Debug.Log ("sellGenerators - numToSell: " + number + "type = " + this.Type + ", totalNumGenerators: " + Number + ", gameTime : " + UnityEngine.Time.timeSinceLevelLoad);
			Analytics.CustomEvent("sellGenerators", new Dictionary<string, object>
			{
				{ "numToSell", number },
				{ "type", this.Type  },
				{ "totalNumGenerators", Number},
				{ "gameTime", UnityEngine.Time.timeSinceLevelLoad},
			});			
        }
    }
}
