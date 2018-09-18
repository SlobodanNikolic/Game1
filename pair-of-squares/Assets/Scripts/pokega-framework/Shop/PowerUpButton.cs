using UnityEngine;
using System.Collections;
//using Facebook.MiniJSON;
//edit stefan
namespace Pokega
{

	public class PowerUpButton : MonoBehaviour {
	
		public BuyCustomItem buyCustomItemScript;
		public string productId;
		public bool armed;
		public InventoryItem inventoryItemScript;
		public GameObject inventoryItemObject;
		public UISprite inventoryItemSprite;
		public int positionIndex;

		public UISprite timerForeground;
		public UISprite timerBackground;

		public float timerSpeed;
		public float timerWait;

		public float fill;

		public bool area;
		public bool slowTime;
		public bool shield;

		public float areaTimePassed;
		public float slowTimePassed;
		public float shieldTimePassed;

		public delegate void PowerUpAction (string productId, PowerUpButton powupButton);
		public static event PowerUpAction PowerUpActivated;


		void OnEnable(){

		}

		void FixedUpdate(){

			if (area) {

				areaTimePassed += Time.deltaTime / Time.timeScale;
				timerForeground.fillAmount = 1f - (areaTimePassed / 10f);
				if(timerForeground.fillAmount <= 0f){
					//InitializationHelper.effects.DestroyAreaBullets();
					areaTimePassed = 0f;
					DisableSprite();
					area = false;
				}
				
			}
			else if (slowTime) {
				
				slowTimePassed += Time.deltaTime / Time.timeScale;
				timerForeground.fillAmount = 1f - (slowTimePassed / 10f);
				if(timerForeground.fillAmount <= 0f){
					//InitializationHelper.effects.StartBulletTime();
					slowTimePassed = 0f;
					DisableSprite();
					slowTime = false;
				}
				
			}
			else if (shield) {
				
				shieldTimePassed += Time.deltaTime / Time.timeScale;
				timerForeground.fillAmount = 1f - (shieldTimePassed / 10f);
				if(timerForeground.fillAmount <= 0f){
					//InitializationHelper.effects.ShieldOff();
					shieldTimePassed = 0f;
					DisableSprite();
					shield = false;
				}
				
			}

		}

		void OnClick()
		{
			if(armed)
			{
				Debug.Log("On click");
				if(PowerUpActivated!=null)
					PowerUpActivated(buyCustomItemScript.productId, gameObject.GetComponent<PowerUpButton>());
				EnablePowerup(productId);
				armed = false;
				inventoryItemScript.equiped = false;
				
				inventoryItemSprite.enabled = false;

				buyCustomItemScript = null;
				inventoryItemScript.productId = "";
				productId = "";

				inventoryItemScript.countLabel = null;
				App.inv.equippedCustomItemList[positionIndex] = null;

				App.inv.PutTogetherInventory();				
				//PlayerPrefs.SetString("Inventory", Json.Serialize(App.inv.inventory));
				PlayerPrefs.Save();				
			}
		}

		public void EnableSprite(){

			this.gameObject.GetComponent<UISprite>().spriteName = buyCustomItemScript.spriteName;
			this.gameObject.GetComponent<UISprite>().enabled = true;
			timerForeground.enabled = true;
			timerForeground.fillAmount = 1f;
			timerBackground.enabled = true;

		}

		public void DisableSprite(){
			Debug.Log ("Disabling sprite");
			timerForeground.fillAmount = 1f;

			gameObject.GetComponent<UISprite> ().enabled = false;
			timerForeground.enabled = false;
			timerBackground.enabled = false;
		}

		public void EnablePowerup(string id){
			switch(id) {
				
			case "com.pokega.pingpunk.shop.slowtime":
				slowTime = true;
				break;
				
			case "com.pokega.pingpunk.shop.shield":

				shield = true;
				break;
				
			case "com.pokega.pingpunk.shop.areahysteria":
				Debug.Log("Enable powerup");
				area = true;
				break;

			case "com.pokega.pingpunk.shop.doubletrouble":
				Debug.Log("Enable powerup");
				DisableSprite();
				break;

			case "com.pokega.pingpunk.shop.bouncer":
				Debug.Log("Enable powerup");
				DisableSprite();
				break;
			}
		}

		public void DisableEffects(){
			if (area) {
				area = false;
				areaTimePassed = 0f;
				DisableSprite();

			} else if (slowTime) {
				slowTime = false;
				slowTimePassed = 0f;
				DisableSprite();

			} else if (shield) {
				shield = false;
				shieldTimePassed = 0f;
				DisableSprite();
			}
		}

	}
}