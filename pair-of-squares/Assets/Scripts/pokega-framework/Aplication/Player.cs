using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pokega{
	public class Player : MonoBehaviour {
		
		//Svi podaci vezani za igraca

		//User id, koji dobijamo od servera
		public string uid;
		//GameCenter id za iOS
		public string gcid;
		public string gcUsername;

		public string diamondCount;
		public string coinsCount;
		
		public string lives;
		
		public UILabel livesLabel;	
		public UILabel gameOverLabel;

		//Labele za coins i diamonds se ne koriste
		public UILabel[] coinLabels;
		public UILabel[] diamondLabels;

		public int maxLives = 3;
		
		public string username;
		public string password;
		public string birthday;
		public string gender;
		
		
		void Awake()
		{
			//Inicijalizacija na nulu			
			diamondCount = Crypting.EncryptInt(0);
			coinsCount = Crypting.EncryptInt(0);
			lives = Crypting.EncryptInt(maxLives);
		}


		void Update()
		{
			
			//PRIKAZUJE SE BROJ COINA
			//mozda ovo ne treba da bude u update-u nego da se poziva updateTreasureLabels nakon kupovine
			foreach(UILabel l in coinLabels)
				l.text = Crypting.DecryptInt(coinsCount).ToString();
			
		}	

		public void SetLives(int amount){
			lives = Crypting.EncryptInt(amount);
		}

		public int GetLives(){
			return Crypting.DecryptInt(lives);
		}

		public void LivesPlus(int amount){
			lives = Crypting.EncryptInt(Crypting.DecryptInt(lives) + amount);
		}

		public void LivesMinus(int amount){
			lives = Crypting.EncryptInt(Crypting.DecryptInt(lives) - amount);
			App.local.PlayerSave();

		}

		public void CoinsPlus(int amount){
			coinsCount = Crypting.EncryptInt (GetCoins () + amount);
		}

		public void CoinsMinus(int amount){
			if ((GetCoins () - amount) < 0) {
				coinsCount = Crypting.EncryptInt (0);
			}
			else
				coinsCount = Crypting.EncryptInt (GetCoins () - amount);
		}

		public int GetCoins(){
			return Crypting.DecryptInt (coinsCount);
		}
	
	}
}