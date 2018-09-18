using UnityEngine;
using System.Collections;

namespace Pokega 
{

	public class Device : MonoBehaviour {

		


		public string iOSGen;
		public string deviceName;
		public enum DeviceType {I_PHONE, I_PAD, I_POD, UNKNOWN};
		public DeviceType deviceType;
		public string deviceOS;
		public string apnToken;


		void Awake () 
		{
			//Ako je iOS, vidi koji device koristi i koji operativni sistem
			deviceOS = SystemInfo.operatingSystem;
			
			#if UNITY_IOS

			UnityEngine.iOS.DeviceGeneration _iOSGen = UnityEngine.iOS.Device.generation;
			
			iOSGen = _iOSGen.ToString();
			deviceName  = SystemInfo.deviceModel;
			
			switch(_iOSGen){
				case UnityEngine.iOS.DeviceGeneration.iPhone3GS:
				case UnityEngine.iOS.DeviceGeneration.iPhone4:
				case UnityEngine.iOS.DeviceGeneration.iPhone4S:
				case UnityEngine.iOS.DeviceGeneration.iPhone5:
				case UnityEngine.iOS.DeviceGeneration.iPhone5C:
				case UnityEngine.iOS.DeviceGeneration.iPhone5S:
				case UnityEngine.iOS.DeviceGeneration.iPhone6:
				case UnityEngine.iOS.DeviceGeneration.iPhone6Plus:
				case UnityEngine.iOS.DeviceGeneration.iPhoneUnknown:
					deviceType = DeviceType.I_PHONE;
					break;

				case UnityEngine.iOS.DeviceGeneration.iPad1Gen:
				case UnityEngine.iOS.DeviceGeneration.iPad2Gen:
				case UnityEngine.iOS.DeviceGeneration.iPad3Gen:
				case UnityEngine.iOS.DeviceGeneration.iPad4Gen:
				case UnityEngine.iOS.DeviceGeneration.iPadMini1Gen:
				case UnityEngine.iOS.DeviceGeneration.iPadMini2Gen:
				case UnityEngine.iOS.DeviceGeneration.iPadMini3Gen:
				case UnityEngine.iOS.DeviceGeneration.iPadAir1:
				case UnityEngine.iOS.DeviceGeneration.iPadAir2:
				case UnityEngine.iOS.DeviceGeneration.iPadUnknown:
					deviceType = DeviceType.I_PAD;
					break;

				case UnityEngine.iOS.DeviceGeneration.iPodTouch1Gen:
				case UnityEngine.iOS.DeviceGeneration.iPodTouch2Gen:
				case UnityEngine.iOS.DeviceGeneration.iPodTouch3Gen:
				case UnityEngine.iOS.DeviceGeneration.iPodTouch4Gen:
				case UnityEngine.iOS.DeviceGeneration.iPodTouch5Gen:
				case UnityEngine.iOS.DeviceGeneration.iPodTouchUnknown:
					deviceType = DeviceType.I_POD;
					break;

				default:
					deviceType = DeviceType.UNKNOWN;
					break;
			}

			//Invoke ("DebugInfo", 2.0f);
			#endif
		}


		void DebugInfo()
		{
			Debug.Log("iOS_GEN: " + iOSGen + " ||| " + "DEVICE: " + deviceName + " ||| " + "DEVICE TYPE: " + deviceType.ToString());
		}
		
		public void SetApnToken(string token)
		{
			apnToken = token;
			Debug.Log("Apn Token: " + token);
			App.local.PlayerSave();
		}
				

	}
}
