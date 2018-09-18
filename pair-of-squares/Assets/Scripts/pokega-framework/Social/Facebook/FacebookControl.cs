using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Facebook.MiniJSON;
using Facebook.Unity;
//using System.Threading.Tasks;

namespace Pokega{

	public class FacebookControl : MonoBehaviour {
		
		
		bool isInit = false;			
		string lastResponse = "";	
		float recheckPeriod;			
		bool alreadyParsed = false;		//sluzi da se ne pozove parsiranje vise puta zaredom, FB.Login zna da pozove LoginCallback vise puta
		//----------------------------------
		
		//UI Objects
		public GameObject loginButton;
		public GameObject logoutButton;
		public GameObject shareButton;
		public UITexture avatar;
		public GameObject avatarParent;
		public UILabel usernameLabel;
		public GameObject friendsScroll;
		public GameObject friendsGridObject;
		UIGrid friendsGrid;
		
		//UI Containers references
		List<GameObject> instanciatedContainers = new List<GameObject>();
		List<string> friendsIds = new List<string>();
		int containerSetupDoneCounter = 0;
		List<FacebookFriend> fbFriends = new List<FacebookFriend>();
		public GameObject fbFriendContainer;

		public string fbid = "";
		public string fbUsername = "";
		public string fbEmail = "";
		public string birthday = "";
		public string gender = "";
		string fbData = "";
		int numberOfFbFriends;
		int numberOfScoresSet = 0;
		
		public Dictionary<string, object> friendsScores;	
		
		public bool linkNewFacebook=false;
		string serverApiFuncToCall = "";
		string just_logged_fbid = "HEJHEJ";	
		
		void Awake() 
		{
			//stefan edit
			//friendsGrid = friendsGridObject.GetComponent<UIGrid>();
			
			//we disable all fb UI components
			//stefan edit
			/*
			loginButton.SetActive(false);
			logoutButton.SetActive(false);
			shareButton.SetActive(false);
			avatar.gameObject.SetActive(false);
			friendsScroll.SetActive(false);
			usernameLabel.gameObject.SetActive(false);
			*/
		}
		
		void Start () 
		{
			FB.Init(OnInitComplete);
		}


		public void AppInvite()
		{
			FB.Mobile.AppInvite(
				new Uri("https://fb.me/810530068992919"),
				new Uri("http://www.spaceanswers.com/wp-content/uploads/2013/07/300x219xAngara7_Launch_HR.jpg.pagespeed.ic.0L5oh9hWj8.jpg"),
				AppInviteCallback
			);
		}

		void AppInviteCallback(IAppInviteResult result)
		{
			Debug.LogError(result.RawResult);
		}

		void OnInitComplete()
		{
			Debug.Log("Facebook Init Complete");
			FacebookCheck();
		}
		
		void OnApplicationPause(bool pauseStatus) 
		{
			if(!pauseStatus)
			{
//				Debug.Log("Checking facebook data when returning from background");
				foreach(GameObject go in instanciatedContainers)
					Destroy (go);
				instanciatedContainers.Clear ();
				friendsIds.Clear();
				containerSetupDoneCounter = 0;
				FacebookCheck();
			}
		}
				
		void FacebookCheck()
		{
			if(!FB.IsInitialized)
			{
				Invoke ("FacebookCheck", 1.0f);
				return;
			}
			if(FB.IsLoggedIn)
			{
				if(!App.conn.isConnected())
				{
					//we got access token, but no internet connection, so we load basic info for player from PP
					loginButton.SetActive(false);
					logoutButton.SetActive(true);
					shareButton.SetActive(true);
					avatar.mainTexture = Base64ToTexture(PlayerPrefs.GetString("profilePicture"));
					avatar.gameObject.SetActive(true);
					avatarParent.SetActive(true);
					friendsScroll.SetActive(false);
					usernameLabel.gameObject.SetActive(true);
					usernameLabel.text = PlayerPrefs.GetString("fbName");
					
					fbid = PlayerPrefs.GetString ("fbid");
					fbEmail = PlayerPrefs.GetString("email");
					fbUsername = PlayerPrefs.GetString("fbName");
				}
				else
				{
					//we got internet connection, got access token, if its expired - login, else - load data
					if(AccessToken.CurrentAccessToken.ExpirationTime > DateTime.Now)
					{
						//logged in, token not expired
						LoadData();
					}
					else
					{
						//token expired, has internet, login again!
						Login();
					}
				}
			}
			else
			{
				//set default facebook UI

				//stefan edit
				/*
				loginButton.SetActive(true);
				logoutButton.SetActive(false);
				shareButton.SetActive(false);
				avatar.gameObject.SetActive(false);
				avatarParent.SetActive(false);
				friendsScroll.SetActive(false);
				usernameLabel.gameObject.SetActive(false);
				*/
			}
		}

#region login/out		

