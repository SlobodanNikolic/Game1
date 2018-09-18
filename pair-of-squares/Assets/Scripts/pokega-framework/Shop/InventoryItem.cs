using UnityEngine;
using System.Collections;

namespace Pokega{

	public class InventoryItem : MonoBehaviour {
	
		public bool equiped;
		public string productId;
		public int index;
		public UILabel countLabel;
		public UISprite countSprite;
		public BuyCustomItem customItemScript;
		public PowerUpButton powerUpButtonScript;
		public GameObject powerUpGameObject;

		// Use this for initialization
		void Start () {
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		void OnClick(){
			UISprite[] sprites = this.gameObject.GetComponentsInChildren<UISprite>();
			sprites[1].enabled = false;
			App.inv.equippedCustomItemList[index] = null;
			if(equiped){
				equiped = false;
				//int pom = int.Parse(inventoryScript.inventory[productId].ToString());
				//pom++;
				//inventoryScript.inventory[productId] = pom;
				countSprite.enabled = true;
				//countLabel.text = inventoryScript.inventory[productId].ToString();
				productId = "";
				string num = countLabel.text;

				int numInt = int.Parse(countLabel.text);
				numInt++;
				countLabel.text = numInt.ToString();
				countLabel.enabled = true;
				countLabel = null;
				powerUpButtonScript.buyCustomItemScript = null;
				powerUpButtonScript.armed = false;
				powerUpGameObject.GetComponent<UISprite>().enabled = false;
				powerUpButtonScript.DisableSprite();

				int bagCount = int.Parse(App.inv.bag[customItemScript.productId].ToString());
				bagCount++;
				App.inv.bag[customItemScript.productId] = bagCount;
			}
		}

		public void PutItemInPowerupSlot(){
			Debug.Log("Put item ON");
			powerUpButtonScript.buyCustomItemScript = customItemScript;
			powerUpButtonScript.armed = true;
			
			powerUpButtonScript.inventoryItemScript = this.gameObject.GetComponent<InventoryItem>();
			powerUpButtonScript.EnableSprite();
			powerUpButtonScript.inventoryItemObject = this.gameObject;
			powerUpButtonScript.inventoryItemSprite = this.gameObject.transform.Find("IconSprite").gameObject.GetComponent<UISprite>();

			powerUpButtonScript.productId = customItemScript.productId;

		}
	}

}