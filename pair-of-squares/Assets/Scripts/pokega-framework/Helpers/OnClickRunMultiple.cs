using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClickRunMultiple : MonoBehaviour {

    public EventDelegate[] actions;

	void OnPress(bool pressed)
    {
		if (!pressed) 
		{
			foreach (EventDelegate action in actions)
				action.Execute ();
		}
    }

    
}
