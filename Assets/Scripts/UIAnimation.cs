using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimation : MonoBehaviour {

    [SerializeField]
    RawImage GreenFeel;


    Color greenDef;

    private void Start()
    {
        greenDef = GreenFeel.color;
        
    }
    public void AnimateGreen()
    {
        StartCoroutine(GreenAnim());
    }
    IEnumerator GreenAnim()
    {
        GreenFeel.gameObject.SetActive(true);
        Color c1 = greenDef;
        Color c2 = new Color(c1.r, c1.g, c1.b, 0);
        float i = 0;
        while (i < 1)
        {
            i += Time.deltaTime * 2;
            GreenFeel.color = Color.Lerp(c1, c2, i);
            yield return null;
        }
        GreenFeel.gameObject.SetActive(false);

    }
}
