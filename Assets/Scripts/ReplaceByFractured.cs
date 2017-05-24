using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceByFractured : MonoBehaviour {
    [SerializeField]
    ParticleSystem PC;

    public float ExplosionForce;
    public float Radius;

    public Vector3 Point;
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == 8)
        {
            print(col.name);
            while(transform.childCount!=0)
            {
                Transform child = transform.GetChild(0);
                child.gameObject.SetActive(true);
                Vector3 newVec = child.transform.position - Point;
                float power = Radius - newVec.magnitude;
                if (power > 0)
                {
                    print("power");
                    child.GetComponent<Rigidbody>().AddForce(newVec*power*ExplosionForce, ForceMode.VelocityChange);
                }
                
                child.parent = transform.parent;
            }

            this.gameObject.SetActive(false);

        }
        
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(Point, Vector3.one);
        Gizmos.DrawWireSphere(Point,Radius);
    }
}
