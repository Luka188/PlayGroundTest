using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PostProcessing;

public class TP : MonoBehaviour {

    [SerializeField]
    RawImage Pointor;
    [SerializeField]
    ParticleSystem PC;
    [SerializeField]
    Image Circle;
    [SerializeField]
    PostProcessingBehaviour PPB;

    PostProcessingProfile PPP;

    public Camera cam;
    public float slowMotionSpeed;
    public float SlowMoDuration;
    public float CoolDown;

    bool Spelling;
    bool SpellReady = true;
    PlayerMovements PM;
    private void Start()
    {
        PM = GetComponent<PlayerMovements>();
        PPP = PPB.profile;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)&&SpellReady)
        {
            StopCoroutine("RemoveVignette");
            StartCoroutine("AddVignette");
            Spelling = true;
        }
        if (Input.GetMouseButton(1)&&SpellReady&&Spelling)
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
        if (Input.GetMouseButtonDown(0)&&Spelling)
        {
            Pointor.enabled = false;
            StopCoroutine("AddVignette");
            StartCoroutine("RemoveVignette");
            Spelling = false;
        }
        if (Input.GetMouseButtonUp(1)&&SpellReady&&Spelling)
        {
            Spelling = false;
            Pointor.enabled = false;
            StopCoroutine("AddVignette");
            StartCoroutine("RemoveVignette");
            RaycastHit hit;
            if (Physics.Raycast(transform.position, cam.transform.forward, out hit, 10))
            {
                transform.position = hit.point+Vector3.up*0.5f;
                PM.ResetVelocity();
                StopCoroutine("Cor");
                StartCoroutine("Cor");
                StartCoroutine(CoolDownCor());
                PC.Emit(200);
            }
        }
    }

    IEnumerator AddVignette()
    {
        VignetteModel.Settings nvignette = PPP.vignette.settings;
        float startp = nvignette.intensity;
        float endp = 0.4f;
        float i = 0;
        while (i < 1)
        {
            i += Time.deltaTime * 8;
            nvignette.intensity = Mathf.Lerp(startp, endp, i);
            PPP.vignette.settings = nvignette; 
            yield return null;
        }
    }
    IEnumerator RemoveVignette()
    {
        VignetteModel.Settings nvignette = PPP.vignette.settings;
        float startp = nvignette.intensity;
        float endp = 0.3f;
        float i = 0;
        while (i < 1)
        {
            i += Time.deltaTime * 4;
            nvignette.intensity = Mathf.Lerp(startp, endp, i);
            PPP.vignette.settings = nvignette;
            yield return null;
        }
    }
    IEnumerator Cor()
    {
        
        Time.timeScale = slowMotionSpeed;
        Time.fixedDeltaTime = slowMotionSpeed * 0.015f;
        float i = 0;
        while (i < 1)
        {
            i += Time.deltaTime * 1/slowMotionSpeed;
            yield return null; 
        }
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.015f;
    }
    IEnumerator CoolDownCor()
    {
        SpellReady = false;
        float i = 0;
        Circle.fillAmount = 0;
        while (i < CoolDown)
        {
            i += Time.deltaTime;
            Circle.fillAmount = i / CoolDown;
            yield return null;
        }
        SpellReady = true;
    }
}
