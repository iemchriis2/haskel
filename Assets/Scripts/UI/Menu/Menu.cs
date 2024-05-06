//Michael Revit

using System.Collections;
using System.Collections.Generic;

using UnityEngine;



/// <summary>
/// This is the base menu class that all menus derrive from.
/// </summary>
public class Menu : MonoBehaviour {

	protected virtual void Start() {
		//GetComponent<Canvas>().worldCamera = Player.Singleton.HeadTransform.GetComponent<Camera>();
	}

}
