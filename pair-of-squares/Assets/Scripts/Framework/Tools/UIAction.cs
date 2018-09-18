using UnityEngine;
using System.Collections;

public class UIAction : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void RunAction() {
		Debug.Log ("RUN ACTION: " + this.name);
		UIPlayTween[] Twins = this.GetComponents<UIPlayTween>();
		foreach (UIPlayTween twin in Twins) {
			twin.Play(true);
		}
		UIPlaySound[] Sounds = this.GetComponents<UIPlaySound>();
		foreach (UIPlaySound sound in Sounds) {
			sound.Play();//Play(true);
		}
	}
}