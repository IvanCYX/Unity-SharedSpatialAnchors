//Laser Class -- Stores data and methods regarding a single laser beam consisting of multiple laser segments 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

[System.Serializable]
public class Laser {
    //Member variables
    public List<LaserSegment> laserSegments;

    public uint laserID;
    public uint laserSegmentCount;
    public float totalLength;
    public Vector3 direction;
    public Vector3 origin;

    [System.NonSerialized]
    private List<Laser> laserBranches;
    private float originBrightness;

    //Constructors
    public Laser(List<LaserSegment> laserSegments, uint laserID) {
        this.laserSegments = laserSegments;
        this.laserID = laserID;
        this.laserSegmentCount = (uint)laserSegments.Count;
        this.direction = laserSegments[0].direction;
        this.origin = laserSegments[0].startPoint;
        this.laserBranches = new List<Laser>(0);
        this.originBrightness = 1.0f;
        assignParentPointers();
        updateTotalLength();
    }

    public Laser(LaserSegment laserSegment, uint laserID) {
        this.laserSegments = new List<LaserSegment>();
        this.laserSegments.Add(laserSegment);
        this.laserID = laserID;
        this.laserSegmentCount = 1;
        this.direction = laserSegments[0].direction;
        this.origin = laserSegments[0].startPoint;
        this.laserBranches = new List<Laser>(0);
        this.originBrightness = 1.0f;
        assignParentPointers();
        updateTotalLength();
    }

    public Laser(Vector3 startPoint, Vector3 direction, float length, uint laserID) {
        this.laserSegments = new List<LaserSegment>();
        this.laserSegments.Add(new LaserSegment(startPoint, direction, length));
        this.laserID = laserID;
        this.laserSegmentCount = 1;
        this.direction = laserSegments[0].direction;
        this.origin = laserSegments[0].startPoint;
        this.laserBranches = new List<Laser>(0);
        this.originBrightness = 1.0f;
        assignParentPointers();
        updateTotalLength();
    }

    public Laser(Vector3 startPoint, Vector3 direction, float length, uint laserID, float originBrightness) {
        this.laserSegments = new List<LaserSegment>();
        this.laserSegments.Add(new LaserSegment(originBrightness, startPoint, direction, length));
        this.laserID = laserID;
        this.laserSegmentCount = 1;
        this.direction = laserSegments[0].direction;
        this.origin = laserSegments[0].startPoint;
        this.laserBranches = new List<Laser>(0);
        this.originBrightness = originBrightness;
        assignParentPointers();
        updateTotalLength();
    }

    public Laser(Vector3 startPoint, Vector3 direction, float length, float radius, uint laserID) {
        this.laserSegments = new List<LaserSegment>();
        this.laserSegments.Add(new LaserSegment(startPoint, direction, length, radius));
        this.laserID = laserID;
        this.laserSegmentCount = 1;
        this.direction = laserSegments[0].direction;
        this.origin = laserSegments[0].startPoint;
        this.laserBranches = new List<Laser>(0);
        this.originBrightness = 1.0f;
        assignParentPointers();
        updateTotalLength();
    }

    public Laser(Vector3 startPoint, Vector3 endPoint, uint laserID) {
        this.laserSegments = new List<LaserSegment>();
        this.laserSegments.Add(new LaserSegment(startPoint, endPoint));
        this.laserID = laserID;
        this.laserSegmentCount = 1;
        this.direction = laserSegments[0].direction;
        this.origin = laserSegments[0].startPoint;
        this.laserBranches = new List<Laser>(0);
        this.originBrightness = 1.0f;
        assignParentPointers();
        updateTotalLength();
    }

