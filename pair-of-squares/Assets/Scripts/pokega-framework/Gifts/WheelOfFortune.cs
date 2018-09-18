using UnityEngine;
using System.Collections;

namespace Pokega{
	
	public class WheelOfFortune : MonoBehaviour {

		public GameObject wheelArrow;
		public GameObject okButton;
		public GameObject spinButton;
		public GameObject backButton;

		public Vector3 touchPos;
		public Vector3 startTouchPos;
		public Vector3 endTouchPos;
		public Vector3 deltaPos;
		public Vector3 lastTouchPos;
		public float deltaTime;
		public float startTime;
		public float endTime;
		public float spinMultiplyCoef;

		public bool wheelTouched;
		public float speed;
		public Vector2 forceVector;
		public Vector2 forcePoint;

		// Use this for initialization
		void Start () {
		}
		
		// Update is called once per frame
		void Update () {

			if (Input.GetKeyDown (KeyCode.Q)) {
				gameObject.GetComponent<Rigidbody2D> ().AddForceAtPosition (forceVector*speed, forcePoint);
			
			}

			if (Input.GetMouseButtonDown(0)) {
				touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

				RaycastHit2D hit;
				if (hit = Physics2D.Raycast (touchPos, Input.mousePosition)) {
					if (hit.collider.tag == "WheelOfFortune") {

						startTouchPos = Input.mousePosition;
						startTime = Time.time;
					}
				}

			}

			if (Input.GetMouseButtonUp(0)) {
				touchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

				endTouchPos = Input.mousePosition;
				endTime = Time.time;

			}

//			for (int i = 0; i < Input.touchCount; ++i) {
//				
//				Touch tac = Input.GetTouch (i);
//				touchPos = Camera.main.ScreenToWorldPoint (tac.position);
//				RaycastHit2D hit;
//				if (hit = Physics2D.Raycast (touchPos, tac.position)) {
//					if (hit.collider.tag == "WheelOfFortune") {
//
//
//				
//					}
//				}
//			}

		}

		public void OpenGiftScreen(){
				
			App.ui.SetScreen ("UI Gift");
			okButton.SetActive (false);
			spinButton.SetActive (true);
			backButton.SetActive (true);
		}


		public void OKClicked(){
			gameObject.GetComponent<Rigidbody2D> ().isKinematic = false;
			wheelArrow.GetComponent<WheelArrow> ().DeactivateCollider ();
			App.ui.SetScreen (App.ui.lastScreenName);
		}

		public void SpinTheWheel(){

		}
			
	}
}