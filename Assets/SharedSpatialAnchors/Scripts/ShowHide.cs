using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class sceneObjects : MonoBehaviour
{
    //private PhotonView pv;
    public GameObject model;

    private void Start()
    {
        model.SetActive(false);
    }
/*    [PunRPC]
    public void HideModelRPC()
    {
        model.SetActive(false);
    }

    public void HideModel()
    {
        pv.RPC("HideModelRPC", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    public void ShowModelRPC()
    {
        model.SetActive(true);
        Debug.Log("Button Pressed");
    }

    public void ShowModel()
    {
        pv.RPC("ShowModelRPC", RpcTarget.AllBufferedViaServer);
    }*/

    public void ShowModel()
    {
        model.SetActive(true);
    }

    public void HideModel()
    {
        model.SetActive(false);
    }
}