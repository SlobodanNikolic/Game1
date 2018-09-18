using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pokega{
	
	public class FacebookFriend : MonoBehaviour {
		
		
		public string id;
		public string username;
		public string picture;
		public List<Score> bestScores;
		public List<string> bestScoresNames;
		
		public UILabel[] bestScoresLabels;
		public UILabel usernameLabel;	
		public UITexture avatar;
		
		int numberOfScores;
		
		
		void Awake()
		{
			foreach(UILabel l in bestScoresLabels)
			{
				l.enabled = false;
			}
		}
		
		
		void Start() 
		{
			numberOfScores = App.score.bestScores.Count;
			//if there are n scores, enable n labels
			for(int i=0; i<numberOfScores; i++)
			{
				if((i+1) <= numberOfScores)
					bestScoresLabels[i].enabled = true;
			}
			usernameLabel.text = username;
			App.fb.ContBasicSetupDone();
		}
		
		public void SetAvatar()
		{
			StartCoroutine(GetAvatar());
		}
		
		IEnumerator GetAvatar()
		{
			WWW www = new WWW(picture);
			yield return www;
			avatar.mainTexture = www.texture;
		}
		
		void SetScoresGui(Score score, int scoreNumber)
		{
			bestScoresLabels[scoreNumber].text = score.scoreName + " : " + score.score.ToString();
		}
		
		public void SetScore(List<Score> scores)
		{
			bestScores = scores;
			for(int i=0; i<bestScores.Count; i++)
			{
				//TODO: Ovde treba postaviti da se narednoj metodi prosledjuje dekriptovan score .toString()
				SetScoresGui(bestScores[i],i);
			}
		}
		
	}
}