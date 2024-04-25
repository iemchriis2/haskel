//Michael Revit

#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;

using UnityEngine;



public class LookAt : MonoBehaviour {

	[SerializeField]
	private Transform _target;
	[SerializeField]
	private Vector3 _addition;
	[SerializeField]
	private bool _reverseX, _reverseY, _reverseZ;



	private void Update() {
		transform.LookAt(_target, _target.up);
		transform.eulerAngles += _addition;
		transform.eulerAngles = new Vector3(transform.eulerAngles.x * (_reverseX ? -1 : 1), transform.eulerAngles.y * (_reverseY ? -1 : 1), transform.eulerAngles.z * (_reverseZ ? -1 : 1));
	}

}
