using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderTransition : MonoBehaviour {
    [SerializeField]
    Material Wireframe;
    [SerializeField]
    Material Normal;
    public float transitionSpeed;
    MeshRenderer ren;
	// Use this for initialization
	void Start () {
        ren = GetComponent<MeshRenderer>();
        StartCoroutine(Cor());
	}
	IEnumerator Cor()
    {
        float i = 0;
        while (i < 1)
        {
            i += Time.deltaTime*transitionSpeed;
            print(ren.transform.name);
            ren.material.Lerp(Wireframe, Normal, i);
            print("gnéé");
            yield return null;
        }
    }
	
}
