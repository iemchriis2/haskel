//Michael Revit

#pragma warning disable 0649

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;



/// <summary>
/// This class is far being the detectee of raycast clicks (on 3D keyboard).
/// </summary>
public class RaycastButton : MonoBehaviour {
	
	#region Internal Definitions

	[Serializable]
	private class State {
		public SubState DefaultState;
		public bool HasShiftState;
		public SubState ShiftState;
	}



	[Serializable]
	private class SubState {
		public char Char;
		public bool HasDifferentRender;
		public string RenderString;
	}

	#endregion



	#region Instance

	#region Variables

	//Event Variables
	public EasyEvent ButtonDownEvent { get; private set; }
	public EasyEvent ButtonUpEvent { get; private set; }
	public EasyEvent HoverEnterEvent { get; private set; }
	public EasyEvent HoverExitEvent { get; private set; }

	//Preference Variables
	[SerializeField]
	private State[] _states;
	[SerializeField]
	private TextMeshProUGUI _text;

	//Script Variables
	public RectTransform RectTransform { get; private set; }
	private int _stateIndex;
	private bool _isShift;

	#endregion



	#region MonoBehaviour Implementation

	private void Awake() {
		RectTransform = transform as RectTransform;

		ButtonDownEvent = new EasyEvent("RaycastButton Down");
		ButtonUpEvent = new EasyEvent("RaycastButton Up");
		HoverEnterEvent = new EasyEvent("RaycastButton HoverEnter");
		HoverExitEvent = new EasyEvent("RaycastButton HoverExit");
	}



	private void Start() {
		Render();
	}



	// private void Update() {
		
	// }

	#endregion



	#region Render

	private void Render() {
		SubState s = _isShift ? _states[_stateIndex].ShiftState : _states[_stateIndex].DefaultState;
		_text.text = s.HasDifferentRender ? s.RenderString : s.Char.ToString();
	}

	#endregion



	#region Public Access

	public void SetState(int index, bool isShift) {
		_stateIndex = _states.Length <= index ? _states.Length - 1 : index;
		_isShift = isShift && _states[_stateIndex].HasShiftState;
		Render();
	}



	public void FlickDown() {
		ButtonDownEvent.Invoke();
		ButtonUpEvent.Invoke();
	}



	public void SetHover(bool isHovering) {
		if (isHovering)
			HoverEnterEvent.Invoke();
		else
			HoverExitEvent.Invoke();
	}



	public char GetOutput() {
		return _isShift ? _states[_stateIndex].ShiftState.Char : _states[_stateIndex].DefaultState.Char;
	}

	#endregion

	#endregion

}
