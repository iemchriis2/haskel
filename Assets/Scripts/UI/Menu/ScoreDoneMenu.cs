//Michael Reivt/

#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;

using UnityEngine;



/// <summary>
/// This menu lets the user decide when they are done looking at the output score.
/// </summary>
public class ScoreDoneMenu : Menu {

	#region Static

	#region Variables

	//Singleton Variable
	public static ScoreDoneMenu Singleton { get; private set; }

	//Event Variables
	public static EasyEvent RequestEvent = new EasyEvent("ScoreDoneMenu Request");

	#endregion



	#region Enable Disable

	public static void Enable() {
		Singleton.gameObject.SetActive(true);
	}



	public static void Disable() {
		Singleton.gameObject.SetActive(false);
	}

	#endregion

	#endregion



	#region Instance

	#region Variables

	//Event Variables
	//public static EasyEvent GameStartEvent = new EasyEvent("Game Start");

	//Preference Variables
	[SerializeField]
	private PhysicalButton _button;

	#endregion



	#region MonoBehaviour Implementation

	private void Awake() {
		Singleton = this;
	}



	protected override void Start() {
		EndMenu.GameEndEvent.Subscribe(this, EV_GameEndEvent);
		_button.ButtonUpEvent.Subscribe(this, EV_Button);

		Disable();
	}

	#endregion



	#region Event Callbacks

	private void EV_GameEndEvent() {
		Enable();
	}



	private void EV_Button() {
		Disable();
		//KeyboardMenu.Disable();
		//NameConfirmationMenu.Enable(KeyboardMenu.Name);
		RequestEvent.Invoke();
	}

	#endregion

	#endregion

}
