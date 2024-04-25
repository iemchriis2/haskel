//Michael Revit

#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;



/// <summary>
/// This menu handles stating the hazard type (if any) of HazardObjects.
/// </summary>
public class AnswerMenu : Menu {

	#region Static

	#region Variables

	//Singleton Variables
	private static AnswerMenu _singleton;

	//Event Variables
	public static EasyEvent CategorySelectedEvent = new EasyEvent("Answer Menu Category Selected");
	public static EasyEvent<bool> DisabledEvent = new EasyEvent<bool>("Answer Menu Disabled");

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
	private PhysicalButton _category0Button, _category1Button, _category2Button, _category3Button, _category4Button, _category5Button, _category6Button, _category7Button, _category8Button;
	[SerializeField]
	private PhysicalButton _cancelButton, _confirmButton;
	[SerializeField]
	private float _horizontalOffset, _verticalOffset;
	[SerializeField]
	private float _lookAtVerticalInfluence;
	[SerializeField]
	private AnimationCurve _canvasGroupAlphaCurve;
	[SerializeField]
	private float _confirmCooldownThreshold;
	[SerializeField]
	private TextMeshProUGUI _nameText;

	//Script Variables
	private float _smoothDampEval, _smoothDampVelocity;
	private Vector3 _posA, _posB;
	private CanvasGroup _canvasGroup;
	private FancyButton[] _categoryButtons;
	private bool[] _categoryButtonsIsDown;
	private int _selectedCategory = -1;
	private HazardObject _hazardObject;
	private bool _confirmCooldown;

	private AudioSource _audioData;

	#endregion



	#region Menu

	private void Awake() {
		_singleton = this;

		_categoryButtons = new FancyButton[] { _category0Button.GetComponent<FancyButton>(), _category1Button.GetComponent<FancyButton>(), _category2Button.GetComponent<FancyButton>(), _category3Button.GetComponent<FancyButton>(), _category4Button.GetComponent<FancyButton>(), _category5Button.GetComponent<FancyButton>(), _category6Button.GetComponent<FancyButton>(), _category7Button.GetComponent<FancyButton>(), _category8Button.GetComponent<FancyButton>() };
		_categoryButtonsIsDown = new bool[_categoryButtons.Length];

		_canvasGroup = GetComponent<CanvasGroup>();
	}



