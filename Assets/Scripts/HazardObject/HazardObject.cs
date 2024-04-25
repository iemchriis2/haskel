//Bonga Maswanganye and Michael Revit and Thomas Pereira

#pragma warning disable 0649

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Collections;
using System.Runtime.Serialization;
using System.Linq;

using UnityEngine;



[Serializable]
public class HazardObject : MonoBehaviour, IComparable<HazardObject> {

	#region Nested Definitions

	//types of hazards
	public enum HazardType {
		HOUSEKEEPING_PPE = 0,
		ELEVATED_PLATFORMS = 1,
		ELECTRICAL = 2,
		FIRE_PROTECTION = 3,
		TRENCHING = 4,
		GUARDING = 5,
		WELDING = 6,
		CRANES = 7,
		NEUTRAL = 8
	}

	#endregion



	#region Static

	#region Variables

	private static readonly bool DEBUG_MODE = true;
	private static readonly float LOOK_AT_TIME_THRESHOLD = .5f;


	//static list containing all hazard objects in the scene
	public static List<HazardObject> HazardList = new List<HazardObject>();

	//static list containing all hazard that were observed in a scene
	public static List<HazardObject> LookedAtHazardList = new List<HazardObject>();

	//List to help debug an issue.
	public List<HazardObject> DebugList;



	private static bool _needsToCalculate;
	private static int _unanswered;
	public static int Unanswered {
		get {
			if (_needsToCalculate)
				Calculate();
				
			return _unanswered;
		}
	}
	private static int _correct;
	public static int Correct {
		get {
			if (_needsToCalculate)
				Calculate();
			return _correct;
		}
	}
	private static int _incorrect;
	public static int Incorrect {
		get {
			if (_needsToCalculate)
				Calculate();
				
			return _incorrect;
		}
	}
	private static int _answeredNeutrals;
	public static int AnsweredNeutrals {
		get {
			if (_needsToCalculate)
				Calculate();
				
			return _answeredNeutrals;
		}
	}
	public static int TotalNonNeutralHazardObjectCount {
		get {
			int count = 0;
			foreach (HazardObject h in HazardList)
				if (h._type != HazardType.NEUTRAL)
					count++;
			return count;
		}
	}

	#endregion



	#region Calculate

	public static void Calculate() {
		Debug.LogWarning("Calculate");
		_needsToCalculate = false;

		_unanswered = _incorrect = _correct = _answeredNeutrals = 0;
		for (int i = 0; i < HazardList.Count; i++) {
			if (HazardList[i]._type == HazardType.NEUTRAL && HazardList[i].IsAnswered) {
				_answeredNeutrals++;
			//}else if (HazardList[i]._type != HazardType.NEUTRAL && !HazardList[i]._isAnswered) {
				//_unanswered++;
			} else if (HazardList[i]._isCorrect) {
				_correct++;
			} else if((((((HazardList[i]._type != HazardType.NEUTRAL && HazardList[i].HasBeenLookedAt && (!HazardList[i].IsAnswered || !HazardList[i]._isCorrect))))))){
				_incorrect++;
			}
			if (HazardList[i]._type != HazardType.NEUTRAL && !HazardList[i].IsAnswered)
				_unanswered++;
		}

		Debug.Log("Correct" + Correct + " Incorrect" + Incorrect + " Unanswered" + Unanswered + " AnsweredNeutrals" + _answeredNeutrals);
	}


	//Doesn't get used. (Thomas)
	//private static void CalculateIfLookedAt() {
	//	Debug.Log("CalculateIfLookedAt");
	//	_needsToCalculate = false;
	//	_unanswered = _incorrect = _correct = _answeredNeutrals = 0;

	//	for (int i = 0; i < HazardList.Count; i++) {
	//		if (HazardList[i].HasBeenLookedAt == true) {
	//			if(HazardList[i]._type == HazardType.NEUTRAL && HazardList[i].IsAnswered) {
	//				_answeredNeutrals++;
	//			} else if(!HazardList[i]._isAnswered) {
	//				_unanswered++;
	//			} else if(HazardList[i]._isCorrect) {
	//				_correct++;
	//			} else {
	//				_incorrect++;
	//			}
	//			if (!LookedAtHazardList.Contains(HazardList[i])) {
	//				Debug.Log("Adding " + HazardList[i].name + " to LookAtList at frame time: " + Time.time);
	//				LookedAtHazardList.Add(HazardList[i]);
	//			}
	//		}
	//	}
	//	Debug.Log("Looked at count " + LookedAtHazardList.Count + " Correct " + Correct + " Incorrect " + Incorrect + " Unanswered " + Unanswered + " AnsweredNeutrals " + _answeredNeutrals);
	//}

