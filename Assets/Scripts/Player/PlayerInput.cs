//Michael Revit, Bonga Maswanganye

#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Analytics;

using Oculus;



/// <summary>
/// Parses for VR input and turns it into events that other scripts can listen for.
/// </summary>
public class PlayerInput : MonoBehaviour {

	#region Static

	#region Variables

	//Static Event Variables
	public static EasyEvent<bool> TeleportInputDownEvent { get; private set; }//(0)bool = isLeftController
	public static EasyEvent<bool> TeleportInputUpEvent { get; private set; }//(0)bool = isLeftController
	public static EasyEvent<bool> InteractInputDownEvent { get; private set; }//(0)bool = isLeftController
	public static EasyEvent<bool> InteractInputUpEvent { get; private set; }//(0)bool = isLeftController
	public static EasyEvent<bool> ReturnHomeInputDownEvent { get; private set; }//(0)bool = isLeftController
	public static EasyEvent<bool> ReturnHomeInputUpEvent { get; private set; }//(0)bool = isLeftController
	public static EasyEvent<bool> SnapRotationEvent { get; private set; }//(0)bool = isClockwise

	#endregion

	#endregion



	#region Instance

	#region Variables

	//Preference Variables
	[SerializeField]
	private float _snapRotationActivationPercentage, _snapRotationDeactivationPercentage;

	//Script Variables
	//private bool _isTeleportDown, _isInteractDown;
	//private bool _isTeleportLeft, _isInteractLeft;
	private bool _isTeleportDownLeft, _isTeleportDownRight;
	private bool _isInteractDownLeft, _isInteractDownRight;
	private bool _isReturnHomeDownLeft, _isReturnHomeDownRight;
	private bool _isPrimarySnapRotationCooldownActive, _isSecondarySnapRotationCooldownActive;

	#endregion



	#region MonoBehaviour Implementation

	private void Awake() {
		TeleportInputDownEvent = new EasyEvent<bool>("TeleportInput Down");
		TeleportInputUpEvent = new EasyEvent<bool>("TeleportInput Up");
		InteractInputDownEvent = new EasyEvent<bool>("InteractInput Down");
		InteractInputUpEvent = new EasyEvent<bool>("InteractInput Up");
		ReturnHomeInputDownEvent = new EasyEvent<bool>("ReturnHomeInput Down");
		ReturnHomeInputUpEvent = new EasyEvent<bool>("ReturnHomeInput Up");
		SnapRotationEvent = new EasyEvent<bool>("SnapRotation");
	}



	private void Start() {
		
	}



	private void OnDestroy() {

	}



	private void Update() {
		//Force update
		OVRInput.Update();

		//Parse teleport input
		if (OVRInput.Get(OVRInput.RawButton.Y)) {
			if (!_isTeleportDownLeft) {
				_isTeleportDownLeft = true;
				TeleportInputDownEvent.Invoke(true);
			}
		} else if (_isTeleportDownLeft) {
			_isTeleportDownLeft = false;
			TeleportInputUpEvent.Invoke(true);
		}
		if (OVRInput.Get(OVRInput.RawButton.B)) {
			if (!_isTeleportDownRight) {
				_isTeleportDownRight = true;
				TeleportInputDownEvent.Invoke(false);
			}
		} else if (_isTeleportDownRight) {
			_isTeleportDownRight = false;
			TeleportInputUpEvent.Invoke(false);
		}

		//Parse interact input
		if (OVRInput.Get(OVRInput.RawButton.LIndexTrigger)) {
			if (!_isInteractDownLeft) {
				_isInteractDownLeft = true;
				InteractInputDownEvent.Invoke(true);
				OVRInput.SetControllerVibration(.5f, .5f, OVRInput.Controller.LTouch);
			}
		} else if (_isInteractDownLeft) {
			_isInteractDownLeft = false;
			InteractInputUpEvent.Invoke(true);
			OVRInput.SetControllerVibration(0,0, OVRInput.Controller.LTouch);
		}
		if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger)) {
			if (!_isInteractDownRight) {
				_isInteractDownRight = true;
				InteractInputDownEvent.Invoke(false);
				OVRInput.SetControllerVibration(.5f, .5f, OVRInput.Controller.RTouch);
			}
		} else if (_isInteractDownRight) {
			_isInteractDownRight = false;
			InteractInputUpEvent.Invoke(false);
			OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
		}

		//Parse return home input
		if (OVRInput.Get(OVRInput.RawButton.X)) {
			if (!_isReturnHomeDownLeft) {
				_isReturnHomeDownLeft = true;
				ReturnHomeInputDownEvent.Invoke(true);
			}
		} else if (_isReturnHomeDownLeft) {
			_isReturnHomeDownLeft = false;
			ReturnHomeInputUpEvent.Invoke(true);
		}
		if (OVRInput.Get(OVRInput.RawButton.A)) {
			if (!_isReturnHomeDownRight) {
				_isReturnHomeDownRight = true;
				ReturnHomeInputDownEvent.Invoke(false);
			}
		} else if (_isReturnHomeDownRight) {
			_isReturnHomeDownRight = false;
			ReturnHomeInputUpEvent.Invoke(false);
		}

		//Parse snap rotation input
		Vector2 primaryThumbstick = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick);
		if (_isPrimarySnapRotationCooldownActive && Mathf.Abs(primaryThumbstick.x) <= _snapRotationDeactivationPercentage) {
			_isPrimarySnapRotationCooldownActive = false;
		} else if (!_isPrimarySnapRotationCooldownActive && Mathf.Abs(primaryThumbstick.x) > _snapRotationActivationPercentage) {
			_isPrimarySnapRotationCooldownActive = true;
			SnapRotationEvent.Invoke(primaryThumbstick.x > 0);
		}
		Vector2 secondaryThumbstick = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);
		if (_isSecondarySnapRotationCooldownActive && Mathf.Abs(secondaryThumbstick.x) <= _snapRotationDeactivationPercentage) {
			_isSecondarySnapRotationCooldownActive = false;
		} else if (!_isSecondarySnapRotationCooldownActive && Mathf.Abs(secondaryThumbstick.x) > _snapRotationActivationPercentage) {
			_isSecondarySnapRotationCooldownActive = true;
			SnapRotationEvent.Invoke(secondaryThumbstick.x > 0);
		}
	}

	#endregion

	#endregion

}
