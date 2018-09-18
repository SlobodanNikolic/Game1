using UnityEngine;
using System.Collections;
using Pokega;
//stefan edit
//using GameAnalyticsSDK;

namespace Pokega{

	public class PaymentControl : MonoBehaviour {
		
		
		//Ovo mi treba zbog uporedjivanja kasnije
		//public const string SOME_COINS;
		
		void Awake() {
			//We are asigning productId-s so we can access them from BuyMe.cs script
			//products [0] = SOME_COINS;

			//for (int i=0; i<storeItems.Length; i++) {
			//	IOSInAppPurchaseManager.Instance.addProductId(storeItems[i].productId);
			//}

			//IOSInAppPurchaseManager.Instance.addProductId(SAVE_ME);
			//IOSInAppPurchaseManager.Instance.addProductId(REMOVE_ADS);


			
		}
		
		void Start(){
            //stefan edit
			//IOSInAppPurchaseManager.Instance.OnStoreKitInitComplete += OnStoreKitInitComplete;
			//IOSInAppPurchaseManager.Instance.OnTransactionComplete += OnTransactionComplete;
			
			//IOSInAppPurchaseManager.Instance.loadStore();
		}

		public static void buyItem(string productId) {
            //stefan edit
            //IOSInAppPurchaseManager.Instance.buyProduct(productId);
            //IOSNativeUtility.ShowPreloader();
        }

        //stefan edit
        /*private static void OnStoreKitInitComplete (ISN_Result result) {
			IOSInAppPurchaseManager.Instance.OnStoreKitInitComplete -= OnStoreKitInitComplete;
			
			if(result.IsSucceeded) {
				Debug.Log("Inited successfully, Avaliable products count: " + IOSInAppPurchaseManager.Instance.products.Count.ToString());
			} else {
				Debug.Log("Failed to init Store Kit :(");
			}
		}*/

        //stefan edit
        /*private void OnTransactionComplete (IOSStoreKitResponse responce) {
			
			IOSNativeUtility.HidePreloader();
			
			Debug.Log("OnTransactionComplete: " + responce.productIdentifier);
			Debug.Log("OnTransactionComplete: state: " + responce.state);
			
			switch(responce.state) {
			case InAppPurchaseState.Purchased:
			case InAppPurchaseState.Restored:
				Debug.Log("STORE KIT GOT BUY: " + responce.productIdentifier);
				Debug.Log("RECIPT: " + responce.receipt);
				
				switch(responce.productIdentifier) {
				case "com.pokega.framework.shop.somecoins":
					
					 // NOTE: this part of code is executing after successful purchase or restore of product
					 //      here is the example of quick kick code
					  
					App.player.diamondCount+=Crypting.EncryptInt(10);
					Debug.Log("Purchase successful, Sack Of Diamonds bought");
					App.analytics.CreateAnalyticEvent("PaymentBoughtSackOfDiamonds", 1);

					break;

				case "com.pokega.framework.shop.somediamonds":
					
					// NOTE: this part of code is executing after successful purchase or restore of product
					//       here is the example of quick kick code
					  
					
					Debug.Log("Purchase successful, Sack Of Diamonds bought");
					App.analytics.CreateAnalyticEvent("PaymentBoughtSackOfDiamonds", 1);
					
					break;
				}
				
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
		
	}
}