    //Modifiers 
    public void rayCastLaser() {
        if(laserSegmentCount == 0 || totalLength == 0) {return;}
        float oldLength = totalLength;
        uint laserIndex = 0;

        //Reset laser and initialize the first segment
        clearLaser();
        addLaserSegment(origin, direction, oldLength);
        getFirstLaserSegment().setBrightness(originBrightness);

        LaserSegment lastLaserSegment;
        RaycastHit laserHit;
        GameObject lastHitObject = null;

        float partialReflectance;
        Vector3 partialReflectionDirection;
        float currentBrightness;
        
        //Recast along laser until we stop hitting physics objects
        while(true) {
            lastLaserSegment = getLastLaserSegment();
            if(getLastLaserSegment().rayCast(out laserHit)) {
                currentBrightness = lastLaserSegment.brightness;
                partialReflectance = 0; partialReflectionDirection = Vector3.zero;

                //Check if we've hit the same object twice
                if(laserHit.collider.gameObject == lastHitObject) {Debug.LogError("Same object hit");}

                //Split the laser at the hit point and compute the outgoing ray direction from the laser's interaction with the hit physics object
                splitLaser(lastLaserSegment, laserHit.point, LaserManager.computeObjectLaserInteraction(laserHit.collider, laserHit.point, lastLaserSegment.direction, ref partialReflectance, ref partialReflectionDirection));
                laserIndex++; 

                getLastLaserSegment().setBrightness(currentBrightness);

                //Check for partial reflection and create a new laser branch if the partial reflecance is above its cutoff
                if(currentBrightness * partialReflectance >= Constants.laserBrightnessCutoff && laserID < Constants.branchLaserID - Constants.maxLaserBranches) {
                    laserBranches.Add(new Laser(laserHit.point, partialReflectionDirection, oldLength - Vector3.Distance(origin,laserHit.point), Constants.branchLaserID, currentBrightness * partialReflectance));
                    if(currentBrightness * (1.0f - partialReflectance) <= Constants.laserBrightnessCutoff) {destroyLastSegment(); break;}
                    getLastLaserSegment().setBrightness(currentBrightness * (1.0f - partialReflectance));
                } else if(currentBrightness * partialReflectance >= Constants.laserBrightnessCutoff && laserID > Constants.branchLaserID - Constants.maxLaserBranches) {
                    laserBranches.Add(new Laser(laserHit.point, partialReflectionDirection, oldLength - Vector3.Distance(origin,laserHit.point), laserID - 1, currentBrightness * partialReflectance));
                    if(currentBrightness * (1.0f - partialReflectance) <= Constants.laserBrightnessCutoff) {destroyLastSegment(); break;}
                    getLastLaserSegment().setBrightness(currentBrightness * (1.0f - partialReflectance));
                }
            } else {break;}

            //Ensure that we have not exceeded the maximum segment limit
            if(laserIndex >= Constants.maxLaserSegments) {break;}
        }

        totalLength = oldLength;
        rayCastBranches();
        return;
    }

    public void destroyBranches() {
        //Recursively destroy each branch using depth first recursion
        foreach(Laser laser in laserBranches) {
            if(laser.laserBranches.Count != 0) {laser.destroyBranches();}
            laser.clearLaser();
        }

        laserBranches.Clear();
    }

    private void rayCastBranches() {
        foreach(Laser laser in laserBranches) {
            laser.rayCastLaser();
        }
    }

    private void splitLaser(LaserSegment splitSegment, Vector3 splitPoint, Vector3 newDirection) {
        if(laserSegmentCount > Constants.maxLaserSegments) return;

        float oldLength = totalLength;
        float segmentCut = Vector3.Distance(splitSegment.endPoint, splitPoint);

        splitSegment.changeEndpoint(splitPoint);
        destroyLaserFromSegment(findLaserSegmentIndex(splitSegment)+1);
        addLaserSegmentToEnd(newDirection, oldLength + segmentCut - totalLength);
        totalLength = oldLength;
    }

    private void castLaser(LaserSegment splitSegment) { 
        float oldLength = totalLength;
        destroyLaserFromSegment(findLaserSegmentIndex(splitSegment) + 1);
        splitSegment.extendEndpoint(oldLength - totalLength);
        totalLength = oldLength; 
    }

    public void addLaserSegment(LaserSegment newLaserSegment) {
        ++laserSegmentCount;
        laserSegments.Add(newLaserSegment);
        getLastLaserSegment().parent = this;
        totalLength += newLaserSegment.length;
    }

    public void addLaserSegment(Vector3 startPoint, Vector3 direction, float length) {
        addLaserSegment(new LaserSegment(startPoint, direction, length));
    }

    public void addLaserSegment(Vector3 startPoint, Vector3 direction, float length, float radius) {
        addLaserSegment(new LaserSegment(startPoint, direction, length, radius));
    }

    public void addLaserSegment(Vector3 startPoint, Vector3 endPoint) {
        addLaserSegment(new LaserSegment(startPoint, endPoint));
    }

    public void addLaserSegmentToEnd(Vector3 direction, float length) {
        addLaserSegment(new LaserSegment(getLastLaserSegmentEndpoint(), direction, length));
    }