	#endregion

	#endregion



	#region Instance

	#region Variables

	//Preference Variables
	[SerializeField]
	private string _name;
	public string Name { get { return _name; } }
	[SerializeField]
	private HazardType _type;
	public HazardType Type { get { return _type; } }
	[SerializeField]
	private GameObject _hazard;
	[SerializeField]
	public bool ExistInScene;
	[SerializeField]
	public bool WasLookedAt = false;
	[SerializeField]
	private Collider[] _colliders;
	public Collider[] Colliders { get { return _colliders; } }
	[SerializeField]
	public float _lookAtSpeed = .1f;
	[SerializeField]
	private MeshRenderer[] _meshRenderers;
	[SerializeField]
	private Material _debugMaterial;
	[SerializeField]
	private SpriteRenderer _iconRender;
	[SerializeField]
	private SpriteRenderer _reverseIconRender;
	public bool DebugLookRaycastLinesVisible;

	//Script Variables
	private bool _isAnswered;
	public bool IsAnswered { get { return _isAnswered; } }
	private bool _isCorrect;
	public bool IsCorrect { get { return _isCorrect; } }
	public HazardType AnsweredType { get; private set; }
	//private bool ExistInScene;
	public float TotalLookAtTime { get; private set; }
	private float _lookAtPercentage, _lookAtPercentageTarget, _lookAtPercentageVelocity;
	public bool HasBeenLookedAt { get { return TotalLookAtTime >= LOOK_AT_TIME_THRESHOLD; } }
	private bool _isClosing;

	#endregion



	#region MonoBehaviour Implementation

	private void Awake() {
		MainMenu.GameStartEvent.Subscribe(this, EV_GameStart);

		AnsweredType = HazardType.NEUTRAL;

		if (!HazardList.Contains(this)) {
			HazardList.Add(this);
			HazardList.Sort();
		}

		if (DEBUG_MODE) {
			foreach (MeshRenderer r in _meshRenderers) {
				r.material = _debugMaterial;
				r.material.EnableKeyword("_EMISSION");
			}
		}
		Debug.Log("Looked at list count at awake: " + LookedAtHazardList.Count);
	}

	private void Start()
	{
		if(_colliders == null || _colliders.Length == 0)
		{
			_colliders = new Collider[] { GetComponent<Collider>() };
		}
		//_iconRender.sprite = null;
		//_reverseIconRender.sprite = _iconRender.sprite;

		//_reverseIconRender = _iconRender.GetComponentInChildren<SpriteRenderer>();
		Debug.Log("Looked at list count at start: " + LookedAtHazardList.Count);
		Debug.Log("Hazard items count: " + HazardList.Count);

	}



	private void OnApplicationClose() {
		_isClosing = true;
	}



	private void OnDestroy() {
		if (_isClosing)
			return;
		if (HazardList.Contains(this))
			HazardList.Remove(this);
	}



	private void OnEnable() {
		if (!HazardList.Contains(this)) {
			HazardList.Add(this);
			HazardList.Sort();
		}
		
	}



	private void OnDisable() {
		if (HazardList.Contains(this))
			HazardList.Remove(this);
		if (_iconRender != null)
			_iconRender.gameObject.SetActive(false);
	}



	private void Update() {
		LookAtPercentageUpdate();
		if (this.HasBeenLookedAt && this._type != HazardType.NEUTRAL)
		{
			if (!LookedAtHazardList.Contains(this))
			{
				LookedAtHazardList.Add(this);
			}
		}

		//DebugList = LookedAtHazardList;
		//Debug.Log("Looked at count " + LookedAtHazardList.Count + " Correct " + Correct + " Incorrect " + Incorrect + " Unanswered " + Unanswered + " AnsweredNeutrals " + _answeredNeutrals);
		//Debug.Log("Looked at list count " + LookedAtHazardList.Count);
		//CalculateIfLookedAt();
	}

	#endregion



	#region IComparable Interface Implementation

	public int CompareTo(HazardObject other) {
		int temp = _type.CompareTo(other._type);
		if (temp != 0)
			return temp;
		return _name.CompareTo(other._name);
	}

	#endregion



	#region Hazard Object State

