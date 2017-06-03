using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Projectiles : MonoBehaviour {
    Vector3 dir;
    public float speed;
    private void Start()
    {
        GetComponent<Skinner.SkinnerGlitch>().source = GameObject.FindGameObjectWithTag("Support").transform.GetChild(2).GetComponent<Skinner.SkinnerSource>();
    }
    public void SendGoal(Vector3 _dir)
    {
        dir = _dir;
        transform.eulerAngles = new Vector3(0,0, Mathf.Tan(dir.y / dir.x)*Mathf.Rad2Deg);
    }
    private void Update()
    {
        if (dir != null)
        {
            transform.position +=  dir *Time.deltaTime * speed;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag != "Player") 
            Destroy(gameObject);
    }
}
