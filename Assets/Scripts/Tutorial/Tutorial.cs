//Michael Revit//

using System.Collections;
using System.Collections.Generic;

using UnityEngine;



/// <summary>
/// This class acts as a sequence system to walk the player through a tutorial that teaches them the fundamentals of the mechanics.
/// </summary>
public class Tutorial : MonoBehaviour {
	
	#region Internal Definitions

	private enum TutorialPhase {
		DISABLED,
		INTERACT,
		SELECT_CATEGORY,
		CONFIRM_CATEGORY,
		TELEPORT,
		END
	}

	#endregion



	#region Static

	#region Variables

	//Singleton Variable
	private static Tutorial Singleton;

	//Event Variables
	public static EasyEvent StartEvent = new EasyEvent("Tutorial Start");
	public static EasyEvent EndEvent = new EasyEvent("Tutorial End");
	public static EasyEvent RequestTeleportEvent = new EasyEvent("Request Teleport Menu");
	public static EasyEvent RequestEndMenuEvent = new EasyEvent("Request End Menu");

	#endregion



	#region Public Access

	public static void StartTutorial() {
		StartEvent.Invoke();

		Singleton.Enable_INTERACT();
	}



	public static void EndTutorial() {
		EndEvent.Invoke();
		
		Singleton.Disable();
	}

	#endregion

	#endregion



	#region Instance

	#region Variables

	//Preference Variables
	[SerializeField]
	private HazardObject[] _hazardObjects;
	[SerializeField]
	private GameObject _hazardObject;
	[SerializeField]
	private GameObject _interactLabel;
	[SerializeField]
	private GameObject _selectCategoryLabel;
	[SerializeField]
	private GameObject _confirmCategoryLabel;
	[SerializeField]
	private GameObject _teleportLabel;
	[SerializeField]
	private Transform _teleportArrow;
	[SerializeField]
	private GameObject _endLabel;
	[SerializeField]
	private float _arrowSpeed, _arrowHeight;
	
	//Script Variables
	private TutorialPhase _phase;
	private Vector3 _arrowDefaultPosition;

	#endregion



	#region MonoBehaviour Implementation

	private void Awake() {
		Singleton = this;

		_arrowDefaultPosition = _teleportArrow.position;
	}



	private void Start() {
		Player.InteractEvent.Subscribe(this, EV_Interact);
		AnswerMenu.CategorySelectedEvent.Subscribe(this, EV_AnswerMenuCategorySelected);
		AnswerMenu.DisabledEvent.Subscribe(this, EV_AnswerMenuDisable);
		Player.TeleportEvent.Subscribe(this, EV_Teleport);
		EndMenu.GameEndEvent.Subscribe(this, EV_GameEnd);

		Disable();
	}



	private void Update() {
		if (_teleportArrow.gameObject.activeSelf)
			_teleportArrow.transform.position = _arrowDefaultPosition + new Vector3(0, Mathf.Abs(Mathf.Sin(Time.time * _arrowSpeed)) * _arrowHeight, 0);
	}

	#endregion



	#region Event Callbacks

	private void EV_Interact(HazardObject hazardObject) {
		if (_phase == TutorialPhase.INTERACT)
			Enable_SELECT_CATEGORY();
	}



	private void EV_AnswerMenuCategorySelected() {
		if (_phase == TutorialPhase.SELECT_CATEGORY)
			Enable_CONFIRM_CATEGORY();
	}



	private void EV_AnswerMenuDisable(bool wasQuestionAnswered) {
		if (_phase == TutorialPhase.CONFIRM_CATEGORY)
			Enable_TELEPORT();
	}



	private void EV_Teleport() {
		if (_phase == TutorialPhase.TELEPORT)
			Enable_END();
	}



	private void EV_GameEnd() {
		Disable();
	}

	#endregion



	#region Coroutines

	private void Enable_INTERACT() {
		_phase = TutorialPhase.INTERACT;

		_hazardObject.SetActive(true);
		_interactLabel.SetActive(true);
	}



	private void Enable_SELECT_CATEGORY() {
		_phase = TutorialPhase.SELECT_CATEGORY;

		_interactLabel.SetActive(false);
		_selectCategoryLabel.SetActive(true);
	}



	private void Enable_CONFIRM_CATEGORY() {
		_phase = TutorialPhase.CONFIRM_CATEGORY;

		_selectCategoryLabel.SetActive(false);
		_confirmCategoryLabel.SetActive(true);
	}



	private void Enable_TELEPORT() {
		_phase = TutorialPhase.TELEPORT;

		RequestTeleportEvent.Invoke();

		_confirmCategoryLabel.SetActive(false);
		_teleportLabel.SetActive(true);
		_teleportArrow.gameObject.SetActive(true);
	}



	private void Enable_END() {
		_phase = TutorialPhase.END;

		RequestEndMenuEvent.Invoke();

		_teleportLabel.SetActive(false);
		_teleportArrow.gameObject.SetActive(false);
		_endLabel.SetActive(true);
	}



	private void Disable() {
		_phase = TutorialPhase.DISABLED;

		_hazardObject.SetActive(false);
		_interactLabel.SetActive(false);
		_selectCategoryLabel.SetActive(false);
		_confirmCategoryLabel.SetActive(false);
		_confirmCategoryLabel.SetActive(false);
		_teleportLabel.SetActive(false);
		_teleportArrow.gameObject.SetActive(false);
		_endLabel.SetActive(false);
	}

	#endregion

	#endregion

}
