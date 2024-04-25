using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR;
public class FPSDisplay : MonoBehaviour
{
    public int FPS { get; private set; }
    public TextMeshPro DisplayFPS;

    private void Start()
    {
        //This is an attempt to make the game look better in the build.
        //XRSettings.eyeTextureResolutionScale = 1.5f;
        //Didn't work, cut fps in half and didn't improve resolution.
    }

    // Update is called once per frame
    void Update()
    {
        float _current = (int)(1f / Time.deltaTime);

        if (Time.frameCount % 50 == 0)
        {
            DisplayFPS.text = "FPS: " + _current.ToString();
        }
    }
}
