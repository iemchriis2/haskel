using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus;

public class ControllerInputTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        OVRInput.Update();
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            Debug.Log("A button pressed");
        }
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            Debug.Log("B button pressed");
        }
        if (OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger))
        {
            Debug.Log("secondary handtrigger button pressed");
        }
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger))
        {
            Debug.Log("Primary handtrigger button pressed");
        }
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
        {
            Debug.Log("Primary Index pressed");
        }
        if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
        {
            Debug.Log("secondary index pressed");
        }
        if (OVRInput.Get(OVRInput.Button.PrimaryThumbstickDown))
        {
            Debug.Log("Primary thubstick pressed");
        }
        if (OVRInput.Get(OVRInput.Button.SecondaryThumbstickDown))
        {
            Debug.Log("secondary thumbstick pressed");
        }
        //Debug.Log(OVRInput.Get(OVRInput.Button.PrimaryThumbstick));
           // Debug.Log(OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger));
           // Debug.Log(OVRInput.Get(OVRInput.Button.SecondaryThumbstick));
           // Debug.Log(OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger));

    }
    void FixedUpdate()
    {
        OVRInput.FixedUpdate();
        if (OVRInput.Get(OVRInput.Button.One))
        {
            Debug.Log("A button pressed");
        }
        if (OVRInput.Get(OVRInput.Button.Two))
        {
            Debug.Log("B button pressed");
        }
    }

}
