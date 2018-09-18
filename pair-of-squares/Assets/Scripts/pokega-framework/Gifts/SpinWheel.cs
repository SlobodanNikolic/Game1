using UnityEngine;
using System.Collections;

public class SpinWheel : MonoBehaviour{

	public Vector2 force;
	public float speed;
	public Vector2 forceApplyOffset;

//	public AnimationCurve animationCurve;   ​
//	private bool spinning;
//
//	void Start (){          
//		spinning = false;
//	}
//
//	void Update ()  
//	{
//		if ( Input.GetMouseButtonDown( 0 ) && !spinning) {
//			StartCoroutine (DoSpin (10f, Random.Range (2000f, 3000f)));         
//		}
//	}    
//
//	public IEnumerator DoSpin( float time,  float angle){          
//		spinning = true; float timer = 0f;
//		float startAngle = transform.eulerAngles.z;         ​
//		while (timer < time){ 
//			float endAngle = animationCurve.Evaluate(timer / time) * angle;       
//			transform.eulerAngles =  new Vector3 ( 0.0f ,  0.0f, (endAngle + startAngle));
//			timer += Time.deltaTime;         
//			yield return 0;          
//		}             
//		spinning = false;
//	}    

	void Update(){
		if (Input.GetKeyDown (KeyCode.W)) {
			gameObject.GetComponent<Rigidbody2D> ().AddForceAtPosition(force*speed, new Vector2(transform.position.x + forceApplyOffset.x, transform.position.y + forceApplyOffset.y), ForceMode2D.Force);
		}
	}
}