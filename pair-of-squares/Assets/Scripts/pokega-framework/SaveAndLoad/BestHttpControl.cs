using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BestHTTP;


namespace Pokega{
	public class BestHttpControl : MonoBehaviour {
	
		//Ne koristi se
		public GameObject connecting;
		public GameObject connected;
		public GameObject notConnected;

		//Referenca na App.cs, ne koristi se
		public App aplication;

		public string status = "first_check";
		
		public string lastResponse;
		
		public int timesWaited;

		//Svi parametri koji se pamte/loaduju, isto kao i u LocalDBControl.cs
		public string gameProgress;
		public string gcid;
		public string apnToken;
		public string username;
		public string password;
		public string fbid;
		public string fbName;
		public string fbEmail;
		public string birthday;
		public string gender;
		public string deviceModel;
		public string deviceName;
		public string deviceId;
		public string deviceType;
		public string timeSaved;
		public string coins;
		public string diamonds;
		public string uid;
		public string timesPlayedBeforeRate;
		public string lives;
		public string shop;
		public string lastLevel;
		public string lastCheckpoint;

		//Bool koji govori da li se prvi put salje rikvest ka serveru
		public bool firstCheck = true;
		
		//Url na kome se nalazi Api sa kojim komuniciramo
		public string apiUrl;

		public List<Score> bestScores = new List<Score>();

		//Akcije koje se pozivaju u slucaju da su razliciti podaci na serveru i u lokalu. Ovo se desava kad korisnik nije imao internet neko vreme, ili je igrao na drugom uredjaju.
		//Pojavljuje se prozor koji pita da li korisnik zeli da ucita podatke sa servera, ili da ucita sa uredjaja.
		public UIAction progressDialogOn;
		public UIAction progressDialogOff;

		//Pomocni dictionary
		public Dictionary<string, object> mapa;

		//Labele vezane za prozor koji se otvara ako podaci nisu isti na serveru i na uredjaju
		public UILabel deviceProgressLabel;
		public UILabel serverProgressLabel;
				

		// Use this for initialization
		void Start () {
			//Setuje pomocnu listu best skorova na nulu
			for(int i = 0; i<App.score.scores.Count; i++)
				bestScores.Add(new Score(App.score.bestScores[i].scoreName, 0f, false));
		}
		
		// Update is called once per frame
		void Update () {
		
		}


		//Merge
		public void Merge(){

			//Salje se rikvest za user sync na server
			HTTPRequest request = new HTTPRequest(new System.Uri(apiUrl+"user/sync"), HTTPMethods.Post, OnMergeFinished);
			
			uid = PlayerPrefs.GetString("uid");
			
			//U rikvest se dodaje polje koje govori koja je funkcija u pitanju. To se posle proverava na serveru
			request.AddField("func", "merge");

			//U slucaju da je korisnik zeleo da linkuje novi fejsbuk nalog sa profilom, dodaje se polje
			if(App.fb.linkNewFacebook) request.AddField("createNew", "yes");
			else request.AddField("createNew", "no");				
				
			//UZIMAJU SE PODACI IZ LOCALDB, odnosno PlayerPrefs i dodaju u rikvest
			request.AddField("timesPlayedBeforeRate", PlayerPrefs.GetInt("timesPlayedBeforeRate").ToString());
			request.AddField("fbid", App.fb.fbid);
			request.AddField("uid", PlayerPrefs.GetString("uid"));
			request.AddField("gameProgress", PlayerPrefs.GetString("gameProgress"));
			request.AddField("gcid", PlayerPrefs.GetString("gcid"));
			request.AddField("apnToken", PlayerPrefs.GetString("apnToken"));
			request.AddField("username", PlayerPrefs.GetString("username"));
			request.AddField("password", PlayerPrefs.GetString("password"));
			request.AddField("fbName", PlayerPrefs.GetString("fbName"));
			request.AddField("fbEmail", PlayerPrefs.GetString("fbEmail"));
			request.AddField("birthday", PlayerPrefs.GetString("birthday"));
			request.AddField("gender", PlayerPrefs.GetString("gender"));
			request.AddField("deviceModel", PlayerPrefs.GetString("deviceModel"));
			request.AddField("deviceName", PlayerPrefs.GetString("deviceName"));
			request.AddField("deviceId", PlayerPrefs.GetString("deviceId"));
			request.AddField("deviceType", PlayerPrefs.GetString("deviceType"));
			request.AddField("timeSaved", PlayerPrefs.GetString("timeSaved"));
			request.AddField("coins", PlayerPrefs.GetString("coins"));
			request.AddField("diamonds", PlayerPrefs.GetString("diamonds"));
			request.AddField("timeZone", PlayerPrefs.GetString("timeZone"));
			request.AddField("shop", PlayerPrefs.GetString("shop"));
			//request.AddField("lastLevel", PlayerPrefs.GetString("lastLevel"));
			request.AddField("lastCheckpoint", PlayerPrefs.GetString("lastCheckpoint"));
			
			//UZIMAJU SE BEST SCOROVI IZ SCORE CONTROLE
			for(int i=0; i<App.score.bestScores.Count; i++)
				request.AddField(App.score.bestScores[i].scoreName, PlayerPrefs.GetString(App.score.bestScores[i].scoreName));

			request.Send();

		}

