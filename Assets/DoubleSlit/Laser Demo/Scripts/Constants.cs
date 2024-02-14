//Constants Class - Stores project-wide constants that can referenced from every other class

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants {
    public const float minimumLaserRadius = 0.0005f;
    public const float maximumLaserRadius = 0.01f;
    public const float defaultLaserRadius = 0.001f;

    public const float laserRadiusIncrement = 0.0005f;
    public const float laserRadiusDecrement = -0.0005f;

    public const float laserEndpointDeadZone = 0.005f;
    public const float rayCastOffset = 0.00011f;
    public const float objectSpawnClearDistance = 0.09f;
    public const float laserBrightnessCutoff = 0.01f;

    public const float minimumLensThickness = -3.33f;
    public const float maximumLensThickness = 3.33f;
    public const float minimumLensScale = 0.15f;
    public const float maximumLensScale = 3.00f;
    public const float minimumLensRadiusOfCurvature = -5.00f;
    public const float maximumLensRadiusOfCurvature = 5.00f;
    public const float lensRadiusOfCurvatureDeadZone = 0.15f;
    public const float minimumRefractiveIndex = 1.1f;
    public const float maximumRefractiveIndex = 5.0f;
    public const float minimumFocalPoint = -25.0f;
    public const float maximumFocalPoint = 25.0f;
    public const float maximumLaserPointerLength = 5.0f;

    public const float glassRefractiveIndex = 1.5f;
    public const float airRefractiveIndex = 1.0f;

    public const float epsilon = 0.0000000001f;

    public const uint laserPointerTypes = 5;
    public const uint maxLaserSegments = 100;
    public const uint maxLaserBranches = 10;
    public const uint branchLaserID = uint.MaxValue;

    public const uint errorNumber = 9999999;
}