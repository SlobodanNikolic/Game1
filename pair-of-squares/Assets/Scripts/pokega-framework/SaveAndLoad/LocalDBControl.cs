using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using Facebook.MiniJSON; 

namespace Pokega{

	public class LocalDBControl : MonoBehaviour 
	{
		//Facebook data
		string fbid;
		string fbEmail;
		string fbData;
		long fbLastSaved;

		//Score lists
		public List<Score> scores;
		public List<Score> bestScores;
		public List<Score> totalScores;
		
		//Time of the last responce
		public string lastResponse;

		//Device data
		public string deviceModel;
		public string deviceName;
		public string deviceType;
		public string deviceId;

		//Should be true on startup if you wish to reset the PlayerPrefs (for testing purposes)
		public bool resetPlayerPrefs;

		public string timeZone;

		//Shows if localDB tasks are done after startup
		public bool localDBReady;

		//A list of gameobjects that have the iLocal interface implemented. On save and load, the interface functions save and load (on the gameobjects that contain the interface) are called
		public List<GameObject> saveAndLoadScripts;
		
		void Awake() 
		{
			localDBReady = false;

			if(resetPlayerPrefs)
				PlayerPrefs.DeleteAll();

			timeZone = TimeZone.CurrentTimeZone.StandardName;			
		}
		
		void Start() 
		{

			//Sets the score arrays
			scores = App.score.scores;
			bestScores = App.score.bestScores;
			totalScores = App.score.totalScores;
			PlayerLoad();
			//App.server.Merge();
		}

		//Loads from gameobjects that have the iLocal interface
		public void LoadFromScripts(){
			foreach(GameObject obj in saveAndLoadScripts){
				ILocal interfaceAcces = obj.GetComponent(typeof(ILocal)) as ILocal;
				interfaceAcces.Load();
			}
				
		}

		//Saves from gameobjects that have the iLocal interface
		public void SaveFromScripts(){
			foreach(GameObject obj in saveAndLoadScripts){
				ILocal interfaceAcces = obj.GetComponent(typeof(ILocal)) as ILocal;
				interfaceAcces.Save();
			}
			
		}

		//Resets all data from gameobjects that have the iLocal interface
		public void ResetFromScripts(){
			foreach(GameObject obj in saveAndLoadScripts){
				ILocal interfaceAcces = obj.GetComponent(typeof(ILocal)) as ILocal;
				interfaceAcces.Reset();
			}
			
		}
		

