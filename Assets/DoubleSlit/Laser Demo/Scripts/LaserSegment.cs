//Laser Segment Class -- Hold information about a single laser segment and methods to raycast and modify it

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LaserSegment {
    public GameObject gameObject;
    [System.NonSerialized]
    public Laser parent;

    //Information based on startpoint - easier to do ray optics with
    public Vector3 startPoint;
    public Vector3 direction;
    public float length; 
    public float radius {get; private set;}
    public Vector3 endPoint {get; private set;}

    //Information based on middlepoint -- what Unity uses
    Vector3 position;
    Vector3 localScale;

    public float brightness;

    //Constructors
    public LaserSegment(Vector3 startPoint, Vector3 direction, float length) {
        this.gameObject = Object.Instantiate(LaserManager.laserSegmentPrefab);
        this.startPoint = startPoint;
        this.direction = direction.normalized;
        this.length = length;

        this.radius = LaserManager.laserSpawner.laserRadius;
        this.endPoint = this.startPoint + this.direction * this.length;
        this.position = this.startPoint + 0.5f * this.direction * this.length;
        this.localScale = new Vector3(this.radius, this.length/2, this.radius);
        this.brightness = 1;

        updateMaterial();
        updateTransform();
    }

    public LaserSegment(float brightness, Vector3 startPoint, Vector3 direction, float length) {
        this.gameObject = Object.Instantiate(LaserManager.laserSegmentPrefab);
        this.startPoint = startPoint;
        this.direction = direction.normalized;
        this.length = length;

        this.radius = LaserManager.laserSpawner.laserRadius;
        this.endPoint = this.startPoint + this.direction * this.length;
        this.position = this.startPoint + 0.5f * this.direction * this.length;
        this.localScale = new Vector3(this.radius, this.length/2, this.radius);
        this.brightness = brightness;

        updateMaterial();
        updateTransform();
    }

    public LaserSegment(Vector3 startPoint, Vector3 direction, float length, float radius) {
        this.gameObject = Object.Instantiate(LaserManager.laserSegmentPrefab);
        this.startPoint = startPoint;
        this.direction = direction.normalized;
        this.length = length;
        this.radius = radius;

        this.endPoint = this.startPoint + this.direction * this.length;
        this.position = this.startPoint + 0.5f * this.direction * this.length;
        this.localScale = new Vector3(this.radius, this.length/2, this.radius);
        this.brightness = 1;

        updateMaterial();
        updateTransform();
    }

    public LaserSegment(Vector3 startPoint, Vector3 endPoint) {
        this.gameObject = Object.Instantiate(LaserManager.laserSegmentPrefab);
        this.startPoint = startPoint;
        this.endPoint = endPoint;
        this.brightness = 1;

        this.length = Vector3.Distance(this.startPoint, this.endPoint);
        this.direction = (this.endPoint - this.startPoint).normalized;
        this.radius = LaserManager.laserSpawner.laserRadius;
        this.position = this.startPoint + 0.5f * this.direction * this.length;
        this.localScale = new Vector3(this.radius, this.length/2, this.radius);
        updateMaterial();
        updateTransform();
    }

    public LaserSegment(float radius, Vector3 startPoint, Vector3 endPoint) {
        this.gameObject = Object.Instantiate(LaserManager.laserSegmentPrefab);
        this.startPoint = startPoint;
        this.endPoint = endPoint;
        this.brightness = 1;

        this.length = Vector3.Distance(this.startPoint, this.endPoint);
        this.direction = (this.endPoint - this.startPoint).normalized;
        this.radius = radius;
        this.position = this.startPoint + 0.5f * this.direction * this.length;
        this.localScale = new Vector3(this.radius, this.length/2, this.radius);
        updateMaterial();
        updateTransform();
    }

    public bool rayCast(out RaycastHit laserHit) {
        return Physics.Raycast(startPoint + direction * Constants.rayCastOffset, direction, out laserHit, length);
    }
    
    public void setBrightness(float newBrightness) {
        brightness = newBrightness;
        updateMaterial();
    }

    public void setBoundingBoxShader(Color color) {
        Material newLaserMaterial = new Material(LaserManager.boundingBoxShader);
        newLaserMaterial.SetColor("_Color", color);
        gameObject.GetComponent<Renderer>().sharedMaterial = newLaserMaterial;
    }

    private void updateMaterial() {
        Material newLaserMaterial = new Material(LaserManager.laserShader);
        newLaserMaterial.SetFloat("_brightness", brightness);
        gameObject.GetComponent<Renderer>().sharedMaterial = newLaserMaterial;
    }

    //Laser transformations
    public void destroy() {
        MonoBehaviour.Destroy(gameObject);
    }

    public void resetTransformInfo() {
        readTransfrom();
        updateIndependentsFromTransform();
    }

    public void changeStartpoint(Vector3 newStartpoint) {
        startPoint = newStartpoint;
        updateInfoFromPointChange();
    }
    
    public void changeEndpoint(Vector3 newEndpoint) {
        endPoint = newEndpoint;
        updateInfoFromPointChange();
    }

    public void extendEndpoint(float lengthExtension) {
        changeEndpoint(endPoint + direction * lengthExtension);
    }

    public void setLength(float setLength) {
        extendEndpoint(setLength - length);
    }

    public void setDirection(Vector3 setDirection) {
        changeEndpoint(startPoint + direction * length);
    }

    public void setRadius(float newRadius) {
        radius = newRadius;
        Mathf.Clamp(radius, Constants.minimumLaserRadius, Constants.maximumLaserRadius);
        updateTransformDependents();
        updateTransform();
    }

    public void increaseRadius() {
        radius += Constants.laserRadiusIncrement;
        Mathf.Clamp(radius, Constants.minimumLaserRadius, Constants.maximumLaserRadius);
        updateTransformDependents();
        updateTransform();
    }

    public void decreaseRadius() {
        radius += Constants.laserRadiusDecrement;
        Mathf.Clamp(radius, Constants.minimumLaserRadius, Constants.maximumLaserRadius);
        updateTransformDependents();
        updateTransform();
    }

    public bool checkDeadZone(Vector3 hitPoint) {
        return Vector3.Distance(hitPoint,startPoint) < Constants.laserEndpointDeadZone || Vector3.Distance(hitPoint,endPoint) < Constants.laserEndpointDeadZone;
    }

    //Update dependent values
    private void updateInfoFromPointChange() {
        length = Vector3.Distance(startPoint, endPoint);
        direction = (endPoint - startPoint).normalized;
        updateTransformDependents();
        updateTransform();
    }

    private void updateTransformDependents() {
        position = startPoint + 0.5f * direction * length;
        localScale = new Vector3(radius, length/2, radius);
    }

    private void updateIndependentsFromTransform() {
        readTransfrom();
        length = localScale.y * 2;
        direction = gameObject.transform.up;
        startPoint = position - 0.5f * direction * length;
        endPoint = startPoint + direction * length;
    }

    private void updateTransform() {
        gameObject.transform.position = position;
        gameObject.transform.localScale = localScale;
        gameObject.transform.up = direction;
    }

    private void readTransfrom() {
        position = gameObject.transform.position;
        localScale = gameObject.transform.localScale;
    }

    //Operator overloading
    public static bool operator ==(LaserSegment laserSegment1, LaserSegment laserSegment2) {
        if (laserSegment1 is null) {return laserSegment2 is null;}
        return laserSegment1.Equals(laserSegment2);
    }

    public static bool operator !=(LaserSegment laserSegment1, LaserSegment laserSegment2) {
        return !(laserSegment1 == laserSegment2);
    }

    public override bool Equals(object obj) {
        if (obj == null) {return false;}
        return obj is LaserSegment laserSegment2? (gameObject.GetInstanceID() == laserSegment2.gameObject.GetInstanceID()): false;

    }

    public override int GetHashCode() {
        return (gameObject.GetInstanceID()).GetHashCode();
    }
}
