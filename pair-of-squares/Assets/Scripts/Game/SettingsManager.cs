using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pokega;

public class SettingsManager : MonoBehaviour {

	// Use this for initialization
	public static SettingsManager instance;
	public UISprite musicOnBtn;
	public UISprite soundOnBtn;
	public UISprite colorBlindBtn;
	public UISprite recordBtn;

	public string onSpriteName;
	public string offSpriteName;

	void Start()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (this);
	}

	void OnEnable () 
	{
		UpdateButtons ();
	}

	public void ToggleMusic()
	{
		SoundManager.instance.musicOn = !SoundManager.instance.musicOn;
		SoundManager.instance.PauseUnPauseMusic ();
		if (SoundManager.instance.musicOn) 
		{
			musicOnBtn.spriteName = onSpriteName;
			App.analytics.CreateAnalyticEvent ("Settings: Music on");
		} 
		else 
		{
			musicOnBtn.spriteName = offSpriteName;
			App.analytics.CreateAnalyticEvent ("Settings: Music off");
		}


		App.local.PlayerSave ();
	}

	public void ToggleSound()
	{
		SoundManager.instance.soundOn = !SoundManager.instance.soundOn;

		if (SoundManager.instance.soundOn) 
		{
			soundOnBtn.spriteName = onSpriteName;
			App.analytics.CreateAnalyticEvent ("Settings: Sound on");
		} 
		else 
		{
			soundOnBtn.spriteName = offSpriteName;
			App.analytics.CreateAnalyticEvent ("Settings: Sound off");
		}
		App.local.PlayerSave ();
	}

	public void ToggleColorblind()
	{
		App.colorBlindMode = !App.colorBlindMode;

		if (App.colorBlindMode) {
			colorBlindBtn.spriteName = onSpriteName;
			App.analytics.CreateAnalyticEvent ("Settings: Colorblind on");
		} 
		else 
		{
			colorBlindBtn.spriteName = offSpriteName;
			App.analytics.CreateAnalyticEvent ("Settings: Colorblind off");
		}
		App.local.PlayerSave ();
	}

	public void ToggleRecoding()
	{
		App.recordGameplay = !App.recordGameplay;

		if (App.recordGameplay) {
			recordBtn.spriteName = onSpriteName;
			App.analytics.CreateAnalyticEvent ("Settings: Record Gameplay on");
		} 
		else 
		{
			recordBtn.spriteName = offSpriteName;
			App.analytics.CreateAnalyticEvent ("Settings: Record Gameplay off");
			RecordControl.StopRecording ();
		}
		App.local.PlayerSave ();
	}

	public void UpdateButtons()
	{
		if (SoundManager.instance.musicOn)
			musicOnBtn.spriteName = onSpriteName;
		else
			musicOnBtn.spriteName = offSpriteName;

		if (SoundManager.instance.soundOn)
			soundOnBtn.spriteName = onSpriteName;
		else
			soundOnBtn.spriteName = offSpriteName;

		if (App.colorBlindMode)
			colorBlindBtn.spriteName = onSpriteName;
		else
			colorBlindBtn.spriteName = offSpriteName;

		if (App.recordGameplay)
			recordBtn.spriteName = onSpriteName;
		else
			recordBtn.spriteName = offSpriteName;
	}
	// Update is called once per frame

}
