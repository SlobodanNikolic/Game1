#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

namespace Pokega{

	public class ProbniMeni : EditorWindow{
		
		public string diamondCount;
		public string diamondItemName;
		public string diamondProductId;
		public string diamondItemLevel;
		public UIAtlas diamondBackSpriteAtlas;
		public string diamondBackSpriteName;
		public UIAtlas diamondItemIconSpriteAtlas;
		public string diamondItemIconSpriteName;
		public UIAtlas diamondContentSpriteAtlas;
		public string diamondContentSpriteName;


		public string coinsCount;
		public string coinsItemName;
		public string coinsProductId;
		public string coinsItemLevel;
		public UIAtlas coinsBackSpriteAtlas;
		public string coinsBackSpriteName;
		public UIAtlas coinsItemIconSpriteAtlas;
		public string coinsItemIconSpriteName;
		public UIAtlas coinsContentSpriteAtlas;
		public string coinsContentSpriteName;

		public string CustomItemDiamondCount;
		public string CustomItemCoinsCount;
		public string CustomItemName;
		public string CustomItemProductId;
		public string itemItemLevel;
		public UIAtlas itemBackSpriteAtlas;
		public string itemBackSpriteName;
		public UIAtlas CustomItemSpriteAtlas;
		public string CustomItemIconSpriteName;
		public UIAtlas itemContentSpriteAtlas;
		public string itemContentSpriteName;
		public bool itemIsOneTime;

		public GameObject customItemPrefab;
		public GameObject coinsItemPrefab;
		public GameObject diamondItemPrefab;

		public GameObject item;
		public GameObject shop;
		public GameObject diamonds;
		public GameObject coins;
		public GameObject otherItems;

		public GameObject storeItem;
		public static GameObject shopObject;
		public static GameObject storeObject;

		private Vector2 scrollPosition;

		[MenuItem ("Shop Editor/Open Shop Editor")]
		private static void OpenShopEditor(){
			if(GameObject.Find("ShopGrid") != null)
				shopObject = GameObject.Find("ShopGrid");
			else
				shopObject = GameObject.Find(Selection.activeGameObject.name);
			if(shopObject == null)
				Debug.LogError("You need to have a Game Object named 'ShopGrid' active in hierarchy");
			

			//Nalazenje grida za itunes kupovinu
			if(GameObject.Find("StoreGrid") != null)
				storeObject = GameObject.Find("StoreGrid");
			
			if(storeObject == null)
				Debug.LogError("You need to have a Game Object named 'StoreGrid' active in hierarchy");



			EditorWindow.GetWindow<ProbniMeni> (true, "Item Maker");

		}

		[MenuItem ("Shop Editor/Open Shop Editor", true)]
		private static bool ShowEditorValidator(){
			return true;
		}

		public void makeHierarchy(){

			if(GameObject.Find("Shop") == null){
				shop = new GameObject("Shop");
				shop.AddComponent<ShopControl> ();
			}
			else shop = GameObject.Find("Shop");


			if(GameObject.Find("Diamond items") == null){
				diamonds = new GameObject("Diamond items");
				diamonds.transform.SetParent(shop.transform);
			}
			else diamonds = GameObject.Find("Diamond items");

			
			if(GameObject.Find("Coin items") == null){
				coins = new GameObject("Coin items");
				coins.transform.SetParent(shop.transform);
			}
			else coins = GameObject.Find("Coin items");

			if(GameObject.Find("Custom items") == null){
				otherItems = new GameObject("Custom items");
				otherItems.transform.SetParent(shop.transform);
			}
			else otherItems = GameObject.Find("Custom items");


			//Ponavljamo deo iz open shop editor buttona
			if(GameObject.Find("ShopGrid") != null)
				shopObject = GameObject.Find("ShopGrid");
			else
				shopObject = GameObject.Find(Selection.activeGameObject.name);
			if(shopObject == null)
				Debug.LogError("You need to have a Game Object named 'ShopGrid' active in hierarchy");
			
			
			//Nalazenje grida za itunes kupovinu
			if(GameObject.Find("StoreGrid") != null)
				storeObject = GameObject.Find("StoreGrid");
			
			if(storeObject == null)
				Debug.LogError("You need to have a Game Object named 'StoreGrid' active in hierarchy");


		}

