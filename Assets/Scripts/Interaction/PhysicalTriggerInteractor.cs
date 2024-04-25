//Michael Revit

#pragma warning disable 0649

using UnityEngine;



/// <summary>
/// This is the detector class (put on the hand) to detect PhysicalTriggerInteracts.
/// </summary>
public class PhysicalTriggerInteractor : MonoBehaviour {

	#region Variables

	//Event Variables
	public EasyEvent<PhysicalTriggerInteract> ActionEvent { get; private set; }
	public EasyEvent<PhysicalTriggerInteract> UnactionEvent { get; private set; }

	#endregion



	#region MonoBehaviour Implementation

	private void Awake() {
		ActionEvent = new EasyEvent<PhysicalTriggerInteract>("PhysicalTapInteractor Action");
		UnactionEvent = new EasyEvent<PhysicalTriggerInteract>("PhysicalTapInteractor Unaction");
	}

	#endregion



	#region Action Unaction

	public virtual void Action(PhysicalTriggerInteract interact) {
		ActionEvent.Invoke(interact);
	}



	public virtual void Unaction(PhysicalTriggerInteract interact) {
		UnactionEvent.Invoke(interact);
	}

	#endregion

}
