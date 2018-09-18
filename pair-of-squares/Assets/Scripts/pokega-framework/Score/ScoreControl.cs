using UnityEngine;
using System.Collections;
//using GameAnalyticsSDK;
using System.Collections.Generic;
//using Facebook.MiniJSON;
//Edit StefAN
//using GooglePlayGames;
using UnityEngine.SocialPlatforms;

namespace Pokega{

	public class ScoreControl : MonoBehaviour {

		public UILabel scoreLabel;
		public UILabel gameOverScoreLabel;
		public UILabel gameOverBestScore;

		//Lista skorova
		public List<Score> scores;
		public List<Score> bestScores;	
		public List<Score> totalScores;	

		void Awake(){
			InitializeScoresToZero();
			InitializeBestScoresToZero();
		}
 
		public void InitializeScoresToZero(){
			foreach(Score s in scores) {
				s.score = Crypting.EncryptFloat(0);
			}
		}
			
		public void InitializeBestScoresToZero(){
			for(int i=0; i<bestScores.Count; i++) {
				bestScores[i].score = Crypting.EncryptFloat(0);
				bestScores[i].scoreName = scores[i].scoreName;
			}
		}

		public void InitializeTotalScoresToZero(){
			for(int i=0; i<totalScores.Count; i++) {
				totalScores[i].score = Crypting.EncryptFloat(0);
			}
		}

		public void SetAndSaveTotalScores()
		{
			
		}
		
		public void SetAndSaveBestScore() {
			Debug.Log("Checking and setting best scores");
			for (int i=0; i<scores.Count; i++) 
			{
				if(Crypting.DecryptInt (scores[i].score) > Crypting.DecryptInt (bestScores[i].score))
				{
					bestScores[i].score = scores[i].score;
				}
			}
			//OVDE TREBA PRVO DA SE POSALJU SKOROVI LOCAL DB-u i SERVER API-ju
			App.local.PlayerSave();
			App.server.Save();
		}

		//Postavljanje vrednosti skora u odgovarajuce labele
		public void SetScoreLabels(){
			foreach(Score sc in scores){
				foreach(UILabel labela in sc.scoreLabels){
					labela.text = GetScore(sc.scoreName).ToString();
				}
			}
		}

        public void SetScoreLabel(string scoreName)
        {
            foreach (Score sc in scores)
            {
                if (sc.scoreName == scoreName)
                {
                    foreach (UILabel labela in sc.scoreLabels)
                    {
                        labela.text = GetScore(sc.scoreName).ToString();
                    }
                }
            }
        }

        //Postavljanje 0 vrednosti skora u odgovarajuce labele
        public void ResetScoreLabels(){
			foreach(Score sc in scores){
				foreach(UILabel labela in sc.scoreLabels){
					labela.text = "0";
				}
			}
		}

		//Postavljanje vrednosti najboljih skorova u odgovarajuce labele

		public void SetBestScoreLabels(){
			foreach(Score sc in bestScores){
				foreach(UILabel labela in sc.scoreLabels){
					labela.text = GetBestScore(sc.scoreName).ToString();
				}
			}
		}

		public void ResetBestScoreLabels(){
			foreach(Score sc in bestScores){
				foreach(UILabel labela in sc.scoreLabels){
					labela.text = "0";
				}
			}
		}

		public void SetTotalScoreLabels(){
			foreach(Score sc in totalScores){
				foreach(UILabel labela in sc.scoreLabels){
					labela.text = GetScore(sc.scoreName).ToString();
				}
			}
		}

		//Plus i minus funkcije povecavaju skor definisan imenom "name", za vrednost definisanu brojem "amount"
		public void TotalScorePlus(float amount, string name)
		{
			for(int i=0; i<scores.Count; i++){
				if(totalScores[i].scoreName.CompareTo(name)==0){
					totalScores[i].score = Crypting.EncryptFloat(Crypting.DecryptFloat(totalScores[i].score) + amount);
					foreach(UILabel labela in totalScores[i].scoreLabels)
						labela.text = GetScore(totalScores[i].scoreName).ToString();
				}
			}
		}

		//Za svaki leaderboard se postavlja vrednost odgovarajuceg skora na GameCenter
		public void SaveScoresToGC(){
			Debug.Log("Reporting score to Game Center");
			foreach (Leaderboard board in App.lb.leaderboards) {
				GameCenterManager.ReportScore((int)GetScore(board.scoreName), board.idIOS);
				Debug.Log ("Score reported to GC " + GetScore(board.scoreName).ToString());
			}
		}

		//Za svaki leaderboard se postavlja vrednost odgovarajuceg skora na GooglePlay

		public void SaveScoresToGP(){
			Debug.Log("Reporting score to GooglePlay");
			foreach (Leaderboard board in App.lb.leaderboards) {
				Social.ReportScore((int)GetScore(board.scoreName), board.idAND, (bool success) => {
					// handle success or failure
				});				
				Debug.Log ("Score reported to GooglePlay " + GetScore(board.scoreName).ToString());
			}
		}

		public void ScorePlus(float amount, string name){
			
			for(int i=0; i<scores.Count; i++){
				if(scores[i].scoreName.CompareTo(name)==0){
					scores[i].score = Crypting.EncryptFloat(Crypting.DecryptFloat(scores[i].score) + amount);
					foreach(UILabel labela in scores[i].scoreLabels)
						labela.text = GetScore(scores[i].scoreName).ToString();
				}
			}

		}

		public void ScoreMinus(float amount, string name){
		
			for(int i=0; i<scores.Count; i++){
				if(scores[i].scoreName.CompareTo(name)==0) {
					scores[i].score = Crypting.EncryptFloat(Crypting.DecryptFloat(scores[i].score) - amount);
					foreach(UILabel labela in scores[i].scoreLabels)
						labela.text = GetScore(scores[i].scoreName).ToString();
				}
			}
		}
		
		
		//Geteri i Seteri vracaju/postavljaju skor sa odgovarajucim imenom za na odgovarajucu vrednost "amount"
		public float GetScore(string name){
			for(int i=0; i<scores.Count; i++){
				if(scores[i].scoreName.CompareTo(name)==0)
					if(scores[i].isFloat) return Crypting.DecryptFloat(scores[i].score);
					else return (int) Crypting.DecryptFloat(scores[i].score);
			}
			return -1;
		}

		public void SetScore(float amount, string name){
			for(int i=0; i<scores.Count; i++){
				if(scores[i].scoreName.CompareTo(name)==0) 
                {
                    scores[i].score = Crypting.EncryptFloat(amount);
                    foreach (UILabel labela in scores[i].scoreLabels)
                        labela.text =amount.ToString();
                }
            }
		}


		public float GetBestScore(string name){
			for(int i=0; i<bestScores.Count; i++)
				if(bestScores[i].scoreName.CompareTo(name)==0) 
					if(bestScores[i].isFloat) return Crypting.DecryptFloat(bestScores[i].score);
					else return (int) Crypting.DecryptFloat(bestScores[i].score);
			return -1;
		}

		public void SetBestScores(List<Score> bs){
			for(int i = 0; i<bestScores.Count; i++){ 
				bestScores[i].score = bs[i].score;
				Debug.Log("Set best scores");
				Debug.Log(bestScores[i].scoreName + " " + bestScores[i].score);
			}
		}

		public void SetTotalScores(List<Score> bs){
			for(int i = 0; i<totalScores.Count; i++){ 
				totalScores[i].score = bs[i].score;
				Debug.Log("Set total scores");
				Debug.Log(totalScores[i].scoreName + " " + totalScores[i].score);
			}
		}


	}
}