		public void Login() 
		{
			if(!FB.IsInitialized)
				return;
			if(App.conn.isConnected())
				FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email", "user_friends" }, LoginCallback);
			else
			{
				//user cannot log into fb without internet connection
				//UIControl.NoInternetConnection();
			}
		}
		
		void LoginCallback(IResult result) 
		{
			if (result.Error != null || !FB.IsLoggedIn)
			{
				lastResponse = "Error Response or Login cancelled by Player\n" + result.Error;
				
				loginButton.SetActive(true);
				logoutButton.SetActive(false);
				shareButton.SetActive(false);
				avatar.gameObject.SetActive(false);
				avatarParent.SetActive(false);
				friendsScroll.SetActive(false);
				usernameLabel.gameObject.SetActive(false);
			}
			else if(FB.IsLoggedIn)
			{				
				Debug.Log("User logged in!");
				fbid = AccessToken.CurrentAccessToken.UserId;
				
				if(App.device.deviceType == Device.DeviceType.UNKNOWN)
					FB.API("me?fields=id", HttpMethod.GET, GetID);
				else
					CheckFbid();
			}
		}
		
		void GetID(IResult result)
		{
			object value;
			Dictionary<string,object> dict = Json.Deserialize(result.RawResult) as Dictionary<string,object>;
			
			if (dict.TryGetValue("id", out value)) {
				just_logged_fbid = value.ToString();
				Debug.Log("fbid ZA EDITOR: " + just_logged_fbid);
			}
			
			CheckFbid();
		}

		public void Logout()
		{
			if(App.conn.isConnected())
			{
				FB.LogOut();
				LogoutCallBack();
			}
			else
			{
				//App.ui.SetScreen(UIControl.UIScreens.NO_CONNECTION);
			}
		}

		void LogoutCallBack()
		{
			FacebookCheck();
			
			foreach(GameObject go in instanciatedContainers)
				Destroy (go);
			instanciatedContainers.Clear ();
			friendsIds.Clear();
			containerSetupDoneCounter = 0;
		}
		
#endregion login/out			
		
#region server_comunication
		
		
		//naredna funkcija se zove od strane ParseUserData() koja je pozvana nakon uspesnog logovanja korisnika
		void CheckFbid() {
			
			Debug.Log("CheckFBID Function");
			Debug.Log("AT fbid: " + AccessToken.CurrentAccessToken.UserId);
			Debug.Log("PP fbid: " + PlayerPrefs.GetString("fbid"));
			Debug.Log("FB fbid: " + fbid);
			
			//Ako je ponovo ulogovan isti korisnik automatski radi merge!
			//ZA EDITOR (NE ULAZI OVDE AKO JE IPHONE)
			if(just_logged_fbid == PlayerPrefs.GetString("fbid")) 
			{
				Debug.Log("Same user as before, do merge (FOR EDITOR)");
				fbid = AccessToken.CurrentAccessToken.UserId;
				
				//da li je ovde false
				linkNewFacebook = false;
				LoadData();
				serverApiFuncToCall = "merge";
			}
			//ZA DEVICE, NE ULAZI OVDE AKO JE EDITOR
			else if(AccessToken.CurrentAccessToken.UserId == PlayerPrefs.GetString("fbid")) 
			{
				Debug.Log("Same user as before, do merge (FOR DEVICE)");
				fbid = AccessToken.CurrentAccessToken.UserId;
				
				
				//da li je ovde false
				linkNewFacebook = false;
				LoadData();
				serverApiFuncToCall = "merge";
			}
			
			//Ako je @FALSE, onda linkuj, To znaci da je prvi put u igri
			else if(PlayerPrefs.GetString("fbid") == "")
			{
				Debug.Log("Linking this device with Faceobok. Better scores will be saved and used");
				
				//da li je ovde true
				linkNewFacebook = true;
				LoadData();
				serverApiFuncToCall = "merge";
				PlayerPrefs.SetString("fbid", fbid);
			}
			else
			{
				Debug.Log("Warning the user");
				Debug.Log("Another facebook account was used on this device. Do you want to unlink the old one, and link this device with the new facebook acount?"); 
				App.ui.SetPopUp("UI Facebook Connect");
			}
		}

