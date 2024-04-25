//Michael Revit

#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;



public class ScoreRenderHelper : MonoBehaviour {

	#region Variables

	//Preference Variables
	[SerializeField]
	private TextMeshProUGUI _nameText, _descriptionText;
	[SerializeField]
	private Color _unansweredColor, _correctColor, _incorrectColor;

	#endregion



	#region MonoBehaviour Implementation

	private void Start() {
		
	}



	private void Update() {
		
	}

	#endregion



	#region Render

	public void Render(ScoreMenu.RenderEntry obj) {
		gameObject.SetActive(true);

		_nameText.text = obj.Name + (obj.Count != 1 ? " (" + obj.Count + ")" : null);
		if (obj.AnswerState == ScoreMenu.AnswerState.UNANSWERED) {
			_descriptionText.color = _unansweredColor;
			_descriptionText.text = "Unanswered";
		} else if (obj.AnswerState == ScoreMenu.AnswerState.CORRECT) {
			_descriptionText.color = _correctColor;
			_descriptionText.text = "Correct";
		} else {
			_descriptionText.color = _incorrectColor;
			_descriptionText.text = "Incorrect";
		}
	}



	public void Hide() {
		gameObject.SetActive(false);
	}

	#endregion
	
}
