using UnityEngine;
using System.Collections;
using Pokega;

namespace Pokega{

	public class GameCenterButton : MonoBehaviour {

		void Awake () {

		}

		//Shows the first leaderboard
		void OnClick() {
            //Debug.Log ("GC Leaderboard ID " + App.gc.leaderboards[0]);
            //GameCenterManager.ShowLeaderboard(App.gc.leaderboards[0].leaderboardId);

            #if UNITY_IPHONE
            GameCenterManager.ShowLeaderboards ();
			App.analytics.CreateAnalyticEvent ("Game center button clicked");
			#endif

			#if UNITY_ANDROID
			Debug.Log("SHOWING LEADERBOARDS");
			//GooglePlayManager.instance.showLeaderBoard(GameCenterControl.LEADERBOARD_NAME);
			//GooglePlayManager.instance.showAchivmentsUI();
			Social.ShowLeaderboardUI();
			App.analytics.CreateAnalyticEvent ("Google play button clicked");
			#endif
		}
	}
}