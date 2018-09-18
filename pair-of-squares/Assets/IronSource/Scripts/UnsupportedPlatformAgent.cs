#if (!UNITY_IPHONE && !UNITY_IOS && !UNITY_ANDROID) || (UNITY_EDITOR)
using UnityEngine;
using System.Collections;

public class UnsupportedPlatformAgent : IronSourceIAgent
{
	public UnsupportedPlatformAgent ()
	{
		Debug.Log ("Unsupported Platform");
	}
	
	#region IronSourceAgent implementation

	public void start ()
	{
		Debug.Log ("Unsupported Platform");
	}

	public void reportAppStarted ()
	{
		Debug.Log ("Unsupported Platform");
	}

	//******************* Base API *******************//

	public void onApplicationPause (bool pause)
	{
		Debug.Log ("Unsupported Platform");
	}
	
	public void setAge (int age)
	{
		Debug.Log ("Unsupported Platform");
	}
	
	public void setGender (string gender)
	{
		Debug.Log ("Unsupported Platform");
	}

	public void setMediationSegment (string segment)
	{
		Debug.Log ("Unsupported Platform");
	}

	public string getAdvertiserId ()
	{
		Debug.Log ("Unsupported Platform");
		return "";
	}
	
	public void validateIntegration ()
	{
		Debug.Log ("Unsupported Platform");
	}
	
	public void shouldTrackNetworkState (bool track)
	{
		Debug.Log ("Unsupported Platform");
	}

	public bool setDynamicUserId (string dynamicUserId)
	{
		Debug.Log ("Unsupported Platform");
		return false;
	}

	//******************* SDK Init *******************//

	public void setUserId (string userId)
	{
		Debug.Log ("Unsupported Platform");
	}

	public void init (string appKey)
	{
		Debug.Log ("Unsupported Platform");
	}

	public void init (string appKey, params string[] adUnits)
	{
		Debug.Log ("Unsupported Platform");
	}

	//******************* RewardedVideo API *******************//
	
	public void showRewardedVideo ()
	{
		Debug.Log ("Unsupported Platform");
	}

	public void showRewardedVideo (string placementName)
	{
		Debug.Log ("Unsupported Platform");
	}
	
	public bool isRewardedVideoAvailable ()
	{
		Debug.Log ("Unsupported Platform");
		return false;
	}

	public bool isRewardedVideoPlacementCapped (string placementName)
	{
		Debug.Log ("Unsupported Platform");
		return true;
	}

	public IronSourcePlacement getPlacementInfo (string placementName)
	{
		Debug.Log ("Unsupported Platform");
		return null;
	}

	//******************* Interstitial API *******************//

	public void loadInterstitial ()
	{
		Debug.Log ("Unsupported Platform");
	}

	public void showInterstitial ()
	{
		Debug.Log ("Unsupported Platform");
	}

	public void showInterstitial (string placementName)
	{
		Debug.Log ("Unsupported Platform");
	}

	public bool isInterstitialReady ()
	{
		Debug.Log ("Unsupported Platform");
		return false;
	}

	public bool isInterstitialPlacementCapped (string placementName)
	{
		Debug.Log ("Unsupported Platform");
		return true;
	}

	//******************* Offerwall API *******************//
	
	public void showOfferwall ()
	{
		Debug.Log ("Unsupported Platform");
	}

	public void showOfferwall (string placementName)
	{
		Debug.Log ("Unsupported Platform");
	}
	
	public void getOfferwallCredits ()
	{
		Debug.Log ("Unsupported Platform");
	}

	public bool isOfferwallAvailable ()
	{
		Debug.Log ("Unsupported Platform");
		return false;
	}
		
	#endregion
}

#endif
