using UnityEngine;
using System.Collections;
using GameAnalyticsSDK;

namespace Pokega{

	public class AnalyticsControl : MonoBehaviour 
	{
        //edit stefan
		//GoogleAnalyticsV3 ga;
		public const GAProgressionStatus start=GAProgressionStatus.Start;
		public const GAProgressionStatus complete=GAProgressionStatus.Complete;
		public const GAProgressionStatus undefined=GAProgressionStatus.Undefined;
		public const GAProgressionStatus fail=GAProgressionStatus.Fail;
		
		void Awake()
		{
			//ga = GameObject.Find("GAv3").GetComponent<GoogleAnalyticsV3>();
			//ga.StartSession();
		}

		public void CreateAnalyticEvent(string eventName)
		{
			GameAnalytics.NewDesignEvent(eventName);
			//ga.LogEvent(category, action, eventName, (long)eventValue);
		}
		
		public void CreateAnalyticEvent(string eventName, float eventValue)
		{
			GameAnalytics.NewDesignEvent(eventName, eventValue);
			//ga.LogEvent(category, action, eventName, (long)eventValue);
		}

		public void CreateAnalyticsProgressionEvent(GAProgressionStatus status, string progression)
		{

			GameAnalytics.NewProgressionEvent(status,progression);
		}

		public void CreateAnalyticsProgressionEvent(GAProgressionStatus status, string progression1, string progression2)
		{

			GameAnalytics.NewProgressionEvent(status,progression1, progression2);
		}

		public void CreateAnalyticsProgressionEvent(GAProgressionStatus status, string progression1, string progression2, string progression3)
		{

			GameAnalytics.NewProgressionEvent(status,progression1, progression2, progression3);
		}



		public void CreateAnalyticScreenEvent(string screenName)
		{
			//ga.LogScreen(screenName);
		}

		public void CreateAnalyticTransactionEvent(string id, string affiliation, int price)
		{
			//GameAnalyticsSDK.GameAnalytics.NewDesignEvent("tansaction:" + id, 1.0f);
			//ga.LogTransaction(id, affiliation, (double)price, 0.0f, 0.0f);
		}	

		public void CreateAnalyticSocialEvent(string network, string action, string target = "default")
		{
			//GameAnalyticsSDK.GameAnalytics.NewDesignEvent(network + " - " + action);
			//ga.LogSocial(network, action, target);
		}


	}
}