	public void Clean() {
		AnsweredType = HazardType.NEUTRAL;
		_isAnswered = false;
		_isCorrect = false;
	}



	public void Answer(HazardType type) {
		Debug.Log("Answer method called");
		_isAnswered = true;
		_isCorrect = _type == type;
		AnsweredType = type;
		_needsToCalculate = true;
		if (_iconRender != null) {
			Debug.Log("Reached conditional");
			_iconRender.gameObject.SetActive(type != HazardType.NEUTRAL);
			//Set icon type
			switch (type) {
				case HazardType.HOUSEKEEPING_PPE:
					_iconRender.sprite = Resources.Load<Sprite>("HeroDangerIcon/HeroHelmet");
					_reverseIconRender.sprite = Resources.Load<Sprite>("HeroDangerIcon/HeroHelmet");
					Debug.Log("Should've assigned helmet sprite");
					break;

				case HazardType.ELEVATED_PLATFORMS:
					_iconRender.sprite = Resources.Load<Sprite>("HeroDangerIcon/HeroHeight");
					_reverseIconRender.sprite = Resources.Load<Sprite>("HeroDangerIcon/HeroHeight");
					Debug.Log("Should've assigned height sprite");
					break;

				case HazardType.ELECTRICAL:
					_iconRender.sprite = Resources.Load<Sprite>("HeroDangerIcon/HeroElectric");
					_reverseIconRender.sprite = Resources.Load<Sprite>("HeroDangerIcon/HeroElectric");
					Debug.Log("Should've assigned electric sprite");
					break;

				case HazardType.FIRE_PROTECTION:
					_iconRender.sprite = Resources.Load<Sprite>("HeroDangerIcon/HeroFlame");
					_reverseIconRender.sprite = Resources.Load<Sprite>("HeroDangerIcon/HeroFlame");
					Debug.Log("Should've assigned fire sprite");
					break;

				case HazardType.TRENCHING:
					_iconRender.sprite = Resources.Load<Sprite>("HeroDangerIcon/HeroTrench");
					_reverseIconRender.sprite = Resources.Load<Sprite>("HeroDangerIcon/HeroTrench");
					Debug.Log("Should've assigned trench sprite");
					break;

				case HazardType.GUARDING:
					_iconRender.sprite = Resources.Load<Sprite>("HeroDangerIcon/HeroFence");
					_reverseIconRender.sprite = Resources.Load<Sprite>("HeroDangerIcon/HeroFence");
					Debug.Log("Should've assigned fence sprite");
					break;

				case HazardType.WELDING:
					_iconRender.sprite = Resources.Load<Sprite>("HeroDangerIcon/HeroWelding");
					_reverseIconRender.sprite = Resources.Load<Sprite>("HeroDangerIcon/HeroWelding");
					Debug.Log("Should've assigned welding sprite");
					break;

				case HazardType.CRANES:
					_iconRender.sprite = Resources.Load<Sprite>("HeroDangerIcon/HeroOverhead");
					_reverseIconRender.sprite = Resources.Load<Sprite>("HeroDangerIcon/HeroOverhead");
					Debug.Log("Should've assigned overhead sprite");
					break;

				case HazardType.NEUTRAL:
					//_iconRender.sprite = Resources.Load<Sprite>("HeroDangerIcon/HeroNeutral"); commented out because this icon doesn't exist yet. (4/29/20)
					break;

			}
		}

		if (TotalLookAtTime < LOOK_AT_TIME_THRESHOLD)
			TotalLookAtTime = LOOK_AT_TIME_THRESHOLD;
	}

	#endregion



	#region Look At Raycast Helpers

	public void SetLookAtPercentageTarget(float target) {
		_lookAtPercentageTarget = target;
	}



	private void LookAtPercentageUpdate() {
		_lookAtPercentage = Mathf.SmoothDamp(_lookAtPercentage, _lookAtPercentageTarget, ref _lookAtPercentageVelocity, _lookAtSpeed);
		if (_lookAtPercentage >= .1f)
			TotalLookAtTime += _lookAtPercentage * Time.deltaTime;

		if (DEBUG_MODE)
			foreach (MeshRenderer r in _meshRenderers)
				r.material.SetColor("_EmissionColor", Color.Lerp(Color.black, Color.white, _lookAtPercentage));
	}

    #endregion

    #region Event Callbacks
	private void EV_GameStart()
	{
		_iconRender.sprite = null;
		_reverseIconRender.sprite = null;

		Clean();
	}
    #endregion
    #endregion

}
