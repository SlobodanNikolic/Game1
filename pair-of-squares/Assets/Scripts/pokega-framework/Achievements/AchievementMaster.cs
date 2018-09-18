using UnityEngine;
using System.Collections.Generic;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;

namespace Pokega{
	
	[System.Serializable]
	public class AchievementMaster : MonoBehaviour {
	
		public List<Achievement> achievements;
	
		void Awake(){

			//Adds all achievements to the list
			achievements = new List<Achievement>();
			Achievement[] achs = gameObject.GetComponentsInChildren<Achievement>();
			for(int i=0; i<this.gameObject.transform.childCount; i++)
				achievements.Add(achs[i]);
		}

		//This function should be called when an achievement is 100% complete
		public void setAchievementGC(string achievementName) {
			foreach(Achievement ach in achievements)
				if(ach.name == achievementName && ach.achieved == false){
                    GameCenterManager.SubmitAchievement (100.0f, ach.idIOS, true);
					ach.achieved = true;
					Debug.Log("Achievement " + ach.name + " set on GC");
				}
		}

		public void setAchievementGP(string achievementName){
			foreach(Achievement ach in achievements)
				if(ach.name == achievementName && ach.achieved == false)
				{
					Social.ReportProgress(ach.idAND, 100.0f, (bool success) => {
						// handle success or failure
					});
					ach.achieved = true;
					Debug.Log("Achievement " + ach.name + " set on GooglePlay");
				}
		}

		public void SetAchievement(string name)
		{
			#if UNITY_IOS
				setAchievementGC (name);
			#endif

			#if UNITY_ANDROID
				setAchievementGP (name);
			#endif
		}
	}
}