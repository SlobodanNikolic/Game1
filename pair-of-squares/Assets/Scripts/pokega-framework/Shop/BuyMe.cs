//#pragma strict
using UnityEngine;
using System.Collections;
using System;

namespace Pokega{
	//OVA SKRIPTA SE NE KORISTI
	public class BuyMe : MonoBehaviour {
		
		public string productId;

		void Awake () {
		}
		
		public void Buy() {
			Debug.Log (productId);
            //edit stefan
            //ShopControl.buyItem(productId);
		}
		
	}
}