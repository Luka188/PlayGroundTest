using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorlogeToDelete : MonoBehaviour {

	
	// Update is called once per frame
	void Update () {
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z + Time.deltaTime * 180);
	}
}
