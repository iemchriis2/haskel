//Michael Revit

#pragma warning disable 0649

using System.Collections.Generic;

using UnityEngine;



/// <summary>
/// This class is for checking overlaps in 3D space with a hand collider entering a trigger.
/// </summary>
public class PhysicalTriggerInteract : MonoBehaviour {

	#region Variables

	//Event Variables
	public EasyEvent InteractionStartEvent { get; private set; }
	public EasyEvent InteractionEndEvent { get; private set; }

	//Preference Variables
	[SerializeField]
	private bool _isInteractable;
	public bool IsInteractable {
		get { return _isInteractable; }
		set {
			if (_isInteractable == value)
				return;
			_isInteractable = value;
			if (_isInteractable) {
				if (!_isInteracting && _interactors.Count > 0) {
					_isInteracting = true;
					SendActionCallbackToAllInteractors();
					InteractionStartEvent.Invoke();
				}
			} else {
				if (_isInteracting) {
					_isInteracting = false;
					SendUnactionCallbackToAllInteractors();
					InteractionEndEvent.Invoke();
				}
			}
		}
	}
	[SerializeField]
	private string _tag;
	public string Tag { get {return _tag; } }
	public BoxCollider BoxCollider { get; private set; }

	//Script Variables
	private List<PhysicalTriggerInteractor> _interactors = new List<PhysicalTriggerInteractor>();
	/// <summary>
	/// Do not modify this list. This accessor is only available so outside code can see it.
	/// </summary>
	public List<PhysicalTriggerInteractor> Interactors { get { return _interactors; } }
	private bool _isInteracting;
	private bool _isClosing;

	#endregion



	#region MonoBehaviour Implementation

	private void Awake() {
		InteractionStartEvent = new EasyEvent("PhysicalTap Interaction Start");
		InteractionEndEvent = new EasyEvent("PhysicalTap Interaction End");

		BoxCollider = GetComponent<BoxCollider>();
	}



	private void Start() {

	}



	private void OnApplicationClose() {
		_isClosing = true;
	}



	protected virtual void OnDestroy() {
		if (_isClosing)
			return;
		if (_isInteracting) {
			SendUnactionCallbackToAllInteractors();
			InteractionEndEvent.Invoke();
		}
	}



	private void OnTriggerEnter(Collider collider) {
		PhysicalTriggerInteractor i = collider.GetComponent<PhysicalTriggerInteractor>();
		if (i != null)
			Action(i);
	}

	public void Interact()
    {
		_isInteracting = false;
		InteractionEndEvent.Invoke();

	}


	private void OnTriggerExit(Collider collider) {
		PhysicalTriggerInteractor i = collider.GetComponent<PhysicalTriggerInteractor>();
		if (i != null)
			Unaction(i);
	}

	#endregion



	#region Action Unaction

	public virtual void Action(PhysicalTriggerInteractor interactor) {
		_interactors.Add(interactor);
		if (_isInteractable) {
			SendActionCallbackToInteractor(interactor);
			if (!_isInteracting) {
				_isInteracting = true;
				InteractionStartEvent.Invoke();
			}
		}
	}



	public virtual void Unaction(PhysicalTriggerInteractor interactor) {
		_interactors.Remove(interactor);
		if (_isInteractable) {
			SendUnactionCallbackToInteractor(interactor);
			if (_isInteracting && _interactors.Count == 0) {
				_isInteracting = false;
				InteractionEndEvent.Invoke();
			}
		}
	}

	#endregion



	#region Internal

	protected virtual void SendActionCallbackToInteractor(PhysicalTriggerInteractor interactor) {
		interactor.Action(this);
	}



	protected virtual void SendUnactionCallbackToInteractor(PhysicalTriggerInteractor interactor) {
		interactor.Unaction(this);
	}



	protected virtual void SendActionCallbackToAllInteractors() {
		foreach (PhysicalTriggerInteractor i in _interactors)
			SendActionCallbackToInteractor(i);
	}



	protected virtual void SendUnactionCallbackToAllInteractors() {
		foreach (PhysicalTriggerInteractor i in _interactors)
			SendUnactionCallbackToInteractor(i);
	}

	#endregion

}
