using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScale : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
    }

    public void ChangeHeight()
    {
        transform.localScale += new Vector3(1, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp("h"))
        {
            ChangeHeight();
        }
    }

    private float remap(float origFrom, float origTo, float targetFrom, float targetTo, float value)
    {
        float rel = Mathf.InverseLerp(origFrom, origTo, value);
        return Mathf.Lerp(targetFrom, targetTo, rel);
    }
}
