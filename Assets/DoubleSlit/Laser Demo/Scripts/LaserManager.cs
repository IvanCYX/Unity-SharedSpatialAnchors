//Laser Manager Class -- Holds public references to all of the important classes and prefabs in addition to handling the managment of existing lasers and physics objects

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class LaserManager {
    public static EventManager UI = Object.FindObjectOfType<EventManager>();
    public static LaserSpawner laserSpawner = Object.FindObjectOfType<LaserSpawner>();
    public static LensEditor lensEditor = Object.FindObjectOfType<LensEditor>();

    public static OVRHand leftHand = laserSpawner.leftHand;
    public static OVRHand rightHand = laserSpawner.rightHand;

    public static GameObject laserSegmentPrefab = laserSpawner.laserSegmentPrefabGameObject;

    public static Shader laserShader = laserSpawner.laserShader;
    public static Shader boundingBoxShader = laserSpawner.boundingBoxShader;
    public static Shader lensShader = laserSpawner.lensShader;

    public static Mesh biConvexLensMesh = lensEditor.biConvexLensMesh;
    public static Mesh biConcaveLensMesh = lensEditor.biConcaveLensMesh;

    public static List<Laser> lasers = laserSpawner.laserList;

    public static List<GameObject> physicsObjects = new List<GameObject>(0);

    static LaserManager() {
        UI.clearScreen += destroyLasers; UI.decreaseLaserRadius += downLaserRadius; UI.increaseLaserRadius += upLaserRadius; UI.updateLasers += updateLasers;
    }

    public static void updateLasers() {
        foreach(Laser laser in lasers) {
            laser.destroyBranches();
            laser.rayCastLaser();
        }
    }

    public static void updateLasersOnChange() {
        if(sceneChanged()) {
            updateLasers();
        }
    }

    private static bool sceneChanged() {
        foreach(GameObject physicsObject in physicsObjects) {
            if(physicsObject.transform.hasChanged) {
                physicsObject.transform.hasChanged = false;
                return true;
            }
        }
        return false;
    }

    public static Vector3 computeObjectLaserInteraction(Collider hitObjectCollider, Vector3 hitPoint, Vector3 rayDirection, ref float partialReflection,  ref Vector3 partialReflectionDirection) {
        GameObject hitObject = hitObjectCollider.gameObject;

        if(hitObject.tag == "Plane Mirror") {
            return LaserPhysics.reflectLaser(hitObject, rayDirection);
        } else if(hitObject.tag == "Lens") {
            return LaserPhysics.lensRefractLaser(hitObject, hitPoint, rayDirection);
        } else if(hitObject.tag == "Prism") {
            return LaserPhysics.prismRefractLaser(hitObject, rayDirection, ref partialReflection, ref partialReflectionDirection);
        } else {
            return rayDirection;
        }
    }

    public static LaserSegment findLaserSegment(GameObject laserSegmentToFind) {
        LaserSegment testSegment;
        foreach(Laser laser in lasers) {
            testSegment = laser.findLaserSegment(laserSegmentToFind);
            if(testSegment != null) {
                if(testSegment.gameObject == laserSegmentToFind) {
                    return testSegment;
                }
            }
        }
        return null;
    }

    public static Laser findLaser(GameObject laserSegmentToFind) {
        return findLaserSegment(laserSegmentToFind).parent;
    }

    private static void destroyLasers() {
        foreach(var laser in LaserManager.lasers) {
            laser.destroyBranches();
            laser.clearLaser();
        }
        LaserManager.lasers.Clear();
        
        foreach(GameObject physicsObject in physicsObjects) {
            Object.Destroy(physicsObject);
        }
        physicsObjects.Clear();
    }

    private static void downLaserRadius() {
        foreach(var laser in lasers) {
            laser.decreaseLaserRadius();  
        }

        if(lasers.Count != 0) {
            if(lasers[0].laserSegmentCount != 0) {
                laserSpawner.laserRadius = lasers[0].laserSegments[0].radius;
            }
        }
    }

    private static void upLaserRadius() {
        foreach(var laser in lasers) {
            laser.increaseLaserRadius();  
        } 

        if(lasers.Count != 0) {
            if(lasers[0].laserSegmentCount != 0) {
                laserSpawner.laserRadius = lasers[0].laserSegments[0].radius;
            }
        }
    }
}
