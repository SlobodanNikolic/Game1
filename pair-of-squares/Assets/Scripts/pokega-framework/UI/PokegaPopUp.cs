using UnityEngine;
using System.Collections;

namespace Pokega{

	public class PokegaPopUp : PokegaScreen {
	
		public UIAction offAction;
		
		public void SetOff()
		{
			offAction.RunAction();
		}
	}
}