		public void OnMergeFinished(HTTPRequest request, HTTPResponse response){

			//Ako je dobijen response
			if(response != null){
				
				//Obradi response
				mapa = BestHTTP.JSON.Json.Decode(response.DataAsText) as Dictionary<string, object>;
				
				if(mapa != null){
					
					fbid = mapa["fbid"].ToString();
					uid = mapa["_id"].ToString();
					//User id koji nam je server poslao se odmah upisuje u PlayerPrefs
					PlayerPrefs.SetString("uid", uid);
					
					PlayerPrefs.Save();

					App.player.uid = uid;
					
					//Puni se pomocna lista best skorova
					for(int i = 0; i<App.score.scores.Count; i++){
						bestScores[i].score = mapa[App.score.scores[i].scoreName].ToString();
					}
					
					//Uzimaju se podaci iz rikvesta
					timesPlayedBeforeRate = mapa["timesPlayedBeforeRate"].ToString();
					gcid = mapa["gcid"].ToString();
					apnToken = mapa["apnToken"].ToString();
					username = mapa["username"].ToString();
					password = mapa["password"].ToString();
					fbName = mapa["fbName"].ToString();
					fbEmail = mapa["fbEmail"].ToString();
					birthday = mapa["birthday"].ToString();
					gender = mapa["gender"].ToString();
					deviceName = mapa["deviceName"].ToString();
					deviceModel = mapa["deviceModel"].ToString();
					deviceType = mapa["deviceType"].ToString();
					deviceId = mapa["deviceId"].ToString();
					timeSaved = mapa["timeSaved"].ToString();
					coins = mapa["coins"].ToString();
					diamonds = mapa["diamonds"].ToString();
					shop = mapa["shop"].ToString();
					//lastLevel = mapa["lastLevel"].ToString();
					lastCheckpoint = mapa["lastCheckpoint"].ToString();
					gameProgress = mapa["gameProgress"].ToString();
					
					//Funkcija koja proverava da li je veci progres na serveru ili u lokalu
					CheckProgress();

				}
				else{
					ContinueGame();
				}	

			}
			else if(request.Exception != null){
				ContinueGame();
			}
			else{
				App.player.lives = Crypting.EncryptInt(3);
				ContinueGame();

			}

		}

		public void ContinueGame(){
			//App.player.lives = Crypting.EncryptInt(3);
			//InitializationHelper.stage.lastLevel = Crypting.EncryptInt(0);
			if(firstCheck){
				if ((PlayerPrefs.GetString ("tutorialPlayed", "false") == "false")) {
					Time.timeScale = 1f;
					App.ui.UnlockUI ();
					//Ovo firstcheck pitanje dal treba ovde
					firstCheck = false;
					//if(App.tut)
					//	App.tut.StartTutorial ();

				} else {
					Time.timeScale = 1f;
					GoToHomeScreen ();
					firstCheck = false;
				}
			}
		}