		void makeDiamondObject(){
			diamonds = GameObject.Find("Diamond items");
			item = new GameObject(diamondItemName);
			item.transform.SetParent(diamonds.transform);
			item.AddComponent<DiamondItem>();
			DiamondItem ajtem = item.GetComponent<DiamondItem>();
			ajtem.backSpriteAtlas = diamondBackSpriteAtlas;
			ajtem.backSpriteName = diamondBackSpriteName;
			try
			{
				ajtem.diamondCount = Crypting.EncryptInt(Int32.Parse(diamondCount));
			}
			catch (FormatException)
			{
				return;
			}
			ajtem.contentSpriteAtlas = diamondContentSpriteAtlas;
			ajtem.contentSpriteName = diamondContentSpriteName;
			ajtem.itemIconSpriteAtlas = diamondItemIconSpriteAtlas;
			ajtem.itemIconSpriteName = diamondItemIconSpriteName;
			ajtem.itemLevel = diamondItemLevel;
			ajtem.itemName = diamondItemName;
			ajtem.productId = diamondProductId;
			
			storeItem = (GameObject)Instantiate(diamondItemPrefab, Vector3.zero, Quaternion.identity);
			Debug.Log(shopObject.name);
			storeItem.transform.SetParent(shopObject.transform);
			storeItem.transform.localScale=new Vector3(1, 1, 1);
			storeItem.name = diamondItemName;

			storeItem.AddComponent<BuyMe>();
			storeItem.GetComponent<BuyMe> ().productId = ajtem.productId;
			shop.GetComponent<ShopControl> ().storeItemIds.Add (ajtem.productId);
			shop.GetComponent<ShopControl> ().diamondItems.Add (ajtem);
			

//			UISprite[] sprajtovi = storeItem.GetComponentsInChildren<UISprite>();
//			sprajtovi[0].spriteName = diamondBackSpriteName;
//			sprajtovi[1].spriteName = diamondItemIconSpriteName;
//			sprajtovi[2].spriteName = diamondContentSpriteName;
//			
//			UILabel[] labele = storeItem.GetComponentsInChildren<UILabel>();
//			labele[0].text = storeItem.name;
//			labele[1].text = ajtem.itemLevel;
//			labele[2].text = Crypting.DecryptInt(ajtem.diamondCount).ToString();

			storeItem.transform.Find("IconSprite").GetComponent<UISprite>().spriteName = diamondItemIconSpriteName;
			storeItem.transform.Find("CountLabel").GetComponent<UILabel>().text = "";
			storeItem.transform.Find("NameLabel").GetComponent<UILabel>().text = storeItem.name;
			storeItem.transform.Find("CostLabel").GetComponent<UILabel>().text = diamondCount + "d";
		}

		void makeCoinsObject(){
			coins = GameObject.Find("Coin items");
			item = new GameObject(coinsItemName);
			item.transform.SetParent(coins.transform);
			item.AddComponent<CoinsItem>();
			CoinsItem ajtem = item.GetComponent<CoinsItem>();
			ajtem.backSpriteAtlas = coinsBackSpriteAtlas;
			ajtem.backSpriteName = coinsBackSpriteName;
			try
			{
				ajtem.coinsCount = Crypting.EncryptInt(Int32.Parse(coinsCount));
			}
			catch (FormatException)
			{
				return;
			}
			ajtem.contentSpriteAtlas = coinsContentSpriteAtlas;
			ajtem.contentSpriteName = coinsContentSpriteName;
			ajtem.itemIconSpriteAtlas = coinsItemIconSpriteAtlas;
			ajtem.itemIconSpriteName = coinsItemIconSpriteName;
			ajtem.itemLevel = coinsItemLevel;
			ajtem.itemName = coinsItemName;
			ajtem.productId = coinsProductId;
			
			storeItem = (GameObject)Instantiate(coinsItemPrefab, Vector3.zero, Quaternion.identity);
			Debug.Log(storeObject.name);
			storeItem.transform.SetParent(storeObject.transform);
			storeItem.transform.localScale=new Vector3(1, 1, 1);
			storeItem.name = coinsItemName;

			storeItem.AddComponent<BuyMe>();
			storeItem.GetComponent<BuyMe> ().productId = ajtem.productId;
			shop.GetComponent<ShopControl> ().storeItemIds.Add (ajtem.productId);
			shop.GetComponent<ShopControl> ().coinsItems.Add (ajtem);
			
//			UISprite[] sprajtovi = storeItem.GetComponentsInChildren<UISprite>();
//			sprajtovi[0].spriteName = coinsBackSpriteName;
//			sprajtovi[1].spriteName = coinsItemIconSpriteName;
//			sprajtovi[2].spriteName = coinsContentSpriteName;
//			
//			UILabel[] labele = storeItem.GetComponentsInChildren<UILabel>();
//			labele[0].text = storeItem.name;
//			labele[1].text = ajtem.itemLevel;
//			labele[2].text = Crypting.DecryptInt(ajtem.coinsCount).ToString();
			storeItem.transform.Find("IconSprite").GetComponent<UISprite>().spriteName = coinsItemIconSpriteName;
			storeItem.transform.Find("CountLabel").GetComponent<UILabel>().text = "";
			storeItem.transform.Find("NameLabel").GetComponent<UILabel>().text = storeItem.name;
			storeItem.transform.Find("CostLabel").GetComponent<UILabel>().text = coinsCount + "c";


		}

