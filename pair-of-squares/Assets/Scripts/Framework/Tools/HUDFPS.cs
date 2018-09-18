using UnityEngine;
using System.Collections;

public class HUDFPS : MonoBehaviour 
{
	
	// Attach this to a GUIText to make a frames/second indicator.
	//
	// It calculates frames/second over each updateInterval,
	// so the display does not keep changing wildly.
	//
	// It is also fairly accurate at very low FPS counts (<10).
	// We do this not by simply counting frames per interval, but
	// by accumulating FPS for each frame. This way we end up with
	// correct overall FPS even if the interval renders something like
	// 5.5 frames.
	
	public  float updateInterval = 1F;
	
	private float accum   = 0; // FPS accumulated over the interval
	private int   frames  = 0; // Frames drawn over the interval
	private float timeleft; // Left time for current interval
	public UILabel FPS;
	public UILabel MinFPS;
	public UILabel MaxFPS;
	float min;
	float max;
	float timing;
	bool StartMinMax;
	
	void Start()
	{
		StartMinMax=false;
		timing=1f;
		min = 6000f;
		max = 0f;
		FPS = this.GetComponent<UILabel> () as UILabel;
		if( !FPS )
		{
			Debug.Log("UtilityFramesPerSecond needs a UILabel component!");
			enabled = false;
			return;
		}

		timeleft = updateInterval;  
	}
	
	void Update()
	{
		if(timing >0f)timing+=Time.deltaTime;
		timeleft -= Time.deltaTime;
		accum += Time.timeScale/Time.deltaTime;
		++frames;
		
		// Interval ended - update GUI text and start new interval
		if( timeleft <= 0.0 )
		{
			// display two fractional digits (f2 format)
			float fps = accum/frames;
			
			string format = System.String.Format("{0:F2}",fps);
			FPS.text = format;
			if (timing >6f)
			{
				StartMinMax = true;
				timing =0f;
			}
			if (StartMinMax)
			{
				if (fps>max) max = fps;
				if (fps<min) min = fps;
				//MinFPS.text = min.ToString();
				//MaxFPS.text = max.ToString();
			}
			/*if(fps < 30)
				guiText.material.color = Color.yellow;
			else 
				if(fps < 10)
					guiText.material.color = Color.red;
			else
				guiText.material.color = Color.green;*/
			//	DebugConsole.Log(format,level);
			timeleft = updateInterval;
			accum = 0.0F;
			frames = 0;
		}
	}
}