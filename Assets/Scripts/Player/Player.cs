//Michael Revit and Thomas Pereira/

#pragma warning disable 0649

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;



/// <summary>
/// This class controls player mechanics: initiating teleports, interacts, and more. Uses PlayerInput.cs events, heavily.
/// </summary>
public class Player : MonoBehaviour {

	#region Static

	#region Variables

	//Event Variables
	public static EasyEvent TeleportEvent = new EasyEvent("Teleport");
	public static EasyEvent<HazardObject> InteractEvent = new EasyEvent<HazardObject>("Interact");

	//Singleton Variable
	public static Player Singleton { get; private set; }

	#endregion

	#endregion



	#region Instance

	#region Variables

	//Preference Variables
	[SerializeField]
	private VRTeleporter _teleporter;
	[SerializeField]
	private Transform _rigTransform, _headTransform, _leftHandTransform, _rightHandTransform;
	public Transform RigTransform { get { return _rigTransform; } }
	public Transform HeadTransform { get { return _headTransform; } }
	[SerializeField]
	private Vector3 _teleporterLocalRotationOffset;
	[SerializeField]
	private float _snapRotationDegrees = 45;
	[SerializeField]
	private float _interactRaycastDistance;
	[SerializeField]
	private LayerMask _interactLayers;
	[SerializeField]
	private LineRenderer _interactLineRenderer;
	[SerializeField]
	private Material _interactNullMaterial, _interactNotNullMaterial;
	[SerializeField]
	private Material _leftHandMaterial, _rightHandMaterial, _leftControllerMaterial, _rightControllerMaterial;
	[SerializeField]
	private AnimationCurve _opacityCurve;
	[SerializeField]
	private float _opacitySmoothDampSpeed;
	[SerializeField]
	private bool _useFadeOnSnapRotation;
	[SerializeField]
	private float _snapRotationFadeSpeed;

	//Script Variables
	private bool _isInGame;
	private bool _isLeftHanded = false;
	public bool IsLeftHanded { get { return _isLeftHanded; } }
	private bool _canInteract = false;
	private bool _isInteractDown, _isInteractLeft;
	private bool _canTeleport = true;
	private bool _isTeleportDown, _isTeleportLeft;
	private HazardObject _hoveredHazardObject;
	private List<PhysicalTriggerInteract> _leftHandOpacityInteractions = new List<PhysicalTriggerInteract>();
	private List<PhysicalTriggerInteract> _rightHandOpacityInteractions = new List<PhysicalTriggerInteract>();
	private float _leftHandAlphaValue, _rightHandAlphaValue;
	private float _leftHandDistanceToCenter, _rightHandDistanceToCenter;
	private float _leftOpacityOpacityPercentage, _rightOpacityOpacityPercentage, _leftOpacityVelocity, _rightOpacityVelocity;
	private bool _isHandMaterialInitialized;
	private bool _isKeyboardEnabled;

	#endregion



	#region MonoBehaviour Implementation

	private void Awake() {
		Singleton = this;
	}



	private void Start() {
		PlayerInput.TeleportInputDownEvent.Subscribe(this, EV_TeleportDown);
		PlayerInput.TeleportInputUpEvent.Subscribe(this, EV_TeleportUp);
		PlayerInput.SnapRotationEvent.Subscribe(this, EV_SnapRotation);
		PlayerInput.InteractInputDownEvent.Subscribe(this, EV_InteractDown);
		PlayerInput.InteractInputUpEvent.Subscribe(this, EV_InteractUp);
		PlayerInput.ReturnHomeInputDownEvent.Subscribe(this, EV_ReturnHomeDown);
		MainMenu.GameStartEvent.Subscribe(this, EV_GameStart);
		EndMenu.GameEndEvent.Subscribe(this, EV_GameEnd);
		Tutorial.StartEvent.Subscribe(this, EV_TutorialStart);
		Tutorial.EndEvent.Subscribe(this, EV_TutorialEnd);
		Tutorial.RequestTeleportEvent.Subscribe(this, EV_TutorialRequest);
		ScoreDoneMenu.RequestEvent.Subscribe(this, EV_DoneLookingAtScoreOutput);
		NameConfirmationMenu.RequestEvent.Subscribe(this, EV_DoneLookingAtScoreOutput);
		KeyboardMenu.EnableDisableEvent.Subscribe(this, EV_KeyboardEnableDisable);
		_leftHandTransform.GetComponent<PhysicalTriggerInteractor>().ActionEvent.Subscribe(this, EV_LeftHandAction);
		_leftHandTransform.GetComponent<PhysicalTriggerInteractor>().UnactionEvent.Subscribe(this, EV_LeftHandUnaction);
		_rightHandTransform.GetComponent<PhysicalTriggerInteractor>().ActionEvent.Subscribe(this, EV_RightHandAction);
		_rightHandTransform.GetComponent<PhysicalTriggerInteractor>().UnactionEvent.Subscribe(this, EV_RightHandUnaction);
		ReturnHomeConfirmationMenu.ReturnHomeRequestEvent.Subscribe(this, EV_RequestHome);
	}



