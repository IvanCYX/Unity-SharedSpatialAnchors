using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DemoManager
{
    public static ReferenceManager referenceManager;

    public static GameObject ironFilling;
    public static GameObject MagnetScene;
    public static GameObject LargeMagnet;

    static DemoManager()
    {
        referenceManager = Object.FindObjectOfType<ReferenceManager>();

        MagnetScene = referenceManager.MagnetScene;
        ironFilling = referenceManager.ironFilling;
        LargeMagnet = referenceManager.LargeMagnet;
    }
}
