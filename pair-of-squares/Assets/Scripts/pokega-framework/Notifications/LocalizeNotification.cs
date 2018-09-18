using UnityEngine;
using System.Collections;
using System.Globalization;
using SmartLocalization;

namespace Pokega{

	public class LocalizeNotification : MonoBehaviour
	{
		
		// No public variables, we'll get the key from the widget field
		string key;
		string mLanguage;
		LanguageManager loc;
		
		// Localize the widget on start.
		private void Awake() {
			loc = LanguageManager.Instance;
		}
		
		private void Start()
		{
			// Reference the language manager
			loc = LanguageManager.Instance;
			
			// Hook up a delegate to run the localize script whenever a language was changed
			loc.OnChangeLanguage += new ChangeLanguageEventHandler(Localize);
			
			// Initial localize run
			//Localize();
		}
		
		public string GetLanguage()
		{
			return loc.LoadedLanguage;
		}
		
		private void OnEnable()
		{
			if(mLanguage != loc.LoadedLanguage)
				Localize();	
		}
		
		// Force-localize the widget.
		private void Localize(LanguageManager thisLanguage=null)
		{
			string value;
			NotificationControl nc = GetComponent<NotificationControl>();
			
			for(int i=0; i<nc.notificationMessages.Length; i++)
			{
				key = nc.notificationMessages[i];
				value = loc.GetTextValue(key);
				if(string.IsNullOrEmpty(value))
					value = "Missing String";
				nc.notificationMessages[i] = value;
			}
			// Set this widget's current language
			mLanguage = loc.LoadedLanguage;
		}
	}
}