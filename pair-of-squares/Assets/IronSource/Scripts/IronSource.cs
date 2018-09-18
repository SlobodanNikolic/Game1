﻿using UnityEngine;
using System.Collections;

public class IronSource : IronSourceIAgent
{
	private IronSourceIAgent _platformAgent ;
	private static IronSource _instance;
	private const string UNITY_PLUGIN_VERSION = "6.5.1";
	public const string GENDER_MALE = "male";
	public const string GENDER_FEMALE = "female";
	public const string GENDER_UNKNOWN = "unknown";

	private IronSource ()
	{
		#if UNITY_EDITOR 
		_platformAgent = new UnsupportedPlatformAgent();	
		#elif (UNITY_IPHONE || UNITY_IOS) 
		_platformAgent = new iOSAgent();		
		#elif UNITY_ANDROID
		_platformAgent = new AndroidAgent ();

		#endif
	}

	#region IronSourceIAgent implementation
	public static IronSource Agent {
		get {
			if (_instance == null) {
				_instance = new IronSource ();
			}
			return _instance;
		}
	}

	public static string pluginVersion ()
	{
		return UNITY_PLUGIN_VERSION;
	}

	public static string unityVersion ()
	{
		return Application.unityVersion;
	}

	public void reportAppStarted ()
	{
		_platformAgent.reportAppStarted ();	
	}

	//******************* Base API *******************//

	public void onApplicationPause (bool pause)
	{
		_platformAgent.onApplicationPause (pause);
	}
	
	public void setAge (int age)
	{
		_platformAgent.setAge (age);
	}
	
	public void setGender (string gender)
	{
		if (gender.Equals (GENDER_MALE))
			_platformAgent.setGender (GENDER_MALE);
		else if (gender.Equals (GENDER_FEMALE))
			_platformAgent.setGender (GENDER_FEMALE);
		else if (gender.Equals (GENDER_UNKNOWN))
			_platformAgent.setGender (GENDER_UNKNOWN);
	}

	public void setMediationSegment (string segment)
	{
		_platformAgent.setMediationSegment (segment);
	}

	public string getAdvertiserId ()
	{
		return _platformAgent.getAdvertiserId ();
	}
	
	public void validateIntegration ()
	{
		_platformAgent.validateIntegration ();
	}
	
	public void shouldTrackNetworkState (bool track)
	{
		_platformAgent.shouldTrackNetworkState (track);
	}

	public bool setDynamicUserId (string dynamicUserId)
	{
		return _platformAgent.setDynamicUserId (dynamicUserId);
	}

	//******************* SDK Init *******************//

	public void setUserId (string userId)
	{
		_platformAgent.setUserId (userId);
	}

	public void init (string appKey)
	{
		_platformAgent.init (appKey);
	}

	public void init (string appKey, params string[] adUnits)
	{
		_platformAgent.init (appKey, adUnits);
	}

	//******************* RewardedVideo API *******************//
	
	public void showRewardedVideo ()
	{
		_platformAgent.showRewardedVideo ();
	}

	public void showRewardedVideo (string placementName)
	{
		_platformAgent.showRewardedVideo (placementName);
	}

	public IronSourcePlacement getPlacementInfo (string placementName)
	{
		return _platformAgent.getPlacementInfo (placementName);
	}

	public bool isRewardedVideoAvailable ()
	{
		return _platformAgent.isRewardedVideoAvailable ();
	}

	public bool isRewardedVideoPlacementCapped (string placementName)
	{
		return _platformAgent.isRewardedVideoPlacementCapped (placementName);
	}

	//******************* Interstitial API *******************//

	public void loadInterstitial ()
	{
		_platformAgent.loadInterstitial ();
	}

	public void showInterstitial ()
	{
		_platformAgent.showInterstitial ();
	}
	
	public void showInterstitial (string placementName)
	{
		_platformAgent.showInterstitial (placementName);
	}

	public bool isInterstitialReady ()
	{
		return _platformAgent.isInterstitialReady ();
	}

	public bool isInterstitialPlacementCapped (string placementName)
	{
		return _platformAgent.isInterstitialPlacementCapped (placementName);
	}
	
	//******************* Offerwall API *******************//

	public void showOfferwall ()
	{
		_platformAgent.showOfferwall ();
	}

	public void showOfferwall (string placementName)
	{
		_platformAgent.showOfferwall (placementName);
	}

	public void getOfferwallCredits ()
	{
		_platformAgent.getOfferwallCredits ();
	}
	
	public bool isOfferwallAvailable ()
	{
		return _platformAgent.isOfferwallAvailable ();
	}
	
	#endregion
}