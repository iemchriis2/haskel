//Michael Revit

#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;



/// <summary>
/// This menu allows for alphanumerical input (for typing name).
/// </summary>
public class KeyboardMenu : Menu {

	#region Static

	#region Variables

	//Singleton Variable
	public static KeyboardMenu Singleton { get; private set; }

	//Event Variables
	public static EasyEvent<bool> EnableDisableEvent = new EasyEvent<bool>("KeyboardMenu Enable/Disable");

	//Access
	public static string Name { get { return Singleton._inputField.text; } }

	#endregion



	#region Enable Disabe

	public static void Enable(bool resetText) {
		if (resetText)
			Singleton._inputField.text = "";
		Singleton.gameObject.SetActive(true);
		EnableDisableEvent.Invoke(true);
	}



	public static void Disable() {
		Singleton.gameObject.SetActive(false);
		EnableDisableEvent.Invoke(false);
	}

	#endregion

	#endregion



	#region Instance

	#region Variables

	//Preference Variables
	[SerializeField]
	private RectTransform _cursor;
	[SerializeField]
	private RaycastButton _shiftButton, _stateButton, _enterButton, _spaceButton, _backspaceButton;
	[SerializeField]
	private List<RaycastButton> _generalButtons;
	[SerializeField]
	private TMP_InputField _inputField;
	[SerializeField]
	private Image _shiftArrowImage;
	[SerializeField]
	private Color _shiftDisableColor, _shiftEnableColor;
	[SerializeField]
	private Transform[] _rows;

	//Script Variables
	public RaycastButton _currentHoveredButton;
	private bool _isShifted;
	private int _stateIndex;
	private RectTransform _rectTransform;
	private Vector2 _halfBounds;

	#endregion



	#region MonoBehaviour Implementation

	private void Awake() {
		Singleton = this;

		_rectTransform = transform as RectTransform;
		_halfBounds = new Vector2(_rectTransform.sizeDelta.x / 2, _rectTransform.sizeDelta.y / -2);
	}



	protected override void Start() {
		base.Start();

		//EndMenu.GameEndEvent.Subscribe(this, EV_GameEnd);
		//MainMenu.GameStartEvent.Subscribe(this, EV_GameStart);
		PlayerInput.InteractInputDownEvent.Subscribe(this, EV_InteractDown);

		foreach (RaycastButton b in _generalButtons)
			b.SetState(_stateIndex, _isShifted);

		StartCoroutine(StartPause());
	}



	private IEnumerator StartPause() {
		yield return null;
		yield return null;

		foreach (Transform t in _rows)
			while (t.childCount > 0)
				t.GetChild(0).SetParent(t.parent, false);

		_cursor.SetAsLastSibling();

		//Disable();
	}



	private void Update() {
		// if (Input.GetKeyDown(KeyCode.Alpha1)) {
		// 	_currentHoveredButton = _shiftButton;
		// 	CalculateClick();
		// } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
		// 	_currentHoveredButton = _stateButton;
		// 	CalculateClick();
		// } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
		// 	_currentHoveredButton = _generalButtons[15];
		// 	_currentHoveredButton.SetHover(true);
		// } else if (Input.GetKey(KeyCode.Alpha3)) {
		// 	//this stops the 'else' from running
		// } else {
		// 	StopCurrentHover();
		// }

		CalculateHoverButtons();
	}

	#endregion



	#region Render

	private void RenderCursor(Vector2 position) {
		_cursor.anchoredPosition = position;
	}

	#endregion



	#region Public Access

	public void SetRaycatHit(Vector3 position) {
		_cursor.gameObject.SetActive(true);

		Vector3 pos = transform.InverseTransformPoint(position);
		RenderCursor(new Vector2(pos.x, pos.y));
	}



	public void SetNoRaycastHit() {
		_cursor.gameObject.SetActive(false);
	}

	#endregion



	#region Internal

	private void CalculateHoverButtons() {
		//If cursor is off menu, end current hover (if it exists), return
		if (!_cursor.gameObject.activeSelf) {
			StopCurrentHover();
			return;
		}

		//Check if we are hovering over a button
		for (int i = 0; i < _generalButtons.Count; i++)
			if (CalculateHoverButton(_generalButtons[i]))
				return;
		if (CalculateHoverButton(_shiftButton))
			return;
		//if (CalculateHoverButton(_stateButton))
		//	return;
		if (CalculateHoverButton(_enterButton))
			return;
		if (CalculateHoverButton(_spaceButton))
			return;
		if (CalculateHoverButton(_backspaceButton))
			return;

		//Stop current hover because getting this far means we are not hovering a button
		StopCurrentHover();
	}



	private bool CalculateHoverButton(RaycastButton button) {
		Vector3 pos = _cursor.anchoredPosition + _halfBounds;
		if (pos.x > button.RectTransform.anchoredPosition.x && pos.x < button.RectTransform.anchoredPosition.x + button.RectTransform.sizeDelta.x) {
			if (pos.y > button.RectTransform.anchoredPosition.y && pos.y < button.RectTransform.anchoredPosition.y + button.RectTransform.sizeDelta.y) {
				
				if (_currentHoveredButton != button) {
					StopCurrentHover();

					_currentHoveredButton = button;
					_currentHoveredButton.SetHover(true);
				}

				return true;

			}
		}

		return false;
	}



	private void StopCurrentHover() {
		_currentHoveredButton?.SetHover(false);
		_currentHoveredButton = null;
	}



	private void CalculateClick() {
		if (_currentHoveredButton == null)
			return;

		_currentHoveredButton.FlickDown();

		if (_currentHoveredButton == _shiftButton) {
			_isShifted = !_isShifted;
			_shiftArrowImage.color = _isShifted ? _shiftEnableColor : _shiftDisableColor;
			foreach (RaycastButton b in _generalButtons)
				b.SetState(_stateIndex, _isShifted);
		} else if (_currentHoveredButton == _stateButton) {
			if (_stateIndex == 0)
				_stateIndex = 1;
			else if (_stateIndex == 1)
				_stateIndex = 0;
			_isShifted = false;
			foreach (RaycastButton b in _generalButtons)
				b.SetState(_stateIndex, _isShifted);
		} else if (_currentHoveredButton == _enterButton) {
			Disable();
		} else if (_currentHoveredButton == _spaceButton) {
			_inputField.text += " ";
		} else if (_currentHoveredButton == _backspaceButton) {
			if (_inputField.text.Length > 0)
				_inputField.text = _inputField.text.Substring(0, _inputField.text.Length - 1);
		} else {
			_inputField.text += _currentHoveredButton.GetOutput();
		}
	}

	#endregion



	#region Event Callbacks

	private void EV_GameEnd() {
		Enable(true);
	}



	private void EV_GameStart()
	{
		_inputField.text = null;
	}



	private void EV_InteractDown(bool isLeftHand) {
		if ((isLeftHand && !Player.Singleton.IsLeftHanded) || (!isLeftHand && Player.Singleton.IsLeftHanded))
			return;

		CalculateClick();
	}

	#endregion

	#endregion

}
