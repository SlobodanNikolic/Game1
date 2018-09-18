using UnityEngine;
using System.Collections;


public class OnClickRun : MonoBehaviour {

	public EventDelegate action;
	
	void OnClick()
	{
		action.Execute();
	}
}
