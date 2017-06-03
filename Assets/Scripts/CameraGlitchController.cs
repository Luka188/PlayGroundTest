using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kino;

public class CameraGlitchController : MonoBehaviour {
    AnalogGlitch Glitch;

    [SerializeField]
    Material Skybox;
    [SerializeField]
    Material oldSky;
	// Use this for initialization
	void Start () {
        Glitch = GetComponent<AnalogGlitch>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.G))
        {
            print("Glitching");
            Glitch.scanLineJitter = 0.15f;
            Glitch.horizontalShake = 0.10f;
            Glitch.colorDrift = 0.3f;
        }
        if(Input.GetKeyUp(KeyCode.G)){
            Glitch.scanLineJitter = 0;
            Glitch.horizontalShake = 0;
            Glitch.colorDrift = 0;
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            RenderSettings.skybox = Skybox;
        }
        if (Input.GetKeyUp(KeyCode.H))
        {
            RenderSettings.skybox = oldSky;
        }

    }
}