		void makeCustomObject(){
			otherItems = GameObject.Find("Custom items");
			item = new GameObject(CustomItemName);
			item.transform.SetParent(otherItems.transform);
			item.AddComponent<CustomItem>();
			CustomItem ajtem = item.GetComponent<CustomItem>();
			//ajtem.backSpriteAtlas = itemBackSpriteAtlas;
			//ajtem.backSpriteName = itemBackSpriteName;
			try
			{
				ajtem.diamondCount = Crypting.EncryptInt(Int32.Parse(CustomItemDiamondCount));
			}
			catch (FormatException)
			{
				return;
			}
			try
			{
				ajtem.coinsCount = Crypting.EncryptInt(Int32.Parse(CustomItemCoinsCount));
			}
			catch (FormatException)
			{
				return;
			}
			//ajtem.contentSpriteAtlas = itemContentSpriteAtlas;
			//ajtem.contentSpriteName = itemContentSpriteName;
			ajtem.itemIconSpriteAtlas = CustomItemSpriteAtlas;
			ajtem.itemIconSpriteName = CustomItemIconSpriteName;
			//ajtem.itemLevel = itemItemLevel;
			ajtem.itemName = CustomItemName;
			ajtem.productId = CustomItemProductId;
			ajtem.isOneTime = itemIsOneTime;			

			storeItem = (GameObject)Instantiate(customItemPrefab, Vector3.zero, Quaternion.identity);
			Debug.Log(shopObject.name);
			storeItem.transform.SetParent(shopObject.transform);
			storeItem.transform.localScale=new Vector3(1, 1, 1);
			storeItem.name = CustomItemName;
			storeItem.AddComponent<BuyCustomItem> ();
			storeItem.GetComponent<BuyCustomItem> ().productId = ajtem.productId;
			storeItem.GetComponent<BuyCustomItem> ().diamondCount = ajtem.diamondCount;
			storeItem.GetComponent<BuyCustomItem> ().coinsCount = ajtem.coinsCount;
			
			UISprite[] sprajtovi = storeItem.GetComponentsInChildren<UISprite>();
			//Debug
			foreach(UISprite sprajt in storeItem.GetComponentsInChildren<UISprite>())
				Debug.Log(sprajt.spriteName);
			//UISprite[] sprajtovi = storeItem.GetComponentsInChildren<UISprite>();
			//sprajtovi[0].spriteName = itemBackSpriteName;
			storeItem.transform.Find("IconSprite").GetComponent<UISprite>().spriteName = CustomItemIconSpriteName;
			//sprajtovi[1].spriteName = CustomItemIconSpriteName;
			//sprajtovi[2].spriteName = itemContentSpriteName;
			
			//UILabel[] labele = storeItem.GetComponentsInChildren<UILabel>();
			storeItem.transform.Find("CountLabel").GetComponent<UILabel>().text = "";
			storeItem.transform.Find("NameLabel").GetComponent<UILabel>().text = storeItem.name;
			storeItem.transform.Find("CostLabel").GetComponent<UILabel>().text = CustomItemCoinsCount + "c";
			
			//labele[0].text = "";
			//labele[1].text = storeItem.name;
			//labele[2].text = CustomItemCoinsCount + "c";

			shop.GetComponent<ShopControl> ().customItems.Add (ajtem);
			//labele[2].text = ajtem.itemCount.ToString();
		}