		//attach-ovana funkcija na button Yes /UI Facebook Connect
		public void SetLinkTrue()
		{
			App.local.PlayerReset();
			linkNewFacebook = true;
			App.ui.SetPopUp("UI Facebook Connect", true);
			LoadData();
			
			Debug.Log("Linking new Facebook account with this device");			
			serverApiFuncToCall = "load";
		}
		
		//attach-ovana funkcija na button No /UI Facebook Connect
		public void SetLinkFalse()
		{
			Logout();
			linkNewFacebook = false;
			App.ui.SetPopUp("UI Facebook Connect", true);
			Debug.Log("User dont want to link new fb account. Logout. Progress in PP and player info stay the same.");
			App.server.Merge();
		}
		
#endregion server_comunication
	
#region parsing_data

		void LoadData() 
		{
			Debug.Log("Getting Facebook Data!!!");
			
			lastResponse += "getting user data\n\n";
			
			FB.API("me?fields=id,email,installed,name,friends.fields(id,picture,name)"
			, HttpMethod.GET, ParseUserData);
			
			FB.API("me?fields=picture", HttpMethod.GET, ParseUserImage);
		}
		
		void ParseUserData(IResult result) 
		{
			Debug.Log("parsing user data:" + result.RawResult);
			
			//Zbog moguceg visestrukog pozivanja ove funkcije 
			if(alreadyParsed)	return;
			else {alreadyParsed = true;	Invoke("ResetAlreadyParsed", 5.0f);	}
			
			if (result.Error == null) {
				if (result.RawResult != "") {
								
					object value;
					Dictionary<string,object> dict = Json.Deserialize(result.RawResult) as Dictionary<string,object>;
					
					if (dict.TryGetValue("id", out value)) {
						fbid = value.ToString();
						Debug.Log("fbid: " + fbid);
					}
					if (dict.TryGetValue("name", out value)) {
						fbUsername = value.ToString();
						Debug.Log("fb username:" + fbUsername);
					}
					if (dict.TryGetValue("email", out value)) {                                           
						fbEmail = value.ToString();
						Debug.Log("fbEmail: " + fbEmail);
					}
					
						
						//TODO: get time from internet biach
					var fbFriends = new List<Dictionary<string,string>>();
					var friends = new List<object>();
					int i = 0;
						
					//Citanje prijatelja i dodavanje u listu fbFriends
					if (dict.TryGetValue ("friends", out value)) 
					{
						friends = (List<object>)(((Dictionary<string, object>)value)["data"]);
						if (friends.Count > 0) 
						{
							numberOfFbFriends = friends.Count;
							Debug.Log("No. of fb friends:"+friends.Count.ToString ());					
							List<string> fbFriendsIds = new List<string>();

							foreach(Dictionary<string,object> friendDict in friends) 
							{
								string id = "";
								string name = "";
								string picture = "";
								
								if (friendDict.TryGetValue("id", out value)) id = value.ToString();
								if (friendDict.TryGetValue("name", out value)) name = value.ToString();
								if (friendDict.TryGetValue("birthday", out value)) birthday = value.ToString();
								if (friendDict.TryGetValue("gender", out value)) gender = value.ToString();
								if (friendDict.TryGetValue("picture", out value)) 
								{
									Dictionary<string,object> valuePicture = (Dictionary<string, object>)value;
									if (valuePicture.TryGetValue("data", out value)) 
									{
										Dictionary<string,object> valueData = (Dictionary<string, object>)value;
										if (valueData.TryGetValue("url", out value))
											picture = value.ToString();
									}
								}
								
								if (id != "") {
									var friend = new Dictionary<string,string>();
									friend["id"] = id;
									friend["name"] = name;
									//field picture je obican link
									friend["picture"] = picture;
									fbFriends.Add(friend);
									i++;

									lastResponse += "id: "+friend["id"]+"\nname: "+friend["name"]+"\npicture: "+friend["picture"]+"\n\n\n";
								}
								//Sledi instanciranje 
								InstanciateFriendContainer(id, name, picture);
								fbFriendsIds.Add(id);
							}	
//							App.server.GetFbFriendsScores(fbFriendsIds);
						}
					}
					
					dict["friends"] = fbFriends;
					fbData = Json.Serialize(dict);
				}
				//nema greske ali je prazan result.text
				else 
				{
					
				}
			}
			
			PlayerPrefs.SetString("fbid", fbid);
			PlayerPrefs.SetString("email", fbEmail);
			PlayerPrefs.SetString("fbName", fbUsername);
			App.local.PlayerSave();
			
			if(serverApiFuncToCall == "merge")
				App.server.Merge();
			else if(serverApiFuncToCall == "load")
				App.server.Load ();
			else
				Debug.Log ("Not calling any PokegaServer API action after load data!");
			//restartovanje pomocne promenljive serverApiFuncToCall
			serverApiFuncToCall = "";
			
			
			//----------------------------------------
			usernameLabel.text = fbUsername;
			usernameLabel.gameObject.SetActive(true);
			loginButton.SetActive(false);
			logoutButton.SetActive(true);
			shareButton.SetActive(true);
			if(numberOfFbFriends!=0)
			{
				Debug.LogError("Moving scroll down");
				friendsScroll.transform.position = new Vector3(friendsScroll.transform.position.x, friendsScroll.transform.position.y-1500, friendsScroll.transform.position.z);
			}
			friendsScroll.SetActive(true);
			
			
			//----------------------------------------
		}
	
