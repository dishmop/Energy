using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SolicitPanel : BuySellPanel
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
            CustomerManager.instance.numCustomers[Id] += number;
        }
    }

    public override void Sell(int number)
    {
        if(CanSell(number))
        {
            CustomerManager.instance.numCustomers[Id] -= number;
        }
    }
}
