using UnityEngine;
using System.Collections;
using Pokega;

public class OnClickOpenPopUp : MonoBehaviour {

	public string popUpToOpenName;

	void OnClick()
	{
		App.ui.SetPopUp(popUpToOpenName);
	}
}