		//Save
		public void Save(){

			//Salje se rikvest
			HTTPRequest request = new HTTPRequest(new System.Uri(apiUrl+"user/sync"), HTTPMethods.Post, OnSaveFinished);
		
			//Polje koje oznacava koja funkcija treba da se izvrsi na serveru
			request.AddField("func", "save");
		
			if(App.fb.linkNewFacebook) request.AddField("createNew", "yes");
				else request.AddField("createNew", "no");
			
			request.AddField("timesPlayedBeforeRate", PlayerPrefs.GetInt("timesPlayedBeforeRate").ToString());
			request.AddField("fbid", App.fb.fbid);
			request.AddField("uid", PlayerPrefs.GetString("uid"));
			request.AddField("gameProgress", PlayerPrefs.GetString("gameProgress"));
			request.AddField("gcid", PlayerPrefs.GetString("gcid"));
			request.AddField("apnToken", PlayerPrefs.GetString("apnToken"));
			request.AddField("username", PlayerPrefs.GetString("username"));
			request.AddField("password", PlayerPrefs.GetString("password"));
			request.AddField("fbName", PlayerPrefs.GetString("fbName"));
			request.AddField("fbEmail", PlayerPrefs.GetString("fbEmail"));
			request.AddField("birthday", PlayerPrefs.GetString("birthday"));
			request.AddField("gender", PlayerPrefs.GetString("gender"));
			request.AddField("deviceModel", PlayerPrefs.GetString("deviceModel"));
			request.AddField("deviceName", PlayerPrefs.GetString("deviceName"));
			request.AddField("deviceId", PlayerPrefs.GetString("deviceId"));
			request.AddField("deviceType", PlayerPrefs.GetString("deviceType"));
			request.AddField("timeSaved", PlayerPrefs.GetString("timeSaved"));
			request.AddField("coins", PlayerPrefs.GetString("coins"));
			request.AddField("diamonds", PlayerPrefs.GetString("diamonds"));
			request.AddField("timeZone", PlayerPrefs.GetString("timeZone"));
			request.AddField("shop", PlayerPrefs.GetString("shop"));
			//request.AddField("lastLevel", PlayerPrefs.GetString("lastLevel"));
			request.AddField("lastCheckpoint", PlayerPrefs.GetString("lastCheckpoint"));

			for(int i=0; i<bestScores.Count; i++)
				request.AddField(bestScores[i].scoreName, PlayerPrefs.GetString(bestScores[i].scoreName));	

			request.Send();

		}

		public void OnSaveFinished(HTTPRequest request, HTTPResponse response){
			
			if(response != null){
				//Obradi responce
				Debug.Log("<color=red>BEST HTTP CONTROL  - Save Callback, Saved to Server</color>");
				Debug.Log(App.score.bestScores[0].scoreName + " " + App.score.bestScores[0].score);
			}
			else if(request.Exception != null){
				Debug.Log("SAVE EXCEPTION");
			}

		}

		//Load
		public void Load(){

			//Debug.Log("<color=red> Load</color>");

			HTTPRequest request = new HTTPRequest(new System.Uri(apiUrl+"user/sync"), HTTPMethods.Post, OnLoadFinished);

			request.AddField("func", "load");
		
			if(App.fb.linkNewFacebook) request.AddField("createNew", "yes");
				else request.AddField("createNew", "no");
			
			request.AddField("timesPlayedBeforeRate", PlayerPrefs.GetInt("timesPlayedBeforeRate").ToString());
			request.AddField("fbid", App.fb.fbid);
			request.AddField("uid", PlayerPrefs.GetString("uid"));
			request.AddField("gameProgress", PlayerPrefs.GetString("gameProgress"));
			request.AddField("gcid", PlayerPrefs.GetString("gcid"));
			request.AddField("apnToken", PlayerPrefs.GetString("apnToken"));
			request.AddField("username", PlayerPrefs.GetString("username"));
			request.AddField("password", PlayerPrefs.GetString("password"));
			request.AddField("fbName", PlayerPrefs.GetString("fbName"));
			request.AddField("fbEmail", PlayerPrefs.GetString("fbEmail"));
			request.AddField("birthday", PlayerPrefs.GetString("birthday"));
			request.AddField("gender", PlayerPrefs.GetString("gender"));
			request.AddField("deviceModel", PlayerPrefs.GetString("deviceModel"));
			request.AddField("deviceName", PlayerPrefs.GetString("deviceName"));
			request.AddField("deviceId", PlayerPrefs.GetString("deviceId"));
			request.AddField("deviceType", PlayerPrefs.GetString("deviceType"));
			request.AddField("timeSaved", PlayerPrefs.GetString("timeSaved"));
			request.AddField("coins", PlayerPrefs.GetString("coins"));
			request.AddField("diamonds", PlayerPrefs.GetString("diamonds"));
			request.AddField("timeZone", PlayerPrefs.GetString("timeZone"));
			request.AddField("shop", PlayerPrefs.GetString("shop"));
			//request.AddField("lastLevel", PlayerPrefs.GetString("lastLevel"));
			request.AddField("lastCheckpoint", PlayerPrefs.GetString("lastCheckpoint"));

			for(int i=0; i<bestScores.Count; i++)
				request.AddField(bestScores[i].scoreName, PlayerPrefs.GetString(bestScores[i].scoreName));	


			request.Send();

		}

