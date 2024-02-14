//Laser Spawner Class -- Handles the creation of new lasers and resetting the scene to default configurations

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LaserSpawner : MonoBehaviour {
    public float laserRadius = 0.0015f;

    //References to hand tracking modules
    public OVRHand leftHand;
    public OVRHand rightHand;

    //Reference to laser segment prefab, start a list of lasers, and reset the laser ID
    public GameObject laserSegmentPrefabGameObject;
    public GameObject laserPointerPrefab;

    public Shader laserShader;
    public Shader boundingBoxShader;
    public Shader lensShader;

    public List<Laser> laserList = new List<Laser>();
    public uint currentLaserID = 0;

    //Init variables to hold info about new laser placement
    bool laserPlacementMode = false;
    bool laserBeingPlaced = false;
    Laser newLaser = null;

    bool leftPinch;
    bool rightPinch;

    void Start() {
        //Locate the UI object and subscribe all of the button actions to action functions
        LaserManager.UI.startSetup1 += setup1; LaserManager.UI.startSetup2 += setup2; LaserManager.UI.startSetup3 += setup3; LaserManager.UI.startSetup4 += setup4; 
        LaserManager.UI.startSetup5 += setup5; LaserManager.UI.startSetup6 += setup6; LaserManager.UI.startSetup7 += setup7; LaserManager.UI.startSetup8 += setup8;
        LaserManager.UI.switchLaserPlacementMode += switchLaserPlacementMode;
    }

    void Update() {
        if(laserPlacementMode) {
            //Determine the current pinch state of each hand
            leftPinch = leftHand.GetFingerIsPinching(OVRHand.HandFinger.Index);
            rightPinch = rightHand.GetFingerIsPinching(OVRHand.HandFinger.Index);
            
            //Determine whether a new laser should be created
            if(leftPinch && laserBeingPlaced == false) {
                laserBeingPlaced = true;
                //Create a new laser at the pinch position of the left hand
                newLaser = new Laser(leftHand.PointerPose.position, Vector3.up, 0.001f, laserRadius, currentLaserID);
            } else if(!leftPinch && laserBeingPlaced == true) {
                //If the left hand is no longer pinching, stop tracking the laser
                newLaser.updateTotalLength();
                newLaser.updateDirection();
                LaserManager.lasers.Add(newLaser);
                ++currentLaserID;
                newLaser = null;
                laserBeingPlaced = false;
                LaserManager.updateLasers();
            } 

            //Move the endpoint of the new laser to the pinch position of the left hand
            if(newLaser != null) {
                newLaser.getFirstLaserSegment().changeEndpoint(leftHand.PointerPose.position);
            }
        }

        //LaserManager.updateLasers();
        LaserManager.updateLasersOnChange();
    }

    void switchLaserPlacementMode() {
        if(laserPlacementMode) {laserPlacementMode = false;} else {laserPlacementMode = true;}
    }

    //Default laser scenes
    void setup1() {LaserScenes.horizontalBeam(ref currentLaserID);}
    void setup2() {LaserScenes.verticalBeam(ref currentLaserID);}
    void setup3() {LaserScenes.horizontalCircularBeamArray(ref currentLaserID);}
    void setup4() {LaserScenes.fiveSparseVerticalGrid(ref currentLaserID);}
    void setup5() {LaserScenes.doubleCone(ref currentLaserID);}
    void setup6() {LaserScenes.nineDenseHorizontalPlane(ref currentLaserID);}
    void setup7() {LaserScenes.nineDenseHorizontalFan(ref currentLaserID);}
    void setup8() {LaserScenes.largeHorizontalFan(ref currentLaserID);}
}