	private void Update() {
		//Find hand materials (if not set)
		if (!_isHandMaterialInitialized) {
			GameObject a = GameObject.Find("hand_left_renderPart_0");
        	GameObject b = GameObject.Find("hand_right_renderPart_0");
			if (a != null && b != null) {
				SkinnedMeshRenderer c = a.GetComponent<SkinnedMeshRenderer>();
				SkinnedMeshRenderer d = b.GetComponent<SkinnedMeshRenderer>();
				if (c != null && d != null) {
					_isHandMaterialInitialized = true;
					c.material = _leftHandMaterial;
					d.material = _rightHandMaterial;
				}
			}
		}

		//Check for interactions
		InteractUpdate();

		//Calculate hand and controller opacity
		if (_isHandMaterialInitialized)
			HandOpacityUpdate();

		//Raycast keyboard helper
		KeyboardMenuUpdate();
	}

	#endregion



	#region Interact

	private void InteractEnable(bool isLeftHand) {
		if (!_canInteract || _isKeyboardEnabled)
			return;
		if (AnswerMenu.IsEnabled || ReturnHomeConfirmationMenu.IsEnabled)
			return;
		_isInteractDown = true;
		_isInteractLeft = isLeftHand;
	}



	private void InteractDisable(bool isLeftHand) {
		if (!_isInteractDown)
			return;
		if ((isLeftHand && !_isInteractLeft) || (!isLeftHand && _isInteractLeft))
			return;
		_isInteractDown = false;

		_interactLineRenderer.enabled = false;

		if (_hoveredHazardObject != null)
			InteractEvent.Invoke(_hoveredHazardObject);
	}



	private void InteractUpdate() {
		if (!_isInteractDown)
			return;

		Transform t = _isInteractLeft ? _leftHandTransform : _rightHandTransform;
		if (Physics.Raycast(t.position, t.forward, out RaycastHit hit, _interactRaycastDistance, _interactLayers)) {
			_hoveredHazardObject = hit.collider.gameObject.GetComponent<HazardObject>();
			if (_hoveredHazardObject != null) {
				_interactLineRenderer.SetPositions(new Vector3[] { t.position, hit.point });
				_interactLineRenderer.material = _interactNotNullMaterial;
			} else {
				_interactLineRenderer.SetPositions(new Vector3[] { t.position, hit.point });
				_interactLineRenderer.material = _interactNullMaterial;
			}
		} else {
			_hoveredHazardObject = null;
			_interactLineRenderer.SetPositions(new Vector3[] { t.position, t.position + t.forward * _interactRaycastDistance });
			_interactLineRenderer.material = _interactNullMaterial;
		}

		if(!_interactLineRenderer.enabled)
			_interactLineRenderer.enabled = true;
	}

	#endregion



	#region Teleport

	private void TeleportEnable(bool isLeftHand) {
		if (_teleporter == null || !_canTeleport || _isKeyboardEnabled)
			return;
		_isTeleportDown = true;
		_isTeleportLeft = isLeftHand;

		_teleporter.transform.SetParent(isLeftHand ? _leftHandTransform : _rightHandTransform, false);
		_teleporter.transform.localPosition = Vector3.zero;
		_teleporter.transform.localEulerAngles = _teleporterLocalRotationOffset;
		_teleporter.ToggleDisplay(true);
	}



