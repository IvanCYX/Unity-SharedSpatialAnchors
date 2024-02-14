using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceManager : MonoBehaviour {
    [Header("Prefabs")]
    public GameObject electronPrefab;
    public GameObject blipPrefab;

    [Header("Static Objects")]
    public GameObject vacuumTubeSetup;
    public GameObject screen;
    public GameObject electronSpawner;
    public GameObject doubleSlit;
    public GameObject singleSlit;
    public GameObject doubleSlitImage;
    public GameObject leftCover;
    public GameObject rightCover;
    public GameObject interferenceWave;

    [Header("Parent Objects")]
    public GameObject blips;

    [Header("Materials")]
    public Material screenWhite;
    public Material interferenceBothOpen;
    public Material interferenceLeftClosed;
    public Material interferenceRightClosed;
    public Material particleInterferencePattern;
}
