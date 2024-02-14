using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixRotation : MonoBehaviour
{
    private Quaternion rotation;
    private float[] eulers;
    void Start()
    {
        rotation = gameObject.transform.rotation;
        eulers = new float[3];
        eulers[0] = rotation.eulerAngles.x;
        eulers[1] = rotation.eulerAngles.y;
        eulers[2] = rotation.eulerAngles.z;

    }

    // Update is called once per frame
    void LateUpdate()
    {
        float rotationX = gameObject.transform.eulerAngles.x;
        float rotationY = gameObject.transform.eulerAngles.y;
        float rotationZ = gameObject.transform.eulerAngles.z;
        gameObject.transform.rotation = Quaternion.Euler(eulers[0], rotationY, eulers[2]);        
    }
}