	protected override void Start() {
		_category0Button.ButtonDownEvent.Subscribe(this, () => EV_CategoryButtonDown(0));
		_category1Button.ButtonDownEvent.Subscribe(this, () => EV_CategoryButtonDown(1));
		_category2Button.ButtonDownEvent.Subscribe(this, () => EV_CategoryButtonDown(2));
		_category3Button.ButtonDownEvent.Subscribe(this, () => EV_CategoryButtonDown(3));
		_category4Button.ButtonDownEvent.Subscribe(this, () => EV_CategoryButtonDown(4));
		_category5Button.ButtonDownEvent.Subscribe(this, () => EV_CategoryButtonDown(5));
		_category6Button.ButtonDownEvent.Subscribe(this, () => EV_CategoryButtonDown(6));
		_category7Button.ButtonDownEvent.Subscribe(this, () => EV_CategoryButtonDown(7));
		_category8Button.ButtonDownEvent.Subscribe(this, () => EV_CategoryButtonDown(8));
		_category0Button.ButtonUpEvent.Subscribe(this, () => EV_CategoryButtonUp(0));
		_category1Button.ButtonUpEvent.Subscribe(this, () => EV_CategoryButtonUp(1));
		_category2Button.ButtonUpEvent.Subscribe(this, () => EV_CategoryButtonUp(2));
		_category3Button.ButtonUpEvent.Subscribe(this, () => EV_CategoryButtonUp(3));
		_category4Button.ButtonUpEvent.Subscribe(this, () => EV_CategoryButtonUp(4));
		_category5Button.ButtonUpEvent.Subscribe(this, () => EV_CategoryButtonUp(5));
		_category6Button.ButtonUpEvent.Subscribe(this, () => EV_CategoryButtonUp(6));
		_category7Button.ButtonUpEvent.Subscribe(this, () => EV_CategoryButtonUp(7));
		_category8Button.ButtonUpEvent.Subscribe(this, () => EV_CategoryButtonUp(8));
		//_cancelButton.ButtonUpEvent.Subscribe(this, EV_Cancel);
		_confirmButton.ButtonDownEvent.Subscribe(this, EV_ConfirmButtonDown);
		_confirmButton.ButtonUpEvent.Subscribe(this, EV_ConfirmButtonUp);
		Player.InteractEvent.Subscribe(this, EV_Interact);
		Player.TeleportEvent.Subscribe(this, EV_Teleport);

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

	private void E() {
		IsEnabled = true;
		gameObject.SetActive(true);

		//Calculate points
		Vector3 additive = Player.Singleton.HeadTransform.forward;
		additive = new Vector3(additive.x, 0, additive.z).normalized * _horizontalOffset;
		additive += new Vector3(0, _verticalOffset, 0);
		_posB = Player.Singleton.HeadTransform.position + additive;
		_posA = _posB + Vector3.up * _translationStartOffset;
		
		//LookAt
		transform.position = _posB;
		Vector3 difference = transform.position - Player.Singleton.HeadTransform.position;
		transform.LookAt(transform.position + difference);

		//Set at starting point
		transform.position = _posA;
		_smoothDampEval = 0;

		//Set all buttons unstuck
		_selectedCategory = (int)_hazardObject.AnsweredType;
		for (int i = 0; i < _categoryButtons.Length; i++) {
			_categoryButtons[i].SetStuckState(i == _selectedCategory);
			_categoryButtons[i].ForceFastRender();
		}

		_nameText.text = _hazardObject.Name + "\n" + GetHazardTypeName();
	}



	private void D(bool wasQuestionAnswered) {
		if (!IsEnabled)
			return;
		IsEnabled = false;

		DisabledEvent.Invoke(wasQuestionAnswered);
	}

	#endregion



	private string GetHazardTypeName(bool useStuckButton = false) {
		switch ((HazardObject.HazardType)_selectedCategory) {
			case HazardObject.HazardType.HOUSEKEEPING_PPE:
				return "Housekeeping Hazard";
			case HazardObject.HazardType.ELEVATED_PLATFORMS:
				return "Platform Hazard";
			case HazardObject.HazardType.ELECTRICAL:
				return "Electrical Hazard";
			case HazardObject.HazardType.FIRE_PROTECTION:
				return "Fire Hazard";
			case HazardObject.HazardType.TRENCHING:
				return "Trenching Hazard";
			case HazardObject.HazardType.GUARDING:
				return "Guarding Hazard";
			case HazardObject.HazardType.WELDING:
				return "Welding Hazard";
			case HazardObject.HazardType.CRANES:
				return "Crane Hazard";
			case HazardObject.HazardType.NEUTRAL:
				return "No Hazard";
		}
		return "error";
	}



	#region Event Callbacks

	private void EV_Interact(HazardObject hazardObject) {
		_hazardObject = hazardObject;
		E();
	}



	private void EV_Teleport() {
		D(false);
	}



	private void EV_CategoryButtonDown(int index) {
		_categoryButtonsIsDown[index] = true;
		_audioData.Play();
		//OVRInput.SetControllerVibration(.75f, .75f, OVRInput.Controller.RTouch);
		//OVRInput.SetControllerVibration(.75f, .75f, OVRInput.Controller.LTouch);
		CalculateStuckButton();
		
		_nameText.text = _hazardObject.Name + "\n" + GetHazardTypeName();
	}
	
	
	
	private void EV_CategoryButtonUp(int index) {
		CalculateStuckButton();
		_categoryButtonsIsDown[index] = false;
		//_audioData.Play();
		OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
		OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);
		CategorySelectedEvent.Invoke();
	}



	private void EV_Cancel() {
		D(false);
	}



	private void EV_ConfirmButtonDown() {
		if (_smoothDampEval < _confirmCooldownThreshold)
			_confirmCooldown = true;
		_audioData.Play();
		OVRInput.SetControllerVibration(.75f, .75f, OVRInput.Controller.RTouch);
		OVRInput.SetControllerVibration(.75f, .75f, OVRInput.Controller.LTouch);
	}



	private void EV_ConfirmButtonUp() {
		if (_selectedCategory == -1)
			return;
		if (_confirmCooldown) {
			_confirmCooldown = false;
			return;
		}
		
		//_hazardObject?.Answer((HazardObject.HazardType)_selectedCategory);
		if (_hazardObject != null) {
			Debug.LogWarning("Calling HO.Answer");
			_hazardObject.Answer((HazardObject.HazardType)_selectedCategory);
		}
		_hazardObject = null;//Set null so it cannot be reactivated when the menu is in the disable transition
		//_audioData.Play();
		OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch);
		OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.LTouch);

		D(true);
	}

	#endregion



	#region Internal

	private void CalculateStuckButton() {
		for (int i = 0; i < _categoryButtonsIsDown.Length; i++)
			_categoryButtons[i].SetStuckState(false);
		
		int onlyDown = -1;
		for (int i = 0; i < _categoryButtonsIsDown.Length; i++) {
			if (_categoryButtonsIsDown[i]) {
				if (onlyDown == -1)
					onlyDown = i;
				else
					return;
			}
		}
		if (onlyDown == -1)
			return;
		_categoryButtons[onlyDown].SetStuckState(true);
		_selectedCategory = onlyDown;
	}

	#endregion

	#endregion

}
