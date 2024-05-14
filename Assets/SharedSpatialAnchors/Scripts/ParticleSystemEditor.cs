using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemEditor : MonoBehaviour
{
    private float zval, limit, backLimit;
    public float scaledVal;
    private Vector3 prevPosition, curPosition;
    public ParticleSystem particleSystem; // Reference to the particle system
    private float startSpeed = 15f; // Default start speed value
    private Vector2 sliderRange = new Vector2(-0.095f, 0.095f); // Range of slider values

    void Start()
    {
        limit = 0.152f;
        backLimit = -0.03f;
        prevPosition = gameObject.transform.localPosition;
        updatePosition(prevPosition);
        // Get the particle system component
        particleSystem = GetComponent<ParticleSystem>();
        // Set the initial start speed
        SetStartSpeed(startSpeed);
    }

    void Update()
    {
        curPosition = gameObject.transform.localPosition;
        if (curPosition != prevPosition)
        {
            updatePosition(curPosition);
            // Update the start speed based on the scaled value
            UpdateStartSpeed(scaledVal);
        }
    }

    private void updatePosition(Vector3 newPos)
    {
        if (newPos.z > limit)
        {
            zval = limit;
        }
        else if (newPos.z < backLimit)
        {
            zval = backLimit;
        }
        else
        {
            zval = newPos.z;
        }
        prevPosition = new Vector3(0f, -0.004f, zval);
        gameObject.transform.localPosition = prevPosition;
        scaledVal = (zval + limit) / (2 * limit);
        Debug.Log(curPosition);
    }

    private void UpdateStartSpeed(float value)
    {
        // Remap the scaled value to the start speed range
        float remappedSpeed = remap(0f, 1f, sliderRange.x, sliderRange.y, value);
        SetStartSpeed(remappedSpeed);
    }

    private void SetStartSpeed(float speed)
    {
        var main = particleSystem.main;
        main.startSpeed = speed;
    }

    private float remap(float origFrom, float origTo, float targetFrom, float targetTo, float value)
    {
        float rel = Mathf.InverseLerp(origFrom, origTo, value);
        return Mathf.Lerp(targetFrom, targetTo, rel);
    }
}
