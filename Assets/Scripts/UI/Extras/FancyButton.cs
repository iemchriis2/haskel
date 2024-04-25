//Michael Revit

#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;



/// <summary>
/// This class makes buttons have pretty animations of color, scale, and transformations for both physcial buttons and raycast buttons.
/// </summary>
public class FancyButton : MonoBehaviour {

	#region Variables

	//Preference Variables
	[SerializeField]
	private PhysicalButton _button;//Should be renamed but thatll mess up a lot of stuff in inspector
	[SerializeField]
	private RaycastButton _raycastButtonInput;

	[SerializeField]
	private Transform _graphicTransform;
	[SerializeField]
	private Image _graphicImage;
	[SerializeField]
	private float _smoothDampTime;
	[SerializeField]
	private float _downZPosition, _hoverZPosition;
	[SerializeField]
	private float _downScaleMultiplier, _hoverScaleMultiplier;
	[SerializeField]
	private Color _downColor, _hoverColor;
	[SerializeField]
	private bool _isQuickDown;

	//Script Variables
	private bool _isDown, _isHover;
	private float _smoothDampEval, _smoothDampVelocity;
	private float _defaultZPosition;
	private Color _defaultColor;
	private bool _isStuck;

	private float _translateVelocity, _translateLerpStart;
	private float _scaleMultiplierVelocity, _scaleLerpStart;
	private Color _colorLerpStart;
	private float _adjustedLerpTime;

	#endregion



	#region MonoBehaviour Implementation

	private void Awake() {
		_defaultZPosition = _graphicTransform.localPosition.z;
		_defaultColor = _colorLerpStart = _graphicImage.color;
	}



	private void Start() {
		if (_button != null) {
			_button.ButtonDownEvent.Subscribe(this, EV_Down);
			_button.ButtonUpEvent.Subscribe(this, EV_Up);
		} else if (_raycastButtonInput != null) {
			_raycastButtonInput.ButtonDownEvent.Subscribe(this, EV_Down);
			_raycastButtonInput.ButtonUpEvent.Subscribe(this, EV_Up);
			_raycastButtonInput.HoverEnterEvent.Subscribe(this, EV_HoverEnter);
			_raycastButtonInput.HoverExitEvent.Subscribe(this, EV_HoverExit);
		}
	}



	private void Update() {
		Render();
	}

	#endregion



	#region Public Access

	public void SetStuckState(bool isStuck) {
		_isStuck = isStuck;
	}



	public void ForceFastRender() {
		Render(true);
	}

	#endregion



	#region Render

	private void Render(bool isFastRender = false) {
		//Calculate
		if (isFastRender)
			_smoothDampEval = GetLerpTarget();
		else
			_smoothDampEval = Mathf.SmoothDamp(_smoothDampEval, /*GetLerpTarget()*/ 1, ref _smoothDampVelocity, _adjustedLerpTime);
		
		//Apply
		_graphicTransform.localPosition = new Vector3(_graphicTransform.localPosition.x, _graphicTransform.localPosition.y, Mathf.Lerp(_defaultZPosition, GetTranslateLerpTarget(), _smoothDampEval));
		transform.localScale = new Vector3(Mathf.Lerp(1, GetScaleLerpTarget(), _smoothDampEval), Mathf.Lerp(1, GetScaleLerpTarget(), _smoothDampEval), 1);
		_graphicImage.color = Color.Lerp(_colorLerpStart, GetColorLerpTarget(), _smoothDampEval);
	}

	#endregion



	#region Internal

	private float GetLerpTarget() {
		return _isDown ? 1 : (_isStuck ? .5f : 0);
	}



	private float GetTranslateLerpTarget() {
		if (_isDown)
			return _downZPosition;
		else if (_isHover)
			return _hoverZPosition;
		else if (_isStuck)
			return Mathf.Lerp(_downZPosition, _defaultZPosition, .5f);
		return _defaultZPosition;
	}



	private float GetScaleLerpTarget() {
		if (_isDown)
			return _downScaleMultiplier;
		else if (_isHover)
			return _hoverScaleMultiplier;
		else if (_isStuck)
			return Mathf.Lerp(_downScaleMultiplier, 1, .5f);
		return 1;
	}



	private Color GetColorLerpTarget() {
		if (_isDown)
			return _downColor;
		else if (_isHover)
			return _hoverColor;
		else if (_isStuck)
			return Color.Lerp(_downColor, _defaultColor, .5f);
		return _defaultColor;
	}



	private void StateChangeCalculations() {
		_translateLerpStart = _graphicTransform.localPosition.z;
		_scaleLerpStart = transform.localScale.x;
		_colorLerpStart = _graphicImage.color;
		_adjustedLerpTime = _smoothDampTime;
		_smoothDampEval = _smoothDampVelocity = 0;
	}

	#endregion



	#region Event Callbacks

	private void EV_Down() {
		StateChangeCalculations();
		_isDown = true;

		if (_isQuickDown) {
			_smoothDampEval = 1;
			_graphicTransform.localPosition = new Vector3(_graphicTransform.localPosition.x, _graphicTransform.localPosition.y, GetTranslateLerpTarget());
			transform.localScale = new Vector3(Mathf.Lerp(1, GetScaleLerpTarget(), _smoothDampEval), Mathf.Lerp(1, GetScaleLerpTarget(), _smoothDampEval), 1);
			_graphicImage.color = GetColorLerpTarget();
		}

		OVRInput.SetControllerVibration(.75f, .75f, OVRInput.Controller.RTouch);
		OVRInput.SetControllerVibration(.75f, .75f, OVRInput.Controller.LTouch);
	}



	private void EV_Up() {
		StateChangeCalculations();
		_isDown = false;
		OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
		OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
	}



	private void EV_HoverEnter() {
		StateChangeCalculations();
		_isHover = true;
	}



	private void EV_HoverExit() {
		StateChangeCalculations();
		_isHover = false;
	}

	#endregion
	
}
