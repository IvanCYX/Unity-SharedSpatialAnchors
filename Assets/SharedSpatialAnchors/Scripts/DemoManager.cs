using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DemoManager
{
    public static ReferenceManager referenceManager;

    public static GameObject ironFilling;
    public static GameObject MagnetScene;
    public static GameObject LargeMagnet;

   /* public static GameObject vacuumTubeSetup;
    public static GameObject screen;
    public static GameObject electronSpawner;
    public static GameObject doubleSlit;
    public static GameObject singleSlit;
    public static GameObject doubleSlitImage;
    public static GameObject leftCover;
    public static GameObject rightCover;
    public static GameObject interferenceWave;

    public static GameObject blips;

    public static Material screenWhite;
    public static Material interferenceBothOpen;
    public static Material interferenceLeftClosed;
    public static Material interferenceRightClosed;
    public static Material particleInterferencePattern;
*/
    static DemoManager()
    {
        referenceManager = Object.FindObjectOfType<ReferenceManager>();

        MagnetScene = referenceManager.MagnetScene;
        ironFilling = referenceManager.ironFilling;
        LargeMagnet = referenceManager.LargeMagnet;

        /*electronPrefab = referenceManager.electronPrefab;
        blipPrefab = referenceManager.blipPrefab;
        doubleSlit = referenceManager.doubleSlit;
        singleSlit = referenceManager.singleSlit;
        doubleSlitImage = referenceManager.doubleSlitImage;
        leftCover = referenceManager.leftCover;
        rightCover = referenceManager.rightCover;
        interferenceWave = referenceManager.interferenceWave;

        blips = referenceManager.blips;

        screenWhite = referenceManager.screenWhite;
        interferenceBothOpen = referenceManager.interferenceBothOpen;
        interferenceLeftClosed = referenceManager.interferenceLeftClosed;
        interferenceRightClosed = referenceManager.interferenceRightClosed;
        particleInterferencePattern = referenceManager.particleInterferencePattern;*/
    }
}