	private void TeleportDisable(bool isLeftHand) {
		if (_teleporter == null || !_isTeleportDown)
			return;
		if ((isLeftHand && !_isTeleportLeft) || (!isLeftHand && _isTeleportLeft))
			return;
		_isTeleportDown = false;

		_teleporter.ToggleDisplay(false);

		Fade.FadeToBlack(() => {
			Teleport();
		}, .25f);
	}



	private void Teleport() {
		if (_teleporter == null)
			return;

		Vector3 offset = _rigTransform.position - _headTransform.position;
		offset = new Vector3(offset.x, 0, offset.z);
		if (_teleporter.Teleport()) {
			_rigTransform.position += offset;
			TeleportEvent.Invoke();
		}
	}



	private void Teleport(Vector3 position) {
		Vector3 offset = _rigTransform.position - _headTransform.position;
		offset = new Vector3(offset.x, 0, offset.z);
		_rigTransform.position = position + offset;
	}

	#endregion



	#region Hand Opacity

	private void HandOpacityUpdate() {
		CalculateHandOpacity(_leftHandOpacityInteractions, _leftHandTransform, _leftHandMaterial, _leftControllerMaterial, ref _leftOpacityOpacityPercentage, ref _leftOpacityVelocity);
		CalculateHandOpacity(_rightHandOpacityInteractions, _rightHandTransform, _rightHandMaterial, _rightControllerMaterial, ref _rightOpacityOpacityPercentage, ref _rightOpacityVelocity);
	}



	private void CalculateHandOpacity(List<PhysicalTriggerInteract> interactions, Transform handTransform, Material handMaterial, Material controllerMaterial, ref float opacityPercentage, ref float velocity) {
		//Get target opacity value
		float calculatedTargetValue = 0;
		foreach (PhysicalTriggerInteract interaction in interactions) {
			Vector3 point = interaction.transform.InverseTransformPoint(handTransform.transform.position);
			Vector3 bounds = new Vector3(
				(Mathf.InverseLerp(interaction.BoxCollider.center.x + interaction.BoxCollider.size.x * .5f, interaction.BoxCollider.center.x - interaction.BoxCollider.size.x * .5f, point.x)),
				(Mathf.InverseLerp(interaction.BoxCollider.center.y + interaction.BoxCollider.size.y * .5f, interaction.BoxCollider.center.y - interaction.BoxCollider.size.y * .5f, point.y)),
				(Mathf.InverseLerp(interaction.BoxCollider.center.z + interaction.BoxCollider.size.z * .5f, interaction.BoxCollider.center.z - interaction.BoxCollider.size.z * .5f, point.z))
			);
			if (bounds.z > calculatedTargetValue)
				calculatedTargetValue = 1 - bounds.z;
		}

		//Apply opacity value to materials
		opacityPercentage = Mathf.SmoothDamp(opacityPercentage, calculatedTargetValue, ref velocity, _opacitySmoothDampSpeed);
		float evaluation = _opacityCurve.Evaluate(opacityPercentage);
		handMaterial.SetFloat("AlphaValue", 1 - evaluation);
		controllerMaterial.SetFloat("AlphaValue", (evaluation));
	}

	#endregion



	#region Keyboard Menu Helper

	private void KeyboardMenuUpdate() {
		//if (!_isKeyboardEnabled)
		//	return;

		Transform t = _isLeftHanded ? _leftHandTransform : _rightHandTransform;
		if (Physics.Raycast(t.position, t.forward, out RaycastHit hit, _interactRaycastDistance, _interactLayers)) {
			if (hit.collider.tag == "Keyboard") 
			{
				KeyboardMenu.Singleton.SetRaycatHit(hit.point);
				_interactLineRenderer.material = _interactNotNullMaterial;
			} else {
				KeyboardMenu.Singleton.SetNoRaycastHit();
				_interactLineRenderer.material = _interactNullMaterial;
			}
		} else {
			KeyboardMenu.Singleton.SetNoRaycastHit();
			_interactLineRenderer.material = _interactNullMaterial;
		}

		if(!_interactLineRenderer.enabled)
			_interactLineRenderer.enabled = true;

		CheckName(t);
	}


