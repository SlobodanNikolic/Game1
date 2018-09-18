using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pokega{
	
	public class UIControl : MonoBehaviour 
	{
		public List<PokegaScreen> screens;
		public List<PokegaPopUp> popUps;
		public string lastScreenName = "";
		public int startScreenIndex;
		
		public string currentScreen = "";
		public string currentPopUp = "";
		public int pausedPopUpIndex = -1;
		
		public bool uiLocked = false;
		
		public delegate void UIChangeAction (string changedTo);
		public static event UIChangeAction ScreenChanged;
		public static event UIChangeAction PopUpOpened;

		private GameObject blocker;

		void Awake()
		{
			SetupBlocker()	;
		} 

		void Start()
		{
			SetScreen(screens[startScreenIndex].gameObject.name);
		}

		public int ScreenIndexOf(string name)
		{
			for(int i = 0; i < screens.Count; i++)
				if(screens[i].gameObject.name == name) return i;
			return -1;
		}

		public int PopUpIndexOf(string name)
		{
			for(int i = 0; i < popUps.Count; i++)
				if(popUps[i].gameObject.name == name) return i;
			return -1;
		}
		
		//-----------------------------------------------------------------------
		
		public string GetCurrentScreen()
		{
			return currentScreen;
		}
		
		public void SetScreen(string screenName)
		{
			Debug.Log ("Setting screen " + screenName);

			if(uiLocked)
			{
				Debug.LogWarning("UI is currently locked - some of the tweens not finished yet");
				return;
			}
			LockUI();
			lastScreenName = currentScreen;
			int index = ScreenIndexOf(screenName);
			if(index == -1)
			{
				Debug.LogError("You called SetScreen with invalid argument: " + screenName + " do not exists in defined screens");
				return;
			}
			
			//set current screen
			currentScreen = screenName;
			screens[index].Set();

			Debug.Log ("Screen " + screenName + " set");

			//turn off all the popUps
			pausedPopUpIndex = -1;
			if(currentPopUp != "")
			{
				UnlockUI();
				SetPopUp(currentPopUp, true);
			}
			else
			{
				InvokeRealTime("UnlockUI", screens[index].tweenTime);
			}
			
			if(ScreenChanged!=null)
				ScreenChanged(currentScreen);

			try
			{
				App.analytics.CreateAnalyticScreenEvent(currentScreen);
			}
			catch
			{
				//Debug.LogError("Baca exception");
			}
		}
		
		//-----------------------------------------------------------------------
		
		public string GetCurrentPopUp()
		{
			return currentPopUp;
		}
		
		public void SetPopUp(string popUpName, bool close = false)
		{
			Debug.Log ("Setting PopUp " + popUpName);
			if(uiLocked)
			{
				Debug.LogWarning("UI is currently locked - some of the tweens not finished yet");
				return;
			}
			LockUI();
			
			int index = PopUpIndexOf(popUpName);
			if(index == -1)
			{
				Debug.LogError("You called SetPopup with invalid argument: " + popUpName + " do not exists in defined screens");
				return;
			}
			
			//close this popUp
			if(close)
			{
				Debug.Log ("Should Close popup");
				popUps[index].SetOff();
				if(pausedPopUpIndex == -1)
				{
					Debug.Log ("Index -1");
					currentPopUp = "";
					InvokeRealTime("UnlockUI", popUps[index].tweenTime);
				}
				else
				{
					Debug.Log ("Index not -1");

					UnlockUI();
					popUps[pausedPopUpIndex].Set();
					InvokeRealTime("UnlockUI", popUps[pausedPopUpIndex].tweenTime);
					currentPopUp = popUps[pausedPopUpIndex].name;
					pausedPopUpIndex = -1;
					if(PopUpOpened!=null)
						PopUpOpened(currentPopUp);
				}
			}
			
			//open popUp
			else
			{
				//if there is another popUp on the screen save its index, and turn it off
				if(currentPopUp != "")
				{
					Debug.Log ("Kad bi mogo da napravis nesto da radi za sve, a ne samo za tebe");
					pausedPopUpIndex = PopUpIndexOf(currentPopUp);
					popUps[pausedPopUpIndex].SetOff();
				}
				Debug.Log ("INDEX IS " + index.ToString());
				popUps[index].Set();
				InvokeRealTime("UnlockUI", popUps[index].tweenTime);
				currentPopUp = popUps[index].name;
				if(PopUpOpened!=null)
					PopUpOpened(currentPopUp);
				Debug.Log ("Sending analytics event");
				//App.analytics.CreateAnalyticScreenEvent(currentPopUp);
			}
			Debug.Log ("End of SetPopUp");
		}
		
		//-----------------------------------------------------------------------
		
		public void UnlockUI()
		{
			uiLocked = false;
			blocker.SetActive(false);
		}

		public void LockUI()
		{
			uiLocked = true;
			blocker.SetActive(true);
		}

		void SetupBlocker()
		{
			GameObject _blocker = new GameObject();
			GameObject root = GameObject.Find("UI Root");
			blocker = Instantiate(_blocker) as GameObject;
			blocker.name = "UI_Blocker";
			blocker.transform.parent = root.transform;
			blocker.AddComponent<UIWidget>();
			blocker.AddComponent<BoxCollider>();
			//blocker.AddComponent<Transform>();
			blocker.GetComponent<UIWidget>().autoResizeBoxCollider = true;
			blocker.GetComponent<UIWidget>().depth = 1000;
			blocker.GetComponent<UIWidget>().width = 4000;
			blocker.GetComponent<UIWidget>().height = 4000;
			blocker.transform.localScale = Vector3.one;
			blocker.transform.localPosition = Vector3.zero;
			blocker.SetActive(false);
		}

		void InvokeRealTime(string functionName, float delay) 
		{
			StartCoroutine(InvokeRealTimeHelper(functionName, delay) );
		}

		IEnumerator InvokeRealTimeHelper(string functionName, float delay) 
		{
			float timeElapsed = 0f;
			while (timeElapsed < delay) {
				timeElapsed += Time.unscaledDeltaTime;
				yield return null;
			}
			SendMessage(functionName);
		}
	}
}