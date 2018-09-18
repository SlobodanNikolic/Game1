using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenGrab : MonoBehaviour {

	// Use this for initialization
	private bool grab;
	private Rect rect;
	public static ScreenGrab instance;
	public static Texture2D lastGrabTexture;
	public static byte[] lastGrabBytes;
	public static Camera camera;
	public static string filePath;

	void Start () 
	{
		grab = false;
		instance = this;
		camera = gameObject.GetComponent<Camera> ();
		gameObject.SetActive (false);
		filePath=Application.persistentDataPath+"/POSLastGrab.png";
	}

	public void GrabRect(int x, int y, int width,int height)
	{
		rect = new Rect (x, y, width, height);
		grab = true;
	}

	void OnPostRender() 
	{
		if(grab) 
		{
			int rw = Mathf.RoundToInt(rect.width);
			int rh = Mathf.RoundToInt(rect.height);
			lastGrabTexture = new Texture2D(rw, rh, TextureFormat.RGB24,false,false);
			camera.Render();
			lastGrabTexture.ReadPixels(rect, 0, 0);
			lastGrabBytes = lastGrabTexture.EncodeToPNG();
			#if UNITY_ANDROID
				System.IO.File.WriteAllBytes(filePath, lastGrabBytes);
			#endif
			grab = false;
		}
	}
}
