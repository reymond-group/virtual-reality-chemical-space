using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleculeVisibility : MonoBehaviour {

    public GameObject molecule;

	// Use this for initialization
	void Start () {

    }

    // Update is called once per frame
    void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "[VRTK][AUTOGEN][HeadsetColliderContainer]" && this.molecule)
        {
            this.molecule.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "[VRTK][AUTOGEN][HeadsetColliderContainer]" && this.molecule)
        {
            this.molecule.SetActive(false);
        }
    }
}
