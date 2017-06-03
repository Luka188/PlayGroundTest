using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSender : MonoBehaviour {
    [SerializeField]
    GameObject proj;

    public Animator anim;
    
    GameObject Go;

    // Update is called once per frame
    void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject z =  Instantiate(proj, transform.position,Quaternion.identity);
            z.GetComponent<Projectiles>().SendGoal(transform.forward);
        }
	}
}
