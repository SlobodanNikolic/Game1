using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Pokega
{

    public class Game : MonoBehaviour
    {

		public string appStoreURL;
		public string playStoreURL;
		public string pokegaURL;
        public float splashTime;
		public UIButton tutorialBtn;
		public UIButton shareReplayBtn;
		public GameObject shareImg;
		public GameObject sharePanel;
		public UILabel shareMsgTxt;
		public UILabel shareScoreTxt;
		public SpriteRenderer shareBG;
		public UIButton homeShareBtn;
		public UIButton gameOverShareBtn;
		public string androidShareUp;
		public string androidShareDown;
		public string iosShareUp;
		public string iosShareDown;

		private Color shareTextColor1;
		private Color shareTextColor2;

        public static Game instance;
        public static bool buttonsEnabled;

        void Start()
        {
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(this);

            buttonsEnabled = true;

			shareTextColor1 = shareScoreTxt.color;
			shareTextColor2 = shareMsgTxt.color;
            //Invoke("SplashToHome", splashTime);

			#if UNITY_IOS
				homeShareBtn.normalSprite=iosShareUp;
				gameOverShareBtn.normalSprite=iosShareUp;
				homeShareBtn.pressedSprite=iosShareDown;
				gameOverShareBtn.pressedSprite=iosShareDown;
			#endif

			#if UNITY_ANDROID
				homeShareBtn.normalSprite=androidShareUp;
				gameOverShareBtn.normalSprite=androidShareUp;
				homeShareBtn.pressedSprite=androidShareDown;
				gameOverShareBtn.pressedSprite=androidShareDown;
			#endif

			App.analytics.CreateAnalyticEvent ("App started");

			shareReplayBtn.isEnabled = App.recordGameplay;
        }

        public void SplashToHome() {
            App.ui.SetScreen("UI Home");
        }

        public void Home()
        {
            if (!buttonsEnabled)
                return;

            StartCoroutine(Transition.instance.StartTransition("TitleScreen", "UI Home"));
        }

		public void OpenSettings()
        {
			
			App.ui.SetPopUp("UI Settings");
			App.analytics.CreateAnalyticEvent ("Settings button clicked");
            //TitleScreenManager.instance.OpenSettings();
        }

		public void CloseSettings()
		{
			App.ui.SetPopUp("UI Settings",true);

			//TitleScreenManager.instance.OpenSettings();
		}

		public void OpenNewGameConfirm()
		{
			App.ui.SetPopUp("UI NewGameConfirm");

		}

		public void CloseNewGameConfirm()
		{
			App.ui.SetPopUp("UI NewGameConfirm",true);
		}

		public void OpenQuitGameConfirm()
		{
			App.ui.SetPopUp("UI QuitGameConfirm");
		}

		public void CloseQuitGameConfirm()
		{
			App.ui.SetPopUp("UI QuitGameConfirm",true);
		}

		public void OpenHelp()
		{
			App.ui.SetPopUp("UI Help");
			tutorialBtn.isEnabled = SaveAndLoadState.SaveFileExists ();
			App.analytics.CreateAnalyticEvent ("Help button clicked");
		}

		public void CloseHelp()
		{
			App.ui.SetPopUp("UI Help",true);
		}

        public void GameOver()
        {
            if (!buttonsEnabled)
                return;

            StartCoroutine(Transition.instance.StartTransition("GameOverScreen", "UI GameOver"));
			//App.ads.ShowRewardedVideo ();
			shareReplayBtn.isEnabled = App.recordGameplay;
			App.analytics.CreateAnalyticEvent ("Score", GameManager.score);
			App.analytics.CreateAnalyticEvent ("Duration", Mathf.Round(GameManager.duration));
			App.analytics.CreateAnalyticEvent ("Level", GameManager.level);
        }

        public void StartNewGame() 
		{

            if (!buttonsEnabled)
                return;

			if (RecordControl.IsRecording())
				RecordControl.StopRecording();
			
            TitleScreenManager.newGame = true;

			if (!Tutorial.instance.showTutorial)
				Tutorial.instance.showTutorial = !SaveAndLoadState.SaveFileExists ();
			
            StartCoroutine(Transition.instance.StartTransition("GameScreen","UI Game"));

            if (GameManager.instance!=null)
                GameManager.instance.pause = false;

			//App.ads.LoadInterstitial ();


        }

		public void NewGame()
		{
			if (SaveAndLoadState.loadable)
				OpenNewGameConfirm ();
			else
				StartNewGame ();
		}

        public void ContinueGame()
        {
            if (!buttonsEnabled)
                return;

            TitleScreenManager.newGame = false;
			Tutorial.instance.showTutorial = false;
            StartCoroutine(Transition.instance.StartTransition("GameScreen", "UI Game"));

            if (GameManager.instance != null)
                GameManager.instance.pause = false;

			//App.ads.LoadInterstitial ();
        }

        public void PauseGame()
        {
            if (!buttonsEnabled)
                return;

			if (LineTrace.lineIsActive) 
			{
				StartCoroutine (WaitAndPause ());
				return;
			}

            StartCoroutine(Transition.instance.StartTransition("TitleScreen", "UI Home"));
            GameManager.instance.PauseGame();
        }
			

		public void OpenTutorial()
		{
			if (!buttonsEnabled)
				return;
			
			Tutorial.instance.showTutorial = true;
			Tutorial.instance.closeAtTheEnd = true;
			StartNewGame ();
			App.analytics.CreateAnalyticEvent ("Replay tutorial button clicked");
		}

		private IEnumerator WaitAndQuit()
		{
			yield return new WaitForEndOfFrame();
			Application.Quit();
		}

		public void QuitGame()
		{
			System.Diagnostics.Process.GetCurrentProcess().Kill();
			//StartCoroutine (WaitAndQuit ());
		} 

		public void OpenPokegaURL()
		{
			Application.OpenURL (pokegaURL);
		}

		#if UNITY_ANDROID
		void Update()
		{
			
			
			if (Input.GetKeyDown(KeyCode.Escape)) 
			{ 
				if (!buttonsEnabled)
					return;
				
				if (App.ui.currentScreen == "UI Home") 
				{
					OpenQuitGameConfirm ();
				}

				if (App.ui.currentScreen == "UI Game") 
				{
					PauseGame ();
				}

				if (App.ui.currentScreen == "UI GameOver") 
				{
					Home ();
				}
			}

		}
		#endif

		public void ShowReplay()
		{
			RecordControl.PlayLastRecording ();
		}

        public void Share()
        {
			App.analytics.CreateAnalyticEvent ("Share button clicked");
			shareMsgTxt.color=shareTextColor1;
			shareScoreTxt.color=shareTextColor2;
			shareMsgTxt.text="Join the fun!";
			shareScoreTxt.text="";
			#if UNITY_IOS
				StartCoroutine(GrabAndShare("Check out Pair of Squares on the App Store."));
			#endif

			#if UNITY_ANDROID
				StartCoroutine(GrabAndShare("Check out Pair of Squares on the Play Store."));
			#endif
        }

		public void ShareScore()
		{
			App.analytics.CreateAnalyticEvent ("Share button clicked");

			//Application.CaptureScreenshot ("myScore.png");
			shareMsgTxt.color=shareTextColor2;
			shareScoreTxt.color=shareTextColor1;
			shareMsgTxt.text="Can you beat that?";
			shareScoreTxt.text="I scored " + GameManager.score;
			StartCoroutine(GrabAndShare("I scored "+ GameManager.score + " points in Pair of Squares!"));
		}

		private IEnumerator GrabAndShare(string text)
		{
			
			ScreenGrab.camera.gameObject.SetActive (true);
			shareImg.SetActive (true);
			sharePanel.SetActive (true);

			Vector3 point00 = ScreenGrab.camera.WorldToScreenPoint(shareImg.transform.position-(shareBG.bounds.size/2f)); 
			Vector3 point11 = ScreenGrab.camera.WorldToScreenPoint(shareImg.transform.position+(shareBG.bounds.size/2f)); 
			int rectX = Mathf.RoundToInt (point00.x);
			int rectY = Mathf.RoundToInt (point00.y);
			int rectW = Mathf.RoundToInt (point11.x-point00.x);
			int rectH = Mathf.RoundToInt (point11.y-point00.y);
			ScreenGrab.instance.GrabRect(rectX,rectY,rectW,rectH);
			yield return null;

			#if UNITY_IOS
				text+=" " +appStoreURL;
				IOSSocialManager.Instance.ShareMedia(text, ScreenGrab.lastGrabTexture);
			#endif

			#if UNITY_ANDROID
				text+=" " + playStoreURL;
				AndroidShare.Share(text, ScreenGrab.filePath, "", "");
			#endif

			ScreenGrab.camera.gameObject.SetActive (false);
			shareImg.SetActive(false);
			sharePanel.SetActive (false);

		}

		private IEnumerator WaitAndPause()
		{
			while (LineTrace.lineIsActive) 
			{
				yield return null;
			}
			yield return new WaitForSeconds (0.1f);

			PauseGame ();
		}

    }
}