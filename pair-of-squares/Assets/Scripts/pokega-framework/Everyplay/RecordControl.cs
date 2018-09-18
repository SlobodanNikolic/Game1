using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pokega{
	public class RecordControl : MonoBehaviour {

	void Start() 
	{
		Everyplay.ReadyForRecording += OnReadyForRecording;
	}

	public void OnReadyForRecording(bool enabled) {
		if(enabled) {
			

		} 
	}

	public static void StartRecording()
	{
		Everyplay.StartRecording ();
	}

	public static void StopRecording()
	{
		Everyplay.StopRecording ();
	}

	public static void PauseRecording()
	{
		Everyplay.PauseRecording ();
	}

	public static void ResumeRecording()
	{
		Everyplay.ResumeRecording ();
	}


	public static void PlayLastRecording()
	{
		Everyplay.PlayLastRecording();
	}

	public static void Show()
	{
		Everyplay.Show();
	}

	public static bool IsPaused()
	{
		return Everyplay.IsPaused();
	}

	public static bool IsRecording()
	{
		return Everyplay.IsRecording();
	}


	public static void SetMetadata(string name, object value)
	{
		Everyplay.SetMetadata(name, value);
	}



	

}
}

