using UnityEngine;
using System.Collections;

namespace Pokega
{
	public class AppRate : MonoBehaviour {

		public int askForRateAfter;
		public string headerMsg;
		public string descriptionMsg;
		public string appStoreUrlOfYourGame;
		public EventDelegate[] notifyWhenRated;
		public bool keepAskingToRateUntilRate;
		public UIButton rateButton;


		//Call on every game end 
		public void CheckRated()
		{	
			int rated = PlayerPrefs.GetInt ("Rated");
			int numberOfTimesPlayed = PlayerPrefs.GetInt ("numberOfTimesPlayed");
			numberOfTimesPlayed++;
			PlayerPrefs.SetInt ("numberOfTimesPlayed", numberOfTimesPlayed);

			if ((rated == 0) && (numberOfTimesPlayed % askForRateAfter == 0)) 
			{
				Rate();
			}
		}

       
        
        private void onRatePopUpClose(IOSDialogResult result) {

			switch(result) {
			case IOSDialogResult.RATED:
				try{
					rateButton.isEnabled = false;
				}
				catch
				{
					
				}
				PlayerPrefs.SetInt("Rated", 1);
				App.analytics.CreateAnalyticEvent("rated", 1);
				foreach(EventDelegate ed in notifyWhenRated)
					ed.Execute();
				Debug.Log ("Rate button pressed");
				break;
			case IOSDialogResult.REMIND:
				PlayerPrefs.SetInt("Rated", 0);
				App.analytics.CreateAnalyticEvent("rate_remind_me", 1);
				Debug.Log ("Remind button pressed");
				break;
			case IOSDialogResult.DECLINED:
				if(!keepAskingToRateUntilRate)
					PlayerPrefs.SetInt("Rated", 1);
				else
					PlayerPrefs.SetInt("Rated", 0);
				App.analytics.CreateAnalyticEvent("rate_declined", 1);
				Debug.Log ("Decline button pressed");
				break;
			}
		}

		public void Rate()
		{
			App.analytics.CreateAnalyticEvent("rate_pop_up", 1.0f);
			Debug.Log("RATE POP UP");
			Debug.Log("Rated in PP:" + PlayerPrefs.GetInt ("Rated"));
			#if UNITY_ANDROID
			#endif

			#if UNITY_IOS
			Debug.Log("Rate PopUp");

			IOSRateUsPopUp rate = IOSRateUsPopUp.Create(headerMsg,descriptionMsg);
			rate.OnComplete += onRatePopUpClose;
			#endif
		}
        
	}
}
