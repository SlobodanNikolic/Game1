using UnityEngine;
using System.Collections;
using Pokega;

public class OnClickSetScreen : MonoBehaviour {
	
	public string screenName;
	
	
	void OnClick()
	{
		if(screenName != "")
			App.ui.SetScreen(screenName);
		
	}
}