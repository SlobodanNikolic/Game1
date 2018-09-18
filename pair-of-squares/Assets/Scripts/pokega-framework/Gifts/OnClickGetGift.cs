using UnityEngine;
using System.Collections;

namespace Pokega
{
	public class OnClickGetGift : MonoBehaviour 
	{
		void OnClick()
		{
			App.gift.GetGift();
		}
	}
}