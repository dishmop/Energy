using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using System.Collections.Generic;
//using UnityEngine.Analytics;

public class CustomerPanel : BuySellPanel
{
    public int Id;

    new void Update()
    {
        base.Update();

        Number = CustomerManager.instance.numCustomers[Id];
    }


    public override bool CanBuy(int number)
    {
        return true;
    }

    public override bool CanSell(int number)
    {
        return CustomerManager.instance.numCustomers[Id] >= number;
    }

    public override void Buy(int number)
    {
        if (CanBuy(number))
        {
            AudioSource.PlayClipAtPoint(Globals.instance.clicklow, transform.position);
            CustomerManager.instance.numCustomers[Id] += number;
//			Debug.Log ("buyCustomers - numToBuy: " + number + ", id :" + Id + ", totalNumCustomers: " + CustomerManager.instance.numCustomers[Id] + ", gameTime : " + UnityEngine.Time.timeSinceLevelLoad);
			GoogleAnalytics.Client.SendEventHit("gameAction", "buyCustomers_" + number.ToString(), "type_" + Id.ToString() , CustomerManager.instance.numCustomers[Id]);
//			
//			Analytics.CustomEvent("buyCustomers", new Dictionary<string, object>
//			{
//				{ "numToBuy", number },
//				{ "id", Id.ToString() },
//				{ "totalNumCustomers", CustomerManager.instance.numCustomers[Id]},
//				{ "gameTime", UnityEngine.Time.timeSinceLevelLoad},
//			});			
        }
    }

    public override void Sell(int number)
    {
        if(CanSell(number))
        {
            AudioSource.PlayClipAtPoint(Globals.instance.clicklow, transform.position);
            CustomerManager.instance.numCustomers[Id] -= number;
//			Debug.Log ("sellCustomers - numToSell: " + number + ", id :" + Id + ", totalNumCustomers: " + CustomerManager.instance.numCustomers[Id] + ", gameTime : " + UnityEngine.Time.timeSinceLevelLoad);
			GoogleAnalytics.Client.SendEventHit("gameAction", "sellCustomers_" + number.ToString(), "type_" + Id.ToString() , CustomerManager.instance.numCustomers[Id]);
//			
//			Analytics.CustomEvent("sellCustomers", new Dictionary<string, object>
//			{
//				{ "numToSell", number },
//				{ "id", Id.ToString() },
//				{ "totalNumCustomers", CustomerManager.instance.numCustomers[Id]},
//				{ "gameTime", UnityEngine.Time.timeSinceLevelLoad},
//			});
			
        }
    }
}
