using UnityEngine;
using System.Collections;

public class OnClickActivate : MonoBehaviour {

	public UIAction obj;
	public void OnClick() {
		obj.RunAction ();
	}
}
