//Michael Revit

#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;



/// <summary>
/// This menu shows the player's output information.
/// </summary>
public class ScoreMenu : Menu {
	
	#region Internal Definitions

	public enum AnswerState {
		CORRECT,
		INCORRECT,
		UNANSWERED
	}



	public class RenderEntry {
		public int Type { get; }
		public string Name { get; }
		public AnswerState AnswerState { get; }
		public int Count = 1;

		public RenderEntry(int type, string name, AnswerState answerState) {
			Type = type;
			Name = name;
			AnswerState = answerState;
		}
	}

	#endregion



	#region Instance

	#region Variables

	//Preference Variables
	[SerializeField]
	private Transform[] _categoryHeaders;
	[SerializeField]
	private TextMeshProUGUI _bottomLabel0, _bottomLabel1, _bottomValue0, _bottomValue1;
	[SerializeField]
	private GameObject _INST_renderHelper;
	[SerializeField]
	private bool _isSecondaryScreen;
	[SerializeField]
	private Color _correctColor, _incorrectColor, _unansweredColor;

	//Script Variables
	private List<ScoreRenderHelper> _renderHelperInstances = new List<ScoreRenderHelper>();

	#endregion



	#region MonoBehaviour Implementation

	private void Awake() {
		_INST_renderHelper.GetComponent<ScoreRenderHelper>().Hide();

		RenderInit();
	}



	protected override void Start() {
		EndMenu.GameEndEvent.Subscribe(this, EV_GameEnd);
		AnalyticsTracker.OverallScoreCalculatedEvent.Subscribe(this, EV_OverallScoreCalculated);

		gameObject.SetActive(false);
	}



	private void Update() {
		
	}

	#endregion



	#region Render

	private void RenderInit() {
		if (_isSecondaryScreen) {
			//_bottomLabel1.gameObject.SetActive(false);
			//_bottomValue1.gameObject.SetActive(false);

			_bottomLabel0.text = "Unanswered";
			_bottomLabel0.color = _bottomValue0.color = _unansweredColor;
			_bottomLabel1.text = "Overall Score";
			_bottomLabel1.color = _bottomValue1.color = Color.black;
		} else {
			_bottomLabel0.text = "Correct";
			_bottomLabel0.color = _bottomValue0.color = _correctColor;
			_bottomLabel1.text = "Incorrect";
			_bottomLabel1.color = _bottomValue1.color = _incorrectColor;
		}
	}



	private void Render() {
		//Header var (and setup)
		bool[] isHeaderRendered = new bool[9];
		for (int i = 0; i < isHeaderRendered.Length; i++)
			_categoryHeaders[i].gameObject.SetActive(false);

		//Prune list
		List<HazardObject> list = new List<HazardObject>();
		foreach (HazardObject h in HazardObject.HazardList)
			if (((int)h.Type <= 4 && !_isSecondaryScreen) || ((int)h.Type > 4 && _isSecondaryScreen))
				list.Add(h);

		//Create list of render entries
		List<RenderEntry> renderEntries = new List<RenderEntry>();
		for (int i = 0; i < list.Count; i++) {
			if (list[i].Type == HazardObject.HazardType.NEUTRAL && !(list[i].IsAnswered && !list[i].IsCorrect))
				continue;
			AnswerState answeredState = AnswerState.UNANSWERED;
			if (list[i].IsAnswered) {
				answeredState = AnswerState.INCORRECT;
				if (list[i].IsCorrect)
					answeredState = AnswerState.CORRECT;
			}
			if (i == 0) {
				renderEntries.Add(new RenderEntry((int)list[i].Type, list[i].Name, answeredState));
			} else {
				bool wasAddedToExistingEntry = false;
				for (int j = renderEntries.Count - 1; j >= 0; j--) {
					if (renderEntries[j].Name == list[i].Name) {
						if (renderEntries[j].AnswerState == answeredState) {
							renderEntries[j].Count++;
							wasAddedToExistingEntry = true;
							break;
						}
					} else {
						//break;
					}
				}
				if (!wasAddedToExistingEntry)
					renderEntries.Add(new RenderEntry((int)list[i].Type, list[i].Name, answeredState));
			}
		}

		//Instantiate renders
		for (int i = _renderHelperInstances.Count; i < renderEntries.Count; i++) {
			GameObject go = Instantiate(_INST_renderHelper);
			go.transform.SetParent(_INST_renderHelper.transform.parent, false);
			_renderHelperInstances.Add(go.GetComponent<ScoreRenderHelper>());
		}

		//Anti-render
		for (int i = 0; i < _renderHelperInstances.Count; i++)
			_renderHelperInstances[i].Hide();

		//Render
		int currentRenderIndex = 0, renderHeperUseIndex = 0;
		for (int i = 0; i < renderEntries.Count; i++) {
			int zoneInt = (int)renderEntries[i].Type;
			if (!isHeaderRendered[zoneInt]) {
				_categoryHeaders[zoneInt].gameObject.SetActive(true);
				//_categoryHeaders[zoneInt].SetParent(currentRenderIndex >= _maxNumberOfElementsInOneColumn ? _content1 : _content0, false);
				_categoryHeaders[zoneInt].SetSiblingIndex(currentRenderIndex++);
				isHeaderRendered[zoneInt] = true;
			}
			_renderHelperInstances[renderHeperUseIndex].Render(renderEntries[i]);
			//_renderHelperInstances[renderHeperUseIndex].transform.SetParent(currentRenderIndex >= _maxNumberOfElementsInOneColumn ? _content1 : _content0, false);
			_renderHelperInstances[renderHeperUseIndex].transform.SetSiblingIndex(currentRenderIndex++);
			renderHeperUseIndex++;
		}

		//Other
		if (_isSecondaryScreen) {
			_bottomValue0.text = HazardObject.Unanswered + "/" + HazardObject.TotalNonNeutralHazardObjectCount;
		} else {
			_bottomValue0.text = HazardObject.Correct + "/" + HazardObject.TotalNonNeutralHazardObjectCount;
			_bottomValue1.text = HazardObject.Incorrect + "/" + HazardObject.TotalNonNeutralHazardObjectCount;
		}
	}



	private IEnumerator RenderPause() {
		yield return null;
		Render();
	}

	#endregion



	#region Event Callbacks

	private void EV_GameEnd() {
		gameObject.SetActive(true);
		StartCoroutine(RenderPause());
	}



	private void EV_OverallScoreCalculated(float overallScore) {
		if (_isSecondaryScreen)
			_bottomValue1.text = overallScore.ToString() + "%";
	}

	#endregion

	#endregion

}
