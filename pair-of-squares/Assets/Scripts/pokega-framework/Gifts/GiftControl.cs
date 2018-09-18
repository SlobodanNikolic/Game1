using UnityEngine;
using System.Collections;
//using Facebook.MiniJSON;

namespace Pokega
{
	public class GiftControl : MonoBehaviour 
	{
		//Koliko koina, dijamanata, ili itema moze da se dobije
		public int[] gettableAmountOfCoins;			//amounts
		public int[] gettableAmountOfDiamonds;		//amounts
		public string[] gettableCustomItems;		//custom item ids
		
		//Sprite za otvoren poklon i labela koja se vidi
		public UISprite openedGiftSprite;
		public UILabel openedGiftLabel;

		public GameObject okButton;

		//Dugmici na koje se klikne da se dobije gift
		public GameObject[] giftButtons;

		public bool canGetGift = false;

		void Awake()
		{
			CanGetGiftCheck();
		}

		void OnApplicationPause(bool pauseStatus) 
		{
			if (pauseStatus == false) //app is unpaused
				CanGetGiftCheck();
		}

		//Uvek vraca tacno zasad
		public bool CanGetGiftCheck()
		{
			//Treba da se odradi provera
			return canGetGift;
		}

		void GiftDelivered()
		{
			//set server (gift) status
		}

		public void OpenGiftPopup(){
		
		}

		public void CloseGiftPopUp(){
			

		}
		
		//Ako ima dostupan gift, enabluj mu dugmice
		public void CheckGiftButtons(){
			Debug.Log ("Can get gift check");
			//Ovo uvek vraca true za sada
			if (CanGetGiftCheck ()) {
				foreach (GameObject btn in giftButtons) {
					btn.SetActive (true);
				}
			} else {
				foreach (GameObject btn in giftButtons) {
					btn.SetActive (false);
				}
			}
		}

		//Uzima random gift i stavlja ga u coins, diamonds ili inventory
		public void GetGift()
		{
			int x,y;
			while(true)
			{
				x = Random.Range(0,3);
				if(x==0 && (gettableAmountOfCoins.Length > 0))
					break;
				else if(x==1 && (gettableAmountOfDiamonds.Length > 0))
					break;
				else if(x==2 && gettableCustomItems.Length > 0)
					break;
			}

			if(x == 0)
			{
				//he will get coins
				y = Random.Range(0, gettableAmountOfCoins.Length);
				App.player.coinsCount = Crypting.EncryptInt(Crypting.DecryptInt(App.player.coinsCount) + gettableAmountOfCoins[y]);
				//openedGiftLabel.text = gettableAmountOfCoins[y].ToString();

				CoinsItem coinItem = App.shop.GetComponentInChildren<CoinsItem>();
				//openedGiftSprite.atlas = coinItem.itemIconSpriteAtlas;
				//openedGiftSprite.spriteName = coinItem.itemIconSpriteName;
			}
			else if(x==1)
			{
				//he will get diamonds
				y = Random.Range(0, gettableAmountOfDiamonds.Length);
				App.player.diamondCount = Crypting.EncryptInt(Crypting.DecryptInt(App.player.diamondCount) + gettableAmountOfDiamonds[y]);
				//openedGiftLabel.text = gettableAmountOfDiamonds[y].ToString();

				DiamondItem diamItem = App.shop.GetComponentInChildren<DiamondItem>();
				//openedGiftSprite.atlas = diamItem.itemIconSpriteAtlas;
				//openedGiftSprite.spriteName = diamItem.itemIconSpriteName;
			}
			else
			{
				//he will get custom item
				while(true)
				{	
					y = Random.Range(0, gettableCustomItems.Length);
					GameObject customItemObj = App.shop.transform.Find("Custom items").Find(gettableCustomItems[y]).gameObject;
					CustomItem cI = customItemObj.GetComponent<CustomItem>();
					if(!cI.isBought)
					{
						//openedGiftLabel.text = cI.name;
						//openedGiftSprite.atlas = cI.itemIconSpriteAtlas;
						//openedGiftSprite.spriteName = cI.itemIconSpriteName;
						if(App.inv.bag.ContainsKey(gettableCustomItems[y])){
							int pom = int.Parse(App.inv.bag[gettableCustomItems[y]].ToString());
							pom++;
							App.inv.bag[gettableCustomItems[y]] = pom;
						}
						else App.inv.bag.Add(gettableCustomItems[y], 1);

						//SASTAVLJA SE INVENTAR OD BAG-a I EQUIPPED ITEMA
						App.inv.PutTogetherInventory();

						//ODMAH SE POSTAVLJA INVENTORY STRING U PP
						//PlayerPrefs.SetString("Inventory", Json.Serialize(App.inv.inventory));
						break;
					}
				}
			}

			okButton.SetActive (true);
			canGetGift = false;
			//Da javi serveru da je preuzet gift
		}
	}
}