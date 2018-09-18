using UnityEngine;
using System.Collections;
using Pokega;

namespace Pokega{

	public class TwitterShareButton : MonoBehaviour {
		
		public Texture2D sharedImage;
		public string sharedText;
		public EventDelegate[] methodCalledOnSucceed;
		
		void Awake() 
		{
			#if UNITY_IOS
			IOSSocialManager.OnTwitterPostResult += OnTwitterPostResult;
			#endif

		}
		
		public void OnClick() 
		{
            #if UNITY_IOS
            
            IOSSocialManager.Instance.TwitterPost(sharedText, null, sharedImage);
			#endif
			App.analytics.CreateAnalyticSocialEvent("Twitter", "ShareClicked");
		}

       
        private void OnTwitterPostResult (SA.Common.Models.Result result) 
		{
			if(result.IsSucceeded)  
			{
				Debug.Log("Twitter Share Succeded");
				App.analytics.CreateAnalyticSocialEvent("Twitter", "ShareSucceded");	
				foreach(EventDelegate ed in methodCalledOnSucceed)
					ed.Execute();
			}
			else 
			{
				Debug.Log("Twitter Share Failed");
				App.analytics.CreateAnalyticSocialEvent("Twitter","ShareFailed");
			}
		}
        
	}
}