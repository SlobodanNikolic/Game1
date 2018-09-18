#if UNITY_IPHONE || UNITY_IOS
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System;

public class iOSAgent : IronSourceIAgent
{
	[DllImport("__Internal")]
	private static extern void CFReportAppStarted ();

	[DllImport("__Internal")]
	private static extern void CFSetPluginData (string pluginType, string pluginVersion, string pluginFrameworkVersion);

	[DllImport("__Internal")]
	private static extern void CFSetAge (int age);

	[DllImport("__Internal")]
	private static extern void CFSetGender (string gender);

	[DllImport("__Internal")]
	private static extern void CFSetMediationSegment (string segment);

	[DllImport("__Internal")]
	private static extern string CFGetAdvertiserId ();
	
	[DllImport("__Internal")]
	private static extern void CFValidateIntegration ();
	
	[DllImport("__Internal")]
	private static extern void CFShouldTrackNetworkState (bool track);

	[DllImport("__Internal")]
	private static extern bool CFSetDynamicUserId (string dynamicUserId);

	//******************* SDK Init *******************//

	[DllImport("__Internal")]
	private static extern void CFSetUserId (string userId);

	[DllImport("__Internal")]
	private static extern void CFInit (string appKey);

	[DllImport("__Internal")]
	private static extern void CFInitWithAdUnits (string appKey, params string[] adUnits);

	//******************* RewardedVideo API *******************//

	[DllImport("__Internal")]
	private static extern void CFShowRewardedVideo ();

	[DllImport("__Internal")]
	private static extern void CFShowRewardedVideoWithPlacementName (string placementName);

	[DllImport("__Internal")]
	private static extern bool CFIsRewardedVideoAvailable ();

	[DllImport("__Internal")]
	private static extern bool CFIsRewardedVideoPlacementCapped (string placementName);

	[DllImport("__Internal")]
	private static extern string CFGetPlacementInfo (string placementName);

	//******************* Interstitial API *******************//

	[DllImport("__Internal")]
	private static extern void CFLoadInterstitial ();

	[DllImport("__Internal")]
	private static extern void CFShowInterstitial ();

	[DllImport("__Internal")]
	private static extern void CFShowInterstitialWithPlacementName (string placementName);

	[DllImport("__Internal")]
	private static extern bool CFIsInterstitialReady ();

	[DllImport("__Internal")]
	private static extern bool CFIsInterstitialPlacementCapped (string placementName);

	//******************* Offerwall API *******************//

	[DllImport("__Internal")]
	private static extern void CFShowOfferwall ();

	[DllImport("__Internal")]
	private static extern void CFShowOfferwallWithPlacementName (string placementName);

	[DllImport("__Internal")]
	private static extern void CFGetOfferwallCredits ();

	[DllImport("__Internal")]
	private static extern bool CFIsOfferwallAvailable ();



	public iOSAgent ()
	{	
	}

	#region IronSourceIAgent implementation
	public void reportAppStarted ()
	{
		CFReportAppStarted ();
	}

	//******************* Base API *******************//

	public void onApplicationPause (bool pause)
	{
	}

	public void setAge (int age)
	{
		CFSetAge (age);
	}
	
	public void setGender (string gender)
	{
		CFSetGender (gender);
	}

	public void setMediationSegment (string segment)
	{
		CFSetMediationSegment (segment);
	}

	public string getAdvertiserId ()
	{
		return CFGetAdvertiserId ();
	}
	
	public void validateIntegration ()
	{
		CFValidateIntegration ();
	}
	
	public void shouldTrackNetworkState (bool track)
	{
		CFShouldTrackNetworkState (track);
	}

	public bool setDynamicUserId (string dynamicUserId)
	{
		return CFSetDynamicUserId (dynamicUserId);
	}

	//******************* SDK Init *******************//

	public void setUserId (string userId)
	{
		CFSetUserId (userId);
	}

	public void init (string appKey) 
	{
		CFSetPluginData ("Unity", IronSource.pluginVersion(), IronSource.unityVersion());
		CFInit (appKey);
	}

	public void init (string appKey, params string[] adUnits)
	{
		CFSetPluginData ("Unity", IronSource.pluginVersion(), IronSource.unityVersion());
		CFInitWithAdUnits (appKey, adUnits);
	}

	//******************* RewardedVideo API *******************//
	
	public void showRewardedVideo ()
	{
		CFShowRewardedVideo ();
	}

	public void showRewardedVideo (string placementName)
	{
		CFShowRewardedVideoWithPlacementName (placementName);
	}

	public bool isRewardedVideoAvailable ()
	{
		return CFIsRewardedVideoAvailable ();
	}

	public bool isRewardedVideoPlacementCapped (string placementName)
	{
		return CFIsRewardedVideoPlacementCapped (placementName);
	}

	public IronSourcePlacement getPlacementInfo (string placementName)
	{
		IronSourcePlacement sp = null;

		string spString = CFGetPlacementInfo (placementName);
		if (spString != null) {
			Dictionary<string,object> spDic = IronSourceJSON.Json.Deserialize (spString) as Dictionary<string,object>;
			string pName = spDic ["placement_name"].ToString ();
			string rewardName = spDic ["reward_name"].ToString ();
			int rewardAmount = Convert.ToInt32 (spDic ["reward_amount"].ToString ());
			sp = new IronSourcePlacement (pName, rewardName, rewardAmount);
		}

		return sp;
	}

	//******************* Interstitial API *******************//

	public void loadInterstitial ()
	{
		CFLoadInterstitial ();
	}
	
	public void showInterstitial ()
	{
		CFShowInterstitial ();
	}

	public void showInterstitial (string placementName)
	{
		CFShowInterstitialWithPlacementName (placementName);
	}

	public bool isInterstitialReady ()
	{
		return CFIsInterstitialReady ();
	}

	public bool isInterstitialPlacementCapped (string placementName)
	{
		return CFIsInterstitialPlacementCapped (placementName);
	}

	//******************* Offerwall API *******************//

	public void showOfferwall ()
	{
		CFShowOfferwall ();
	}

	public void showOfferwall (string placementName)
	{
		CFShowOfferwallWithPlacementName (placementName);
	}

	public void getOfferwallCredits ()
	{
		CFGetOfferwallCredits ();		
	}

	public bool isOfferwallAvailable ()
	{
		return CFIsOfferwallAvailable ();
	}
	#endregion
}
#endif
