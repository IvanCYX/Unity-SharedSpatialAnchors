//Object spawner class -- A general spawner class to allow for the creation of physics objects

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour {
    public GameObject objectPrefab;
    GameObject lastObject = null;
    
    void Start() {
        lastObject = Instantiate(objectPrefab, transform.position, transform.rotation);
        setPhysicsObjectDefaults(lastObject);
        LaserManager.physicsObjects.Add(lastObject);

        LaserManager.UI.clearScreen += Clear;
    }
    
    void Update() {
        //Create a new physics object if the last one has moved far enough outside the bounding box of the spawner
        if(lastObjectMoved()) {
            lastObject = Instantiate(objectPrefab, transform.position, transform.rotation);
            setPhysicsObjectDefaults(lastObject);
            LaserManager.physicsObjects.Add(lastObject);
        }
    } 

    void Clear() {lastObject = null;}

    bool lastObjectMoved() {
        if(lastObject == null) {return true;}
        return Mathf.Abs(lastObject.transform.position.x - transform.position.x) > Constants.objectSpawnClearDistance ||
               Mathf.Abs(lastObject.transform.position.y - transform.position.y) > Constants.objectSpawnClearDistance/4 ||
               Mathf.Abs(lastObject.transform.position.z - transform.position.z) > Constants.objectSpawnClearDistance;
    }

    void setPhysicsObjectDefaults(GameObject physicsObject) {
        if(physicsObject.tag == "Plane Mirror Object") {
            
        } else if(physicsObject.tag == "Bi-Convex Lens Object") {
            LensPhysics.initializeMaterial(physicsObject, 0.5f);
        } else if(physicsObject.tag == "Bi-Concave Lens Object") {
            LensPhysics.initializeMaterial(physicsObject, -0.5f);
        } else if(physicsObject.tag == "Prism Object") {
            LensPhysics.initializeMaterial(physicsObject, 0.0f);
        } else if(physicsObject.tag == "Laser Pointer Object") {
            LaserPointerObject laserPointer = physicsObject.GetComponent<LaserPointerObject>();
            laserPointer.laserPointerType = 1;
            laserPointer.SpawnLasers();
        }
    }
} 
