using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//edit stefan
//using GameAnalyticsSDK;

namespace Pokega{

	public class GameCenterControl : MonoBehaviour {



		//List of achievements and leaderboards (strings)
		public List<Achievement> achievementList;
		public List<Leaderboard> leaderboards;
		public string gcId;
		public string gcName;
		public GameObject gameCenterButtonHome;
		public GameObject gameCenterButtonGameOver;


#if UNITY_IOS

        
        void Awake() {
			Debug.Log("<color=green>GAME CENTER CONTROL AWAKE</color>");
			
			GameCenterManager.OnAuthFinished += OnAuthFinished;
			GameCenterManager.OnScoreSubmitted += OnScoreSubmited;
			GameCenterManager.OnAchievementsLoaded += OnAchievementsLoaded;
			GameCenterManager.OnAchievementsReset += OnAchievementsReset;
			GameCenterManager.OnAchievementsProgress += OnAchievementsProgress;
			
			GameCenterManager.init();
			
		}
        
		void Start(){
			achievementList = App.ach.achievements;
			leaderboards = App.lb.leaderboards;

			Debug.Log ("GC Control - Broj achievemenata je " + achievementList.Count.ToString());
			
			foreach (Achievement ach in achievementList) {
              
                GameCenterManager.RegisterAchievement(ach.idIOS);
				Debug.Log ("Registrovan achievement " + ach.name);
			}
		}
			
        
        private void OnAuthFinished (SA.Common.Models.Result res) {
			if (res.IsSucceeded) {
				
				//GameCenterManager.ResetAchievements();
				App.analytics.CreateAnalyticEvent("Game center: Auth succeeded");
				Debug.Log("Game Center gotova autentifikacija");
                if(gameCenterButtonHome != null)
				    gameCenterButtonHome.SetActive(true);

                if(gameCenterButtonGameOver != null)
				    gameCenterButtonGameOver.SetActive(true);
				SetGameCenter(GameCenterManager.Player.Id, GameCenterManager.Player.DisplayName);
 			}
			else {
				Debug.Log ("GC FAILED");
				App.analytics.CreateAnalyticEvent("Game center: Auth failed");
				gameCenterButtonHome.SetActive(false);
				gameCenterButtonGameOver.SetActive(false);
				SetGameCenter("", "");
			}
		}
		
		private void OnScoreSubmited (SA.Common.Models.Result result) {
			if(result.IsSucceeded)  {
				Debug.Log("Score Submited");
			}
			else {
				Debug.Log("Score Submit Failed");
			}
		}
		
		private void OnAchievementsLoaded(SA.Common.Models.Result 
            
            result) {
			
			if(result.IsSucceeded) {
				Debug.Log ("Achievemnts was loaded from IOS Game Center");
				
				foreach(GK_AchievementTemplate tpl in GameCenterManager.Achievements) {
					Debug.Log (tpl.Id + ":  " + tpl.Progress);
				}
			}
			else {
				Debug.Log ("Achievemnts DID NOT LOAD!");
			}
		}
		
		private void OnAchievementsProgress(SA.Common.Models.Result result) {
			
			if(result.IsSucceeded) {
				Debug.Log ("OnAchievementProgress");
				
				foreach(GK_AchievementTemplate tpl in GameCenterManager.Achievements) {
					Debug.Log (tpl.Id + ":  " + tpl.Progress);
				}
				
			}
			else {
				Debug.Log ("OnAchievementProgress ERROR!");
			}
		}
		
		private void OnAchievementsReset (SA.Common.Models.Result result) {
			if(result.IsSucceeded)  {
				Debug.Log("Achievements reset OK");
			}
			else {
				Debug.Log("Achievements reset FAIL");
			}
		}
        
		public void SetGameCenter(string id, string usr) {
			gcId = id;
			gcName = usr;
			App.local.PlayerSave();
		}

		
#endif

#if UNITY_ANDROID
        //		public const string LEADERBOARD_NAME = "leaderboard_hall_of_fame";
        //		public const string leaderBoardId = "CgkIipfs2qcGEAIQAA";
        //		
        //		void Start() {
        //			GameData.SetGameCenter("","");
        //			//listen for GooglePlayConnection events
        //			GooglePlayConnection.instance.addEventListener (GooglePlayConnection.PLAYER_CONNECTED, OnPlayerConnected);
        //			GooglePlayConnection.instance.addEventListener (GooglePlayConnection.PLAYER_DISCONNECTED, OnPlayerDisconnected);
        //			GooglePlayConnection.instance.addEventListener (GooglePlayConnection.CONNECTION_RESULT_RECEIVED, OnConnectionResult);
        //			
        //			//listen for GooglePlayManager events
        //			//GooglePlayManager.instance.addEventListener (GooglePlayManager.ACHIEVEMENT_UPDATED, OnAchivmentUpdated);
        //			//GooglePlayManager.instance.addEventListener (GooglePlayManager.SCORE_SUBMITED, OnScoreSubmited);
        //			
        //			if (GooglePlayConnection.state == GPConnectionState.STATE_CONNECTED) {
        //				//checking if player already connected
        //				OnPlayerConnected();
        //			} 
        //			else {
        //				GooglePlayConnection.instance.connect();
        //			}
        //		}
        //		
        //		private void OnPlayerDisconnected() {
        //			GameCenterButtonHome.SetActive(false);
        //			GameCenterButtonGameOver.SetActive(false);
        //			Debug.Log("disconnected");
        //		}
        //		
        //		private void OnPlayerConnected() {
        //			GameCenterButtonHome.SetActive(true);
        //			GameCenterButtonGameOver.SetActive(true);
        //			Debug.Log("connected");
        //		}
        //		
        //		private void OnConnectionResult(CEvent e) {
        //			GooglePlayConnectionResult result = e.data as GooglePlayConnectionResult;
        //			Debug.Log("connection status");
        //			Debug.Log(result.code.ToString());
        //		}
        //		
        //		void OnDestroy() {
        //			if(!GooglePlayConnection.IsDestroyed) {
        //				GooglePlayConnection.instance.removeEventListener (GooglePlayConnection.PLAYER_CONNECTED, OnPlayerConnected);
        //				GooglePlayConnection.instance.removeEventListener (GooglePlayConnection.PLAYER_DISCONNECTED, OnPlayerDisconnected);
        //			}
        //			if(!GooglePlayManager.IsDestroyed) {
        //				//GooglePlayManager.instance.removeEventListener (GooglePlayManager.ACHIEVEMENT_UPDATED, OnAchivmentUpdated);
        //				//GooglePlayManager.instance.removeEventListener (GooglePlayManager.SCORE_SUBMITED, OnScoreSubmited);
        //			}
        //		}
#endif
    }
}