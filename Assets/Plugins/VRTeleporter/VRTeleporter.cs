using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRTeleporter : MonoBehaviour
{

    public GameObject positionMarker; // marker for display ground position

    public Transform bodyTransforn; // target transferred by teleport

    public LayerMask excludeLayers; // excluding for performance

    public float angle = 45f; // Arc take off angle

    public float strength = 10f; // Increasing this value will increase overall arc length

    public float dotProductThreshold = .1f;

    public float markerOffset = .04f;

    public Transform lookToTarget;

    public Material pathUsable, pathUnusable, markerUsable, markerUnusable;

    public MeshRenderer markerMeshRenderer;


    int maxVertexcount = 100; // limitation of vertices for performance. 

    private float vertexDelta = 0.08f; // Delta between each Vertex on arc. Decresing this value may cause performance problem.

    private LineRenderer arcRenderer;

    private Vector3 velocity; // Velocity of latest vertex

    private Vector3 groundPos; // detected ground position

    private Vector3 lastNormal; // detected surface normal

    private GameObject lastHit;

    private bool groundDetected = false;

    private List<Vector3> vertexList = new List<Vector3>(); // vertex on arc

    private bool displayActive = false; // don't update path when it's false.

    private float dotProduct;


    // Teleport target transform to ground position
    public bool Teleport()
    {
        if (groundDetected && dotProduct > dotProductThreshold && LayerMask.NameToLayer("NoTeleport") != lastHit.layer)
        {
            bodyTransforn.position = groundPos + lastNormal * 0.01f;
			return true;
        }
        else
        {
            //Debug.Log("Ground wasn't detected");
			return false;
        }
    }

    // Active Teleporter Arc Path
    public void ToggleDisplay(bool active)
    {
        arcRenderer.enabled = active;
        positionMarker.SetActive(active);
        displayActive = active;
    }





    private void Awake()
    {
        arcRenderer = GetComponent<LineRenderer>();
        arcRenderer.enabled = false;
        positionMarker.SetActive(false);
    }

    private void Update()
    {
        if (displayActive)
        {
            UpdatePath();
        }
    }


    private void UpdatePath()
    {
        groundDetected = false;

        vertexList.Clear(); // delete all previouse vertices


        velocity = Quaternion.AngleAxis(-angle, transform.right) * transform.forward * strength;

        RaycastHit hit;

        lastHit = null;


        Vector3 pos = transform.position; // take off position

        vertexList.Add(pos);

        while (!groundDetected && vertexList.Count < maxVertexcount)
        {
            Vector3 newPos = pos + velocity * vertexDelta
                + 0.5f * Physics.gravity * vertexDelta * vertexDelta;

            velocity += Physics.gravity * vertexDelta;

            vertexList.Add(newPos); // add new calculated vertex

            // linecast between last vertex and current vertex
            if (Physics.Linecast(pos, newPos, out hit, ~excludeLayers))
            {
                groundDetected = true;
                groundPos = hit.point;
                lastNormal = hit.normal;
                dotProduct = Vector3.Dot(Vector3.up, hit.normal);
                lastHit = hit.collider.gameObject;
                Debug.Log(lastHit.gameObject.name);
            }
            pos = newPos; // update current vertex as last vertex
        }


        positionMarker.SetActive(groundDetected);

        if (groundDetected)
        {
            positionMarker.transform.position = groundPos + lastNormal * markerOffset;
            positionMarker.transform.LookAt(lookToTarget, Vector3.up);
            Vector3 desiredAngle = positionMarker.transform.eulerAngles;
            positionMarker.transform.LookAt(groundPos);
            if (dotProduct > dotProductThreshold && LayerMask.NameToLayer("NoTeleport") != lastHit.layer) {
                arcRenderer.material = pathUsable;
                markerMeshRenderer.material = markerUsable;
                // Vector2 offset = new Vector2(lookToTarget.position.x, lookToTarget.position.z) - new Vector2(transform.position.x, transform.position.z);
                // float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
                // positionMarker.transform.Rotate(Vector3.up, Mathf.DeltaAngle(positionMarker.transform.eulerAngles.y, angle), Space.World);
                positionMarker.transform.eulerAngles = new Vector3( positionMarker.transform.eulerAngles.x, desiredAngle.y + 180,  positionMarker.transform.eulerAngles.z);
            } else {
                arcRenderer.material = pathUnusable;
                markerMeshRenderer.material = markerUnusable;
            }
        } else {
            arcRenderer.material = pathUnusable;
            markerMeshRenderer.material = markerUnusable;
        }

        // Update Line Renderer

        arcRenderer.positionCount = vertexList.Count;
        arcRenderer.SetPositions(vertexList.ToArray());
    }


}