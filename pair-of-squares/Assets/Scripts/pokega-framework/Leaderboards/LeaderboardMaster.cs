using UnityEngine;
using System.Collections.Generic;

namespace Pokega{

	[System.Serializable]
	public class LeaderboardMaster : MonoBehaviour {
	
		public List<Leaderboard> leaderboards;
	
		void Awake(){
//			Debug.Log("<color=green>LEADERBOARD MASTER AWAKE</color>");
		}

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}
}