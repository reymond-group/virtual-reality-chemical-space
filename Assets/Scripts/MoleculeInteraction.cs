using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.UI;

public class MoleculeInteraction : MonoBehaviour {
    public bool isTouched = false;
    public string text = "";
    public String id = "";

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

    private IEnumerator LoadStructureImage(String id)
    {
        WWW www = new WWW("https://www.drugbank.ca/structures/" + id + "/image.png");
        yield return www;
        var sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));

        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Structure"))
        {
            var structure = obj.GetComponent<Image>();
            structure.sprite = sprite;
        }
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
            // Load the 2D structure drawing form drugbank
            Debug.Log("Id: " + this.id);
            StartCoroutine(this.LoadStructureImage(this.id));

            this.toolTipContent.GetComponent<VRTK_ObjectTooltip>().UpdateText(this.text);
            this.toolTip.transform.localPosition = this.gameObject.transform.localPosition;
        }

        this.isTouched = true;
    }

    // Update is called once per frame
    void Update () {
        
	}
}
