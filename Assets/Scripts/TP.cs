using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TP : MonoBehaviour {

    [SerializeField]
    GameObject visualGO;

    GameObject visual;

    public Camera cam;
    private void Start()
    {
        
        visual =  Instantiate(visualGO);
        visual.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            visual.SetActive(true);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, cam.transform.forward,out hit,10))
            {
                visual.transform.position = hit.point;
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            visual.SetActive(false);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, cam.transform.forward, out hit, 10))
            {
                transform.position = hit.point+Vector3.up*0.5f;
                StartCoroutine(Cor());
                
            }
            
        }
    }
    IEnumerator Cor()
    {
        cam.fieldOfView += 20;
        float max = cam.fieldOfView;
        float min = cam.fieldOfView - 20;
        float i = 0;
        while (i < 1)
        {
            cam.fieldOfView = Mathf.Lerp(max, min, i);
            i += Time.deltaTime * 8;
            yield return null;
        }
        cam.fieldOfView = min;
    }
}
