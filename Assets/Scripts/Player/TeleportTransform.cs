//Michael Revit

#pragma warning disable 0649

using System.Collections.Generic;

using UnityEngine;



public class TeleportTransform : MonoBehaviour {

	#region Static

	#region Variables

	private static List<TeleportTransform> _instances = new List<TeleportTransform>();

	#endregion



	#region Public Access

	public static Transform GetTransformFromName(string name) {
		foreach (TeleportTransform t in _instances)
			if (t._name == name)
				return t.transform;
		return null;
	}

	#endregion

	#endregion



	#region Instance

	#region Variables

	//Preference Variables
	[SerializeField]
	private string _name;

	//Script Variables
	private bool _isClosing;

	#endregion



	#region MonoBehaviour Implementation

	private void Awake() {
		_instances.Add(this);
	}



	private void OnApplicationClose() {
		_isClosing = true;
	}



	private void OnDestroy() {
		if (_isClosing)
			return;
		_instances.Remove(this);
	}

	#endregion

	#endregion

}
