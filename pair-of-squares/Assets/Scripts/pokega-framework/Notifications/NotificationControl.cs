using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
//using UnionAssets.FLE;
using System.Linq;
//stefan edit

namespace Pokega{

	public class NotificationControl : MonoBehaviour {
		
		public string[] notificationMessages;				//messages have to be localized (set in smartlocalization root) as keys
		public int[] notificationAfterSeconds;
		public string[] notificationSoundNames;
		
		public bool shuffleNotifications;
		public bool rescheduleNotificationsOnEveryPause;
			
		public string OneSignalAppId; //push notifications
		//stefan edit
		//public UIToggle notificationsToggle1;
		public bool notificationsOn;
		
		float enteredGameTime;
		float leavedGameTime;
		float onAppPauseTime;
		float onAppResumeTime;
		float enteredShopTime;
		float exitedShopTime;

		public string notifications;

		void Awake ()
		{		
			if(notificationMessages.Length != notificationAfterSeconds.Length || notificationMessages.Length != notificationSoundNames.Length)
				Debug.LogError("Lengths of local notif. messages and other parameters have to be the same!");	
		}
		
		void Start () 
		{
			//OneSignal.Init(OneSignalAppId, null, HandleNotification);
			Invoke ("GetUserToken", 2.0f);
			Invoke ("SetSubsc", 3.0f);
			
			//EventDelegate.Add(notificationsToggle1.onChange, OnNotificationsToggleChange);
			int notifications = PlayerPrefs.GetInt ("NotificationsOnOff");
			if (notifications == 1) 
			{
				Debug.Log("Notifications are ON.");
				notificationsOn = true;
				//stefan edit
				//notificationsToggle1.value = false;
			}
			else 
			{
				Debug.Log("Notifications are OFF.");
				notificationsOn = false;
				//stefan edit
				//notificationsToggle1.value = true;
			}
			
			SendBestScoresTags();
			SendTags();
		}
			
		void SendBestScoresTags()
		{
			if(!App.local.localDBReady)
				Invoke ("SendBestScoresTags", 1.0f);
			else
			{
				Debug.Log("Sending best scores on OneSignal.");
				foreach(Score s in App.score.bestScores)
				{
					//OneSignal.SendTag(s.scoreName, s.score);
				}
			}
		}
		
		public void SetEnteredShopTime()
		{
			enteredShopTime = Time.timeSinceLevelLoad;
		}	
		
		public void SetExitedShopTime()
		{
			exitedShopTime = Time.timeSinceLevelLoad;
			float timeSpentInShop = PlayerPrefs.GetFloat("timeSpentInShop");
			timeSpentInShop += exitedShopTime - enteredShopTime;
			PlayerPrefs.SetFloat("timeSpentInShop", timeSpentInShop);
			//OneSignal.SendTag("timeSpentInShop", timeSpentInShop.ToString());
		}
			
	    void SetSubsc()
	    {
			int n = PlayerPrefs.GetInt("NotificationsOnOff");
			/*if(n==1)
				OneSignal.SetSubscription(true);
			else 
				OneSignal.SetSubscription(false);
                */
	    }
		
		void SendTags()
		{
			//Initialization
			onAppResumeTime = Time.timeSinceLevelLoad;
			
			//bundle version
			//ovako radi samo za editor: OneSignal.SendTag("bundleVersion", PlayerSettings.bundleVersion.ToString ());
			//OneSignal.SendTag("bundleVersion", App.bundleVersion);
			//device OS
			//OneSignal.SendTag("deviceOS", SystemInfo.operatingSystem );
			//sessionCount
			int sessionCount = PlayerPrefs.GetInt("sessionCount");
			sessionCount++;
			PlayerPrefs.SetInt("sessionCount", sessionCount);
			//OneSignal.SendTag("sessionCount", sessionCount.ToString());		
			//timeSpentInShop
			//samo da bi poslalo odmah na ulazu u igru
			float timeSpentInShop = PlayerPrefs.GetFloat("timeSpentInShop");
			//OneSignal.SendTag("timeSpentInShop",timeSpentInShop.ToString());
			//timeSpentInGame
			//samo da bi poslalo odmah na ulazu u igru
			float timeSpentInGame = PlayerPrefs.GetFloat("timeSpentInGame");
			//OneSignal.SendTag("timeSpentInGame", timeSpentInGame.ToString());
			//gameCount
			//samo da bi poslalo odmah na ulazu u igru
			int gameCount = PlayerPrefs.GetInt("gameCount");
			//OneSignal.SendTag("gameCount", gameCount.ToString());
		}
		
