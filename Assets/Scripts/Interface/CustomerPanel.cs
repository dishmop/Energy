using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
        }
    }

    public override void Sell(int number)
    {
        if(CanSell(number))
        {
            AudioSource.PlayClipAtPoint(Globals.instance.clicklow, transform.position);
            CustomerManager.instance.numCustomers[Id] -= number;
        }
    }
}
