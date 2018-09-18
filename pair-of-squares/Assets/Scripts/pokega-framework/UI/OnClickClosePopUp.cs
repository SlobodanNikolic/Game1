using UnityEngine;
using System.Collections;
using Pokega;

//this class showld be put on every X (close) button 
public class OnClickClosePopUp : MonoBehaviour {

	public string thisPopUpName;
	
	void OnClick()
	{
		App.ui.SetPopUp(thisPopUpName, true);
	}
}