		public void PlayerLoad () {
			PrintPP();

			//POKUSAJ UCITAVANJE STRINGA, PROVERA DA LI PlayerPrefs POSTOJI
			string loadedString = PlayerPrefs.GetString ("gameProgress");
			
			//IGRAC NIJE PRVI PUT U IGRI
			if (loadedString != "") {
				
				//UCITAVANJE PP U STRING PROMENLJIVE
				string uid = PlayerPrefs.GetString("uid");
				string gcid = PlayerPrefs.GetString("gcid");
				string apnToken = PlayerPrefs.GetString("apnToken");
				string username = PlayerPrefs.GetString("username");
				string password = PlayerPrefs.GetString("password");
				string diamondCount = PlayerPrefs.GetString("diamonds");
				string coinsCount = PlayerPrefs.GetString("coins");				

				//Ucitavanje ostalih podataka iz PlayerPrefs				
				string fbid = PlayerPrefs.GetString("fbid");
				string fbName = PlayerPrefs.GetString("fbName");
				string fbEmail = PlayerPrefs.GetString("fbEmail");
				string birthday = PlayerPrefs.GetString ("birthday");
				string gender = PlayerPrefs.GetString("gender");
				string lives = PlayerPrefs.GetString("lives");
				string lastLevel = PlayerPrefs.GetString("lastLevel");
				string lastCheckPoint = PlayerPrefs.GetString("lastCheckpoint");	
				string inventory = PlayerPrefs.GetString("Inventory");
				
				string sound = PlayerPrefs.GetString("sound");
				string music = PlayerPrefs.GetString("music");
				string colorBlindMode = PlayerPrefs.GetString("colorBlindMode");
				string recordGameplay = PlayerPrefs.GetString("recordGameplay");
				string notifications = PlayerPrefs.GetString("notifications");
				
				//UCITAVANJE SKOROVA
				for(int i = 0; i<bestScores.Count; i++){
					bestScores[i].score = PlayerPrefs.GetString(bestScores[i].scoreName);
					if(bestScores[i].score == "")
						bestScores[i].score = Crypting.EncryptInt(0);
				}

				for(int i = 0; i<totalScores.Count; i++){
					totalScores[i].score = PlayerPrefs.GetString(totalScores[i].scoreName);
					if(totalScores[i].score == "")
						totalScores[i].score = Crypting.EncryptInt(0);
				}
				
				//Ucitavanje shop JSONa
				string shop = PlayerPrefs.GetString("shop");	


				//OVO SE NE KORISTI, SAMO JE TU ZA TESTIRANJE
				//string prehash = secretKey + diamondCount + coinsCount;
				
				//for(int i=0; i<bestScores.Count;i++)
				//	prehash += bestScores[i].score;
				
				//string hashed = mdFiveSum(prehash);
			
				//PROVERA DA LI JE PETLJANO SA PLAYER PREFS
				//AKO JESTE, RESETUJ IGRACA - NE KORISTI SE
				if (false) {
					Debug.LogError("CHEATEEEEEEEEEEEEER! :)");
					//PlayerReset();
				}
				
				//AKO NIJE, UPISI SVE GDE TREBA
				else {

					//Svi podaci se upisuju gde treba
					//Podaci o playeru
					App.player.diamondCount = diamondCount;
					App.player.coinsCount = coinsCount;

					//Labele za koine se apdejtuju da prikazuju stvaran broj koina
					App.shop.UpdateCoinsLabels(Crypting.DecryptInt(coinsCount).ToString());
					
					App.player.uid = uid;

					App.sound.soundOn = bool.Parse(sound);
					App.sound.musicOn = bool.Parse(music);
					App.colorBlindMode = bool.Parse (colorBlindMode);
					App.recordGameplay = bool.Parse (recordGameplay);
					App.notif.notifications = notifications;

					//DESERIJALIZACIJA SHOP JSONA
					//Shop JSON se prebacuje u dictionary
				    App.shop.itemDictionary = Json.Deserialize(shop) as Dictionary<string, object>;
					if(shop == "null") App.shop.itemDictionary = new Dictionary<string, object>();
					
					App.player.lives = lives;
										
					//DESERIJALIZACIJA INVENTARA
					//Ako u inventaru nema niceg, napravi prazan dictionary za inventory i bag

					if(inventory == "null" || inventory == "") {						
						App.inv.inventory = new Dictionary<string, object>();
						App.inv.bag = new Dictionary<string, object>();
					}

					//Ako je nesto kupovano, JSON se deserijalizuje i puni se inventory
					else
                        App.inv.inventory = Json.Deserialize(inventory) as Dictionary<string, object>;
					
					//INICIJALIZUJE SE TORBA
					App.inv.InitializeBag();			
										
					//POSTAVLJAJU SE BEST SCOROVI
					App.score.SetBestScores(bestScores);
					App.score.SetTotalScores(totalScores);
					
					//Poziva se load za sve GameObject koji imaju iLocal interfejs
					LoadFromScripts();
					
				}

				localDBReady = true;

			}
			
			//AKO JE PRVI PUT U IGRI, KREIRAJ NALOG NA SERVERU I AZURIRAJ PLAYER PREFS
			else {
				
				//SVE SE POSTAVLJA NA INICIJALNE VREDNOSTI
				//string prehash2 = secretKey + App.player.diamondCount + App.player.coinsCount;

				//Shop je prazan u App.shop.itemDictionary
				string shop = Json.Serialize(App.shop.itemDictionary);						
				
				//for(int i=0; i<bestScores.Count;i++)
				//	prehash2 += bestScores[i].score;
				
				//string hashed2 = mdFiveSum(prehash2);
				
				PlayerPrefs.SetString("gameProgress", "Played");
				PlayerPrefs.SetString("deviceModel", deviceModel);
				PlayerPrefs.SetString("deviceType", deviceType);
				PlayerPrefs.SetString("deviceId", deviceId);
				PlayerPrefs.SetString("deviceName", deviceName);
				PlayerPrefs.SetString("diamonds", Crypting.EncryptInt(0).ToString());
				PlayerPrefs.SetString("coins", Crypting.EncryptInt(0).ToString());
				PlayerPrefs.SetString("timeZone", timeZone);
				PlayerPrefs.SetString("shop", shop);
				PlayerPrefs.SetString("lives", Crypting.EncryptInt(App.player.maxLives));				
				PlayerPrefs.SetString("lastLevel", Crypting.EncryptInt(0));
				PlayerPrefs.SetString("lastCheckpoint", Crypting.EncryptInt(0));

				PlayerPrefs.SetString("sound", "on");
				//App.sound.sound = "on";
				PlayerPrefs.SetString("music", "on");
				//App.sound.music = "on";
				PlayerPrefs.SetString("notifications", "on");
				
				//INVENTAR I TRANSAKCIJE SE POSTAVLJAJU NA INICIJALNE VREDNOSTI
				App.inv.inventory = new Dictionary<string, object>();
				PlayerPrefs.SetString("Inventory", "");
				App.inv.bag = new Dictionary<string, object>();
				App.shop.itemDictionary = new Dictionary<string, object>();
				PlayerPrefs.SetString("shop", "");
				
				App.player.diamondCount = Crypting.EncryptInt(0);		
				App.player.coinsCount = Crypting.EncryptInt(0);
				App.shop.UpdateCoinsLabels("0");

				//Svi objekti koji imaju iLocal interfejs zovu Reset funkciju, koja postavlja sve na 0
				ResetFromScripts();
				
				//Postavljaju se totalScores i beestScores na 0 u PlayerPrefs
				for(int i = 0; i<bestScores.Count; i++){	
					PlayerPrefs.SetString(bestScores[i].scoreName, bestScores[i].score);
				}
				for(int i = 0; i<totalScores.Count; i++){	
					PlayerPrefs.SetString(totalScores[i].scoreName, totalScores[i].score);
				}
								
				//PAMTI SE SVE U PP
				
				PlayerSave();

				//Svi iLocal interfejsi pozivaju funkciju Save()
				SaveFromScripts();				
				
			}

			//Provera da li dugmici za sound treba da su on ili off
		//	App.sound.CheckButtons ();
			
			//Zove se merge funkcija na serveru, koja sinhronizuje podatke izmedju servera i lokalne baze
			App.server.Merge();		
		}
		

