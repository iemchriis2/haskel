//Thomas Pereira
using System.Collections;
using System.Collections.Generic;

using UnityEngine;



public class InteractEvent : MonoBehaviour
{
	public static EasyEvent<HazardObject> InteractEventt = new EasyEvent<HazardObject>("Interact");//(0) = hazard object

	#region Public variables
	//public LineRenderer RaycastLine; //line that will give a visual presentation of raycast
	public float LineWidth;
	public float MaxLineLength;
	//public bool LineRendererToggle;
	//public Transform StartLinePosition;
	#endregion

	#region Private variables
	LineRenderer RaycastLine;
	[SerializeField]
	bool _hazardObjectHit;
	HazardObject _hazardObject;

	Vector3[] _startLinePosition;
	#endregion
    // Start is called before the first frame update
    void Start()
    {
		//Subscribe to input events
        PlayerInput.InteractInputDownEvent.Subscribe(this, InteractEV_Down);
		PlayerInput.InteractInputUpEvent.Subscribe(this, InteractEV_Up);

		_startLinePosition = new Vector3[2] {Vector3.zero, Vector3.zero};
		RaycastLine = GetComponent<LineRenderer>();

		RaycastLine.SetPositions(_startLinePosition);
		RaycastLine.startWidth = LineWidth;
		RaycastLine.startColor = Color.blue;
		RaycastLine.enabled = false;  
    }

	private void InteractEV_Down(bool isLeftHand)
	{
		ShootRaycast(this.transform.position, this.transform.forward, MaxLineLength);
		RaycastLine.enabled = true;
		Debug.Log("Raycast is gonna shoot out of here");
	}

    private void InteractEV_Up(bool isLeftHand)
	{
		RaycastLine.enabled = false;
		Debug.Log("stopped shooting raycast");
		if (_hazardObject != null)
			InteractEventt.Invoke(_hazardObject);
	}

	private void ShootRaycast(Vector3 _targetPosition, Vector3 _direction, float _length)
	{
		Vector3 _lineRendererEndPosition = _targetPosition + (_length * _direction);

		Ray _ray = new Ray(_targetPosition, _direction);
		RaycastHit _hit;
		bool _isHazardObject;

		if(Physics.Raycast(_ray, out _hit))
		{
			_hazardObject = _hit.collider.gameObject.GetComponent<HazardObject>();
			_isHazardObject = _hazardObject != null;

			_lineRendererEndPosition = _hit.point;

			if(_isHazardObject)
			{
				Debug.Log("ray has hit a hazard object");
			}
		}
		else
		{
			_hazardObject = null;
			Debug.Log("ray has not hit anything");
		}
		RaycastLine.SetPosition(0,_targetPosition);
		RaycastLine.SetPosition(1, _lineRendererEndPosition);
	}

	private void DrawLineRenderer()
	{
		//LineRendererToggle = true;
		RaycastLine.enabled = true;

		// if(LineRendererToggle)
		// {
		// 	ShootRaycast(transform.position, transform.forward, MaxLineLength);
		// }
	}
}
