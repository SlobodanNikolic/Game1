using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Pokega{
	
[System.Serializable]

	public class Score{
	
		public string scoreName;
		public bool isFloat;

		//Kriptovani skor
		public string score;

		//Lista labela u kojima se prikazuje skor
		public List<UILabel> scoreLabels;

		public Score(float sc){
			score = Crypting.EncryptFloat(sc);
			
		}

		public Score(string name, float sc, bool isItFloat){
			score = Crypting.EncryptFloat(sc);
			scoreName = name;
			isFloat = isItFloat;
		}

	}
}