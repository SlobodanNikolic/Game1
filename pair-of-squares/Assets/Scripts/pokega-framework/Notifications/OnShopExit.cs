using UnityEngine;
using System.Collections;

namespace Pokega
{
	public class OnShopExit : MonoBehaviour {

		void OnClick()
		{
			App.notif.SetExitedShopTime();
		}
	}
}