using UnityEngine;
using System.Collections;
using Pokega;
using System;

namespace Pokega{

	public class PokegaScreen : MonoBehaviour {
	
		string name;
		public UIAction onAction;
		public float tweenTime;
		protected bool inTrasition;
		
		void Awake()
		{
			name = gameObject.name;
			inTrasition = false;
		}

		public void Set()
		{
			Debug.Log ("Running action, setting " + name);
			onAction.RunAction();
		}
	}
}