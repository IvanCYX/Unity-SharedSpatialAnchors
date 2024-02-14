using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleSlit : MonoBehaviour {
    public float deflectionAmplitude = 0.05f;
    public float deflectionTimeScale = 2f;

    public Vector3 restPosition;
    private float t = 0;
    private int deflectionDirection = 0;
    public bool displacementToggle;

    void Start() {
        restPosition = transform.localPosition;
    }

    void Update() {
        if(!displacementToggle) {
            if(deflectionDirection == -1) {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -deflectionAmplitude * (-(2 * t - 1) * (2 * t - 1) + 1));
                t += Time.deltaTime * deflectionTimeScale;
                if(t >= 1) {
                    deflectionDirection = 0;
                    transform.localPosition = restPosition;
                }
            } else if(deflectionDirection == 1) {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, deflectionAmplitude * (-(2 * t - 1) * (2 * t - 1) + 1));
                t += Time.deltaTime * deflectionTimeScale;
                if(t >= 1) {
                    deflectionDirection = 0;
                    transform.localPosition = restPosition;
                }
            }
        } else {
            if(deflectionDirection == -1) {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -deflectionAmplitude * (-(2 * t - 1) * (2 * t - 1) + 1) + DemoManager.singleSlitOffset.z);
                t += Time.deltaTime * deflectionTimeScale;
                if(t >= 1) {
                    deflectionDirection = 0;
                    transform.localPosition = restPosition + DemoManager.singleSlitOffset;
                }
            } else if(deflectionDirection == 1) {
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, deflectionAmplitude * (-(2 * t - 1) * (2 * t - 1) + 1) + DemoManager.singleSlitOffset.z);
                t += Time.deltaTime * deflectionTimeScale;
                if(t >= 1) {
                    deflectionDirection = 0;
                    transform.localPosition = restPosition + DemoManager.singleSlitOffset;
                }
            }
        }
    }

    public void StartLeftDeflection() {
        transform.localPosition = restPosition;
        if(displacementToggle) {transform.localPosition += DemoManager.singleSlitOffset;}
        t = 0;
        deflectionDirection = -1;
    }

    public void StartRightDeflection() {
        transform.localPosition = restPosition;
        if(displacementToggle) {transform.localPosition += DemoManager.singleSlitOffset;}
        t = 0;
        deflectionDirection = 1;
    }

    public void SwitchRestPosition() {
        if(DemoManager.electronSpawner.GetComponent<ElectronSpawner>().shootingMode != 7) {displacementToggle = true;}

        displacementToggle = !displacementToggle;

        transform.localPosition = restPosition;
        if(displacementToggle) {transform.localPosition += DemoManager.singleSlitOffset;}
    }
}
