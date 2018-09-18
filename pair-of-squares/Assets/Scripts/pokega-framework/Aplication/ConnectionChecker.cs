using UnityEngine;
using System.Collections;


namespace Pokega
{
public class ConnectionChecker : MonoBehaviour {

	public bool connected;
	
	#if !UNITY_WEBGL 
	void Start () 
	{
		TestConnection ();
		
	}
	
	void TestConnection()
	{
//		Debug.Log("Testing Connection...");
		StartCoroutine(CheckConnection());
	}
	
	IEnumerator CheckConnection()
	{

		//5 sekundi ce pokusavati da proveri konekciju
		//ako za to vreme ne uspe pokusace opet za 10sek
		const float timeout = 5.0f;
		float startTime = Time.timeSinceLevelLoad;
		
		//google pingovanje
			var ping = new Ping("8.8.8.8");
		while (true)
		{	
			if (ping.isDone)
			{
				connected = true;
				Invoke("TestConnection",15.0f);
				yield break;
			}
			else if (Time.timeSinceLevelLoad - startTime > timeout)
			{
				connected = false;
				Invoke("TestConnection",15.0f);
				yield break;
			}
			yield return new WaitForSeconds(1.0f);
		}
	}
	
	#endif
	public bool isConnected()
	{
		return connected;
	}
	
}
}