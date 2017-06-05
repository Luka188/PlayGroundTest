using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour {
    public Vector3 ResetPoint = Vector3.zero;
    [SerializeField]
    bool Trigger = true;

    GameObject[] GreenTab;
	// Use this for initialization
	void Start () {
        GreenTab = GameObject.FindGameObjectsWithTag("Boost");
	}
	
    void Reset()
    {
        if (GreenTab != null)
        {
            foreach (GameObject k in GreenTab)
            {
                k.SetActive(true);
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (Trigger)
        {
            if (other.tag == "Player")
            {
                other.transform.position = ResetPoint;
                Reset();
                other.GetComponent<PlayerMovementsCC>().Reset();
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(ResetPoint, Vector3.one);
    }
}
