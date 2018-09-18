using UnityEngine;
using System.Collections;
namespace Pokega{

	public class NotEnoughCoinsButton : MonoBehaviour {
	
		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	
		void OnClick(){
			App.ui.SetPopUp("UI Not Enough Coins", true);
		}
	}
}