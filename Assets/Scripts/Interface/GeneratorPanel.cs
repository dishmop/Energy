using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GeneratorPanel : BuySellPanel {

    public int Cost;

    public Text cost;

    public string Type;

    System.Type type;

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

    new void Update()
    {
        Number = GameManager.instance.generators.FindAll(delegate(Generator gen) { return gen.GetType() == type; }).Count;
        cost.text = Utilities.MoneyToString((ulong)Cost);
        base.Update();
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
                GameManager.instance.generators.Add((Generator)System.Activator.CreateInstance(type));
                GameManager.instance.money -= (ulong)Cost;
            }
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
            }
        }
    }
}
