using UnityEngine;
using System.Collections;
using Facebook.Unity;
using System;
using SmartLocalization;

namespace Pokega 
{
	public class FacebookShare : MonoBehaviour {
			
		
		public string contentURL;
		public string contentTitleKey;		//smart localization key for share title
		public string contentDescriptionKey;	//smart localization key for share description
		public string photoURL;
		public EventDelegate rewardMethodForShare;
		string mLanguage;
		LanguageManager loc;
		string contentTitle;
		string contentDescription;
		
	
		private void Awake() 
		{
			loc = LanguageManager.Instance;
		}
		
		private void Start()
		{
			// Reference the language manager
			loc = LanguageManager.Instance;
			
			// Hook up a delegate to run the localize script whenever a language was changed
			loc.OnChangeLanguage += new ChangeLanguageEventHandler(Localize);
		}
	
		public void OnClick() 
		{
			Share ();
		}
		
		public void Share()
		{
			FB.ShareLink(new Uri(contentURL), contentTitle, contentDescription, new Uri(photoURL), ShareCallback);
			App.analytics.CreateAnalyticSocialEvent("Facebook", "ShareClicked");
		}
	
		void ShareCallback (IShareResult result) 
		{
			if (result.Cancelled || !String.IsNullOrEmpty(result.Error)) 
			{
				App.analytics.CreateAnalyticSocialEvent("Facebook", "ShareFailed");
				Debug.Log("ShareLink Error: "+result.Error);
			}
			else if (!String.IsNullOrEmpty(result.PostId)) 
			{
				// Print post identifier of the shared content
				Debug.Log(result.PostId);
			}
			else 
			{
				App.analytics.CreateAnalyticSocialEvent("Facebook", "ShareSucceded");
				Debug.Log("ShareLink success!");
				rewardMethodForShare.Execute();
			}
		}
		
		public string GetLanguage()
		{
			return loc.LoadedLanguage;
		}
		
		private void OnEnable()
		{
			Localize();	
		}
		
		// Force-localize the widget.
		private void Localize(LanguageManager thisLanguage=null)
		{
			string value;
			
			value = loc.GetTextValue(contentTitleKey);
			if(string.IsNullOrEmpty(value))
				value = "Missing String";
			else
				contentTitle = value;
			
			value = loc.GetTextValue(contentDescriptionKey);
			if(string.IsNullOrEmpty(value))
				value = "Missing String";
			else
				contentDescription = value;
		}	
	}
}