using UnityEngine;
using System.Collections;

namespace Pokega{
	public class WheelArrow : MonoBehaviour {

		public string chosenFieldName;
		public Vector3 startPos;

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public void ActivateCollider(){
			gameObject.GetComponent<PolygonCollider2D> ().enabled = true;
		}

		public void DeactivateCollider(){
			gameObject.GetComponent<PolygonCollider2D> ().enabled = false;

		}
			
		void OnCollisionEnter2D(Collision2D coll){
			chosenFieldName = coll.collider.name;
			Debug.Log (coll.collider.name);
		}
	}
}