using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class Electron : MonoBehaviour {
    //Member variables
    public Vector3 sourceToScreenDirection;
    public Vector3 deflectionDirection;
    public Vector3 direction;

    public float speed;
    bool deflected = false;

    private static float c1 = 4.1f;
    private static float c2 = 9.6f;
    public static Vector3 detectorPosition = new Vector3(-0.3256f, 1.692f, -0.428f);

    //Constructors
    public Electron(Vector3 direction, float speed) {
        this.direction = direction;
        this.speed = speed;
    }

    void Update() {
        gameObject.transform.position += direction * speed * Time.deltaTime;
        if(gameObject.transform.localPosition.magnitude > 2) {Destroy(gameObject);}
        if(gameObject.transform.localPosition.magnitude > 0.6f && deflected) {Destroy(gameObject);}
    }

    private void OnTriggerEnter(Collider collider) { 
        if(collider.gameObject.tag == "screen") {
            GameObject blip = Instantiate(DemoManager.blipPrefab, gameObject.transform.position, Quaternion.identity);
            blip.transform.parent = DemoManager.screen.transform;
            blip.transform.localPosition = new Vector3(0.47f, blip.transform.localPosition.y, blip.transform.localPosition.z);
            blip.transform.parent = DemoManager.blips.transform;
            blip.transform.up = DemoManager.blips.transform.right;

            Destroy(gameObject);
        } else if(collider.gameObject.tag == "slit" && !deflected) {
            Destroy(gameObject);
        } else if(collider.gameObject.tag == "deflector" && DemoManager.electronSpawner.GetComponent<ElectronSpawner>().shootingMode == 6) {
            if(DemoManager.electronSpawner.GetComponent<ElectronSpawner>().electronCount % 2 == 0) {
                direction = Quaternion.AngleAxis(3.5f, Vector3.up) * direction;
                collider.gameObject.GetComponent<SingleSlit>().StartLeftDeflection();
            } else {
                direction = Quaternion.AngleAxis(-3.5f, Vector3.up) * direction;
                collider.gameObject.GetComponent<SingleSlit>().StartRightDeflection();
            }

            deflected = true;

            /*
            direction = direction - deflectionDirection * Vector3.Dot(direction, deflectionDirection);
            direction += deflectionDirection * GetAngleFromDoubleSlitInterference();
            direction = Quaternion.AngleAxis(Random.Range(-5.0f, 5.0f), deflectionDirection) * direction;
            deflected = true; 
            */
        } else if(collider.gameObject.tag == "deflector" && DemoManager.electronSpawner.GetComponent<ElectronSpawner>().shootingMode == 7) {
            /*
            if(Random.Range(0, 10f) > 5) {
                direction = Quaternion.AngleAxis(3.5f, Vector3.up) * direction;
            } else {
                direction = Quaternion.AngleAxis(-3.5f, Vector3.up) * direction;
            }

            if(Random.Range(0, 10f) > 5) {
                collider.gameObject.GetComponent<SingleSlit>().StartLeftDeflection();
            } else {
                collider.gameObject.GetComponent<SingleSlit>().StartRightDeflection();
            } */

            direction = Quaternion.AngleAxis(-3.5f, Vector3.up) * direction;
            collider.gameObject.GetComponent<SingleSlit>().StartRightDeflection();

            deflected = true;
        }
    }

    private float GetAngleFromDoubleSlitInterference() {
        float angle;
        float probability;
        float doubleSlitProbability;
        while(true) {
            angle = Random.Range(-1f, 1f);
            probability = Random.Range(0.1f, 1.0f) * Random.Range(0.02f, 1.0f);
            doubleSlitProbability = Mathf.Pow(Mathf.Sin(c1 * Mathf.Sin(angle)) / (c1 * Mathf.Sin(angle)), 2) *
                                  Mathf.Pow(Mathf.Cos(c2 * Mathf.Sin(angle)), 2);
            if(doubleSlitProbability > probability) {return angle * 0.2f;}
        }
    } 
}