		void ParseUserImage(IResult result) {
			if (result.Error == null) 
			{
				object value;
				Dictionary<string,object> dict = Json.Deserialize(result.RawResult) as Dictionary<string,object>;
				if(dict.TryGetValue("picture", out value))
				{
					var dict2 = (Dictionary<string, object>)value;
					if(dict2.TryGetValue("data", out value))
					{
						var dict3 = (Dictionary<string, object>) value;
						if(dict3.TryGetValue("url", out value))
						{
							string url = value.ToString();
							StartCoroutine(SetProfilePictureFromUrl(url));
						}
					}
				}
			}
		}
		
#endregion parsing_data
			
		private void InstanciateFriendContainer(string id, string name, string picture)
		{
			Debug.LogError("Instantiate id: " + name);
			GameObject helper = fbFriendContainer;
			FacebookFriend helperScript = helper.GetComponent<FacebookFriend>();
			helperScript.id = id;
			helperScript.username = name;
			helperScript.picture = picture;
			friendsIds.Add(id);
			GameObject instantiated = Instantiate(helper, Vector3.zero, Quaternion.identity) as GameObject;
			instanciatedContainers.Add(instantiated);
			FacebookFriend fbf = instantiated.GetComponent<FacebookFriend>();
			fbFriends.Add(fbf);
			instantiated.transform.SetParent(friendsGridObject.transform);
			instantiated.transform.localScale = Vector3.zero;
			instantiated.transform.localPosition = new Vector3(0.0f, 600.0f, 1.0f);
		} 
		
