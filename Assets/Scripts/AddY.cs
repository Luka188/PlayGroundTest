using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddY : MonoBehaviour {
    public float speed;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += new Vector3(0, Time.deltaTime * speed, 0);
	}
    private void OnCollisionEnter(Collision collision)
    {
        transform.gameObject.SetActive(false);
    }
}
