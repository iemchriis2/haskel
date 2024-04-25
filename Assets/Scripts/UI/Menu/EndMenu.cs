//Michael Revit

#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;



/// <summary>
/// This menu allos for the player to end the tuitorial or stage.
/// </summary>
public class EndMenu : Menu {

	#region Static

	#region Variables

	//Event Variables
	public static EasyEvent GameEndEvent = new EasyEvent("Game End");

	//Other
	public static string TimeString { get; private set; }

	#endregion

	#endregion



	#region Variables

	//Preference Variables
	[SerializeField]
	private PhysicalButton _endButton;
	[SerializeField]
	private TextMeshProUGUI[] _timerText;
	[SerializeField]
	private float _smoothDampTime;

	//Script Variables
	private float _startTime;
	private int _lastDisplayedTime;
	private bool _isRendering = true;
	private CanvasGroup _canvasGroup;
	private float _smoothDampEval, _smoothDampVelocity;
	private bool _isTutorialMode;

	#endregion



	#region MonoBehaviour Implementation

	private void Awake() {
		_canvasGroup = GetComponent<CanvasGroup>();
	}



	protected override void Start() {
		MainMenu.GameStartEvent.Subscribe(this, EV_GameStart);
		_endButton.ButtonUpEvent.Subscribe(this, EV_EndButton);
		Tutorial.StartEvent.Subscribe(this, EV_TutorialStart);
		Tutorial.RequestEndMenuEvent.Subscribe(this, EV_TutorialRequest);

		gameObject.SetActive(false);
	}



	private void Update() {
		//Timer
		int temp = Mathf.FloorToInt(Time.time - _startTime);
		if (temp != _lastDisplayedTime) {
			_lastDisplayedTime = temp;
			if (_smoothDampEval != 0)
				RenderTimer();
		}

		//Canvas group alpha lerp
		if (_smoothDampEval != 1) {
			_smoothDampEval = Mathf.SmoothDamp(_smoothDampEval, _isRendering ? 1 : 0, ref _smoothDampVelocity, _smoothDampTime);
			_canvasGroup.alpha = _smoothDampEval;
		}
	}

	#endregion



	#region Render

	private void RenderTimer() {
		for (int i = 0; i < _timerText.Length; i++)
			_timerText[i].text = FormatTime(_lastDisplayedTime);
	}

	#endregion



	#region Internal

	private string FormatTime(int seconds) {
		float minutes = 0;
		while (seconds >= 60) {
			minutes++;
			seconds -= 60;
		}
		return (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds;
	}

	#endregion



	#region Event Callbacks

	private void EV_GameStart() {
		gameObject.SetActive(true);

		_startTime = Time.time;
		RenderTimer();

		_isRendering = true;
		_canvasGroup.alpha = _smoothDampEval = 1;
		_isTutorialMode = false;
	}



	private void EV_EndButton() {
		gameObject.SetActive(false);

		TimeString = FormatTime(_lastDisplayedTime);
		if (_isTutorialMode)
			Tutorial.EndTutorial();
		else
			GameEndEvent.Invoke();
	}



	private void EV_TutorialStart() {
		gameObject.SetActive(true);

		_isRendering = false;
		_canvasGroup.alpha = _smoothDampEval = 0;
		_isTutorialMode = true;
	}



	private void EV_TutorialRequest() {
		_startTime = Time.time;
		RenderTimer();

		_isRendering = true;
	}

	#endregion
	
}
