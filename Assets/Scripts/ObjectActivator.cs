using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectActivator : MonoBehaviour {
    [SerializeField]
    float distToRender;
    [SerializeField]
    float animationSpeed;

    bool done = false;

    MeshRenderer ren;
    Transform player;
	// Use this for initialization
	void Start () {
        ren = GetComponent<MeshRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {
        print((Vector3.Distance(player.position, transform.position)));
        if (Vector3.Distance(player.position, transform.position) < distToRender&&!done)
        {
            print("go");
            done = true;
            StartCoroutine(Anim());
        }
	}
    IEnumerator Anim()
    {
        float i = 0;
        Material wire = ren.materials[0];
        Color a0 = new Color(1, 1, 1, 0);
        Color a1 = new Color(1,1,1,1);
        while (i < 1)
        {
            i += Time.deltaTime * animationSpeed;
            wire.color = Color.Lerp(a0, a1, i);
            yield return null;
        }
        i = 0;
        while (i < 1)
        {
            i += Time.deltaTime * animationSpeed;
            for (int y = 1; y < ren.materials.Length; y++)
            {
                ren.materials[y].color = Color.Lerp(a0, a1, i);
            }
            yield return null;
        }

    }
}
