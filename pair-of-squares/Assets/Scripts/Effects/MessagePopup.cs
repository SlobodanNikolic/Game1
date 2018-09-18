using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessagePopup : MonoBehaviour {

	public GameObject messagePrefab;


	public static MessagePopup instance;
	private GameObject popup;
	private bool isShowing;
	private bool hidden;
	private Text message;
	private GameObject pointer;
	private float pointerY;
	private Coroutine hideCoroutine;

	void Start()
	{
		instance=this;	
		popup = Instantiate (messagePrefab, Vector3.one*1000f, Quaternion.identity) as GameObject;
		popup.transform.SetParent (gameObject.transform);
		message = popup.transform.Find ("Canvas").gameObject.transform.Find ("message").GetComponent<Text> ();
		pointer = popup.transform.Find ("Canvas").gameObject.transform.Find ("pointer").gameObject;
		pointerY = pointer.transform.localPosition.y;
		popup.transform.localScale=Vector3.zero;
		isShowing = false;

	}

	public IEnumerator ShowMessage(string text, Vector3 pos, float duration=-1f, float pointX=-5000f)
	{


		if ((isShowing) && (duration<0))
		{
			hideCoroutine=StartCoroutine(HideMessage (popup));
			while (!hidden) 
			{
				yield return null;
			}
		}

		isShowing = true;
		hidden = false;
		popup.transform.position = pos;

		message.text = text;
		pointer.transform.localPosition=new Vector3(pointX*100f,pointerY,0f);

		float dur = 15f;
		float currentScale = popup.transform.localScale.x;
		for (int i = 1; i <= dur; i++) 
		{
			if (!isShowing)
				yield break;
			
			popup.transform.localScale = Easing.EaseOutBack (currentScale,1f,i/dur) * Vector3.one;
			yield return null;
		}


		if (duration > 0f) 
		{
			if (hideCoroutine != null)
				StopCoroutine (hideCoroutine);

			hideCoroutine=StartCoroutine(HideMessage (popup,duration));
		}

	}

	public IEnumerator HideMessage(GameObject msg=null, float wait=0f)
	{
		yield return new WaitForSeconds (wait);

		if (msg == null)
			msg=popup;

		isShowing = false;
		float dur = 15f;
		float currentScale = msg.transform.localScale.x;
		for (int i = 1; i <= dur; i++) 
		{
			//if (isShowing)
			//	yield break;
			
			msg.transform.localScale = Easing.EaseInBack (currentScale,0f,i/dur) * Vector3.one;
			yield return null;
		}

		hidden = true;
		hideCoroutine=null;
	}




}
