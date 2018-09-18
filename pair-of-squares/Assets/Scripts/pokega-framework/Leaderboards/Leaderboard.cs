using UnityEngine;
using System.Collections.Generic;

namespace Pokega{
	
	[System.Serializable]
	public class Leaderboard : MonoBehaviour {
		
		
		public string leaderboardName;
		//Sprajt koji je pozadina za custom leaderboard - ne koristi se zasad
		public UISprite background;
		//Ne koristi se
		public List<LeaderboardMember> leaders;
		//Id leaderborda za iOS i Android
		public string idIOS;
		public string idAND;

		//Ime skora koje se koristi za leaderboard
		public string scoreName;
	
		// Use this for initialization
		void Start () {
			
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}
}