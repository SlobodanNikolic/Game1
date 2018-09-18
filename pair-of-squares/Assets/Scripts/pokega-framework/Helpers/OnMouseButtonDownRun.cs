using UnityEngine;
using System.Collections;

public class OnMouseButtonDownRun : MonoBehaviour {

	public EventDelegate[] methodsToRun;
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetMouseButtonDown(0))
		{
			foreach(EventDelegate ed in methodsToRun)
				ed.Execute();
				
		}
	}
}