		// next 3 methods called by game script
		public void OnGameStart()
		{
			enteredGameTime = Time.timeSinceLevelLoad;
			int gameCount = PlayerPrefs.GetInt("gameCount");
			gameCount++;
			PlayerPrefs.SetInt("gameCount",gameCount);
			//OneSignal.SendTag("gameCount", gameCount.ToString());
		}
		
		public void OnGameResume()
		{
			enteredGameTime = Time.timeSinceLevelLoad;
		}
		
		public void OnGamePauseOrEnd()
		{
			leavedGameTime = Time.timeSinceLevelLoad;
			float timeSpentInGame = leavedGameTime - enteredGameTime;
			float totalTimeSpentInGame = PlayerPrefs.GetFloat("timeSpentInGame");
			totalTimeSpentInGame += timeSpentInGame;
			PlayerPrefs.SetFloat("timeSpentInGame", totalTimeSpentInGame);
			//OneSignal.SendTag ("timeSpentInGame", totalTimeSpentInGame.ToString ());
		}	
			
		void GetUserToken ()
		{
			//OneSignal.GetIdsAvailable((userId, pushToken) => {
				/*App.device.SetApnToken(pushToken.ToString());
			});*/
		}
			
		/*static void HandleNotification (string message, Dictionary<string, object> additionalData, bool isActive) {
			print("GameControllerExample:HandleNotification");
			print(message);
			// When isActive is true this means the user is currently in your game.
			// Use isActive and your own game logic so you don't interupt the user with a popup or menu when they are in the middle of playing your game.
			if (additionalData != null) {
				if (additionalData.ContainsKey("discount")) {
					// Take user to your store.
				}
				else if (additionalData.ContainsKey("actionSelected")) {
					// actionSelected equals the id on the button the user pressed.
					// actionSelected will equal "__DEFAULT__" when the notification itself was tapped when buttons were present.
				}
			}
		}*/
			
		void ScheduleLocalNotifications ()
		{
			int notifications = PlayerPrefs.GetInt ("NotificationsOnOff");
			int notificationsSet = PlayerPrefs.GetInt ("NotificationsSet");
			if (notifications == 1 && (notificationsSet == 0 || rescheduleNotificationsOnEveryPause)) 
			{
				#if UNITY_IOS

				UnityEngine.iOS.LocalNotification notif = new UnityEngine.iOS.LocalNotification(); 
				if(shuffleNotifications)
				{
					System.Random rnd=new System.Random();
					notificationMessages = notificationMessages.OrderBy(x => rnd.Next()).ToArray(); 
				}
				for(int i=0; i<notificationMessages.Length; i++)
				{
					notif = new UnityEngine.iOS.LocalNotification();
					notif.alertAction = "alert";
					notif.fireDate = DateTime.Now.AddSeconds(notificationAfterSeconds[i]);
					notif.alertBody = notificationMessages[i];
					notif.soundName = notificationSoundNames[i];
					notif.applicationIconBadgeNumber = -1;
					UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(notif);
				}
				#endif

				Debug.Log("Local Notifications are set!");
				PlayerPrefs.SetInt ("NotificationsSet", 1);
			}
			else
			{
				Debug.Log("Local Notifications are (disabled) or (already set and option reschedule is OFF) !");
			}
		}
		
		void OnApplicationPause(bool pauseStatus) 
		{
			if (pauseStatus == true) 
			{
				Debug.Log("Application is paused");
				ScheduleLocalNotifications();
				
				//time spent in app
				onAppPauseTime = Time.timeSinceLevelLoad;
				float timeSpentInApp = PlayerPrefs.GetFloat("timeSpentInApp");
				timeSpentInApp += (onAppPauseTime - onAppResumeTime);
				PlayerPrefs.SetFloat("timeSpentInApp", timeSpentInApp);
				//OneSignal.SendTag("timeSpentInApp", timeSpentInApp.ToString ());
				SendBestScoresTags();
			}
			else 
			{
				SendTags();
				Debug.Log("Application has unpaused");
			}
		}
	
		void OnDestroy() 
		{
			//stefan edit
			//EventDelegate.Remove(notificationsToggle1.onChange, OnNotificationsToggleChange);
		}
			
		void OnNotificationsToggleChange() 
		{
			//stefan edit
			/*
			if (notificationsToggle1.value) 
			{
				Debug.Log("Notifications- OFF");
				//OneSignal.SetSubscription(false);
				notificationsOn = false;
				PlayerPrefs.SetInt ("NotificationsOnOff", 0);
				#if UNITY_IOS
				UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();
                //stefan edit
                //IOSNotificationController.instance.CancelAllLocalNotifications();
				#endif
			} 
			else 
			{
				Debug.Log("Notifications - ON");
				notificationsOn = true;
				//OneSignal.SetSubscription(true);
				PlayerPrefs.SetInt ("NotificationsOnOff", 1);
			}
			*/
		}
	}
}