		//called by every container after he done basic setup	
		public void ContBasicSetupDone()
		{
			containerSetupDoneCounter++;
			//gets in after all are set-up
			if(containerSetupDoneCounter == numberOfFbFriends)
			{
				Debug.LogError("All containers basic setup is done. " + containerSetupDoneCounter + " ?= " + numberOfFbFriends);
				foreach(GameObject go in instanciatedContainers)
					go.transform.localScale = Vector3.one;
				//stefan edit
				//friendsGrid.enabled = true;
				containerSetupDoneCounter = 0;
				//TODO: obrisati narednu invoke kao i helpericu
				Invoke ("Helperica", 1.0f);
			}
		}
		
		//called after all containers basic setup is done
		//this function manualy call function to manualy set scores for each fb friend in grid
		void Helperica()
		{
			for(int i=0; i<numberOfFbFriends; i++)
			{
				Score sk = new Score("aloalo", i, false);
				List<Score> skores = new List<Score>();
				skores.Add(sk);
				SetFriendsScores(fbFriends[i].id, skores );
			}
		}
		
		//this function sets manualy results to all fb friends
		//after its set for every friend 
		public void SetFriendsScores(string id, List<Score> scores)
		{
			numberOfScoresSet++;
			Debug.LogError("Set Friend Scores:" + numberOfScoresSet);
			/*foreach(FacebookFriend fbf in fbFriends)
			{
				if(fbf.id == id)
				{
					fbf.SetScore(scores);
				}
			}*/
			if(numberOfScoresSet == numberOfFbFriends)
			{
				numberOfScoresSet = 0;
				Debug.Log("Same number of fb friends and pulled scores from Pokega server. Perfect.");
				if(numberOfFbFriends!=0)
				{
					friendsScroll.SetActive(true);
					Debug.LogError("Set scroll active");
				}
				//now after enable of gameobject grid (friends grid), users gonna custom sort by desired scores
				//invoke next lines for 0.5 seconds, so grid can sort elements
				//on the end of sort call setfriendscontainers function
				Invoke ("SetAvatarForFirstFriends", 1.0f);
			}
		}
		
		void SetAvatarForFirstFriends()
		{
			Debug.LogError("Setting avatar for the first friends.");
			//stefan edit
			//List<Transform> containers = friendsGrid.GetChildList();
			int numberOfContainersToGetAvatar = numberOfFbFriends;
			if(numberOfContainersToGetAvatar > 10)
				numberOfContainersToGetAvatar = 10;
			//stefan edit
			//for(int i=0; i<numberOfContainersToGetAvatar ; i++)
				//containers[i].gameObject.GetComponent<FacebookFriend>().SetAvatar();

			//translate scroll to sart position
			Invoke ("ReturnScroll", 0.5f);
		}
		
		void ReturnScroll()
		{
			Debug.LogError("Returning scroll up");
			friendsScroll.transform.position = new Vector3(friendsScroll.transform.position.x, friendsScroll.transform.position.y+1500, friendsScroll.transform.position.z);
		}
		
		
		
#region little_help_functions	
				
		void ResetAlreadyParsed()
		{
			alreadyParsed = false;
		}
		
		IEnumerator SetProfilePictureFromUrl (string url) 
		{
			WWW www = new WWW(url);
			yield return www;
			avatar.mainTexture = www.texture;
			avatar.gameObject.SetActive(true);
			avatarParent.SetActive(true);
			PlayerPrefs.SetString("profilePicture", TextureToBase64(www.texture));
		}
		
		public static string TextureToBase64 (Texture2D tex)
		{
			byte[] imageBytes = tex.EncodeToPNG();
			string base64String = Convert.ToBase64String(imageBytes);
			return base64String;
		}
		
		public static Texture2D Base64ToTexture (string base64String)
		{
			byte[] imageBytes = Convert.FromBase64String(base64String);
			Texture2D tex = new Texture2D(50,50);
			tex.LoadImage(imageBytes);
			return tex;
		}

#endregion little_help_functions
	}
}