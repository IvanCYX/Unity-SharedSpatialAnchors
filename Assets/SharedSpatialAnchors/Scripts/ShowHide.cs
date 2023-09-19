using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class sceneObjects : MonoBehaviour
{
    private PhotonView pv;
    // Start is called before the first frame update
    public GameObject model;

    private void Start()
    {
        model.SetActive(true);
    }
    [PunRPC]
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
    }
}