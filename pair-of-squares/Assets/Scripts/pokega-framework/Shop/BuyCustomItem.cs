using UnityEngine;
using System.Collections;
//using Facebook.MiniJSON;
//edit stefan

namespace Pokega{

	public class BuyCustomItem : MonoBehaviour {

		public string productId;
		public string diamondCount;
		public string coinsCount; 
		public string spriteName;
		public App aplication;


		void Awake(){
			aplication = GameObject.Find("Aplication").GetComponent<App>();
		}

		void Start(){
			//NIZ SPRAJTOVA NA SHOP ITEM PREFABU
			UISprite[] spriteArray = this.gameObject.GetComponentsInChildren<UISprite>();
			spriteName = spriteArray[1].spriteName;			
		}

		//FUNKCIJA ZA PROVERU DA LI ITEM MOZE DA SE KUPI
		public void PreBuyCheck() {
			if (CanBuy ())
				Buy ();
			else
				NotEnough();
		}

		bool CanBuy(){
			//PROVERA DA LI PLAYER IMA DOVOLJNO DIJAMANATA I COINA
			if (Crypting.DecryptInt(coinsCount) <= Crypting.DecryptInt(App.player.coinsCount) && Crypting.DecryptInt(diamondCount) <= Crypting.DecryptInt(App.player.diamondCount))
				return true;
			else{
				Debug.Log("Cant buy item, not enough coins or diamonds");
				return false;
			}
		}

		void Buy(){
			Debug.Log (App.player.coinsCount + " BUY, Coins and diamond count " + App.player.diamondCount);

			//ODUZIMA SE ODGOVARAJUCI BROJ COINA I DIJAMANATA
			if(Crypting.DecryptInt(diamondCount) != 0)	App.player.diamondCount = Crypting.EncryptInt(Crypting.DecryptInt(App.player.diamondCount) - Crypting.DecryptInt(diamondCount));
			if(Crypting.DecryptInt(coinsCount)!=0)	App.player.coinsCount = Crypting.EncryptInt(Crypting.DecryptInt(App.player.coinsCount) - Crypting.DecryptInt(coinsCount));			//Checking for errors
			
			//DODAJE SE ITEM U BAG ILI SE POVECAVA NJEGOV COUNT
			if(App.inv.bag.ContainsKey(productId)){
				int pom = int.Parse(App.inv.bag[productId].ToString());
				pom++;
				App.inv.bag[productId] = pom;
			}
			else App.inv.bag.Add(productId, 1);

			//SASTAVLJA SE INVENTAR OD BAG-a I EQUIPPED ITEMA
			App.inv.PutTogetherInventory();

			//ODMAH SE POSTAVLJA INVENTORY STRING U PP
			//PlayerPrefs.SetString("Inventory", Json.Serialize(App.inv.inventory));

			//ZOVE SE ON PURCHASE FUNKCIJA U SHOP CONTROLE
			App.shop.OnPurchase(productId);
		
			//UKLJUCUJE SE COUNT SPRITE NA ITEMU I LABELA POSTAVLJA NA ODGOVARAJUCU VREDNOST
			ShowCount();
		}

		void NotEnough(){
			App.ui.SetPopUp("UI Not Enough Coins");
		}
	
		void ShowCount(){
			//UILabel[] nizLabela = this.gameObject.GetComponentsInChildren<UILabel>();
			//Debug.Log(inventory.bag[productId].ToString());
			this.gameObject.transform.Find("CountLabel").GetComponent<UILabel>().enabled = true;
			this.gameObject.transform.Find("CountLabel").GetComponent<UILabel>().text = App.inv.bag[productId].ToString();
			//UISprite[] nizSprajtova = this.gameObject.GetComponentsInChildren<UISprite>();
			this.gameObject.transform.Find("CountSprite").GetComponent<UISprite>().enabled = true;
		}
	}
}