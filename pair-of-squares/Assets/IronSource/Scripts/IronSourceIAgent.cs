
public interface IronSourceIAgent
{
	void reportAppStarted ();

	//******************* Base API *******************//

	void onApplicationPause (bool pause);

	void setAge (int age);

	void setGender (string gender);

	void setMediationSegment (string segment);

	string getAdvertiserId ();

	void validateIntegration ();

	void shouldTrackNetworkState (bool track);

	bool setDynamicUserId (string dynamicUserId);

	//******************* SDK Init *******************//

	void setUserId (string userId);

	void init (string appKey);

	void init (string appKey, params string[] adUnits);

	//******************* RewardedVideo API *******************//

	void showRewardedVideo ();

	void showRewardedVideo (string placementName);

	bool isRewardedVideoAvailable ();
	
	bool isRewardedVideoPlacementCapped (string placementName);

	IronSourcePlacement getPlacementInfo (string name);
		
	//******************* Interstitial API *******************//

	void loadInterstitial ();

	void showInterstitial ();

	void showInterstitial (string placementName);

	bool isInterstitialReady ();

	bool isInterstitialPlacementCapped (string placementName);

	//******************* Offerwall API *******************//

	void showOfferwall ();

	void showOfferwall (string placementName);

	bool isOfferwallAvailable ();

	void getOfferwallCredits ();
}

public static class IronSourceAdUnits
{
	public static string REWARDED_VIDEO { get { return "rewardedvideo"; } }

	public static string INTERSTITIAL { get { return "interstitial"; } }

	public static string OFFERWALL { get { return "offerwall"; } } 
}
