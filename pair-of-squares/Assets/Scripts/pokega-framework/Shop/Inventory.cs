using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pokega{
	public class Inventory : MonoBehaviour {
	
		public Dictionary<string, object> inventory;
		public Dictionary<string, object> bag;
	
//		public CustomItem equippedItem1;
//		public CustomItem equippedItem2;
//		public CustomItem equippedItem3;
		//public CustomItem equippedItem4;
	
		//Broj slotova za equipovanje je count ove liste, podesiti rucno
		public List<GameObject> equippedObjectList;
		public List<InventoryItem> equippedInventoryItemList;
		public List<CustomItem> equippedCustomItemList;

		// Use this for initialization
		void Start () {
//			Debug.Log (inventory);
			if (inventory == null) {
				inventory = new Dictionary<string, object> ();
//				Debug.Log("INVENTORY WAS NULL, reinitialized");
			}

			if (bag == null) {
				bag = new Dictionary<string, object> ();
//				Debug.Log("BAG WAS NULL, reinitialized");
			}

//			Debug.Log (inventory);
//			equippedItemList = new List<CustomItem>();
			
//			equippedItemList.Add(equippedItem1);
//			equippedItemList.Add(equippedItem2);
//			equippedItemList.Add(equippedItem3);
			//equippedItemList.Add(equippedItem4);

			//OSPOSOBITI OVO ZBOG GRESKE

//			foreach(GameObject obj in equippedObjectList)
//				equippedCustomItemList.Add(obj);

		}
		
		// Update is called once per frame
		void Update () {
			if(Input.GetKeyDown(KeyCode.I)){

				Debug.Log("**********INVENTORY*********");
				foreach(string kljuc in inventory.Keys)
					Debug.Log(kljuc + "  " + inventory[kljuc].ToString());
				
				Debug.Log("**********INVENTORY*********");


				Debug.Log("**********BAG*********");
				foreach(string kljuc in bag.Keys)
					Debug.Log(kljuc + "  " + bag[kljuc].ToString());
				
				Debug.Log("**********BAG*********");

				Debug.Log("**********EQUIPPED*********");
				foreach(CustomItem ajtem in equippedCustomItemList)
					if(ajtem != null)
						Debug.Log(ajtem.productId);
				
				Debug.Log("**********EQUIPPED*********");
			}
		}

		public void InitializeBag(){
			bag = new Dictionary<string, object>();
			foreach(string kljuc in inventory.Keys){
				bag[kljuc] = inventory[kljuc];
			}
		}

		public void PutTogetherInventory(){
			
			inventory = new Dictionary<string, object>();
//			Debug.Log(inventory);
//			Debug.Log(bag);
//			Debug.Log(equippedCustomItemList);
			
			foreach(string kljuc in bag.Keys){
				if(inventory.ContainsKey(kljuc)){
					inventory[kljuc] = bag[kljuc];
				}
				else{
					inventory.Add(kljuc, bag[kljuc]);
				}
			}

			foreach(CustomItem ajtem in equippedCustomItemList){
				if(ajtem != null){
					if(inventory.ContainsKey(ajtem.productId)){
						int pom = int.Parse(inventory[ajtem.productId].ToString());
						pom++;
						inventory[ajtem.productId] = pom;
					}
					else{
						inventory.Add(ajtem.productId, 1);
					}
				}
			}
		}
	}
}