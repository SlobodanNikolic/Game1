using UnityEngine;
using System.Collections;

namespace Pokega{

	//Interfejs koji sadrzi deklaracije funkcija za Save, Load, Reset
	public interface ILocal{
		
		void Save();
	
		void Load();

		void Reset();
	}

}