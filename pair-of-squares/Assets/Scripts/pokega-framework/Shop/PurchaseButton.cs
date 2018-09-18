using UnityEngine;
using System.Collections;

namespace Pokega{
	public class PurchaseButton : MonoBehaviour {
	
		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	
		void OnClick(){
			this.transform.parent.GetComponent<BuyMe>().Buy();
		}
	}
}