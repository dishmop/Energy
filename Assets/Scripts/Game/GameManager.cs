using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
//using UnityEngine.Analytics;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    public Rigidbody generatorHandle;

    public Text lightButtonText;

    public float supply = 0.0f; // in W
    public float demand = 0.0f; // in W
    public float surplus;

    public Dictionary<System.Type, float> supplyByType = new Dictionary<System.Type, float>();

    public float totalCarbon = 0.0f; // in g

    public ulong money = 0; // in £ 
    
	ulong recMoneyExp = 0;

    CustomerManager customerDemand;
    public List<Generator> generators = new List<Generator>();

    float sampleInterval;

    float prevSample = 0;
    float nextSample = 0;
    
    

    float timeSinceSample = float.PositiveInfinity;

    enum PayLevel
    {
        none,
        threeh,
        sixh,
        twelveh,
        day
    }

	int recMonths = 0;
	
    int dayssincepc = 0;

    PayLevel pay = PayLevel.none;

    void Start()
    {
        instance = this;
        customerDemand = new CustomerManager();
//		Debug.Log ("startGame");
		GoogleAnalytics.Client.SendEventHit("gameFlow", "startGame");
		GoogleAnalytics.Client.SendScreenHit("startGame" + name);
//		
//		Analytics.CustomEvent("startGame", new Dictionary<string, object>
//		                      {
//			{ "dummy", 0 },
//		});
    }

    Queue<float> handGenAngVel = new Queue<float>();

    void Update()
    {
    
		if (money > Mathf.Pow (10, recMoneyExp)){
		//	Debug.Log ("moneyGain - threshold = " + Mathf.Pow (10, recMoneyExp).ToString()  + ", gameTime : " + UnityEngine.Time.timeSinceLevelLoad);
			GoogleAnalytics.Client.SendTimedEventHit("gameFlow", "moneyThreshold", Mathf.Pow (10, recMoneyExp).ToString(), UnityEngine.Time.timeSinceLevelLoad);
			GoogleAnalytics.Client.SendScreenHit("moneyThreshold_" +  Mathf.Pow (10, recMoneyExp).ToString());
//			Analytics.CustomEvent("moneyGain", new Dictionary<string, object>
//			{
//				{ "threshold", Mathf.Pow (10, recMoneyExp).ToString() },
//				{ "gameTime", UnityEngine.Time.timeSinceLevelLoad},
//			});
			
			recMoneyExp++;
		}
		
		if (Time.instance.Month-7 != recMonths ){
		//	Debug.Log ("monthInc - currentMonth = " + (Time.instance.Month-7).ToString()  + ", gameTime : " + UnityEngine.Time.timeSinceLevelLoad);
			GoogleAnalytics.Client.SendTimedEventHit("gameFlow", "monthInc", "currentMonth" + (Time.instance.Month-7).ToString() , UnityEngine.Time.timeSinceLevelLoad);
//			Analytics.CustomEvent("monthInc", new Dictionary<string, object>
//			                      {
//				{ "currentMonth", (Time.instance.Month-7).ToString() },
//				{ "gameTime", UnityEngine.Time.timeSinceLevelLoad},
//			});
			
			recMonths = Time.instance.Month - 7;
			
		}
        timeSinceSample += Time.instance.DeltaTime;

        sampleInterval = 0.05f / (customerDemand.TotalCustomers > 0 ? customerDemand.TotalCustomers : 1);

        if (timeSinceSample > sampleInterval)
        {
            prevSample = nextSample;

            float mean = customerDemand.TotalMean(Time.instance.WeekDay, Time.instance.CurrentSeason, Mathf.Repeat(Time.instance.DayFraction + sampleInterval, 1f), 0);
            float variance = customerDemand.TotalVariance(Time.instance.WeekDay, Time.instance.CurrentSeason, Mathf.Repeat(Time.instance.DayFraction + sampleInterval, 1f), 0);

            nextSample = Sample.Normal(mean, Mathf.Sqrt(variance), true);
            timeSinceSample = 0;
        }

        demand = Mathf.Lerp(prevSample, nextSample, timeSinceSample / sampleInterval);

        supply = 0;

        var types = new List<System.Type>(supplyByType.Keys);
        foreach (var type in types)
        {
            supplyByType[type] = 0f;
        }

        foreach (var generator in generators)
        {
            generator.CallUpdate();
            float supp = generator.Output(Time.instance.DayFraction, Time.instance.CurrentSeason);

            if (supplyByType.ContainsKey(generator.GetType()))
            {
                supplyByType[generator.GetType()] += supp;
            }
            else
            {
                supplyByType.Add(generator.GetType(), supp);
            }

            supply += supp;
        }

        handGenAngVel.Enqueue(generatorHandle.angularVelocity.magnitude);

        while(handGenAngVel.Count>16){
            handGenAngVel.Dequeue();
        }

        float handgensupply = 0;

        foreach(float angvel in handGenAngVel){
            handgensupply += angvel;
        }

        supply += (40 * handgensupply) / handGenAngVel.Count;

        surplus = supply - demand;

        switch (pay)
        {
            case PayLevel.none:
                if (Time.instance.HoursSincePowerCut >= 3)
                {
                    if(Pay(1)) pay = PayLevel.threeh;
                    
                }
                break;
            case PayLevel.threeh:
                if (Time.instance.HoursSincePowerCut >= 6)
                {
                    if (Pay(3)) pay = PayLevel.sixh;
                }
                break;
            case PayLevel.sixh:
                if (Time.instance.HoursSincePowerCut >= 12)
                {
                    if (Pay(7)) pay = PayLevel.twelveh;
                }
                break;
            case PayLevel.twelveh:
                if (Time.instance.DaysSincePowerCut >= 1)
                {
                    if (Pay(15)) pay = PayLevel.day;
                }
                break;
            case PayLevel.day:
                while (Time.instance.DaysSincePowerCut > dayssincepc)
                {
                    dayssincepc++;
                    Pay(25+dayssincepc);
                }
                break;
        }

        dayssincepc = Time.instance.DaysSincePowerCut;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TurnHandle();
        }

        if(surplus<0 || customerDemand.TotalCustomers<=0)
        {
            Time.instance.PowerCut();
            pay = PayLevel.none;
        }
    }

    public void TurnHandle()
    {
        generatorHandle.maxAngularVelocity = 50;
        generatorHandle.AddTorque(new Vector3(1, 0, 0), ForceMode.Impulse);
        if (Time.instance.secondsPerDay > 15.0f)
        {
            Globals.instance.clickPlayer.Play();
        }
    }

    bool Pay(int multiplier)
    {
        ulong moneyold = money;
        for (int i = 0; i < 8; i++)
        {
            money += (ulong)(multiplier*customerDemand.numCustomers[i]*CustomerManager.basepay[i]);
        }

        if (moneyold != money)
        {

            if(Time.instance.secondsPerDay >=5f)
                AudioSource.PlayClipAtPoint(Globals.instance.coin, transform.position);
            return true;
        }
        return false;
    }
}
