//Michael Revit

#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;

using UnityEngine;



/// <summary>
/// This class uses PhysicalTriggerInteracts to create a button that is pressed in 3D space with a hand collider entering a trigger.
/// </summary>
public class PhysicalButton : MonoBehaviour {

	#region Variables

	//Event Variables
	public EasyEvent ButtonDownEvent { get; private set; }
	public EasyEvent ButtonUpEvent { get; private set; }

	//Preference Variables
	[SerializeField]
	private PhysicalTriggerInteract _mainTrigger, _cooldownTrigger;
	public PhysicalTriggerInteract MainTrigger { get { return _mainTrigger; } }
	public PhysicalTriggerInteract CooldownTrigger { get { return _cooldownTrigger; } }

	//Script Variables
	private bool _isMainTriggerOnly;
	private bool _isMainTriggerActivated, _isCooldownTriggerActivated;
	private bool _isDown, _isCooldownActive;

	#endregion



	#region MonoBehaviour Implementation

	private void Awake() {
		_isMainTriggerOnly = _cooldownTrigger == null;

		ButtonDownEvent = new EasyEvent("PhysicalButton Down");
		ButtonUpEvent = new EasyEvent("PhysicalButton Up");

		//Error checking
		if (_mainTrigger == null)
			Debug.LogError("Main trigger of a physical button is null!");
	}



	private void Start() {
		_mainTrigger.InteractionStartEvent.Subscribe(this, EV_MainTriggerStart);
		_mainTrigger.InteractionEndEvent.Subscribe(this, EV_MainTriggerEnd);
		if (_cooldownTrigger != null) {
			_cooldownTrigger.InteractionStartEvent.Subscribe(this, EV_CooldownTriggerStart);
			_cooldownTrigger.InteractionEndEvent.Subscribe(this, EV_CooldownTriggerEnd);
		}
	}



	private void Update() {
		
	}

	#endregion



	#region Event Callbacks

	private void EV_MainTriggerStart() {
		if (_isCooldownActive)
			return;
		
		if (!_isMainTriggerOnly)
			_isCooldownActive = true;
		ButtonDownEvent.Invoke();
	}



	private void EV_MainTriggerEnd() {
		ButtonUpEvent.Invoke();
	}



	private void EV_CooldownTriggerStart() {

	}



	private void EV_CooldownTriggerEnd() {
		_isCooldownActive = false;
	}

	#endregion

}