		//Upisuju se podaci u PlayerPrefs
		public void PlayerSave () {
			
			//hashovanje bitnih podataka koje korisnik ne sme da menja u PlayerPrefs. Ako ih promeni, resetuje mu se game.
		//	string prehash = secretKey + App.player.diamondCount + App.player.coinsCount;
			
			//for(int i=0; i<bestScores.Count;i++)
			//	prehash += bestScores[i].score;
			
			//string hashed = mdFiveSum(prehash);
			//Debug.Log("GAME PROGRESS " + PlayerPrefs.GetString("gameProgress"));
			
			
			//POSTAVLJAJU SE STRINGOVI U PP
			PlayerPrefs.SetString("gameProgress", "Played");

			//Skorovi
			for (int i=0; i<bestScores.Count; i++)
				PlayerPrefs.SetString(bestScores[i].scoreName, bestScores[i].score);
			for (int i=0; i<totalScores.Count; i++)
				PlayerPrefs.SetString(totalScores[i].scoreName, totalScores[i].score);
			

			string shop = Json.Serialize(App.shop.itemDictionary);
			
			PlayerPrefs.SetString("shop", shop);
			PlayerPrefs.SetString("gcid", App.gc.gcId);

			#if UNITY_IOS
			PlayerPrefs.SetString("apnToken", App.device.apnToken);
			#endif

			PlayerPrefs.SetString("username", App.player.username);
			PlayerPrefs.SetString("password", App.player.password);
			
			PlayerPrefs.SetString("uid", App.player.uid);
			
			PlayerPrefs.SetString("deviceModel", deviceModel);
			PlayerPrefs.SetString("deviceName", deviceName);
			PlayerPrefs.SetString("deviceId", deviceId);			
			PlayerPrefs.SetString("deviceType", deviceType);
			
			PlayerPrefs.SetString("fbid", App.fb.fbid);
			PlayerPrefs.SetString("fbName", App.fb.fbUsername);
			PlayerPrefs.SetString("fbEmail", App.fb.fbEmail);
			PlayerPrefs.SetString("birthday", App.player.birthday);
			PlayerPrefs.SetString("gender", App.player.gender);
			
			PlayerPrefs.SetString("diamonds", App.player.diamondCount);
			PlayerPrefs.SetString("coins", App.player.coinsCount);
			
			PlayerPrefs.SetString("timeSaved", DateTime.Now.ToString());
			PlayerPrefs.SetString("timeZone", timeZone);
			PlayerPrefs.SetString("lives", App.player.lives);

			PlayerPrefs.SetString("sound", App.sound.soundOn.ToString());
			PlayerPrefs.SetString("music", App.sound.musicOn.ToString());
			PlayerPrefs.SetString("colorBlindMode", App.colorBlindMode.ToString());
			PlayerPrefs.SetString("recordGameplay", App.recordGameplay.ToString());
			PlayerPrefs.SetString("notifications", App.notif.notifications);

			
			//SASTAVLJA SE INVENTORY STRING, KOJI SE SASTOJI OD BAG-a I EQUIPPED ITEM-a
			App.inv.PutTogetherInventory();

			//Pamti se iz svih iLocal interfejsa
			SaveFromScripts();

			PlayerPrefs.SetString("Inventory", Json.Serialize(App.inv.inventory));
			
			PlayerPrefs.Save();

		}
		

