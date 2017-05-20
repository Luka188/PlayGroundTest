using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitTimeToGrav : MonoBehaviour {
    public float TimeToWait;
	// Use this for initialization
	IEnumerator Start () {
        yield return new WaitForSeconds(TimeToWait);
        GetComponent<Rigidbody>().useGravity = true;
	}
	
	
}
