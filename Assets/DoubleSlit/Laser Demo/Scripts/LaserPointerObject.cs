using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointerObject : MonoBehaviour {
    public uint laserPointerType;
    Laser[] lasers;


    public void SpawnLasers() {
        switch(laserPointerType) {
            case 1: 
                SpawnType1();
                break;
            case 2:
                SpawnType2();
                break;
            case 3:
                SpawnType3();
                break;
            case 4:
                SpawnType4();
                break;
            case 5:
                SpawnType5();
                break;
        }

        LaserManager.physicsObjects.Add(gameObject);
    }

    void Update() {
        switch(laserPointerType) {
            case 1: 
                UpdateType1();
                break;
            case 2: 
                UpdateType2();
                break;
            case 3: 
                UpdateType3();
                break;
            case 4: 
                UpdateType4();
                break;
            case 5: 
                UpdateType5();
                break;
        }
    }

    public float getLength() {
        if(lasers.Length == 0) {return -1;}

        return lasers[0].totalLength;
    }

    public void setLength(float newLength) {
        foreach(Laser laser in lasers) {
            laser.totalLength = newLength;
            laser.getFirstLaserSegment().setLength(newLength);
        }
    }

    public void changeType(uint newType) {
        if(laserPointerType == newType) {return;}
        Clear();
        laserPointerType = newType;
        SpawnLasers();
    }

    void Clear() {
        foreach(Laser laser in lasers) {
            laser.destroyBranches();
            laser.clearLaser();
            LaserManager.lasers.Remove(laser);
        }
    }

    void SpawnType1() {
        transform.localScale = new Vector3(0.015f, 0.03f, 0.03f);

        lasers = new Laser[1];
        lasers[0] = new Laser(transform.position, Vector3.right, 2.5f, LaserManager.laserSpawner.currentLaserID);
        ++LaserManager.laserSpawner.currentLaserID;
       
        foreach(Laser laser in lasers) {
            LaserManager.lasers.Add(laser);
        }
    }

    void SpawnType2() {
        transform.localScale = new Vector3(0.015f, 0.1f, 0.03f);

        lasers = new Laser[5];
        for(int i = 0; i < 5; i++) {
            lasers[i] = new Laser(transform.position + transform.up * 0.02f * (i - 2), Vector3.right, 2.5f, LaserManager.laserSpawner.currentLaserID);
            ++LaserManager.laserSpawner.currentLaserID;
        }
       
        foreach(Laser laser in lasers) {
            LaserManager.lasers.Add(laser);
        }
    }

    void SpawnType3() {
        transform.localScale = new Vector3(0.015f, 0.03f, 0.03f);

        lasers = new Laser[5];
        for(int i = 0; i < 5; i++) {
            lasers[i] = new Laser(transform.position, Vector3.right, 2.5f, LaserManager.laserSpawner.currentLaserID);
            ++LaserManager.laserSpawner.currentLaserID;
        }
       
        foreach(Laser laser in lasers) {
            LaserManager.lasers.Add(laser);
        }
    }

    void SpawnType4() {
        transform.localScale = new Vector3(0.015f, 0.03f, 0.03f);

        lasers = new Laser[8];
        for(int i = 0; i < 8; i++) {
            lasers[i] = new Laser(transform.position + transform.up * 0.01f * Mathf.Sin(i * 45 * Mathf.Deg2Rad) + transform.forward * 0.01f * Mathf.Cos(i * 45 * Mathf.Deg2Rad), Vector3.right, 2.5f, LaserManager.laserSpawner.currentLaserID);
            ++LaserManager.laserSpawner.currentLaserID;
        }
       
        foreach(Laser laser in lasers) {
            LaserManager.lasers.Add(laser);
        }
    }

    void SpawnType5() {
        transform.localScale = new Vector3(0.015f, 0.03f, 0.03f);

        lasers = new Laser[8];
        for(int i = 0; i < 8; i++) {
            lasers[i] = new Laser(transform.position, Vector3.right, 2.5f, LaserManager.laserSpawner.currentLaserID);
            ++LaserManager.laserSpawner.currentLaserID;
        }
       
        foreach(Laser laser in lasers) {
            LaserManager.lasers.Add(laser);
        }
    }

    void UpdateType1() {
        lasers[0].origin = transform.position;
        lasers[0].direction = transform.right;
        lasers[0].getFirstLaserSegment().changeStartpoint(lasers[0].origin);
        lasers[0].getFirstLaserSegment().setDirection(lasers[0].direction);
    }

    void UpdateType2() {
        for(int i = 0; i < 5; i++) {
            lasers[i].origin = transform.position + transform.up * 0.02f * (i - 2);
            lasers[i].direction = transform.right;
            lasers[i].getFirstLaserSegment().changeStartpoint(lasers[i].origin);
            lasers[i].getFirstLaserSegment().setDirection(lasers[i].direction);
        }
    }

    void UpdateType3() {
        for(int i = 0; i < 5; i++) {
            lasers[i].origin = transform.position;
            lasers[i].direction =  Quaternion.AngleAxis((i - 2) * 4, transform.forward) * transform.right;
            lasers[i].getFirstLaserSegment().changeStartpoint(lasers[i].origin);
            lasers[i].getFirstLaserSegment().setDirection(lasers[i].direction);
        }
    }

    void UpdateType4() {
        for(int i = 0; i < 8; i++) {
            lasers[i].origin = transform.position + transform.up * 0.01f * Mathf.Sin(i * 45 * Mathf.Deg2Rad) + transform.forward * 0.01f * Mathf.Cos(i * 45 * Mathf.Deg2Rad);
            lasers[i].direction = transform.right;
            lasers[i].getFirstLaserSegment().changeStartpoint(lasers[i].origin);
            lasers[i].getFirstLaserSegment().setDirection(lasers[i].direction);
        }
    }

    void UpdateType5() {
        for(int i = 0; i < 8; i++) {
            lasers[i].origin = transform.position;
            lasers[i].direction = Quaternion.AngleAxis(Mathf.Sin(i * 45 * Mathf.Deg2Rad) * 4, transform.up) * Quaternion.AngleAxis(Mathf.Cos(i * 45 * Mathf.Deg2Rad) * 4, transform.forward) * transform.right;
            lasers[i].getFirstLaserSegment().changeStartpoint(lasers[i].origin);
            lasers[i].getFirstLaserSegment().setDirection(lasers[i].direction);
        }
    }
}
