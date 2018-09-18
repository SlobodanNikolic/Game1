using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using Facebook.MiniJSON;
//edit stefan
//using GameAnalyticsSDK;


namespace Pokega{

	public class ShopControl : MonoBehaviour {

		public delegate void ItemPurchased(string productId);
		public static event ItemPurchased ItemPurchasedEvent;
		

		public GameObject shopItemPrefab;
		public bool isDictionaryInitialized;
	
		//Lista item-a koja ce dinamicki da se menja
		public List<string> storeItemIds = new List<string>();
		public List<DiamondItem> diamondItems = new List<DiamondItem>();
		public List<CoinsItem> coinsItems = new List<CoinsItem>();
		public List<CustomItem> customItems = new List<CustomItem>();
		//public List<CustomItem>	oneTimeItems = new List<CustomItem>();
		public List<CustomItem> purchasedItems = new List<CustomItem>();
		public GameObject testObject;
		
		public List<UILabel> coinsLabels;

		public Dictionary<string, object> itemDictionary;

	
		void Awake() {
//			Debug.Log("<color=green>SHOP CONTROL AWAKE</color>");
			
			//We are asigning productId-s so we can access them from BuyMe.cs script
			//products [0] = SOME_COINS;
			
			//for (int i=0; i<storeItems.Length; i++) {
			//	IOSInAppPurchaseManager.Instance.addProductId(storeItems[i].productId);
			//}
			
			//IOSInAppPurchaseManager.Instance.addProductId(SAVE_ME);
			//IOSInAppPurchaseManager.Instance.addProductId(REMOVE_ADS);			

			for (int i=0; i<storeItemIds.Count; i++) {
//				Debug.Log(storeItemIds[i]);
                //edit stefan
				//IOSInAppPurchaseManager.Instance.addProductId(storeItemIds[i]);
			//	Debug.Log("Added shop item with ID: " + storeItemIds[i]);
			}
			
			
		}
		
		void Start(){
			App.shop.UpdateCoinsLabels(Crypting.DecryptInt(App.player.coinsCount).ToString());
            /*
			IOSInAppPurchaseManager.Instance.OnStoreKitInitComplete += OnStoreKitInitComplete;
			IOSInAppPurchaseManager.Instance.OnTransactionComplete += OnTransactionComplete;
			IOSInAppPurchaseManager.Instance.loadStore();
			*/
			//foreach(CustomItem ajtem in customItems)
			//	if(ajtem.isOneTime) oneTimeItems.Add(ajtem);
			
		}

		void Update(){
			
		}
        //edit stefan
        /*public static void buyItem(string productId) {
			IOSInAppPurchaseManager.Instance.buyProduct(productId);
			IOSNativeUtility.ShowPreloader();
		}

        private static void OnStoreKitInitComplete (ISN_Result result) {
			IOSInAppPurchaseManager.Instance.OnStoreKitInitComplete -= OnStoreKitInitComplete;
			
			if(result.IsSucceeded) {
				Debug.Log("Inited successfully, Avaliable products count: " + IOSInAppPurchaseManager.Instance.products.Count.ToString());
			} else {
				Debug.Log("Failed to init Store Kit :(");
			}
		}

        private void OnTransactionComplete (IOSStoreKitResponse responce) {
			
			IOSNativeUtility.HidePreloader();
			
			Debug.Log("OnTransactionComplete: " + responce.productIdentifier);
			Debug.Log("OnTransactionComplete: state: " + responce.state);
			
			switch(responce.state) {
				//TODO: razjasniti (non)-consumable razliku i kad se salje tag da je kupljeno, kad da je restored jer ima razlike!
			case InAppPurchaseState.Purchased:
			case InAppPurchaseState.Restored:
				Debug.Log("STORE KIT GOT BUY: " + responce.productIdentifier);
				Debug.Log("RECIPT: " + responce.receipt);
				
				//Pisanje koliko puta je user kupio odredjeni product - u PP i slanje OS tag-a na remote
				int timesPaid = PlayerPrefs.GetInt("timesPaid:" + responce.productIdentifier);
				PlayerPrefs.SetInt("timesPaid:" + responce.productIdentifier, (timesPaid + 1));
				//OneSignal.SendTag("timesPaid:" + responce.productIdentifier, (timesPaid + 1).ToString());
				
				OnPurchase(responce.productIdentifier);
				

				break;

			case InAppPurchaseState.Deferred:
				//iOS 8 introduces Ask to Buy, which lets 
				//parents approve any purchases initiated by children
				//You should update your UI to reflect this 
				//deferred state, and expect another Transaction 
				//Complete  to be called again with a new transaction state 
				//reflecting the parent's decision or after the 
				//transaction times out. Avoid blocking your UI 
				//or gameplay while waiting for the transaction to be updated.
				break;
			case InAppPurchaseState.Failed:
				//Our purchase flow is failed.
				//We can unlock interface and report user that the purchase is failed. 
				Debug.Log("Transaction failed with error, code: " + responce.error.code);
				Debug.Log("Transaction failed with error, description: " + responce.error.description);
				IOSNativePopUpManager.showMessage("Transaction failed", "product " + responce.productIdentifier + " state: " + responce.state.ToString());
				break;
			}
			
			IOSNativePopUpManager.showMessage("Store Kit Response", "product " + responce.productIdentifier + " state: " + responce.state.ToString(), "OK");
		}*/
		
