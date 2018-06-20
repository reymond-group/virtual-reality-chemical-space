using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyListener : MonoBehaviour {
    public Material defaultSkybox;
    public Material alternativeSkybox;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.F))
        {
            // Toggle floor
            var floor = GameObject.Find("Plane").GetComponent<Renderer>();

            if (floor.enabled)
            {
                floor.enabled = false;
            }
            else
            {
                floor.enabled = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            // Toggle background
            if (RenderSettings.skybox == defaultSkybox)
            {
                RenderSettings.skybox = alternativeSkybox;
            }
            else
            {
                RenderSettings.skybox = defaultSkybox;
            }
        }
	}
}
