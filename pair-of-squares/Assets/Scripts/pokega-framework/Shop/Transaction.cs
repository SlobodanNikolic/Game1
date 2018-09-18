using UnityEngine;
using System.Collections;

namespace Pokega{
	public class Transaction{
	
		public string productId;
		public string productName;
		public string dateAndTime;
		public string coinsCount;
		public string diamondCount;
		public bool isOneTime;
		public CustomItem customItem;
		public CoinsItem coinsItem;
		public DiamondItem diamondItem;
		public static int transactionNumber;

		public Transaction(){
			dateAndTime = System.DateTime.Now.ToString();
			coinsCount = App.player.coinsCount;
			diamondCount = App.player.diamondCount;
			transactionNumber++;
		}

		public string Print(){
			return "Purchase: product ID - " + productId + " Product name - " + productName + " Date and Time - " + dateAndTime + " Player coins - " 
					+ coinsCount + " Player diamonds - " + diamondCount
					+ " Is this a one time purchase - " + isOneTime.ToString() + " Transaction number " + transactionNumber.ToString();
		}
	}
}