using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager: MonoBehaviour{

    static TimeManager Instance;

    public float coolDown;
    float lastSpell;
    float pourcent;
    private void Start()
    {
        Instance = this;
    }
    public static void ChangeTime(float p)
    {
        Instance.DoTheCheck(p);
    }
    public static void ResetTime()
    {
        Instance.DoReset();
    }
    void DoReset()
    {
        StopCoroutine("TimeCor");
        StartCoroutine("Reset");
    }
    void DoTheCheck(float p)
    {
        pourcent = p;
        if (Time.time - lastSpell >= coolDown)
        {
            lastSpell = Time.time;
            StopCoroutine("ResetTime");
            StartCoroutine("TimeCor");
        }
    }

    IEnumerator Reset()
    {
        float startF = Time.fixedDeltaTime;
        float startS = Time.timeScale;
        float endF = 0.015f;
        float endS = 1;
        float i = 0;
        while (i < 1)
        {
            i += Time.deltaTime * 2;
            Time.fixedDeltaTime = Mathf.Lerp(startF, endF, i);
            Time.timeScale = Mathf.Lerp(startS, endS, i);
            yield return null;
        }
    }
    IEnumerator TimeCor()
    {
        float startF = Time.fixedDeltaTime;
        float startS = Time.timeScale;
        float endF = startF * (pourcent / 100);
        float  endS = startS* (pourcent / 100);
        float i = 0;
        while (i < 1)
        {
            i += Time.deltaTime*2*(1/Time.timeScale);
            Time.fixedDeltaTime = Mathf.Lerp(startF, endF, i);
            Time.timeScale = Mathf.Lerp(startS, endS, i);
            yield return null;
        }
        Time.fixedDeltaTime = endS;
        Time.fixedDeltaTime = endF;
        print("t" + Time.fixedDeltaTime);
        print("l" + Time.timeScale);
    }

}
