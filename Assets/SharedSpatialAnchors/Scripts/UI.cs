using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Photon.Pun;
using UnityEngine;

public class UI : MonoBehaviour
{
    public GameObject model;
    public GameObject earth;
    public Behaviour handGrabComponent1;
    public Behaviour handGrabComponent2;
    public Rigidbody rb;

    private void Start()
    {
        enableHandGrab();
    }

    public void enableHandGrab()
    {
        gameObject.GetComponent<Oculus.Interaction.OneGrabFreeTransformer>().enabled = true;
        gameObject.GetComponent<Oculus.Interaction.Grabbable>().enabled = true;
        rb.isKinematic = true;
        handGrabComponent1.enabled = true;
        handGrabComponent2.enabled = true;
        SampleController.Instance.Log("Hand Grab Enabled");
    }

    public void disableHandGrab()
    {
        gameObject.GetComponent<Oculus.Interaction.OneGrabFreeTransformer>().enabled = false;
        gameObject.GetComponent<Oculus.Interaction.Grabbable>().enabled = false;
        rb.isKinematic = false;
        handGrabComponent1.enabled = false;
        handGrabComponent2.enabled = false;
        SampleController.Instance.Log("Hand Grab Disabled");
    }

    public void ShowModel()
    {
        model.SetActive(true);
        earth.SetActive(false);
    }

    public void HideModel()
    {
        model.SetActive(false);
        earth.SetActive(true);
    }
}