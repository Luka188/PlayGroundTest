using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TP : MonoBehaviour {

    [SerializeField]
    RawImage Pointor;
    [SerializeField]
    ParticleSystem PC;
    [SerializeField]
    Image Circle;

    public Camera cam;
    public float slowMotionSpeed;
    public float SlowMoDuration;
    public float CoolDown;

    bool SpellReady = true;


    private void Update()
    {
        if (Input.GetMouseButton(1)&&SpellReady)
        {
            Pointor.enabled = true;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, cam.transform.forward,out hit,10))
            {
                
                Pointor.color = Color.green;
            }
            else
            {
                Pointor.color = Color.black;
            }
        }
        if (Input.GetMouseButtonUp(1)&&SpellReady)
        {
            Pointor.enabled = false;
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
        Time.timeScale = slowMotionSpeed;
        Time.fixedDeltaTime = slowMotionSpeed * 0.015f;
        cam.fieldOfView += 20;
        float max = cam.fieldOfView;
        float min = cam.fieldOfView - 20;
        float i = 0;
        while (i < 1)
        {
            cam.fieldOfView = Mathf.Lerp(max, min, i);
            i += Time.deltaTime * SlowMoDuration;
            yield return null;
        }
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.015f;
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