    public void addLaserSegmentToEnd(Vector3 direction, float length, float radius) {
        addLaserSegment(new LaserSegment(getLastLaserSegmentEndpoint(), direction, length, radius));
    }

    public void addLaserSegmentToEnd(Vector3 endPoint) {
        addLaserSegment(new LaserSegment(getLastLaserSegmentEndpoint(), endPoint));
    }

    public void deleteLaserSegment(LaserSegment laserSegmentToDelete) {
        totalLength -= laserSegmentToDelete.length;
        laserSegmentCount--;
        Object.Destroy(laserSegmentToDelete.gameObject);
        laserSegments.Remove(laserSegmentToDelete);
    }

    public void deleteLaserSegment(GameObject laserSegmentToDelete) {
        deleteLaserSegment(findLaserSegment(laserSegmentToDelete));
    }

    private void destroyLastSegment() {
        getLastLaserSegment().destroy();
        laserSegments.RemoveAt(laserSegments.Count - 1);
    }

    private void destroyLaserFromSegment(uint laserSegment) {
        ++laserSegment;
        if(laserSegment > laserSegmentCount) {return;}
        for(uint i = laserSegmentCount; i >= laserSegment; i--) {
            Debug.Assert(laserSegments.Last() != null);
            deleteLaserSegment(laserSegments.Last());
        }
    }

    public void clearLaser() {
        foreach(LaserSegment laserSegment in laserSegments) {
            Object.Destroy(laserSegment.gameObject);
        }
        laserSegments.Clear();
        laserSegmentCount = 0;
        totalLength = 0;
    }

    public void setLaserRadius(float radius) {
        foreach(LaserSegment laserSegment in laserSegments) {
            laserSegment.setRadius(radius);
        }    
    }

    public void increaseLaserRadius() {
        foreach(LaserSegment laserSegment in laserSegments) {
            laserSegment.increaseRadius();
        }    
    }

    public void decreaseLaserRadius() {
        foreach(LaserSegment laserSegment in laserSegments) {
            laserSegment.decreaseRadius();
        }    
    }

    public void assignParentPointers() {
        foreach(LaserSegment laserSegment in laserSegments) {laserSegment.parent = this;}
    }

    //Accessors 
    public bool checkLaserSegmentInLaser(LaserSegment laserSegmentToFind) {
        foreach(LaserSegment laserSegment in laserSegments) {
            if(laserSegment == laserSegmentToFind) {return true;}
        }

        return false;
    } 

    public LaserSegment findLaserSegment(GameObject laserSegmentToFind) {
        foreach(LaserSegment laserSegment in laserSegments) {
            if(laserSegment.gameObject == laserSegmentToFind) {return laserSegment;}
        }

        Debug.LogError("No laser segment could be found; laser gameobject does not exist in this laser");
        return null;
    }

    private uint findLaserSegmentIndex(LaserSegment laserSegmentToFind) {
        uint index = 0;
        foreach(LaserSegment laserSegment in laserSegments) {
            if(laserSegment == laserSegmentToFind) {return index;}
            ++index;
        }

        Debug.LogError("No laser segment index could be found; laser segment does not exist in this laser");
        return Constants.errorNumber;
    }

    public void updateTotalLength() {
        totalLength = 0;
        foreach(LaserSegment laserSegment in laserSegments) {
            totalLength += laserSegment.length;
        }
    }

    public void updateDirection() {
        if(laserSegmentCount != 0) {direction = laserSegments[0].direction;}
    }

    public LaserSegment getFirstLaserSegment() {
        if(laserSegmentCount != 0) {return laserSegments[0];}
        return null;
    }

    public LaserSegment getLastLaserSegment() {
        if(laserSegmentCount != 0) {return laserSegments.Last();}
        return null;
    }

    private Vector3 getLastLaserSegmentEndpoint() {
        return getLastLaserSegment().endPoint;
    }
    
    //Operator overloading
    public static bool operator ==(Laser laser1, Laser laser2) {
        if (laser1 is null) {return laser2 is null;}
        return laser1.Equals(laser2);
    }

    public static bool operator !=(Laser laser1, Laser laser2) {
        return !(laser1 == laser2);
    }

    public override bool Equals(object obj) {
        if (obj == null) {return false;}
        return obj is Laser laser2? (laserID == laser2.laserID): false;

    }

    public override int GetHashCode() {
        return (laserID).GetHashCode();
    }
}