		public void PlayerReset () {
			Debug.LogError ("<color=red> Reseting player prefs </color>");
			
			//SVE SE INICIJALIZUJE NA NULU
			App.score.InitializeBestScoresToZero();
			App.player.uid = "";
			App.player.password = "";
			App.player.username = "";
			App.gc.gcId = "";
			App.device.apnToken = "";
			App.fb.fbid = "";
			App.fb.fbUsername = "";
			App.fb.fbEmail = "";
			App.player.birthday = "";
			App.player.gender = "";

			App.sound.soundOn = true;
			App.sound.musicOn = true;
			App.colorBlindMode = false;
			App.recordGameplay = true;
			App.notif.notifications = "on";

			App.player.diamondCount = Crypting.EncryptInt(0);
			App.player.coinsCount = Crypting.EncryptInt(0);
			App.shop.itemDictionary.Clear();
			App.player.lives = Crypting.EncryptInt(App.player.maxLives);
			
			App.inv.inventory = new Dictionary<string, object>();

			ResetFromScripts();
			PlayerSave();
			
		}
		
		public void PrintPP(){
			//FUNKCIJA KOJA STAMPA PP
			Debug.Log("***************PLAYER PREFS**************" + Environment.NewLine);
			
			string uid = PlayerPrefs.GetString("uid");
			string gcid = PlayerPrefs.GetString("gcid");
			string apnToken = PlayerPrefs.GetString("apnToken");
			string username = PlayerPrefs.GetString("username");
			string password = PlayerPrefs.GetString("password");
			string diamondCount = PlayerPrefs.GetString("diamonds");
			string coinsCount = PlayerPrefs.GetString("coins");
			string fbid = PlayerPrefs.GetString("fbid");
			string fbName = PlayerPrefs.GetString("fbName");
			string fbEmail = PlayerPrefs.GetString("fbEmail");
			string birthday = PlayerPrefs.GetString ("birthday");
			string gender = PlayerPrefs.GetString("gender");
			string loadedString = PlayerPrefs.GetString ("gameProgress");
			string shop = Json.Serialize(App.shop.itemDictionary);
			string lives = PlayerPrefs.GetString("lives");
			string lastLevel = PlayerPrefs.GetString("lastLevel");
			string lastCheck = PlayerPrefs.GetString("lastCheckpoint");
			string inventory = PlayerPrefs.GetString("Inventory");
			
			for(int i=0; i<bestScores.Count; i++){
				Debug.Log("best score " + bestScores[i].scoreName + " " + PlayerPrefs.GetString(bestScores[i].scoreName));
			}
			for(int i=0; i<totalScores.Count; i++){
				Debug.Log("total score " + totalScores[i].scoreName + " " + PlayerPrefs.GetString(totalScores[i].scoreName));
			}
			
			Debug.Log("uid: " + uid + Environment.NewLine +"gcid: " + gcid + Environment.NewLine +"fbid: " + fbid + Environment.NewLine + "diamonds: " + diamondCount + "coins: " + coinsCount + Environment.NewLine +
			          "game progress: " + loadedString + " shop " + shop + " lives " + lives + " lastLevel" + lastLevel + " lastCheckpoint " + lastCheck);
		}
		
	
		//Funkcija koja pravi dictionary od  JSON stringa
		Dictionary<string, int> ParseDict (string input)
		{
			if(input == "")
				return new Dictionary<string, int >(0);
			Dictionary<string, object> dict = Json.Deserialize(input) as Dictionary<string, object>;
			Dictionary<string, int> retDict = new Dictionary<string, int>(dict.Count);
			foreach(KeyValuePair<string, object> kvp in dict)
			{
				string k = kvp.Key;
				string num = kvp.Value.ToString();
				int v = Int32.Parse(num);
				retDict.Add(k, v);
			}
			return retDict;
		}
		
		

		public static string mdFiveSum(string strToEncrypt){
			UTF8Encoding encoding = new System.Text.UTF8Encoding();
			byte[] bytes = encoding.GetBytes(strToEncrypt);
			
			// encrypt bytes
			MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
			byte[] hashBytes = md5.ComputeHash(bytes);
			
			// Convert the encrypted bytes back to a string (base 16)
			string hashString = "";
			for (long i = 0; i < hashBytes.Length; i++) {
				hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, "0"[0]);
			}
			
			return hashString.PadLeft(32, "0"[0]);
		}
	}
}