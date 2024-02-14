using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DistanceHandGrabToggle : MonoBehaviour {
    public Behaviour distanceHandGrabComponent1;
    public Behaviour distanceHandGrabComponent2;

    public void enableDistanceHandGrab() {
        gameObject.GetComponent<Oculus.Interaction.OneGrabFreeTransformer>().enabled = true;
        gameObject.GetComponent<Oculus.Interaction.HandGrab.MoveAtSourceProvider>().enabled = true;
        distanceHandGrabComponent1.enabled = true;
        distanceHandGrabComponent2.enabled = true;
    }

    public void disableDistanceHandGrab() {
        gameObject.GetComponent<Oculus.Interaction.OneGrabFreeTransformer>().enabled = false;
        gameObject.GetComponent<Oculus.Interaction.HandGrab.MoveAtSourceProvider>().enabled = false;
        distanceHandGrabComponent1.enabled = false;
        distanceHandGrabComponent2.enabled = false;
    }

    /*
    void Update() {
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            enableDistanceHandGrab();
        } else if(Input.GetKeyDown(KeyCode.Alpha2)) {
            disableDistanceHandGrab();
        }
    } */
}
