using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.MiniJSON;
using Facebook.Unity;
using System;

namespace Pokega{

public class FacebookChallengeFriend : MonoBehaviour {

	private string requestDataText;
	public string id;
	private string[] to = new string[1];
	
	private FacebookFriend facebookFriend;
	private UISprite challengeSprite;
	private BoxCollider myBoxCollider;
	
	public string invitationMessage;
	
	void Awake()
	{
		facebookFriend = gameObject.GetComponentInParent<FacebookFriend>();
		myBoxCollider = gameObject.GetComponent<BoxCollider>();
		if(String.IsNullOrEmpty(invitationMessage))
			Debug.LogError("You have to write down message for friend invitation (challenge)!");
	}
	
	void Start()
	{
		id = facebookFriend.id;
		to[0] = id;
	}

	void OnClick() 
	{
		Debug.Log ("Challanging friend!");
		FB.AppRequest(
			"I challenge you to play with POKEGA",
			to,
			null,
			null,
			null,
			"{\"friend_invite\":\"true\"}",
			"Challenge friend",
			appRequestCallback
		);
	}

	void appRequestCallback(IResult result)
	{
		if (result.Error == null) {
			Dictionary<string,object> resultData = Json.Deserialize(result.RawResult) as Dictionary<string,object>;
			object value;
			if (resultData.TryGetValue("to", out value)) {
				challengeSprite.enabled = false;
				myBoxCollider.enabled = false;
				Debug.LogError("usho");
				var tos = (List<object>)value;
				foreach(object to in tos) 
					Debug.LogError (to.ToString());
			}
			else 
			{/*Cancelled*/}
		}
	}
}
}