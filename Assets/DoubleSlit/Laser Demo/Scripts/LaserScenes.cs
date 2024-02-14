//Laser Scenes Class -- Contains all of the preset laser scenes 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LaserScenes {
    public static void horizontalBeam(ref uint currentLaserID) {
        LaserManager.lasers.Add(new Laser(new Vector3(-4, 1.3f, 0), Vector3.right, 8, currentLaserID));
        ++currentLaserID;
    }
    
    public static void verticalBeam(ref uint currentLaserID) {
        LaserManager.lasers.Add(new Laser(new Vector3(0, 0, 0), Vector3.up, 3, currentLaserID));;
        ++currentLaserID;
    }

    public static void fiveSparseHorizontalGrid(ref uint currentLaserID) {
        for(float x = -2; x <= 2; x++) {
            for(float y = -2; y <= 2; y++) {
                LaserManager.lasers.Add(new Laser(new Vector3(-4, 1.3f + 0.05f * x, 0.05f * y), Vector3.right, 8, currentLaserID));
                ++currentLaserID;
            }    
        }
    }

    public static void fiveSparseVerticalGrid(ref uint currentLaserID) {
        for(float x = -2; x <= 2; x++) {
            for(float y = -2; y <= 2; y++) {
                LaserManager.lasers.Add(new Laser(new Vector3(0.05f * x, 0, 0.05f * y), Vector3.up, 3, currentLaserID));
                ++currentLaserID;
            }    
        }
    }

    public static void nineDenseHorizontalGrid(ref uint currentLaserID) {
        for(float x = -4; x <= 4; x++) {
            for(float y = -4; y <= 4; y++) {
                LaserManager.lasers.Add(new Laser(new Vector3(-4, 1.3f + 0.025f * x, 0.025f * y), Vector3.right, 8, currentLaserID));
                ++currentLaserID;
            }    
        }
    }

    public static void nineDenseVerticalGrid(ref uint currentLaserID) {
        for(float x = -4; x <= 4; x++) {
            for(float y = -4; y <= 4; y++) {
                LaserManager.lasers.Add(new Laser(new Vector3(0.025f * x, 0, 0.025f * y), Vector3.up, 3, currentLaserID));
                ++currentLaserID;
            }    
        }
    }

    public static void doubleCone(ref uint currentLaserID) {
        for(float x = 0; x < 36; x++) {
            Vector3 startPoint = new Vector3(Mathf.Cos(x * 10 * Mathf.Deg2Rad),0,Mathf.Sin(x * 10 * Mathf.Deg2Rad));  
            Laser newLaser = new Laser(startPoint, Vector3.up * 1.5f, currentLaserID);
            newLaser.getLastLaserSegment().extendEndpoint(1.5f);
            newLaser.updateTotalLength();
            LaserManager.lasers.Add(newLaser);
            ++currentLaserID;
        }
    }

    public static void nineDenseHorizontalPlane(ref uint currentLaserID) {
        for(float x = 0; x < 9; x++) {
            LaserManager.lasers.Add(new Laser(new Vector3(-1, 1.3f + 0.01f * x, 0), Vector3.right, 2, currentLaserID));
            ++currentLaserID;
        }
    }

    public static void nineDenseHorizontalFan(ref uint currentLaserID) {
        for(float x = -4; x <= 4; x++) {
            LaserManager.lasers.Add(new Laser(new Vector3(-1, 1.3f, 0), new Vector3(1, 1.3f + 0.05f * x, 0), currentLaserID));
            ++currentLaserID;
        }
    }

    public static void horizontalCircularBeamArray(ref uint currentLaserID) {
        for(float x = 0; x < 8; x++) {
            LaserManager.lasers.Add(new Laser(new Vector3(-1, 1.3f + 0.01f * Mathf.Sin(x * 45 * Mathf.Deg2Rad),  0.01f * Mathf.Cos(x * 45 * Mathf.Deg2Rad)), Vector3.right, 2, currentLaserID));
            ++currentLaserID;
        }
    }

    public static void largeHorizontalFan(ref uint currentLaserID) {
        for(float x = 0; x < 25; x++) {
            LaserManager.lasers.Add(new Laser(Vector3.zero, new Vector3(2.5f * Mathf.Cos(x * 180f/25 * Mathf.Deg2Rad),  2.5f * Mathf.Sin(x * 180f/25 * Mathf.Deg2Rad),0), currentLaserID));
            ++currentLaserID;
        }
    }

    public static void laser45(ref uint currentLaserID) {
        LaserManager.lasers.Add(new Laser(new Vector3(-1,1,0), new Vector3(3, 2,0), currentLaserID));
        ++currentLaserID;
    }
}