		public void OnPurchase(string productIdentifier){			

			Debug.Log("ON PURCHASE FUNCTION STARTED, product id is " + productIdentifier);
			Transaction trans = new Transaction();
			
			for(int i=0; i<customItems.Count; i++){

				App.shop.UpdateCoinsLabels(Crypting.DecryptInt(App.player.coinsCount).ToString());

				if(customItems[i].productId.CompareTo(productIdentifier)==0){
					//Debug.Log("ON PURCHASE FUNCTION, FOR LOOP");
					trans.productName = customItems[i].itemName;
					trans.customItem = customItems[i];
					trans.isOneTime = customItems[i].isOneTime;
					trans.productId = productIdentifier;
					
					purchasedItems.Add(customItems[i]);
					
					Debug.Log(itemDictionary);					
			
					if(itemDictionary.ContainsKey(customItems[i].productId)){
						string broj =  itemDictionary[customItems[i].productId].ToString();
						int brojInt = System.Int32.Parse(broj);
						brojInt++;
						itemDictionary[customItems[i].productId] = brojInt;
						customItems[i].howMuchIsBought++;
					}
					else{
						customItems[i].howMuchIsBought++;
						itemDictionary.Add(customItems[i].productId, customItems[i].howMuchIsBought);
					}

					//PlayerPrefs.SetString("shop", Json.Serialize(itemDictionary));
					//for(int k=0; k<itemDictionary.Count; k++)
						//Debug.Log(itemDictionary.ElementAt(k).Value.ToString());
				

					//Ubacujemo u inventar
//					if(inventory.inventory.ContainsKey(customItems[i].productId)){
//						int count =  inventory.inventory[customItems[i].productId];
//						count++;
//						inventory.inventory[customItems[i].productId] = count;
//					}
//					else{
//						inventory.inventory.Add(customItems[i].productId, 1);
//					}


				}
			}

			for(int i=0; i<diamondItems.Count; i++){
				if(diamondItems[i].productId.CompareTo(productIdentifier)==0){
					Debug.Log("ON PURCHASE FUNCTION, FOR LOOP");
					trans.productName = diamondItems[i].itemName;
					trans.diamondItem = diamondItems[i];
					trans.productId = productIdentifier;
					trans.isOneTime = false;
					if(ItemPurchasedEvent != null)
						ItemPurchasedEvent(productIdentifier);
				}
			}

			for(int i=0; i<coinsItems.Count; i++){
				if(coinsItems[i].productId.CompareTo(productIdentifier)==0){
					Debug.Log("ON PURCHASE FUNCTION, FOR LOOP");
					trans.productName = coinsItems[i].itemName;
					trans.coinsItem = coinsItems[i];
					trans.productId = productIdentifier;
					trans.isOneTime = false;
					if(ItemPurchasedEvent != null)
						ItemPurchasedEvent(productIdentifier);
				}
			}
			
			App.server.SavePurchase(trans);
//			if(trans.isOneTime) serverAPIControl.SaveOneTimePurchase(trans);
			//itemEffect.ApplyEffect(productIdentifier);
			App.local.PlayerSave();
			App.server.Save();
			Debug.Log(trans.Print());

		}

		public void RegulateCounts(){
			Debug.Log ("Regulate counts function");
			GameObject shopGrid = GameObject.Find("ShopGrid");
			Debug.Log (shopGrid);
			foreach(Transform child in shopGrid.transform){
				string productId = child.gameObject.GetComponent<BuyCustomItem>().productId;
				Debug.Log(App.inv.inventory);
				if(App.inv.inventory.ContainsKey(productId)){
					int count = int.Parse(App.inv.inventory[productId].ToString());
					Debug.Log("Regulate Counts : Inventory contains id " + productId + " and its count is " + count.ToString());

					if(count>0){
						child.Find("CountSprite").GetComponent<UISprite>().enabled = true;
						child.Find("CountLabel").GetComponent<UILabel>().text = App.inv.inventory[productId].ToString();
					}
					else{
						child.Find("CountSprite").GetComponent<UISprite>().enabled = false;
						child.Find("CountLabel").GetComponent<UILabel>().text = "";
					}
				}
				
			}
		}

		public void UpdateCoinsLabels(string text){
			foreach (UILabel label in coinsLabels) {
				if(label != null)
					label.text = text;
			}
		}

		/*	
		public void RestoreOneTimeItems(){
			serverAPIControl.LoadOneTimePurchase();
		}
*/
    }
}
