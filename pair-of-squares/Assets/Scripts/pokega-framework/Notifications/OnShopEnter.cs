using UnityEngine;
using System.Collections;

namespace Pokega
{
	public class OnShopEnter : MonoBehaviour {

		void OnClick()
		{
			App.notif.SetEnteredShopTime();
		}
	}
}
