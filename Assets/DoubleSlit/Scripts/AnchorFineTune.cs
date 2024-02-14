using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorFineTune : MonoBehaviour
{
    private float rotation, length, rotationIncrement;
    private Vector3 right, away;

    // Start is called before the first frame update
    void Start()
    {
        rotation = 0f;
        rotationIncrement = 0.25f;
        length = 0.002f;
        updateVectors();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateVectors()
    {
        rotation = gameObject.transform.localEulerAngles.y;
        float radian = rotation * Mathf.PI / 180f;
        right = new Vector3(Mathf.Cos(radian), 0, -Mathf.Sin(radian)) * length;
        Debug.Log(right.x * 50);
        Debug.Log(right.z * 50);
        away = new Vector3(Mathf.Cos(radian + Mathf.PI/2f), 0, -Mathf.Sin(radian + Mathf.PI / 2f)) * length;
    }

    public void rotRight()
    {
        transform.Rotate(new Vector3(0f, rotationIncrement, 0f));
        //rotation += rotationIncrement;
        updateVectors();
    }

    public void rotLeft()
    {
        transform.Rotate(new Vector3(0f, -rotationIncrement, 0f));
        //rotation -= rotationIncrement;
        updateVectors();
    }

    public void incX()
    {
        transform.localPosition += right;
    }

    public void decX()
    {
        transform.localPosition -= right;
    }

    public void incY()
    {
        gameObject.transform.localPosition += away;
    }

    public void decY()
    {
        gameObject.transform.localPosition -= away;
    }

    public void incZ()
    {
        gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y - length, gameObject.transform.localPosition.z);
    }

    public void decZ()
    {
        gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y + length, gameObject.transform.localPosition.z);
    }
}
