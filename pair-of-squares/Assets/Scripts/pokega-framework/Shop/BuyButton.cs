using UnityEngine;
using System.Collections;

namespace Pokega{
	
	public class BuyButton : MonoBehaviour {
	
		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	
		//FUNKCIJA SE POZIVA KADA SE KLIKNE NA DUGME
		void OnClick(){
			this.transform.parent.GetComponent<BuyCustomItem>().PreBuyCheck();
		}
	}
}