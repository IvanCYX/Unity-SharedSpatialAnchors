using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LensEditor : MonoBehaviour {
    public Mesh biConvexLensMesh;
    public Mesh biConcaveLensMesh;

    public GameObject indexOfRefractionSlider;
    public GameObject radiusOfCurvatureSlider;
    public GameObject focalLengthSlider;
    public GameObject lensThicknessSlider;
    public GameObject lensScaleSlider;

    private Vector2 sliderRange = new Vector2(-0.095f, 0.095f);

    public TextMeshPro indexOfRefractionReadout;
    public TextMeshPro radiusOfCurvatureReadout;
    public TextMeshPro focalLengthReadout;
    public TextMeshPro lensThicknessReadout;
    public TextMeshPro lensScaleReadout;

    public TextMeshPro radiusOfCurvatureMinReadout;
    public TextMeshPro radiusOfCurvatureMaxReadout;
    public TextMeshPro focalLengthMinReadout;
    public TextMeshPro focalLengthMaxReadout;

    public GameObject focalPoint1;
    public GameObject focalPoint2;

    private float indexOfRefraction;
    private float focalLength;
    private float radiusOfCurvature;
    private float lensThickness;
    private float lensScale;

    public bool sliderLock = true;
    [SerializeField]
    private uint sliderModified; 

    private Vector3 centerPosition = new Vector3(-0.5f, 1.45f, 0.6f);
    private Vector3 boundingBoxScale = new Vector3(0.15f, 0.15f, 0.15f);
    private float boundingBoxThickness = 0.003f;
    private LaserSegment[] boundingBox = new LaserSegment[12];

    private GameObject lens = null;
    private GameObject prism = null;

    void Start() {
        LaserManager.UI.clearScreen += clear;
        transform.position = centerPosition;
        createBoundingBox();
        hideLensVisualization();
    }

    void Update() {
        if(lens == null && prism == null) {
            if(checkForLens()) {
                snapLensToCenter();
                readLensValues();
                updateSliders();
                changeBoundingBoxColor(Color.green);
                updateLensAndReadout();
                showLensVisualization();
            } else if(checkForPrism()) {
                snapPrismToCenter();
                readPrismValues();
                updateSliders();
                updatePrismAndReadout();
                changeBoundingBoxColor(Color.green);
            }
        } else if(lens == null && prism != null) {
            if(!checkForPrism()) {
                changeBoundingBoxColor(Color.red);
                return;  
            } 

            snapPrismToCenter();
            if(sliderLock) {determineSliderModified();}
            calculatePrismSliderValuesAfterModification();
            updateSliders();
            updatePrismAndReadout();
        } else {
            if(!checkForLens()) {
                changeBoundingBoxColor(Color.red);
                hideLensVisualization();
                return;  
            } 

            snapLensToCenter();
            if(sliderLock) {determineSliderModified();}
            calculateSliderValuesAfterModification();
            updateSliders();
            updateLensAndReadout();
        } 
    }

    private void clear() {
        lens = null;
        prism = null;
        changeBoundingBoxColor(Color.red);
        hideLensVisualization();
    }

    private void updateLensAndReadout() {
        float postCutoffRadiusOfCurvature;
        if(radiusOfCurvature < 0) {
            postCutoffRadiusOfCurvature = Mathf.Min(radiusOfCurvature, -Constants.lensRadiusOfCurvatureDeadZone);
        } else {
            postCutoffRadiusOfCurvature = Mathf.Max(radiusOfCurvature, Constants.lensRadiusOfCurvatureDeadZone);
        }

        LensPhysics.setIndexOfRefraction(lens, indexOfRefraction);
        LensPhysics.setRadiusOfCurvature(lens, postCutoffRadiusOfCurvature);
        LensPhysics.updateLensThickness(lens); 
        LensPhysics.setOverallScale(lens, LensPhysics.getThickness(lens), lensScale);

        indexOfRefractionReadout.text = decimal.Round((decimal)indexOfRefraction,2).ToString();
        focalLengthReadout.text = decimal.Round((decimal)(LensPhysics.findFocalLength(indexOfRefraction, postCutoffRadiusOfCurvature) * lensScale),2).ToString();
        radiusOfCurvatureReadout.text = decimal.Round((decimal)(postCutoffRadiusOfCurvature * lensScale),2).ToString();
        lensThicknessReadout.text = decimal.Round((decimal)LensPhysics.calculateLensThickness(postCutoffRadiusOfCurvature),2).ToString();
        lensScaleReadout.text = decimal.Round((decimal)lensScale,2).ToString();

        radiusOfCurvatureMinReadout.text = decimal.Round((decimal)(Constants.minimumLensRadiusOfCurvature * lensScale),2).ToString();
        radiusOfCurvatureMaxReadout.text = decimal.Round((decimal)(Constants.maximumLensRadiusOfCurvature * lensScale),2).ToString();
        focalLengthMinReadout.text = decimal.Round((decimal)(Constants.minimumFocalPoint * lensScale),2).ToString();
        focalLengthMaxReadout.text = decimal.Round((decimal)(Constants.maximumFocalPoint * lensScale),2).ToString();

        updateFocalPoints(LensPhysics.findFocalLength(indexOfRefraction, postCutoffRadiusOfCurvature) * lensScale);
    }

    private void updatePrismAndReadout() {
        LensPhysics.setIndexOfRefraction(prism, indexOfRefraction);
        LensPhysics.setOverallScale(prism, 1, lensScale);

        indexOfRefractionReadout.text = decimal.Round((decimal)indexOfRefraction,2).ToString();
        lensScaleReadout.text = decimal.Round((decimal)lensScale,2).ToString();
    }

    private void calculateSliderValuesAfterModification() {
        switch(sliderModified) {
            case 0: //No slider has been moved
                return;
            case 1: //Refractive index slider has been moved
                indexOfRefraction = readRefractiveIndexSlider();
                focalLength = LensPhysics.findFocalLength(indexOfRefraction, radiusOfCurvature);
                return;
            case 2: //Radius of curvature slider has been moved
                radiusOfCurvature = readRadiusOfCurvatureSlider();
                focalLength = LensPhysics.findFocalLength(indexOfRefraction, radiusOfCurvature);
                lensThickness = LensPhysics.calculateLensThickness(radiusOfCurvature);
                return;
            case 3: //Focal length slider has been moved
                focalLength = readFocalLengthSlider();
                focalLength = Mathf.Clamp(focalLength, -LensPhysics.getFocalLengthRange(indexOfRefraction).y, LensPhysics.getFocalLengthRange(indexOfRefraction).y);
                radiusOfCurvature = LensPhysics.calculateRadiusOfCurvature(indexOfRefraction, focalLength);
                lensThickness = LensPhysics.calculateLensThickness(radiusOfCurvature);
                return;
            case 4: //Lens thickness slider has been moved
                lensThickness = readLensThicknessSlider();
                radiusOfCurvature = Mathf.Clamp(LensPhysics.calculateRadiusOfCurvature(lensThickness), Constants.minimumLensRadiusOfCurvature,Constants.maximumLensRadiusOfCurvature);
                focalLength = LensPhysics.findFocalLength(indexOfRefraction, radiusOfCurvature);
                return;
            case 5:
                lensScale = readLensScaleSlider();
                return;
        }
    }

    private void calculatePrismSliderValuesAfterModification() {
        switch(sliderModified) {
            case 0: //No slider has been moved
                return;
            case 1: //Refractive index slider has been moved
                indexOfRefraction = readRefractiveIndexSlider();
                return;
            case 2: //Radius of curvature slider has been moved
                return;
            case 3: //Focal length slider has been moved
                return;
            case 4: //Lens thickness slider has been moved
                //lensThickness = readLensThicknessSlider();
                //radiusOfCurvature = Mathf.Clamp(LensPhysics.calculateRadiusOfCurvature(lensThickness), Constants.minimumLensRadiusOfCurvature,Constants.maximumLensRadiusOfCurvature);
                //focalLength = LensPhysics.findFocalLength(indexOfRefraction, radiusOfCurvature);
                return;
            case 5:
                lensScale = readLensScaleSlider();
                return;
        }
    }

    private void determineSliderModified() {
        sliderModified = 0;
        float closestDstToSlider = 0.1f;
        Vector3 handPosition;

        if(Vector3.Distance(focalLengthSlider.transform.position, LaserManager.leftHand.PointerPose.position) < Vector3.Distance(focalLengthSlider.transform.position, LaserManager.rightHand.PointerPose.position)) {
            handPosition = LaserManager.leftHand.PointerPose.position;
        } else {
            handPosition = LaserManager.rightHand.PointerPose.position;
        }

        if(Vector3.Distance(handPosition, indexOfRefractionSlider.transform.position) < closestDstToSlider) {sliderModified = 1; closestDstToSlider = Vector3.Distance(handPosition, indexOfRefractionSlider.transform.position);}
        if(Vector3.Distance(handPosition, radiusOfCurvatureSlider.transform.position) < closestDstToSlider) {sliderModified = 2; closestDstToSlider = Vector3.Distance(handPosition, radiusOfCurvatureSlider.transform.position);}
        if(Vector3.Distance(handPosition, focalLengthSlider.transform.position) < closestDstToSlider) {sliderModified = 3; closestDstToSlider = Vector3.Distance(handPosition, focalLengthSlider.transform.position);}
        if(Vector3.Distance(handPosition, lensThicknessSlider.transform.position) < closestDstToSlider) {sliderModified = 4; closestDstToSlider = Vector3.Distance(handPosition, lensThicknessSlider.transform.position);}
        if(Vector3.Distance(handPosition, lensScaleSlider.transform.position) < closestDstToSlider) {sliderModified = 5;}
    }

    private void updateSliders() {
        setRefractiveIndexSlider();
        setFocalLengthSlider();
        setRadiusOfCurvatureSlider();
        setLensThicknessSlider();
        setLensScaleSlider();
    }

    private void readLensValues() {
        indexOfRefraction = LensPhysics.getIndexOfRefraction(lens);
        focalLength = LensPhysics.findFocalLength(lens);
        radiusOfCurvature = LensPhysics.getRadiusOfCurvature(lens);
        lensThickness = Mathf.Sign(radiusOfCurvature) * LensPhysics.getThickness(lens);
        lensScale = LensPhysics.getScale(lens);
    }

    private void readPrismValues() {
        indexOfRefraction = LensPhysics.getIndexOfRefraction(prism);
        lensScale = LensPhysics.getScale(prism);
    }

    private void snapLensToCenter() {
        lens.transform.rotation = Quaternion.identity;
        lens.transform.position = centerPosition;
    }

    private void snapPrismToCenter() {
        prism.transform.rotation = Quaternion.identity;
        prism.transform.position = centerPosition;
    }

    private bool checkForLens() {
        foreach(GameObject physicsObject in LaserManager.physicsObjects) {
            if(physicsObject.tag == "Bi-Convex Lens Object" || physicsObject.tag == "Bi-Concave Lens Object") {
                if(Mathf.Abs(physicsObject.transform.position.x - centerPosition.x) < boundingBoxScale.x + 0.05 &&
                Mathf.Abs(physicsObject.transform.position.y - centerPosition.y) < boundingBoxScale.y + 0.05 &&
                Mathf.Abs(physicsObject.transform.position.z - centerPosition.z) < boundingBoxScale.z + 0.05) {
                    lens = physicsObject;
                    prism = null;
                    return true;
                } 
            }
        }

        lens = null;
        return false;
    }

    private bool checkForPrism() {
        foreach(GameObject physicsObject in LaserManager.physicsObjects) {
            if(physicsObject.tag == "Prism Object") {
                if(Mathf.Abs(physicsObject.transform.position.x - centerPosition.x) < boundingBoxScale.x + 0.05 &&
                Mathf.Abs(physicsObject.transform.position.y - centerPosition.y) < boundingBoxScale.y + 0.05 &&
                Mathf.Abs(physicsObject.transform.position.z - centerPosition.z) < boundingBoxScale.z + 0.05) {
                    prism = physicsObject;
                    lens = null;
                    return true;
                } 
            }
        }


        prism = null;
        return false;
    }

    private float readRefractiveIndexSlider() {
        return remap(sliderRange.x, sliderRange.y, 
                     Constants.minimumRefractiveIndex, Constants.maximumRefractiveIndex, 
                     readSliderPosition(indexOfRefractionSlider));
    }

    private float readFocalLengthSlider() {
        return remap(sliderRange.x, sliderRange.y,
                     Constants.minimumFocalPoint, Constants.maximumFocalPoint, 
                     readSliderPosition(focalLengthSlider));
    }

    private float readRadiusOfCurvatureSlider() {
        return remap(sliderRange.x, sliderRange.y, 
                     Constants.minimumLensRadiusOfCurvature, Constants.maximumLensRadiusOfCurvature, 
                     readSliderPosition(radiusOfCurvatureSlider));
    }

    private float readLensThicknessSlider() {
        return remap(sliderRange.x, sliderRange.y, 
                     Constants.minimumLensThickness, Constants.maximumLensThickness, 
                     readSliderPosition(lensThicknessSlider));
    }

    private float readLensScaleSlider() {
        return remap(sliderRange.x, sliderRange.y, 
                     Constants.minimumLensScale, Constants.maximumLensScale, 
                     readSliderPosition(lensScaleSlider));
    }

    private float readSliderPosition(GameObject slider) {
        return slider.transform.localPosition.x;
    }

    private void setRefractiveIndexSlider() {
        setSliderToPosition(indexOfRefractionSlider, remap(Constants.minimumRefractiveIndex, Constants.maximumRefractiveIndex, 
                                                           sliderRange.x, sliderRange.y, 
                                                           indexOfRefraction));
    }

    private void setFocalLengthSlider() {
        setSliderToPosition(focalLengthSlider, remap(Constants.minimumFocalPoint, Constants.maximumFocalPoint, 
                                                     sliderRange.x, sliderRange.y, 
                                                     focalLength));
    }

    private void setRadiusOfCurvatureSlider() {
        setSliderToPosition(radiusOfCurvatureSlider, remap(Constants.minimumLensRadiusOfCurvature, Constants.maximumLensRadiusOfCurvature, 
                                                           sliderRange.x, sliderRange.y, 
                                                           radiusOfCurvature));
    }

    private void setLensThicknessSlider() {
        setSliderToPosition(lensThicknessSlider, remap(Constants.minimumLensThickness, Constants.maximumLensThickness, 
                                                           sliderRange.x, sliderRange.y, 
                                                           lensThickness));
    }

    private void setLensScaleSlider() {
        setSliderToPosition(lensScaleSlider, remap(Constants.minimumLensScale, Constants.maximumLensScale, 
                                                           sliderRange.x, sliderRange.y, 
                                                           lensScale));
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

    private void hideLensVisualization() {
        hideFocalPoints();
    }

    private void showLensVisualization() {
        showFocalPoints();
    }

    private void hideFocalPoints() {
        focalPoint1.SetActive(false);
        focalPoint2.SetActive(false);
    }

    private void showFocalPoints() {
        focalPoint1.SetActive(true);
        focalPoint2.SetActive(true);
    }

    private void updateFocalPoints(float newFocalLength) {
        focalPoint1.transform.localPosition = new Vector3(-newFocalLength, 0, 0);
        focalPoint2.transform.localPosition = new Vector3(newFocalLength, 0, 0);
    }

    private float remap(float origFrom, float origTo, float targetFrom, float targetTo, float value){
        float rel = Mathf.InverseLerp(origFrom, origTo, value);
        return Mathf.Lerp(targetFrom, targetTo, rel);
    }
}
