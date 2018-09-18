using UnityEngine;
using System.Collections;

namespace Pokega{

	public class PleaseLogIn : MonoBehaviour {

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public void WarnUser(){
			if (PlayerPrefs.GetString ("userWarnedToLogin", "false") == "false" && PlayerPrefs.GetString("fbid") == "") {
				PlayerPrefs.SetString ("userWarnedToLogin", "true");
				App.ui.SetPopUp ("UI Warn to login");
			}
		}

		public void CloseWarning(){
			App.ui.SetPopUp ("UI Warn to login", true);

		}
	}
}