//Michael Revit

#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;



/// <summary>
/// This menu handles stating the hazard type (if any) of HazardObjects.
/// </summary>
public class ReturnHomeConfirmationMenu : Menu {

	#region Static

	#region Variables

	//Singleton Variables
	public static ReturnHomeConfirmationMenu Singleton { get; private set; }

	//Event Variables
	public static EasyEvent ReturnHomeRequestEvent = new EasyEvent("Answer Menu Category Selected");

	//State Variables
	public static bool IsEnabled { get; private set; }

	#endregion

	#endregion



	#region Instance

	#region Variables

	//Preference Variables
	[SerializeField]
	private float _smoothDampTime;
	[SerializeField]
	private float _translationStartOffset;
	[SerializeField]
	private PhysicalButton _noButton, _yesButton;
	[SerializeField]
	private float _horizontalOffset, _verticalOffset;
	[SerializeField]
	private float _lookAtVerticalInfluence;
	[SerializeField]
	private AnimationCurve _canvasGroupAlphaCurve;
	[SerializeField]
	private float _confirmCooldownThreshold;

	//Script Variables
	private float _smoothDampEval, _smoothDampVelocity;
	private Vector3 _posA, _posB;
	private CanvasGroup _canvasGroup;
	private bool _confirmCooldown;

	private AudioSource _audioData;

	#endregion



	#region Menu

	private void Awake() {
		Singleton = this;

		_canvasGroup = GetComponent<CanvasGroup>();
	}



	protected override void Start() {
		_noButton.ButtonDownEvent.Subscribe(this, EV_NoButtonDown);
		_noButton.ButtonUpEvent.Subscribe(this, EV_NoButtonUp);
		_yesButton.ButtonDownEvent.Subscribe(this, EV_YesButtonDown);
		_yesButton.ButtonUpEvent.Subscribe(this, EV_YesButtonUp);
		//Player.TeleportEvent.Subscribe(this, EV_Teleport);

		_audioData = GetComponent<AudioSource>();

		_canvasGroup.alpha = 0;
		gameObject.SetActive(false);
	}



	private void Update() {
		// if ((!_isEnabled && _smoothDampEval == 0) || (_isEnabled && _smoothDampEval == 1))
		// 	return;

		_smoothDampEval = Mathf.SmoothDamp(_smoothDampEval, IsEnabled ? 1 : 0, ref _smoothDampVelocity, _smoothDampTime);
		// if (_smoothDampEval < .01f)
		// 	_smoothDampEval = 0;
		// else if (_smoothDampEval > .99f)
		// 	_smoothDampEval = 1;

		transform.position = Vector3.Lerp(_posA, _posB, _smoothDampEval);
		_canvasGroup.alpha = _canvasGroupAlphaCurve.Evaluate(_smoothDampEval);

		if (!IsEnabled && _smoothDampEval == 0)
			gameObject.SetActive(false);
	}

	#endregion



	#region Enable Disable

	public void E() {
		IsEnabled = true;
		gameObject.SetActive(true);

		//Calculate points
		//Vector3 additive = Player.Singleton.HeadTransform.forward;
		//additive = new Vector3(additive.x, 0, additive.z).normalized * _horizontalOffset;
		//additive += new Vector3(0, _verticalOffset, 0);
		//_posB = Player.Singleton.HeadTransform.position + additive;
		_posA = _posB + Vector3.up * _translationStartOffset;
		
		//LookAt
		transform.position = _posB;
		Vector3 difference = transform.position;//- Player.Singleton.HeadTransform.position;
		transform.LookAt(transform.position + difference);

		//Set at starting point
		transform.position = _posA;
		_smoothDampEval = 0;
	}



	private void D() {
		if (!IsEnabled)
			return;
		IsEnabled = false;
	}

	#endregion



	#region Event Callbacks

	private void EV_Teleport() {
		D();
	}



	private void EV_NoButtonDown() {
		if (_smoothDampEval < _confirmCooldownThreshold)
			_confirmCooldown = true;
	}
	
	
	
	private void EV_NoButtonUp() {
		if (_confirmCooldown) {
			_confirmCooldown = false;
			return;
		}
		D();
	}



	private void EV_YesButtonDown() {
		if (_smoothDampEval < _confirmCooldownThreshold)
			_confirmCooldown = true;
	}



	private void EV_YesButtonUp() {
		if (_confirmCooldown) {
			_confirmCooldown = false;
			return;
		}
		ReturnHomeRequestEvent.Invoke();
		D();
	}

	#endregion

	#endregion

}
