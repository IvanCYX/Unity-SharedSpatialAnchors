using UnityEngine;
using Photon.Pun;
using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;

public class GrabController : MonoBehaviourPun
{
    private GameObject grabObject;
    private GameObject handGrabObject;
    private GameObject earthSolarWindObject;

    void Start()
    {
        grabObject = GameObject.FindGameObjectWithTag("Grab");
        handGrabObject = GameObject.FindGameObjectWithTag("HandGrab");
        earthSolarWindObject = GameObject.FindGameObjectWithTag("EarthSolarWind");
    }

    public void EnableGrab()
    {
        photonView.RPC("SetGrabObjectsActive", RpcTarget.All, true);
        SampleController.Instance.Log("Grab Enabled");
    }

    public void DisableGrab()
    {
        photonView.RPC("SetGrabObjectsActive", RpcTarget.All, false);
        SampleController.Instance.Log("Grab Disabled");
    }

    [PunRPC]
    void SetGrabObjectsActive(bool isActive)
    {
        if (grabObject != null)
        {
            grabObject.SetActive(isActive);
        }

        if (handGrabObject != null)
        {
            handGrabObject.SetActive(isActive);
        }

        if (earthSolarWindObject != null)
        {
            SetComponentsActive(earthSolarWindObject, isActive);
        }
    }

    private void SetComponentsActive(GameObject targetObject, bool isActive)
    {
        // Enable or disable components as required
        var grabbable = targetObject.GetComponent<Grabbable>();
        var physicsGrabbable = targetObject.GetComponent<PhysicsGrabbable>();
        var photonGrabbableObject = targetObject.GetComponent<PhotonGrabbableObject>();

        if (grabbable != null) grabbable.enabled = isActive;
        if (physicsGrabbable != null) physicsGrabbable.enabled = isActive;
        if (photonGrabbableObject != null) photonGrabbableObject.enabled = isActive;
    }
}
