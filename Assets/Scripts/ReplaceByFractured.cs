using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceByFractured : MonoBehaviour
{
    [SerializeField]
    ParticleSystem PC;

    public float timeBeforeExplose;
    public float ExplosionForce;
    public float Radius;

    public Vector3 Point;

    private void Start()
    {
        int count = transform.childCount;
        for (int i = 0; i < count; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (timeBeforeExplose >= 0 && Time.time > timeBeforeExplose)
        {
            Explose();
            timeBeforeExplose = -1;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer == 8)
        {
            Explose();
        }

    }

    private void Explose()
    {
        while (transform.childCount != 0)
        {
            Transform child = transform.GetChild(0);
            child.gameObject.SetActive(true);
            Vector3 newVec = child.transform.position - (Point + transform.position);
            float power = Radius - newVec.magnitude;
            if (power > 0)
            {
                Rigidbody rigidB = child.GetComponent<Rigidbody>();
                if (rigidB != null)
                    rigidB.AddForce(newVec * power * ExplosionForce, ForceMode.VelocityChange);
            }

            child.parent = transform.parent;
        }

        this.gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(Point + transform.position, Vector3.one);
        Gizmos.DrawWireSphere(Point + transform.position, Radius);
    }
}
