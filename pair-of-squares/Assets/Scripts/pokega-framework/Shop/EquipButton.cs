using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pokega{

	public class EquipButton : MonoBehaviour {
	
		public BuyCustomItem customItemScript;
		public GameObject inventoryItem1;
		public GameObject inventoryItem2;
		public GameObject inventoryItem3;
		public GameObject inventoryItem4;
		public List<GameObject> inventoryItemList;
		public GameObject bla;
		public GameObject bla2;
	
		
		// Use this for initialization
		void Start () {
			InitializeScripts();
			inventoryItemList = new List<GameObject>();

		}
		
		void InitializeScripts(){
			customItemScript = this.gameObject.transform.parent.GetComponent<BuyCustomItem>();
		}
		
		// Update is called once per frame
		void Update () {
		
		}


		void OnClick(){

			//KADA SE KLIKNE NA EQUIP, DODAJU SE INVENTORY ITEMI U LISTU, ZATO STO NISU
			//DOSTUPNI NA POCETKU, A IZBEGAVA SE RUCNO PREVLACENJE KOMPONENTI
//			inventoryItem1 = GameObject.Find("InventoryItem1");
//			inventoryItem2 = GameObject.Find("InventoryItem2");
//			inventoryItem3 = GameObject.Find("InventoryItem3");
//
//			inventoryItemList.Add(inventoryItem1);
//			inventoryItemList.Add(inventoryItem2);
//			inventoryItemList.Add(inventoryItem3);
			
			//AKO IMAMO TAJ ITEM U BAGU
			if(App.inv.bag.ContainsKey(customItemScript.productId)){
				Debug.Log("Imamo ajtem u bagu");
				//AKO JE COUNT ITEMA VECI OD 0
				if(int.Parse(App.inv.bag[customItemScript.productId].ToString()) > 0){
					Debug.Log("imamo vise od 0 tog itema");
					//PRODJI KROZ LISTU I STAVI ITEM U PRVI SLOBODAN SLOT
					for(int i = 0; i < App.inv.equippedObjectList.Count; i++){
						Debug.Log("Idemo kroz listu inventory itema, index je " + i);
						//AKO SLOT NIJE ZAUZET
						if(!App.inv.equippedObjectList[i].GetComponent<InventoryItem>().equiped){
							Debug.Log("Naisli smo na prvi slobodan slot za ekvipovanje");
							//POSTAVI ODGOVARAJUCI SPRITE U SLOT
							//UISprite[] inventorySprites = inventoryItemList[i].GetComponentsInChildren<UISprite>();
							App.inv.equippedObjectList[i].transform.Find("IconSprite").GetComponent<UISprite>().spriteName = customItemScript.spriteName;
							App.inv.equippedObjectList[i].transform.Find("IconSprite").GetComponent<UISprite>().enabled = true;

							//KAZI DA JE SLOT SADA ZAUZET I KAZI MU STA JE U NJEMU
							App.inv.equippedObjectList[i].GetComponent<InventoryItem>().equiped = true;
							App.inv.equippedObjectList[i].GetComponent<InventoryItem>().productId = customItemScript.productId;
			
							//POSTAVI CUSTOM ITEM U LISTU EQUIPPED ITEMA
							App.inv.equippedCustomItemList[i] = GameObject.Find("Custom items").transform.Find(customItemScript.gameObject.name).gameObject.GetComponent<CustomItem>();
							Debug.Log("Postavili smo CustomItem skript u equipped custom items list u inventory skriptu");
							//KAZI INVENTORY SLOTU KOJA COUNT LABELA I COUNT SPRAJT SE ODNOSE NA OVAJ ITEM
							App.inv.equippedObjectList[i].GetComponent<InventoryItem>().countLabel = this.transform.parent.Find("CountLabel").gameObject.GetComponent<UILabel>();
							App.inv.equippedObjectList[i].GetComponent<InventoryItem>().countSprite = this.transform.parent.Find("CountSprite").gameObject.GetComponent<UISprite>();
							App.inv.equippedObjectList[i].GetComponent<InventoryItem>().customItemScript = customItemScript;
						
							Debug.Log("Povezali smo count label, count sprite i custom item skript u equipped object listi i prefabu");
							//POSTAVI ITEM U SLOT ZA POWERUP U GAME SKRINU
							App.inv.equippedObjectList[i].GetComponent<InventoryItem>().PutItemInPowerupSlot();
						
							Debug.Log("Stavili smo item u powerup slot");
							
							//skinuli smo jedan item iz baga
							int bagCount = int.Parse(App.inv.bag[customItemScript.productId].ToString());
							bagCount--;
							App.inv.bag[customItemScript.productId] = bagCount;

							this.transform.parent.Find("CountLabel").gameObject.GetComponent<UILabel>().text = bagCount.ToString();

							if(bagCount == 0){
								this.transform.parent.Find("CountLabel").gameObject.GetComponent<UILabel>().enabled = false;
								this.transform.parent.Find("CountSprite").gameObject.GetComponent<UISprite>().enabled = false;
							}
							
							i = App.inv.equippedObjectList.Count;
						}
				
					}


				}
			}
			
			
			

			 
			
		}
	}
}