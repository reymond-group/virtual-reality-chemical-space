using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class MoleculeInteraction : MonoBehaviour {
    public bool isTouched = false;
    public string text = "";

    private GameObject toolTip;
    private GameObject toolTipContent;

	// Use this for initialization
	void Start () {
        this.toolTip = GameObject.FindGameObjectWithTag("ToolTip");
        this.toolTipContent = GameObject.FindGameObjectWithTag("ToolTipContent");

        VRTK_InteractableObject interactable = this.gameObject.GetComponent<VRTK_InteractableObject>();

        interactable.InteractableObjectTouched += Interactable_InteractableObjectTouched;
        interactable.InteractableObjectUntouched += Interactable_InteractableObjectUntouched;
    }

    private void Interactable_InteractableObjectUntouched(object sender, InteractableObjectEventArgs e)
    {
        if (this.toolTip && this.gameObject)
        {
            this.toolTip.transform.localPosition = new Vector3(-99999.0F, 0.0F, 0.0F);
        }

        this.isTouched = false;
    }

    private void Interactable_InteractableObjectTouched(object sender, InteractableObjectEventArgs e)
    {
        if (this.toolTip && this.gameObject)
        {
            this.toolTipContent.GetComponent<VRTK_ObjectTooltip>().UpdateText(this.text);
            this.toolTip.transform.localPosition = this.gameObject.transform.localPosition;
        }

        this.isTouched = true;
    }

    // Update is called once per frame
    void Update () {
        
	}
}
