//Michael Revit

#pragma warning disable 0649

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;



/// <summary>
/// This class handles black-screen-fades when transitioning between scenes, teleporting, turning, and more.
/// </summary>
public class Fade : MonoBehaviour {

	#region Static

	#region Variables

	//Singleton Variable
	private static Fade _singleton;

	#endregion



	#region Public Access

	public static void FadeToBlack(Action action, float speedMultiplier = 1, bool isLockedInDark = false) {
		_singleton._actionList.Add(action);
		_singleton._speedMultiplier = speedMultiplier;
		_singleton._isLockedInDark = isLockedInDark;
	}



	public static void Unlock() {
		_singleton._isLockedInDark = false;
	}

	#endregion

	#endregion



	#region Instance

	#region Variables

	//Preference Variables
	[SerializeField]
	private float _smoothDampTime;
	[SerializeField]
	private Image _image;

	//Script Variables
	private List<Action> _actionList = new List<Action>();
	private float _speedMultiplier;
	private bool _isLockedInDark, _isInitializing = true;
	private float _evaluation, _smoothDampVelocity, _extraTime;

	#endregion



	#region MonoBehaviour Implementation

	private void Awake() {
		_singleton = this;
	}



	private IEnumerator Start() {
		yield return new WaitForSeconds(1.25f);
		
		float time = 1f;
		while (time > 0) {
			yield return null;
			time -= Time.deltaTime;
			_image.color = new Color(0, 0, 0, ((time / 1f)));
		}
		_isInitializing = false;
	}



	private void Update() {
		if (_isInitializing)
			return;

		//Get evaluation
		if (!(_isLockedInDark && _evaluation == 1))
			_evaluation = Mathf.SmoothDamp(_evaluation, GetTarget(), ref _smoothDampVelocity, _smoothDampTime * _speedMultiplier);
		if (GetTarget() == 1 && _evaluation >= .995f)
			_evaluation = 1;
		if (GetTarget() == 0 && _evaluation <= .005f)
			_evaluation = 0;

		//Extra time
		if (_evaluation == 1 && _extraTime > 0) {
			_extraTime -= Time.deltaTime;
			if (_extraTime < 0)
				_extraTime = 0;
		}

		//Call actions
		if (_evaluation == 1 && _extraTime <= 0 && _actionList.Count != 0) {
			List<Action> tempList = new List<Action>(_actionList);
			_actionList.Clear();
			foreach (Action a in tempList)
				a?.Invoke();
		}

		//Color
		_image.color = new Color(0, 0, 0, _evaluation);
	}

	#endregion



	#region Internal

	private int GetTarget() {
		return _actionList.Count == 0 ? 0 : 1;
	}

	#endregion

	#endregion

}