		public void OnLoadFinished(HTTPRequest request, HTTPResponse response){

			//Debug.Log("<color=red>Load Callback, PP Loaded</color>");

			if(response != null){
				//Obradi responce

				mapa = BestHTTP.JSON.Json.Decode(response.DataAsText) as Dictionary<string, object>;
				
				if(mapa != null){
					
					//Sve sa servera se ucitava u lokalne promenljive

					fbid = mapa["fbid"].ToString();
					uid = mapa["_id"].ToString();
					Debug.Log("UID from Merge callback is " + uid);
					PlayerPrefs.SetString("uid", uid);
	
					for(int i = 0; i<App.score.scores.Count; i++){
						bestScores[i].score = mapa[App.score.scores[i].scoreName].ToString();
					}

					timesPlayedBeforeRate = mapa["timesPlayedBeforeRate"].ToString();
					gcid = mapa["gcid"].ToString();
					apnToken = mapa["apnToken"].ToString();
					username = mapa["username"].ToString();
					password = mapa["password"].ToString();
					fbName = mapa["fbName"].ToString();
					fbEmail = mapa["fbEmail"].ToString();
					birthday = mapa["birthday"].ToString();
					gender = mapa["gender"].ToString();
					deviceName = mapa["deviceName"].ToString();
					deviceModel = mapa["deviceModel"].ToString();
					deviceType = mapa["deviceType"].ToString();
					deviceId = mapa["deviceId"].ToString();
					timeSaved = mapa["timeSaved"].ToString();
					coins = mapa["coins"].ToString();
					diamonds = mapa["diamonds"].ToString();
					shop = mapa["shop"].ToString();
					//lastLevel = mapa["lastLevel"].ToString();
					lastCheckpoint = mapa["lastCheckpoint"].ToString();
					gameProgress = mapa["gameProgress"].ToString();
	
					
					//SVE SE PAMTI U PP
					PlayerPrefs.SetString("uid", uid);
					PlayerPrefs.SetString("gameProgress", gameProgress);
					//Debug.Log(gameProgress);
					PlayerPrefs.SetString("gcid", gcid);
					PlayerPrefs.SetString("apnToken", apnToken);
					PlayerPrefs.SetString("username", username);
					PlayerPrefs.SetString("password", password);
					PlayerPrefs.SetString("fbid", fbid);
					PlayerPrefs.SetString("fbName", fbName);
					PlayerPrefs.SetString("fbEmail", fbEmail);
					PlayerPrefs.SetString("birthday", birthday);
					PlayerPrefs.SetString("gender", gender);
					PlayerPrefs.SetString("deviceModel", deviceModel);
					PlayerPrefs.SetString("deviceName", deviceName);
					PlayerPrefs.SetString("deviceId", deviceId);
					PlayerPrefs.SetString("deviceType", deviceType);
					PlayerPrefs.SetString("timeSaved", timeSaved);
					PlayerPrefs.SetString("shop", shop);
				
					PlayerPrefs.SetString("coins", coins.ToString());
					PlayerPrefs.SetString("diamonds", diamonds.ToString());
					PlayerPrefs.SetString("timesPlayedBeforeRate", timesPlayedBeforeRate);
					//PlayerPrefs.SetString("lastLevel", lastLevel);
					PlayerPrefs.SetString("lastCheckpoint", lastCheckpoint);
				
					for(int i = 0; i<bestScores.Count; i++)
						PlayerPrefs.SetString(bestScores[i].scoreName, bestScores[i].score);
					
					PlayerPrefs.Save();
					
					//Debug.Log("GAME PROGRESS " + PlayerPrefs.GetString("gameProgress"));
					
					//NAKON STO JE GOTOVO UCITAVANJE SA SERVERA, ZOVE SE LOAD KOJI UCITAVA PP U GAME
					App.local.PlayerLoad();
				}


			}
			else if(request.Exception != null){
				Debug.Log("Load EXCEPTION");
			}

		}

