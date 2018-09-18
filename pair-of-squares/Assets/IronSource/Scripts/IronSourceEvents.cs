using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class IronSourceEvents : MonoBehaviour
{
	private const string ERROR_CODE = "error_code";
	private const string ERROR_DESCRIPTION = "error_description";

	void Awake ()
	{
		gameObject.name = "IronSourceEvents";			//Change the GameObject name to IronSourceEvents.
		DontDestroyOnLoad (gameObject);					//Makes the object not be destroyed automatically when loading a new scene.
	}
	
	// ******************************* Rewarded Video Events *******************************
	private static event Action<IronSourceError> _onRewardedVideoAdShowFailedEvent;

	public static event Action<IronSourceError> onRewardedVideoAdShowFailedEvent {
		add {
			if (_onRewardedVideoAdShowFailedEvent == null || !_onRewardedVideoAdShowFailedEvent.GetInvocationList ().Contains (value)) {
				_onRewardedVideoAdShowFailedEvent += value;
			}
		}
		
		remove {
			if (_onRewardedVideoAdShowFailedEvent.GetInvocationList ().Contains (value)) {
				_onRewardedVideoAdShowFailedEvent -= value;
			}
		}
	}
	
	public void onRewardedVideoAdShowFailed (string description)
	{
		if (_onRewardedVideoAdShowFailedEvent != null) {
			IronSourceError sse = getErrorFromErrorString (description);
			_onRewardedVideoAdShowFailedEvent (sse);
		}
	}

	private static event Action _onRewardedVideoAdOpenedEvent;

	public static event Action onRewardedVideoAdOpenedEvent {
		add {
			if (_onRewardedVideoAdOpenedEvent == null || !_onRewardedVideoAdOpenedEvent.GetInvocationList ().Contains (value)) {
				_onRewardedVideoAdOpenedEvent += value;
			}
		}
		
		remove {
			if (_onRewardedVideoAdOpenedEvent.GetInvocationList ().Contains (value)) {
				_onRewardedVideoAdOpenedEvent -= value;
			}
		}
	}
	
	public void onRewardedVideoAdOpened (string empty)
	{
		if (_onRewardedVideoAdOpenedEvent != null) {
			_onRewardedVideoAdOpenedEvent ();
		}
	}

	private static event Action _onRewardedVideoAdClosedEvent;

	public static event Action onRewardedVideoAdClosedEvent {
		add {
			if (_onRewardedVideoAdClosedEvent == null || !_onRewardedVideoAdClosedEvent.GetInvocationList ().Contains (value)) {
				_onRewardedVideoAdClosedEvent += value;
			}
		}
		
		remove {
			if (_onRewardedVideoAdClosedEvent.GetInvocationList ().Contains (value)) {
				_onRewardedVideoAdClosedEvent -= value;
			}
		}
	}
	
	public void onRewardedVideoAdClosed (string empty)
	{
		if (_onRewardedVideoAdClosedEvent != null) {
			_onRewardedVideoAdClosedEvent ();
		}
	}

	private static event Action _onRewardedVideoAdStartedEvent;

	public static event Action onRewardedVideoAdStartedEvent {
		add {
			if (_onRewardedVideoAdStartedEvent == null || !_onRewardedVideoAdStartedEvent.GetInvocationList ().Contains (value)) {
				_onRewardedVideoAdStartedEvent += value;
			}
		}
		
		remove {
			if (_onRewardedVideoAdStartedEvent.GetInvocationList ().Contains (value)) {
				_onRewardedVideoAdStartedEvent -= value;
			}
		}
	}
	
	public void onRewardedVideoAdStarted (string empty)
	{
		if (_onRewardedVideoAdStartedEvent != null) {
			_onRewardedVideoAdStartedEvent ();
		}
	}

	private static event Action _onRewardedVideoAdEndedEvent;

	public static event Action onRewardedVideoAdEndedEvent {
		add {
			if (_onRewardedVideoAdEndedEvent == null || !_onRewardedVideoAdEndedEvent.GetInvocationList ().Contains (value)) {
				_onRewardedVideoAdEndedEvent += value;
			}
		}
		
		remove {
			if (_onRewardedVideoAdEndedEvent.GetInvocationList ().Contains (value)) {
				_onRewardedVideoAdEndedEvent -= value;
			}
		}
	}
	
	public void onRewardedVideoAdEnded (string empty)
	{
		if (_onRewardedVideoAdEndedEvent != null) {
			_onRewardedVideoAdEndedEvent ();
		}
	}

	private static event Action<IronSourcePlacement> _onRewardedVideoAdRewardedEvent;

	public static event Action<IronSourcePlacement> onRewardedVideoAdRewardedEvent {
		add {
			if (_onRewardedVideoAdRewardedEvent == null || !_onRewardedVideoAdRewardedEvent.GetInvocationList ().Contains (value)) {
				_onRewardedVideoAdRewardedEvent += value;
			}
		}
		
		remove {
			if (_onRewardedVideoAdRewardedEvent.GetInvocationList ().Contains (value)) {
				_onRewardedVideoAdRewardedEvent -= value;
			}
		}
	}
	
	public void onRewardedVideoAdRewarded (string description)
	{
		if (_onRewardedVideoAdRewardedEvent != null) {
			IronSourcePlacement ssp = getPlacementFromString (description);
			_onRewardedVideoAdRewardedEvent (ssp);
		}	
	}

	private static event Action<bool> _onRewardedVideoAvailabilityChangedEvent;

	public static event Action<bool> onRewardedVideoAvailabilityChangedEvent {
		add {
			if (_onRewardedVideoAvailabilityChangedEvent == null || !_onRewardedVideoAvailabilityChangedEvent.GetInvocationList ().Contains (value)) {
				_onRewardedVideoAvailabilityChangedEvent += value;
			}
		}

		remove {
			if (_onRewardedVideoAvailabilityChangedEvent.GetInvocationList ().Contains (value)) {
				_onRewardedVideoAvailabilityChangedEvent -= value;
			}
		}
	}

	public void onRewardedVideoAvailabilityChanged (string stringAvailable)
	{
		bool isAvailable = (stringAvailable == "true") ? true : false;
		if (_onRewardedVideoAvailabilityChangedEvent != null)
			_onRewardedVideoAvailabilityChangedEvent (isAvailable);
	}
	

	// ******************************* Interstitial Events *******************************
	private static event Action _onInterstitialAdReadyEvent;

	public static event Action onInterstitialAdReadyEvent {
		add {
			if (_onInterstitialAdReadyEvent == null || !_onInterstitialAdReadyEvent.GetInvocationList ().Contains (value)) {
				_onInterstitialAdReadyEvent += value;
			}
		}
		
		remove {
			if (_onInterstitialAdReadyEvent.GetInvocationList ().Contains (value)) {
				_onInterstitialAdReadyEvent -= value;
			}
		}
	}
	
	public void onInterstitialAdReady ()
	{
		if (_onInterstitialAdReadyEvent != null)
			_onInterstitialAdReadyEvent ();
	}

	private static event Action<IronSourceError> _onInterstitialAdLoadFailedEvent;

	public static event Action<IronSourceError> onInterstitialAdLoadFailedEvent {
		add {
			if (_onInterstitialAdLoadFailedEvent == null || !_onInterstitialAdLoadFailedEvent.GetInvocationList ().Contains (value)) {
				_onInterstitialAdLoadFailedEvent += value;
			}
		}
		
		remove {
			if (_onInterstitialAdLoadFailedEvent.GetInvocationList ().Contains (value)) {
				_onInterstitialAdLoadFailedEvent -= value;
			}
		}
	}
	
	public void onInterstitialAdLoadFailed (string description)
	{
		if (_onInterstitialAdLoadFailedEvent != null) {
			IronSourceError sse = getErrorFromErrorString (description);
			_onInterstitialAdLoadFailedEvent (sse);
		}
		
	}

	private static event Action _onInterstitialAdOpenedEvent;

	public static event Action onInterstitialAdOpenedEvent {
		add {
			if (_onInterstitialAdOpenedEvent == null || !_onInterstitialAdOpenedEvent.GetInvocationList ().Contains (value)) {
				_onInterstitialAdOpenedEvent += value;
			}
		}
		
		remove {
			if (_onInterstitialAdOpenedEvent.GetInvocationList ().Contains (value)) {
				_onInterstitialAdOpenedEvent -= value;
			}
		}
	}
	
	public void onInterstitialAdOpened (string empty)
	{
		if (_onInterstitialAdOpenedEvent != null) {
			_onInterstitialAdOpenedEvent ();
		}
	}

	private static event Action _onInterstitialAdClosedEvent;

	public static event Action onInterstitialAdClosedEvent {
		add {
			if (_onInterstitialAdClosedEvent == null || !_onInterstitialAdClosedEvent.GetInvocationList ().Contains (value)) {
				_onInterstitialAdClosedEvent += value;
			}
		}
		
		remove {
			if (_onInterstitialAdClosedEvent.GetInvocationList ().Contains (value)) {
				_onInterstitialAdClosedEvent -= value;
			}
		}
	}
	
	public void onInterstitialAdClosed (string empty)
	{
		if (_onInterstitialAdClosedEvent != null) {
			_onInterstitialAdClosedEvent ();
		}
	}

	private static event Action _onInterstitialAdShowSucceededEvent;

	public static event Action onInterstitialAdShowSucceededEvent {
		add {
			if (_onInterstitialAdShowSucceededEvent == null || !_onInterstitialAdShowSucceededEvent.GetInvocationList ().Contains (value)) {
				_onInterstitialAdShowSucceededEvent += value;
			}
		}
		
		remove {
			if (_onInterstitialAdShowSucceededEvent.GetInvocationList ().Contains (value)) {
				_onInterstitialAdShowSucceededEvent -= value;
			}
		}
	}
	
	public void onInterstitialAdShowSucceeded (string empty)
	{
		if (_onInterstitialAdShowSucceededEvent != null) {
			_onInterstitialAdShowSucceededEvent ();
		}
	}

	private static event Action<IronSourceError> _onInterstitialAdShowFailedEvent;

	public static event Action<IronSourceError> onInterstitialAdShowFailedEvent {
		add {
			if (_onInterstitialAdShowFailedEvent == null || !_onInterstitialAdShowFailedEvent.GetInvocationList ().Contains (value)) {
				_onInterstitialAdShowFailedEvent += value;
			}
		}
		
		remove {
			if (_onInterstitialAdShowFailedEvent.GetInvocationList ().Contains (value)) {
				_onInterstitialAdShowFailedEvent -= value;
			}
		}
	}
	
	public void onInterstitialAdShowFailed (string description)
	{
		if (_onInterstitialAdShowFailedEvent != null) {
			IronSourceError sse = getErrorFromErrorString (description);
			_onInterstitialAdShowFailedEvent (sse);
		}
			
	}

	private static event Action _onInterstitialAdClickedEvent;

	public static event Action onInterstitialAdClickedEvent {
		add {
			if (_onInterstitialAdClickedEvent == null || !_onInterstitialAdClickedEvent.GetInvocationList ().Contains (value)) {
				_onInterstitialAdClickedEvent += value;
			}
		}
		
		remove {
			if (_onInterstitialAdClickedEvent.GetInvocationList ().Contains (value)) {
				_onInterstitialAdClickedEvent -= value;
			}
		}
	}
	
	public void onInterstitialAdClicked (string empty)
	{
		if (_onInterstitialAdClickedEvent != null) {
			_onInterstitialAdClickedEvent ();
		}
	}

	// ******************************* Rewarded Interstitial Events *******************************

	private static event Action _onInterstitialAdRewardedEvent;
	
	public static event Action onInterstitialAdRewardedEvent {
		add {
			if (_onInterstitialAdRewardedEvent == null || !_onInterstitialAdRewardedEvent.GetInvocationList ().Contains (value)) {
				_onInterstitialAdRewardedEvent += value;
			}
		}
		
		remove {
			if (_onInterstitialAdRewardedEvent.GetInvocationList ().Contains (value)) {
				_onInterstitialAdRewardedEvent -= value;
			}
		}
	}
	
	public void onInterstitialAdRewarded (string empty)
	{
		if (_onInterstitialAdRewardedEvent != null) {
			_onInterstitialAdRewardedEvent ();
		}
	}

	// ******************************* Offerwall Events *******************************	
	private static event Action _onOfferwallOpenedEvent;

	public static event Action onOfferwallOpenedEvent {
		add {
			if (_onOfferwallOpenedEvent == null || !_onOfferwallOpenedEvent.GetInvocationList ().Contains (value)) {
				_onOfferwallOpenedEvent += value;
			}
		}
		
		remove {
			if (_onOfferwallOpenedEvent.GetInvocationList ().Contains (value)) {
				_onOfferwallOpenedEvent -= value;
			}			
		}
	}
	
	public void onOfferwallOpened (string empty)
	{
		if (_onOfferwallOpenedEvent != null) {
			_onOfferwallOpenedEvent ();
		}
	}

	private static event Action<IronSourceError> _onOfferwallShowFailedEvent;

	public static event Action<IronSourceError> onOfferwallShowFailedEvent {
		add {
			if (_onOfferwallShowFailedEvent == null || !_onOfferwallShowFailedEvent.GetInvocationList ().Contains (value)) {
				_onOfferwallShowFailedEvent += value;
			}
		}
		
		remove {
			if (_onOfferwallShowFailedEvent.GetInvocationList ().Contains (value)) {
				_onOfferwallShowFailedEvent -= value;
			}
		}
	}
	
	public void onOfferwallShowFailed (string description)
	{
		if (_onOfferwallShowFailedEvent != null) {
			IronSourceError sse = getErrorFromErrorString (description);
			_onOfferwallShowFailedEvent (sse);
		}
	}

	private static event Action _onOfferwallClosedEvent;

	public static event Action onOfferwallClosedEvent {
		add {
			if (_onOfferwallClosedEvent == null || !_onOfferwallClosedEvent.GetInvocationList ().Contains (value)) {
				_onOfferwallClosedEvent += value;
			}
		}
		
		remove {
			if (_onOfferwallClosedEvent.GetInvocationList ().Contains (value)) {
				_onOfferwallClosedEvent -= value;
			}		
		}
	}
	
	public void onOfferwallClosed (string empty)
	{
		if (_onOfferwallClosedEvent != null) {
			_onOfferwallClosedEvent ();
		}
	}

	private static event Action<IronSourceError> _onGetOfferwallCreditsFailedEvent;

	public static event Action<IronSourceError> onGetOfferwallCreditsFailedEvent {
		add {
			if (_onGetOfferwallCreditsFailedEvent == null || !_onGetOfferwallCreditsFailedEvent.GetInvocationList ().Contains (value)) {
				_onGetOfferwallCreditsFailedEvent += value;
			}
		}
		
		remove {
			if (_onGetOfferwallCreditsFailedEvent.GetInvocationList ().Contains (value)) {
				_onGetOfferwallCreditsFailedEvent -= value;
			}
		}
	}
	
	public void onGetOfferwallCreditsFailed (string description)
	{
		if (_onGetOfferwallCreditsFailedEvent != null) {
			IronSourceError sse = getErrorFromErrorString (description);
			_onGetOfferwallCreditsFailedEvent (sse);

		}
	}

	private static event Action<Dictionary<string,object>> _onOfferwallAdCreditedEvent;

	public static event Action<Dictionary<string,object>> onOfferwallAdCreditedEvent {
		add {
			if (_onOfferwallAdCreditedEvent == null || !_onOfferwallAdCreditedEvent.GetInvocationList ().Contains (value)) {
				_onOfferwallAdCreditedEvent += value;
			}
		}

		remove {
			if (_onOfferwallAdCreditedEvent.GetInvocationList ().Contains (value)) {
				_onOfferwallAdCreditedEvent -= value;
			}
		}
	}

	public void onOfferwallAdCredited (string json)
	{
		if (_onOfferwallAdCreditedEvent != null)
			_onOfferwallAdCreditedEvent (IronSourceJSON.Json.Deserialize (json) as Dictionary<string,object>);
	}

	private static event Action<bool> _onOfferwallAvailableEvent;
	
	public static event Action<bool> onOfferwallAvailableEvent {
		add {
			if (_onOfferwallAvailableEvent == null || !_onOfferwallAvailableEvent.GetInvocationList ().Contains (value)) {
				_onOfferwallAvailableEvent += value;
			}
		}
		
		remove {
			if (_onOfferwallAvailableEvent.GetInvocationList ().Contains (value)) {
				_onOfferwallAvailableEvent -= value;
			}
		}
	}
	
	public void onOfferwallAvailable (string stringAvailable)
	{
		bool isAvailable = (stringAvailable == "true") ? true : false;
		if (_onOfferwallAvailableEvent != null)
			_onOfferwallAvailableEvent (isAvailable);
	}
	
	// ******************************* Helper methods *******************************	
	public IronSourceError getErrorFromErrorString (string description)
	{
		IronSourceError sse;
		if (!String.IsNullOrEmpty (description)) {
			Dictionary<string,object> error = IronSourceJSON.Json.Deserialize (description) as Dictionary<string,object>;
			// if there is a IronSource error
			if (error != null) {
				int eCode = Convert.ToInt32 (error [ERROR_CODE].ToString ());
				string eDescription = error [ERROR_DESCRIPTION].ToString ();
				sse = new IronSourceError (eCode, eDescription);
			} 
		// else create an empty one
		else {
				sse = new IronSourceError (-1, "");
			}
		} else {
			sse = new IronSourceError (-1, "");
		}

		return sse;
	}

	public IronSourcePlacement getPlacementFromString (string jsonPlacement)
	{		
		Dictionary<string,object> placementJSON = IronSourceJSON.Json.Deserialize (jsonPlacement) as Dictionary<string,object>;
		IronSourcePlacement ssp;
		int rewardAmount = Convert.ToInt32 (placementJSON ["placement_reward_amount"].ToString ());
		string rewardName = placementJSON ["placement_reward_name"].ToString ();
		string placementName = placementJSON ["placement_name"].ToString ();
		
		ssp = new IronSourcePlacement (placementName, rewardName, rewardAmount);
		return ssp;
	}
}
