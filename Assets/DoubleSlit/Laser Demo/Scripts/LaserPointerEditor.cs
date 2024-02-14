using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LaserPointerEditor : MonoBehaviour {
    public GameObject laserArrayTypeSlider;
    public GameObject laserArrayLengthSlider;

    private Vector2 sliderRange = new Vector2(-0.095f, 0.095f);

    public TextMeshPro laserArrayTypeReadout;
    public TextMeshPro laserArrayLengthReadout;


    private uint laserArrayType;
    private float laserArrayLength;

    public bool sliderLock = true;
    [SerializeField]
    private uint sliderModified; 

    private Vector3 centerPosition = new Vector3(0.6f, 1.45f, 0.6f);
    private Vector3 boundingBoxScale = new Vector3(0.15f, 0.15f, 0.15f);
    private float boundingBoxThickness = 0.003f;
    private LaserSegment[] boundingBox = new LaserSegment[12];

    private LaserPointerObject laserArray = null;
    private GameObject laserPointerGameObject;

    void Start() {
        LaserManager.UI.clearScreen += clear;
        transform.position = centerPosition;
        createBoundingBox();
    }

    void Update() {
        if(laserArray == null) {
            if(checkForLaserArray()) {
                snapLaserArrayToCenter();
                readLaserArrayValues();
                updateSliders();
                changeBoundingBoxColor(Color.green);
                updateLaserArrayAndReadout();
            }
        } else {
            if(!checkForLaserArray()) {
                changeBoundingBoxColor(Color.red);
                return;  
            } 

            snapLaserArrayToCenter();
            if(sliderLock) {determineSliderModified();}
            calculateSliderValuesAfterModification();
            updateSliders();
            updateLaserArrayAndReadout();
            LaserManager.updateLasers();
        } 
    }

    private void clear() {
        laserArray = null;
        laserPointerGameObject = null;
        changeBoundingBoxColor(Color.red);
    }

    private void updateLaserArrayAndReadout() {
        laserArray.changeType(laserArrayType);
        laserArray.setLength(laserArrayLength);

        laserArrayTypeReadout.text = decimal.Round((decimal)laserArrayType,2).ToString();
        laserArrayLengthReadout.text = decimal.Round((decimal)laserArrayLength,2).ToString();
    }

    private void calculateSliderValuesAfterModification() {
        switch(sliderModified) {
            case 0: //No slider has been moved
                return;
            case 1: //Refractive index slider has been moved
                laserArrayType = (uint)readLaserArrayTypeSlider();
                return;
            case 2: //Radius of curvature slider has been moved
                laserArrayLength = readLaserArrayLengthSlider();
                return;
        }
    }

    private void determineSliderModified() {
        sliderModified = 0;
        float closestDstToSlider = 0.1f;
        Vector3 handPosition;

        if(Vector3.Distance(laserArrayLengthSlider.transform.position, LaserManager.leftHand.PointerPose.position) < Vector3.Distance(laserArrayLengthSlider.transform.position, LaserManager.rightHand.PointerPose.position)) {
            handPosition = LaserManager.leftHand.PointerPose.position;
        } else {
            handPosition = LaserManager.rightHand.PointerPose.position;
        }

        if(Vector3.Distance(handPosition, laserArrayTypeSlider.transform.position) < closestDstToSlider) {sliderModified = 1; closestDstToSlider = Vector3.Distance(handPosition, laserArrayTypeSlider.transform.position);}
        if(Vector3.Distance(handPosition, laserArrayLengthSlider.transform.position) < closestDstToSlider) {sliderModified = 2;}
    }

    private void updateSliders() {
        setLaserArrayTypeSlider();
        setLaserArrayLengthSlider();
    }

    private void readLaserArrayValues() {
        laserArrayType = laserArray.laserPointerType;
        laserArrayLength = laserArray.getLength();
    }

    private void snapLaserArrayToCenter() {
        laserPointerGameObject.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
        laserPointerGameObject.transform.position = centerPosition;
    }

    private bool checkForLaserArray() {
        foreach(GameObject physicsObject in LaserManager.physicsObjects) {
            if(physicsObject.tag == "Laser Pointer Object") {
                if(Mathf.Abs(physicsObject.transform.position.x - centerPosition.x) < boundingBoxScale.x + 0.025 &&
                Mathf.Abs(physicsObject.transform.position.y - centerPosition.y) < boundingBoxScale.y + 0.025 &&
                Mathf.Abs(physicsObject.transform.position.z - centerPosition.z) < boundingBoxScale.z + 0.025) {
                    laserArray = physicsObject.GetComponent<LaserPointerObject>();
                    laserPointerGameObject = physicsObject;
                    return true;
                } 
            }
        }

        laserArray = null;
        return false;
    }

    private float readLaserArrayTypeSlider() {
        return remap(sliderRange.x, sliderRange.y, 
                     1, Constants.laserPointerTypes, 
                     readSliderPosition(laserArrayTypeSlider));
    }

    private float readLaserArrayLengthSlider() {
        return remap(sliderRange.x, sliderRange.y,
                     0, Constants.maximumLaserPointerLength, 
                     readSliderPosition(laserArrayLengthSlider));
    }

    private float readSliderPosition(GameObject slider) {
        return slider.transform.localPosition.x;
    }

    private void setLaserArrayTypeSlider() {
        setSliderToPosition(laserArrayTypeSlider, remap(1, Constants.laserPointerTypes, 
                                                           sliderRange.x, sliderRange.y, 
                                                           laserArrayType));
    }

    private void setLaserArrayLengthSlider() {
        setSliderToPosition(laserArrayLengthSlider, remap(0, Constants.maximumLaserPointerLength, 
                                                     sliderRange.x, sliderRange.y, 
                                                     laserArrayLength));
    }

    private void setSliderToPosition(GameObject slider, float position) {
        slider.transform.localPosition = new Vector3(position, slider.transform.localPosition.y, slider.transform.localPosition.z);
    }

    private void changeBoundingBoxColor(Color color) {
        foreach(LaserSegment laserSegment in boundingBox) {
            laserSegment.setBoundingBoxShader(color);
        }
    }

    private void createBoundingBox() {
        Vector3[] verticesOnBox = {new Vector3(-boundingBoxScale.x,-boundingBoxScale.y,-boundingBoxScale.z), new Vector3(-boundingBoxScale.x,-boundingBoxScale.y,boundingBoxScale.z), 
                                   new Vector3(boundingBoxScale.x,-boundingBoxScale.y,boundingBoxScale.z), new Vector3(boundingBoxScale.x,-boundingBoxScale.y,-boundingBoxScale.z),
                                   new Vector3(-boundingBoxScale.x,boundingBoxScale.y,-boundingBoxScale.z), new Vector3(-boundingBoxScale.x,boundingBoxScale.y,boundingBoxScale.z), 
                                   new Vector3(boundingBoxScale.x,boundingBoxScale.y,boundingBoxScale.z), new Vector3(boundingBoxScale.x,boundingBoxScale.y,-boundingBoxScale.z)};

        boundingBox[0] = new LaserSegment(boundingBoxThickness, verticesOnBox[0], verticesOnBox[1]);
        boundingBox[1] = new LaserSegment(boundingBoxThickness, verticesOnBox[1], verticesOnBox[2]);
        boundingBox[2] = new LaserSegment(boundingBoxThickness, verticesOnBox[2], verticesOnBox[3]);
        boundingBox[3] = new LaserSegment(boundingBoxThickness, verticesOnBox[3], verticesOnBox[0]);

        boundingBox[4] = new LaserSegment(boundingBoxThickness, verticesOnBox[4], verticesOnBox[5]);
        boundingBox[5] = new LaserSegment(boundingBoxThickness, verticesOnBox[5], verticesOnBox[6]);
        boundingBox[6] = new LaserSegment(boundingBoxThickness, verticesOnBox[6], verticesOnBox[7]);
        boundingBox[7] = new LaserSegment(boundingBoxThickness, verticesOnBox[7], verticesOnBox[4]);

        boundingBox[8] = new LaserSegment(boundingBoxThickness, verticesOnBox[4], verticesOnBox[0]);
        boundingBox[9] = new LaserSegment(boundingBoxThickness, verticesOnBox[5], verticesOnBox[1]);
        boundingBox[10] = new LaserSegment(boundingBoxThickness, verticesOnBox[6], verticesOnBox[2]);
        boundingBox[11] = new LaserSegment(boundingBoxThickness, verticesOnBox[7], verticesOnBox[3]);

        foreach(LaserSegment laserSegment in boundingBox) {
            Transform segmentTransform = laserSegment.gameObject.transform;

            segmentTransform.position += centerPosition;
            segmentTransform.SetParent(gameObject.transform);

            laserSegment.resetTransformInfo();
        }

        changeBoundingBoxColor(Color.red);
    }

    private float remap(float origFrom, float origTo, float targetFrom, float targetTo, float value){
        float rel = Mathf.InverseLerp(origFrom, origTo, value);
        return Mathf.Lerp(targetFrom, targetTo, rel);
    }
}
