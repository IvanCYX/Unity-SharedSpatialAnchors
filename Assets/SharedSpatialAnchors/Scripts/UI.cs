using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Photon.Pun;
using UnityEngine;

public class UI : MonoBehaviour
{
    public GameObject mag;
    public GameObject earth;
    public GameObject renderer;
    public GameObject solarWind;
    public GameObject rand;
    public Behaviour handGrabComponent1;
    public Behaviour handGrabComponent2;
    public Rigidbody rb;
    private PhotonView pv;

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

    [PunRPC]
    public void ShowAlignRPC()
    {
        mag.SetActive(true);
        earth.SetActive(false);
        renderer.SetActive(true);
        solarWind.SetActive(false);
    }

    public void ShowAlign()
    {
        pv.RPC("ShowAlignRPC", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    public void HideAlignRPC()
    {
        mag.SetActive(true);
        earth.SetActive(true);
        renderer.SetActive(true);
        solarWind.SetActive(true);
    }

    public void HideAlign()
    {
        pv.RPC("HideModelRPC", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    public void ShowRandRPC()
    {
        rand.SetActive(true);
    }

    public void ShowRand()
    {
        pv.RPC("ShowRandRPC", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    public void HideRandRPC()
    {
        rand.SetActive(false);
    }

    public void HideRand()
    {
        pv.RPC("HideRandRPC", RpcTarget.AllBufferedViaServer);
    }
}