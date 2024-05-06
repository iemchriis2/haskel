//Michael Revit

#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;



/// <summary>
/// This is the confirmation menu for when the player presses 'done' when entering a name.
/// </summary>
public class NameConfirmationMenu : InteractionBase {

	#region Static

	#region Variables

	//Singleton Variable
	public static NameConfirmationMenu Singeton { get; private set; }
	//Event Variables
	public static EasyEvent RequestEvent = new EasyEvent("ScoreDoneMenu Request");
	#endregion



	#region Enable Disable

	public static void Enable(string name) {
		Singeton.gameObject.SetActive(true);
		if (string.IsNullOrWhiteSpace(name))
			Singeton.RenderBlankField();
		else
			Singeton.RenderNameField(name);
	}



	private static void Disable() {
		Singeton.gameObject.SetActive(false);
	}

	#endregion

	#endregion



	#region Instance

	#region Variables

	//Preference Variables
	[SerializeField]
	private TextMeshProUGUI[] _text;
	[SerializeField]
	private PhysicalButton _button, _buttonNo, _buttonYes;

	#endregion



	#region MonoBehaviour Implementation

	private void Awake() {
		Singeton = this;
	}



	private void Start() {
		KeyboardMenu.EnableDisableEvent.Subscribe(this, EV_KeyboardEnableDisableEvent);
		_button.ButtonUpEvent.Subscribe(this, EV_Button);
		_buttonNo.ButtonUpEvent.Subscribe(this, EV_Button_No);
		_buttonYes.ButtonUpEvent.Subscribe(this, EV_Button_Yes);
	//	PlayerInput.InteractInputUpEvent.Subscribe(this, EV_InteractDown);
	
		Enable(null);
		_rectTransform = transform as RectTransform;
		_halfBounds = new Vector2(_rectTransform.sizeDelta.x / 2, _rectTransform.sizeDelta.y / -2);
        KeyboardMenu.Enable(true);

    }



	private void Update() 
	{
		CalculateHoverButtons();
	}

	private void CalculateHoverButtons()
	{
		//If cursor is off menu, end current hover (if it exists), return
		if (!_cursor.gameObject.activeSelf)
		{
			StopCurrentHover();
			return;
		}

		//Check if we are hovering over a button
		if (CalculateHoverButton(_button))
			return;
		if (CalculateHoverButton(_buttonNo))
			return;
	
		if (CalculateHoverButton(_buttonYes))
			return;
		/*
		if (CalculateHoverButton(_spaceButton))
			return;
		if (CalculateHoverButton(_backspaceButton))
			return;
		*/
		//Stop current hover because getting this far means we are not hovering a button
		StopCurrentHover();
	}

	#endregion






	private void EV_InteractDown(bool isLeftHand)
	{
		Debug.Log("Button INterat");
		currentPhysicalButton.MainTrigger.Interact();
	//	if ((isLeftHand && !Player.Singleton.IsLeftHanded) || (!isLeftHand && Player.Singleton.IsLeftHanded))
			return;

		//currentPhysicalButton.MainTrigger.Interact();
	}

	#region Render

	private void RenderBlankField() {
		foreach (TextMeshProUGUI t in _text)
			t.text = "Please enter a name...";
		_button.gameObject.SetActive(true);
		_buttonNo.gameObject.SetActive(false);
		_buttonYes.gameObject.SetActive(false);
	}



	private void RenderNameField(string name) {
		foreach (TextMeshProUGUI t in _text)
			t.text = "Is the name \"" + name + "\" ok?";
		_button.gameObject.SetActive(false);
		_buttonNo.gameObject.SetActive(true);
		_buttonYes.gameObject.SetActive(true);
	}

	private void RenderCursor(Vector2 position)
	{
		_cursor.anchoredPosition = position;
	}

	#endregion



	#region Public Access

	public void SetRaycatHit(Vector3 position)
	{
		_cursor.gameObject.SetActive(true);

		Vector3 pos = transform.InverseTransformPoint(position);
		RenderCursor(new Vector2(pos.x, pos.y));
	}



	public void SetNoRaycastHit()
	{
		_cursor.gameObject.SetActive(false);
	}

	#endregion



	#region Event Callbacks

	private void EV_KeyboardEnableDisableEvent(bool isEnabled) {
		if (!isEnabled)
			Enable(KeyboardMenu.Name);
	}



	public void EV_Button() {
		if (!gameObject.activeSelf)
			return;
		Disable();
		KeyboardMenu.Enable(false);
		//ScoreDoneMenu.Enable();
	}



	public void EV_Button_No() {
		if (!gameObject.activeSelf)
			return;
		Disable();
		KeyboardMenu.Enable(false);
		//ScoreDoneMenu.Enable();
	}



	public void EV_Button_Yes() {
		if (!gameObject.activeSelf)
			return;
			
		Disable();
		RequestEvent.Invoke();
	}

	#endregion

	#endregion
	
}
