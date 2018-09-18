using UnityEngine;
using System.Collections;

namespace Pokega{
	
	public class ShopTabSwitch : MonoBehaviour {
	
		public GameObject[] objectsToDisable;
		public GameObject[] objectsToEnable;

	
		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	
		void OnClick(){
			Debug.Log("clicked on " + this.name);
			foreach (GameObject obj in objectsToEnable)
				obj.SetActive (true);

			foreach (GameObject obj in objectsToDisable)
				obj.SetActive (false);
		}
	
	}
}