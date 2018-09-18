using UnityEngine;
using System.Collections;

namespace Pokega{
	
	public class App : MonoBehaviour {
		//After how many games a rate me popup shows
		public static string bundleVersion = "1.0";
		
		public static AdsControl ads;
		public static NotificationControl notif;
		public static LocalDBControl local;
		public static BestHttpControl server;
		public static ScoreControl score;
		public static GameCenterControl gc;
		public static FacebookControl fb;
		public static ConnectionChecker conn;
		public static LeaderboardMaster lb;
		public static AchievementMaster ach;
		public static ShopControl shop;
		public static Inventory inv;
		public static Device device;
		public static UIControl ui;
		public static Player player;
		public static AnalyticsControl analytics;
		public static GiftControl gift;
        //public static SoundControl sound;
        public static SoundManager sound;
		public static bool colorBlindMode=false;
		public static bool recordGameplay=true;
		public static AppRate appRate;
		//public static Tutorial tut;
		public static Game game;

		void Awake()
		{
			InitializeScripts();
		}

		void InitializeScripts()
		{
			analytics = GameObject.Find("AnalyticsControl").GetComponent<AnalyticsControl>();
			ads = GameObject.Find("Ads").GetComponent<AdsControl>();
			notif = GameObject.Find("Notifications").GetComponent<NotificationControl>();
			local = GameObject.Find("LocalDB").GetComponent<LocalDBControl>();
			server = GameObject.Find("ServerAPI").GetComponent<BestHttpControl>();
			score = GameObject.Find("ScoreControl").GetComponent<ScoreControl>();
			gc = GameObject.Find("GameCenter").GetComponent<GameCenterControl>();
			fb = GameObject.Find("Facebook").GetComponent<FacebookControl>();
			conn = GameObject.Find("ConnectionChecker").GetComponent<ConnectionChecker>();
			lb = GameObject.Find("LeaderboardMaster").GetComponent<LeaderboardMaster>();
			ach = GameObject.Find("AchievementMaster").GetComponent<AchievementMaster>();
			shop = GameObject.Find("Shop").GetComponent<ShopControl>();
			inv = GameObject.Find("Inventory").GetComponent<Inventory>();
			device = GameObject.Find("Device").GetComponent<Device>();
			ui = GameObject.Find("UIControl").GetComponent<UIControl>();
			player = GameObject.Find("Player").GetComponent<Player>();
			gift = GameObject.Find("GiftControl").GetComponent<GiftControl>();
            //sound = GameObject.Find ("SoundControl").GetComponent<SoundControl>();
            sound = GameObject.Find("SoundManager").GetComponent<SoundManager>();
			appRate = GameObject.Find("AppRate").GetComponent<AppRate>();
			//game = GameObject.Find("Game").GetComponent<Game>();

			if (GameObject.Find ("Tutorial")) {
			//	tut = GameObject.Find ("Tutorial").GetComponent<Tutorial> ();
			}

		}		

		void Start () 
		{
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = 60;
			Screen.sleepTimeout = 60;
		}
			
		public void Quit(){
			Application.Quit ();
		}

		public void NormalizeTime(){
			Time.timeScale = 1f;
		}
	}
}