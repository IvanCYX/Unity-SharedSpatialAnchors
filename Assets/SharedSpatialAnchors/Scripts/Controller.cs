using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public float coordIncrement = 1f;
    public float rotIncrement = 1f;
    
    public void DecreaseX() { DemoManager.MagnetScene.transform.position = new Vector3(DemoManager.MagnetScene.transform.position.x - coordIncrement, DemoManager.MagnetScene.transform.position.y, DemoManager.MagnetScene.transform.position.z); }
    public void DecreaseY() { DemoManager.MagnetScene.transform.position = new Vector3(DemoManager.MagnetScene.transform.position.x, DemoManager.MagnetScene.transform.position.y - coordIncrement, DemoManager.MagnetScene.transform.position.z); }
    public void DecreaseZ() { DemoManager.MagnetScene.transform.position = new Vector3(DemoManager.MagnetScene.transform.position.x, DemoManager.MagnetScene.transform.position.y, DemoManager.MagnetScene.transform.position.z - coordIncrement); }
    public void DecreaseXRot() { DemoManager.MagnetScene.transform.rotation = DemoManager.MagnetScene.transform.rotation * Quaternion.AngleAxis(-rotIncrement, DemoManager.MagnetScene.transform.right); }
    public void DecreaseYRot() { DemoManager.MagnetScene.transform.rotation = DemoManager.MagnetScene.transform.rotation * Quaternion.AngleAxis(-rotIncrement, DemoManager.MagnetScene.transform.up); }
    public void DecreaseZRot() { DemoManager.MagnetScene.transform.rotation = DemoManager.MagnetScene.transform.rotation * Quaternion.AngleAxis(-rotIncrement, DemoManager.MagnetScene.transform.forward); }

    public void IncreaseX() { DemoManager.MagnetScene.transform.position = new Vector3(DemoManager.MagnetScene.transform.position.x + coordIncrement, DemoManager.MagnetScene.transform.position.y, DemoManager.MagnetScene.transform.position.z); }
    public void IncreaseY() { DemoManager.MagnetScene.transform.position = new Vector3(DemoManager.MagnetScene.transform.position.x, DemoManager.MagnetScene.transform.position.y + coordIncrement, DemoManager.MagnetScene.transform.position.z); }
    public void IncreaseZ() { DemoManager.MagnetScene.transform.position = new Vector3(DemoManager.MagnetScene.transform.position.x, DemoManager.MagnetScene.transform.position.y, DemoManager.MagnetScene.transform.position.z + coordIncrement); }
    public void IncreaseXRot() { DemoManager.MagnetScene.transform.rotation = DemoManager.MagnetScene.transform.rotation * Quaternion.AngleAxis(rotIncrement, DemoManager.MagnetScene.transform.right); }
    public void IncreaseYRot() { DemoManager.MagnetScene.transform.rotation = DemoManager.MagnetScene.transform.rotation * Quaternion.AngleAxis(rotIncrement, DemoManager.MagnetScene.transform.up); }
    public void IncreaseZRot() { DemoManager.MagnetScene.transform.rotation = DemoManager.MagnetScene.transform.rotation * Quaternion.AngleAxis(rotIncrement, DemoManager.MagnetScene.transform.forward); }

    public void SpawnNewMagnet()
    {
        GameObject Magnet = Instantiate(DemoManager.LargeMagnet, gameObject.transform.position, Quaternion.identity);
    }

    public void HideModel()
    {
        DemoManager.ironFilling.SetActive(false);
    }
    public void ShowModel()
    {
        DemoManager.ironFilling.SetActive(true);
    }

}
