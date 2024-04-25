//Michael Revit

#pragma warning disable 0649

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;



/// <summary>
/// This class determines what objects the player is most likely looking at.
/// </summary>
public class LookRaycast : MonoBehaviour {

	#region Variables

	//Preference Variables
	[SerializeField]
	private LayerMask _raycastLayers;
	[SerializeField]
	private bool _useFinalRaycastCheck;

	[Header("Fine Tuning")]
	[SerializeField]
	private Vector3 _forwardVectorAdditive;
	[SerializeField]
	private float _p1Weight, _p2Weight, _p3Weight;
	[SerializeField, Range(0, 1)]
	private float _defaultClosestPointBias = .65f;
	[SerializeField, Range(0, 1)]
	private float _closestPointDistanceMultiplier = .8f;
	[SerializeField]
	private AnimationCurve _maxActivationAngle, _angleOffsetActivationDropoff;

	private float _maxDetectionLength;

	#endregion



	#region MonoBehaviour Implementation

	private void Awake() {
		_maxDetectionLength = _maxActivationAngle.keys[_maxActivationAngle.keys.Length - 1].time;
	}



	private void Update() {
		bool realHit = false;

		//Helper points
		Vector3 modifiedForward = (transform.forward + transform.TransformDirection(_forwardVectorAdditive)).normalized;
		Vector3 raycastPoint = transform.position + modifiedForward * _maxDetectionLength;
		Debug.DrawLine(transform.position, transform.position + transform.forward * _maxDetectionLength, Color.white);
		Debug.DrawLine(transform.position, transform.position + modifiedForward * _maxDetectionLength, Color.gray);

		//Raycast
		if (Physics.Raycast(transform.position, modifiedForward, out RaycastHit hit, _maxDetectionLength, _raycastLayers)) {
			realHit = true;
			Debug.DrawLine(hit.point, hit.point - Vector3.up * .1f, Color.magenta);
			raycastPoint = hit.point;
		}

		//Iterathe through all objects and calculate look percentage
		for (int i = 0; i < HazardObject.HazardList.Count; i++) {
			HazardObject h = HazardObject.HazardList[i];
			float highestPercentage = 0;
			foreach (Collider c in h.Colliders) {
				//If direct raycast hit, set to max threshold and continue
				if (realHit && c == hit.collider) {
					highestPercentage = 1;
					continue;
				}

				//Get points
				Vector3 p1 = c.ClosestPoint(transform.position);
				Vector3 p2 = c.ClosestPoint(raycastPoint);
				Vector3 p3 = c.ClosestPoint(transform.position + modifiedForward * Vector3.Distance(transform.position, p1) * _closestPointDistanceMultiplier);//_colliders[i].C.ClosestPoint(raycastPoint);

				//Get single, averaged point
				Vector3 point = p1 * _p1Weight + p2 * _p2Weight + p3 * _p3Weight;
				point /= (_p1Weight + _p2Weight + _p3Weight);

				//Debug lines
				if (h.DebugLookRaycastLinesVisible) {
					Debug.DrawLine(p1, transform.position, Color.red);
					Debug.DrawLine(p2, transform.position, Color.green);
					Debug.DrawLine(p3, transform.position, Color.blue);
					Debug.DrawLine(point, transform.position, Color.magenta);
				}

				//Check if visible
				if (_useFinalRaycastCheck)
					if (Physics.Raycast(transform.position, hit.point, out RaycastHit innerHit, _maxDetectionLength, _raycastLayers)) {
						if (innerHit.collider != c) {
							//h.SetLookAtPercentageTarget(0);
							continue;
						}
					}

				//Calculate look-at percentage
				float distance = Vector3.Distance(transform.position, point);
				float angle = Vector3.Angle(modifiedForward, (point - transform.position).normalized);
				float maximumAllowedAngle = _maxActivationAngle.Evaluate(distance);
				if (angle > maximumAllowedAngle) {
					h.SetLookAtPercentageTarget(0);
					continue;
				}
				float lookAtPercentage = Mathf.Clamp01(_angleOffsetActivationDropoff.Evaluate(angle / maximumAllowedAngle));

				//Calculate highest percentage
				if (lookAtPercentage > highestPercentage)
					highestPercentage = lookAtPercentage;
				if (lookAtPercentage == 1)
					break;
			}

			//Set calculated percentage
			h.SetLookAtPercentageTarget(highestPercentage);
		}
	}

	#endregion

}
