using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TP : MonoBehaviour {

    [SerializeField]
    GameObject visualGO;
    [SerializeField]
    ParticleSystem PC;
    [SerializeField]
    Image Circle;
    GameObject visual;

    public Camera cam;

    public float CoolDown;
    bool SpellReady = true;
    private void Start()
    {
        
        visual =  Instantiate(visualGO);
        visual.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButton(1)&&SpellReady)
        {
            visual.SetActive(true);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, cam.transform.forward,out hit,10))
            {
                visual.transform.position = hit.point;
            }
            else
            {
                if (visual.activeSelf)
                {
                    visual.SetActive(false);
                }
            }
        }
        if (Input.GetMouseButtonUp(1)&&SpellReady)
        {
            visual.SetActive(false);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, cam.transform.forward, out hit, 10))
            {
                transform.position = hit.point+Vector3.up*0.5f;
                StopCoroutine("Cor");
                StartCoroutine("Cor");
                StartCoroutine(CoolDownCor());
                PC.Emit( 200);
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
    IEnumerator CoolDownCor()
    {

        SpellReady = false;
        float i = 0;
        Circle.fillAmount = 0;
        while (i < CoolDown)
        {
            Circle.fillAmount = i / CoolDown;
            i += Time.deltaTime;
            yield return null;
        }
        SpellReady = true;
    }
}