	void CheckName(Transform t)
    {
		if (Physics.Raycast(t.position, t.forward, out RaycastHit hit, _interactRaycastDistance, _interactLayers))
		{
			if (hit.collider.tag == "Name")
			{
				NameConfirmationMenu.Singeton.SetRaycatHit(hit.point);
				_interactLineRenderer.material = _interactNotNullMaterial;
			}
			else
			{
				NameConfirmationMenu.Singeton.SetNoRaycastHit();
				_interactLineRenderer.material = _interactNullMaterial;
			}
		}
		else
		{
			NameConfirmationMenu.Singeton.SetNoRaycastHit();
			_interactLineRenderer.material = _interactNullMaterial;
		}
	}

	#endregion



	#region Internal

	private void Rotate(float degrees, bool isClockwise = true) {
			Vector3 position = _headTransform.position;
			_rigTransform.eulerAngles += new Vector3(0, degrees * (isClockwise ? 1 : -1), 0);
			Vector3 offset = position - _headTransform.position;
			_rigTransform.position += offset;
		}

	#endregion



	#region Event Callbacks

	private void EV_TeleportDown(bool isLeftHand) {
		TeleportEnable(isLeftHand);
	}



	private void EV_TeleportUp(bool isLeftHand) {
		TeleportDisable(isLeftHand);
	}



	private void EV_SnapRotation(bool isClockwise) {
		if (_useFadeOnSnapRotation) {
			Fade.FadeToBlack(() => { Rotate(_snapRotationDegrees, isClockwise); }, _snapRotationFadeSpeed);
		} else {
			Rotate(_snapRotationDegrees, isClockwise);
		}
	}



	private void EV_InteractDown(bool isLeftHand) {
		InteractEnable(isLeftHand);
	}



	private void EV_InteractUp(bool isLeftHand) {
		InteractDisable(isLeftHand);
	}



	private void EV_ReturnHomeDown(bool isLeftHand) {
		if (!_isInGame || AnswerMenu.IsEnabled)
			return;
		ReturnHomeConfirmationMenu.Singleton.E();
	}



	private void EV_GameStart() {
		_isInGame = _canInteract = _canTeleport = true;

		Fade.FadeToBlack(() => {
			Teleport(TeleportTransform.GetTransformFromName("GAME_AREA").position);
		});
	}



	private void EV_GameEnd() {
		_isInGame = _canInteract = false;
		_canTeleport = true;
		
		Fade.FadeToBlack(() => {
			Rotate(-HeadTransform.eulerAngles.y + 180);
			Teleport(TeleportTransform.GetTransformFromName("SCORE_AREA").position);
		});
	}



	private void EV_TutorialStart() {
		_canInteract = true;
		_canTeleport = false;

		Fade.FadeToBlack(() => {
			Teleport(TeleportTransform.GetTransformFromName("TUTORIAL_AREA").position);
		});
	}



	private void EV_TutorialEnd() {
		_canInteract = false;
		_canTeleport = true;
		
		Fade.FadeToBlack(() => {
			Teleport(TeleportTransform.GetTransformFromName("MAIN_AREA").position);
		});
	}



	private void EV_TutorialRequest() {
		_canTeleport = true;
	}



	private void EV_DoneLookingAtScoreOutput() {
		Fade.FadeToBlack(() => {
			Teleport(TeleportTransform.GetTransformFromName("MAIN_AREA").position);
		});
	}



	private void EV_KeyboardEnableDisable(bool isEnabled) {
		_isKeyboardEnabled = isEnabled;
		if (!_isKeyboardEnabled)
			_interactLineRenderer.enabled = false;
	}



	private void EV_LeftHandAction(PhysicalTriggerInteract interact) {
		if (interact.Tag == "OPACITY")
			_leftHandOpacityInteractions.Add(interact);
	}



	private void EV_LeftHandUnaction(PhysicalTriggerInteract interact) {
		if (interact.Tag == "OPACITY")
			_leftHandOpacityInteractions.Remove(interact);
	}



	private void EV_RightHandAction(PhysicalTriggerInteract interact) {
		if (interact.Tag == "OPACITY")
			_rightHandOpacityInteractions.Add(interact);
	}



	private void EV_RightHandUnaction(PhysicalTriggerInteract interact) {
		if (interact.Tag == "OPACITY")
			_rightHandOpacityInteractions.Remove(interact);
	}



	private void EV_RequestHome() {
		Fade.FadeToBlack(() => {
			Rotate(-HeadTransform.eulerAngles.y);
			Teleport(TeleportTransform.GetTransformFromName("GAME_AREA").position);
		});
	}

	#endregion

	#endregion

}
