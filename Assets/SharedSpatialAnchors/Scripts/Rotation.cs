using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public float speed = 50;

    private void Update()
    {
        transform.Rotate(0, speed * Time.deltaTime, 0);
    }

    public void IncreaseSpeed()
    {
        speed += 10;
        Debug.Log("Speed increased to: " + speed);
    }

    public void DecreaseSpeed()
    {
        speed -= 10;
        if (speed < 0)
            speed = 0;
        Debug.Log("Speed decreased to: " + speed);
    }
}