		void OnGUI(){

			EditorGUILayout.LabelField ("Make or refresh Shop Hierarchy");

			if (GUILayout.Button ("Hierarchy check")){
				makeHierarchy();
			}

			EditorGUILayout.LabelField ("Make a Diamond item");

				diamondItemName = EditorGUILayout.TextField ("Item name", diamondItemName);
				diamondCount = EditorGUILayout.TextField ("Number of diamonds", diamondCount);
				diamondProductId = EditorGUILayout.TextField ("Product id", diamondProductId);
				diamondItemLevel = EditorGUILayout.TextField ("Item level", diamondItemLevel);
				diamondBackSpriteAtlas = (UIAtlas)EditorGUILayout.ObjectField("Back sprite atlas", diamondBackSpriteAtlas, typeof(UIAtlas), true);
				diamondBackSpriteName = EditorGUILayout.TextField ("Back sprite name", diamondBackSpriteName);
				diamondItemIconSpriteAtlas = (UIAtlas)EditorGUILayout.ObjectField("Icon sprite atlas", diamondItemIconSpriteAtlas, typeof(UIAtlas), true);
				diamondItemIconSpriteName = EditorGUILayout.TextField ("Item icon sprite name", diamondItemIconSpriteName);
				diamondItemPrefab = (GameObject)EditorGUILayout.ObjectField("Diamond item prefab", diamondItemPrefab, typeof(GameObject), true);
				diamondContentSpriteAtlas = (UIAtlas)EditorGUILayout.ObjectField("Content sprite atlas", diamondContentSpriteAtlas, typeof(UIAtlas), true);
				diamondContentSpriteName = EditorGUILayout.TextField ("Content sprite name", diamondContentSpriteName);

			if (GUILayout.Button ("Make item")){
				makeDiamondObject();
			}

			EditorGUILayout.LabelField ("Make a coins item");

			coinsItemName = EditorGUILayout.TextField ("Item name", coinsItemName);
			coinsCount = EditorGUILayout.TextField ("Number of coinss", coinsCount);
			coinsProductId = EditorGUILayout.TextField ("Product id", coinsProductId);
			coinsItemLevel = EditorGUILayout.TextField ("Item level", coinsItemLevel);
			coinsBackSpriteAtlas = (UIAtlas)EditorGUILayout.ObjectField("Back sprite atlas", coinsBackSpriteAtlas, typeof(UIAtlas), true);
			coinsBackSpriteName = EditorGUILayout.TextField ("Back sprite name", coinsBackSpriteName);
			coinsItemIconSpriteAtlas = (UIAtlas)EditorGUILayout.ObjectField("Icon sprite atlas", coinsItemIconSpriteAtlas, typeof(UIAtlas), true);
			coinsItemIconSpriteName = EditorGUILayout.TextField ("Item icon sprite name", coinsItemIconSpriteName);
			coinsItemPrefab = (GameObject)EditorGUILayout.ObjectField("Coins item prefab", coinsItemPrefab, typeof(GameObject), true);
			
			coinsContentSpriteAtlas = (UIAtlas)EditorGUILayout.ObjectField("Content sprite atlas", coinsContentSpriteAtlas, typeof(UIAtlas), true);
			coinsContentSpriteName = EditorGUILayout.TextField ("Content sprite name", coinsContentSpriteName);

			if (GUILayout.Button ("Make item")){
				makeCoinsObject();
			}

			EditorGUILayout.LabelField ("Make a custom shop item");

			CustomItemName = EditorGUILayout.TextField ("Item name", CustomItemName);
			CustomItemDiamondCount = EditorGUILayout.TextField ("Number of diamonds required", CustomItemDiamondCount);
			CustomItemCoinsCount = EditorGUILayout.TextField ("Number of coins required", CustomItemCoinsCount);
			CustomItemProductId = EditorGUILayout.TextField ("Product id", CustomItemProductId);
			//itemItemLevel = EditorGUILayout.TextField ("Item level", itemItemLevel);
			//itemBackSpriteAtlas = (UIAtlas)EditorGUILayout.ObjectField("Back sprite atlas", itemBackSpriteAtlas, typeof(UIAtlas), true);
			//itemBackSpriteName = EditorGUILayout.TextField ("Back sprite name", itemBackSpriteName);
			CustomItemSpriteAtlas = (UIAtlas)EditorGUILayout.ObjectField("Icon sprite atlas", CustomItemSpriteAtlas, typeof(UIAtlas), true);
			CustomItemIconSpriteName = EditorGUILayout.TextField ("Item icon sprite name", CustomItemIconSpriteName);
			customItemPrefab = (GameObject)EditorGUILayout.ObjectField("Custom item prefab", customItemPrefab, typeof(GameObject), true);
			//itemContentSpriteAtlas = (UIAtlas)EditorGUILayout.ObjectField("Content sprite atlas", itemContentSpriteAtlas, typeof(UIAtlas), true);
			//itemContentSpriteName = EditorGUILayout.TextField ("Content sprite name", itemContentSpriteName);
			itemIsOneTime = EditorGUILayout.Toggle("Is one time", itemIsOneTime);			

			if (GUILayout.Button ("Make item")){
				makeCustomObject();
			}

		}
	}

}
#endif