		public void SavePurchase(Transaction t){
			//FUNKCIJA ZA PAMCENJE TRANSAKCIJE
			
			HTTPRequest request = new HTTPRequest(new System.Uri(apiUrl+"user/sync"), HTTPMethods.Post, OnSavePurchaseFinished);			

			request.AddField("func", "shop");
			request.AddField("productId", t.productId);
			request.AddField("dateAndTime", t.dateAndTime);
			request.AddField("isOneTime", t.isOneTime.ToString());
			request.AddField("userId", PlayerPrefs.GetString("uid"));
			request.AddField("diamondsLeft", t.diamondCount);
			request.AddField("coinsLeft", t.coinsCount);
			request.AddField("productName", t.productName);	

			request.Send();	

		}
		
		public void OnSavePurchaseFinished(HTTPRequest request, HTTPResponse response){
			if(response != null){
				Debug.Log("Save purchase Finished");
			}
			else if(request.Exception != null){
				Debug.Log("Save Purchase EXCEPTION");
			}
		}

		public void CheckProgress(){
			//Debug.Log("Checking progress");

			//UZIMAJU SE PODACI IZ PP, KOJI SU BITNI ZA ODREDJIVANJE PROGRESA
			int prefsScore, prefsCoins, prefsDiamonds;

			prefsCoins = Crypting.DecryptInt(PlayerPrefs.GetString("coins"));

			prefsDiamonds = Crypting.DecryptInt(PlayerPrefs.GetString("diamonds"));
			//Debug.Log("Checking progress2");

			for(int i = 0; i < bestScores.Count; i++){
				prefsScore = Crypting.DecryptInt(PlayerPrefs.GetString(bestScores[i].scoreName));

				if(bestScores[i] == null || coins == null || diamonds == null){
					if(bestScores[i]==null) Debug.Log("BEST SCORES ARE NULL, GOING TO SAVE");
					if(coins == null) Debug.Log("COINS ARE NULL");
					if(diamonds == null) Debug.Log("DIAMONDS ARE NULL");
					Save();
					GoToHomeScreen();
					return;
				}
					
					//Ako se vrednosti na serveru i u lokalu za skorove ne podudaraju, otvara se prozor koji pita korisnika koje zeli da koristi
					if(Crypting.DecryptInt(bestScores[i].score) != prefsScore){
						ChooseProgress ();
				
						return;
					}
			
			}
			//COINS SU RAZLICITI NA SERVERU I LOCALDB
			
			if(Crypting.DecryptInt(coins) != prefsCoins){
				ChooseProgress ();
			
				return;
			}
			//DIAMONDS SU RAZLICITI NA SERVERU I LOCALDB
			if(Crypting.DecryptInt(diamonds) != prefsDiamonds){
				ChooseProgress ();
			
				return;		
			}

			Time.timeScale = 1f;
			GoToHomeScreen();
		}

		public void ChooseProgress(){
			//Postavlja se tekst u labele na prozoru za choose progress
			deviceProgressLabel.text = "Device " + PlayerPrefs.GetString ("timeSaved");
			serverProgressLabel.text = "Server " + timeSaved;

			//Otvara se choose progress prozor
			App.ui.SetPopUp("UI Choose Progress");
		}

		public void closeMergeDialog(){
			App.ui.SetPopUp("UI Choose Progress", true);
		}


		public void GoToHomeScreen(){
			//POSTAVLJA STANJE APLIKACIJE NA HOME
			//HAK ZA DOZETOV BAG, UNLOCK UI, ALI NE MOZE OVAKO, MORA TO LEPO DA SE RESI
			App.ui.UnlockUI ();
			App.ui.SetScreen("UI Home");
			//App.game.Home ();
		}
		
	
	}
}