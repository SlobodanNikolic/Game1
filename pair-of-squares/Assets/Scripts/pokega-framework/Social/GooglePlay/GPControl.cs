using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

using UnityEngine.SocialPlatforms;

namespace Pokega{
	public class GPControl : MonoBehaviour {

		void Awake(){
			


		}

		// Use this for initialization
		void Start () {
			#if UNITY_ANDROID
			PlayGamesPlatform.DebugLogEnabled = true;
			PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder ().Build ();
			// Activate the Google Play Games platform
			PlayGamesPlatform.Activate();
			Authenticate ();
			#endif
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public void Authenticate(){
#if UNITY_ANDROID
            Social.localUser.Authenticate((bool success) => {
				if(success){
					((PlayGamesLocalUser)Social.localUser).GetStats((rc, stats) =>
						{
							// -1 means cached stats, 0 is succeess
							// see  CommonStatusCodes for all values.

							if (rc <= 0 && stats.HasDaysSinceLastPlayed()) {
								Debug.Log("It has been " + stats.DaysSinceLastPlayed + " days");
								//Ovde uzeti sta hocemo iz stats
							}
							else{
								Debug.Log("Authentication failed");
							}
						});
				}
				else{
					Debug.Log("GOOGLE PLAY AUTHENTICATION FAILED");
				}
			});
#endif
        }


    }
}