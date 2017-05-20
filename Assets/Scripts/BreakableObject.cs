using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject : MonoBehaviour {
    [SerializeField]
    GameObject Fractured;
	
	void Destruct()
    {
        Rigidbody oriBody = GetComponent<Rigidbody>();
        oriBody.isKinematic = true;
        GetComponent<MeshCollider>().enabled = false;
        GameObject k= Instantiate(Fractured,transform.position,transform.rotation);
        k.transform.localScale = transform.localScale;
        Destroy(transform.gameObject);
    }
    private void OnCollisionEnter(Collision col)
    {
        if(col.transform.tag!="Player"&&col.transform.tag!="Support")
            Destruct();
    }
}
