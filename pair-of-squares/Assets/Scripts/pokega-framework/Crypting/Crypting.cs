using UnityEngine;
using System.Collections;

namespace Pokega{

	//Klasa koja se bavi kriptovanjem
	public class Crypting : MonoBehaviour{
	
		public static int salt = 25;
		public static float floatSalt = 25f;

		//Kriptovanje inta
		public static string EncryptInt(int num) {
			return (((num + salt) * salt) + salt).ToString();
		}
		
		public static int DecryptInt(string num) {
			if(num != null){
				int n = System.Int32.Parse(num);
				return (((n - salt) / salt) - salt);
			}
			else return -1;
		}
		
		//Kriptovanje floata
		public static string EncryptFloat(float num) {
			return (((num + salt) * salt) + salt).ToString();
		}
		
		public static float DecryptFloat(string num) {
			if(num != null){
				float n = System.Single.Parse(num);
				return (((n - salt) / salt) - salt);
			}
			else return -1;
		}

	}
}