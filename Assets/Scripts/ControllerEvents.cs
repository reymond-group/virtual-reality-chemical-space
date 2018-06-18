using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class ControllerEvents : MonoBehaviour {
    private bool isGripping = false;
    private Vector3 previousControllerPosition;
    private GameObject move;

	// Use this for initialization
	void Start () {
        GetComponent<VRTK_ControllerEvents>().GripPressed += new ControllerInteractionEventHandler(GripPressed);
        GetComponent<VRTK_ControllerEvents>().GripReleased += new ControllerInteractionEventHandler(GripReleased);

        this.move = GameObject.FindGameObjectWithTag("Mover");
    }

    private void GripPressed(object sender, ControllerInteractionEventArgs e)
    {
        var controller = VRTK_DeviceFinder.GetActualController(this.gameObject);
        previousControllerPosition = controller.transform.localPosition;

        this.isGripping = true;
    }

    private void GripReleased(object sender, ControllerInteractionEventArgs e)
    {
        this.isGripping = false;
    }

    // Update is called once per frame
    void Update () {
		if (this.isGripping)
        {
            var controller = VRTK_DeviceFinder.GetActualController(this.gameObject);

            var delta = previousControllerPosition - controller.transform.localPosition;

            if (this.move)
            {
                this.move.transform.localPosition += 2.0f * delta;
            }

            previousControllerPosition = controller.transform.localPosition;
        }
	}
}
