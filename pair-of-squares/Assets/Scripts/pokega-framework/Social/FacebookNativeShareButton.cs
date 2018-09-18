using UnityEngine;
using System.Collections;
using Pokega;

public class FacebookNativeShareButton : MonoBehaviour {

	public Texture2D sharedImage;
	public string sharedText;
	public EventDelegate[] methodCalledOnSucceed;

	void Awake() 
	{
        //#if UNITY_IOS
        IOSSocialManager.OnFacebookPostResult += OnFacebookPostResult;

        //#endif

    }

	public void OnClick() 
	{
        #if UNITY_IOS
        IOSSocialManager.Instance.FacebookPost(sharedText, null, sharedImage);
		#endif
		App.analytics.CreateAnalyticSocialEvent("Facebook", "ShareClicked");
	}

    
    private void OnFacebookPostResult (SA.Common.Models.Result result) 
	{
		if(result.IsSucceeded)  
		{
			Debug.Log("Facebook Share Succeded");
			//App.analytics.CreateAnalyticSocialEvent("Twitter", "ShareSucceded");	
			foreach(EventDelegate ed in methodCalledOnSucceed)
				ed.Execute();
		}
		else 
		{
			Debug.Log("Facebook Share Failed");
			//App.analytics.CreateAnalyticSocialEvent("Twitter","ShareFailed");
		}
	}
    
}
