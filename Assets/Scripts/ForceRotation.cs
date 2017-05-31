using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceRotation : MonoBehaviour {
    Quaternion rot;
    
    public void SetRotWithNormal(Vector3 normal)
    {
        rot = Quaternion.LookRotation(normal);
    }
	
	// Update is called once per frame
	void Update () {
        if (transform.rotation != rot)
        {
            transform.rotation = rot;
        }	
	}
}
