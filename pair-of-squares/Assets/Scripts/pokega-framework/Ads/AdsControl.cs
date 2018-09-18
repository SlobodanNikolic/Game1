using UnityEngine;
using System.Collections;


namespace Pokega {

	public class AdsControl : MonoBehaviour 
	{

		public bool freeVersion;
		
		public string ironsrcAppKeyIOS;
		public string ironsrcAppKeyAND;

		public string userId;

		public int showVideoAfterXGameOvers;
		public int showInterAfterXGameOvers;


		public EventDelegate[] videoRewardMethods;
		public EventDelegate[] interstitialRewardMethods;

		int gamesCount;

		public GameObject earnButton;
		public GameObject videoForLifeButton;

		void Awake()
		{
			if (!freeVersion)
				return;
			
			gamesCount = 0;
			userId = SystemInfo.deviceUniqueIdentifier;

            
			IronSource.Agent.setUserId(userId);

		}

		void Start() 
		{
			if (!freeVersion)
				return;

			#if UNITY_IOS
				IronSource.Agent.init (ironsrcAppKeyIOS);
			#endif

			#if UNITY_ANDROID
				IronSource.Agent.init (ironsrcAppKeyAND);
			#endif

			//interstitial events
			IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
			IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;        
			IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent; 
			IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent; 
			IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
			IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
			IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;
			//reward events
			IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
			IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent; 
			IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
			IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
			IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
			IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent; 
			IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;

			IronSource.Agent.validateIntegration();
            
		}

        

		void OnApplicationPause(bool isPaused) 
		{   
			if (!freeVersion)
				return;
			
			IronSource.Agent.onApplicationPause(isPaused);
		}

		public void CheckRewardButtons(){
            //edit stefan
            /*if (IsRewardedVideoAvailable ()) {
				Debug.Log ("Rewarded video available");
				earnButton.SetActive (true);
				if (videoForLifeButton != null)
					videoForLifeButton.SetActive (true);
			} else {
				earnButton.SetActive (false);
				if (videoForLifeButton != null)
					videoForLifeButton.SetActive (false);
				Debug.Log ("Rewarded video not available");

			}*/
		}

		public void ShowRewardedVideo()
		{
			if (!freeVersion)
				return;
			
			Debug.Log("SHOW REW VIDEO");

			if (IronSource.Agent.isRewardedVideoAvailable ()) 
			{
				IronSource.Agent.showRewardedVideo();
			}
		
		}

		public void LoadInterstitial()
		{
			if (!freeVersion)
				return;
			
			Debug.Log("LOAD INTER");
			IronSource.Agent.loadInterstitial();
		}

		public void ShowInterstitial()
		{
			if (!freeVersion)
				return;
			
			Debug.Log("SHOW INTER");

			if (IronSource.Agent.isInterstitialReady ()) 
			{
				IronSource.Agent.showInterstitial();
			}


		}

		public bool isInterstitialReady()
		{
			if (!freeVersion)
				return false;
			
			return IronSource.Agent.isInterstitialReady ();
		}

		public bool isRewardedVideoAvailable()
		{
			if (!freeVersion)
				return false;
			
			return IronSource.Agent.isRewardedVideoAvailable ();
		}



		public void ShowOfferwall()
		{

            /*
			bool available = Supersonic.Agent.isOfferwallAvailable();
			if(available)
			{
				Supersonic.Agent.showOfferwall();
			}
            */
		}

		public void GameOverEvent()
		{
			gamesCount++;

			if((showInterAfterXGameOvers!=0) && (gamesCount % showInterAfterXGameOvers == 0))
				ShowInterstitial();
		}


		//========================================================================================================



		private void RequestInterstitial()
		{
			LoadInterstitial ();
		}


		void VideoEndEvent()
		{
			Debug.Log("Video end event");

		}

		void ISShowSuccess()
		{
			foreach(EventDelegate ed in interstitialRewardMethods)
			{
				ed.Execute();
			}
		}

		void OfferwallClosedEvent()
		{
			Debug.Log("Offerwall closed!");
			//Debug.Log(Supersonic.Agent.getOfferwallCredits());

		}



		void InterstitialAdLoadFailedEvent (IronSourceError error) {
		}
		//Invoked right before the Interstitial screen is about to open.
		void InterstitialAdShowSucceededEvent() {
		}
		//Invoked when the ad fails to show.
		//@param description - string - contains information about the failure.
		void InterstitialAdShowFailedEvent(IronSourceError error) {
		}
		// Invoked when end user clicked on the interstitial ad
		void InterstitialAdClickedEvent () {
		}
		//Invoked when the interstitial ad closed and the user goes back to the application screen.
		void InterstitialAdClosedEvent () {
		}
		//Invoked when the Interstitial is Ready to shown after load function is called
		void InterstitialAdReadyEvent() {
		}
		//Invoked when the Interstitial Ad Unit has opened
		void InterstitialAdOpenedEvent() {
		}


		//Invoked when the RewardedVideo ad view has opened.
		//Your Activity will lose focus. Please avoid performing heavy 
		//tasks till the video ad will be closed.
		void RewardedVideoAdOpenedEvent() {
		}  

		//Invoked when the RewardedVideo ad view is about to be closed.
		//Your activity will now regain its focus.
		void RewardedVideoAdClosedEvent() {
		}
		//Invoked when there is a change in the ad availability status.
		//@param - available - value will change to true when rewarded videos are available. 
		//You can then show the video by calling showRewardedVideo().
		//Value will change to false when no videos are available.
		void RewardedVideoAvailabilityChangedEvent(bool available) {
			//Change the in-app 'Traffic Driver' state according to availability.
			bool rewardedVideoAvailability = available;
		}
		//Invoked when the video ad starts playing.
		void RewardedVideoAdStartedEvent() {
		}
		//Invoked when the video ad finishes playing.
		void RewardedVideoAdEndedEvent() {
		}
		//Invoked when the user completed the video and should be rewarded. 
		//If using server-to-server callbacks you may ignore this events and wait for 
		//the callback from the ironSource server.
		//@param - placement - placement object which contains the reward data
		void RewardedVideoAdRewardedEvent(IronSourcePlacement placement) {
		}
		//Invoked when the Rewarded Video failed to show
		//@param description - string - contains information about the failure.
		void RewardedVideoAdShowFailedEvent (IronSourceError error){
		}


	}
}