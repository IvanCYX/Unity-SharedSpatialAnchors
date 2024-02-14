using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LensPhysics {
    //Lensmaker methods
    public static float findFocalLength(float indexOfRefraction, float radiusOfCurvature) {
        return radiusOfCurvature / (2.0f * (indexOfRefraction - 1.0f));
    }

    public static float findFocalLength(GameObject lens) {
        return findFocalLength(getIndexOfRefraction(lens), getRadiusOfCurvature(lens));
    }

    public static void matchFocalLength(float focalLength, ref float indexOfRefraction, ref float radiusOfCurvature) {
        float testRadiusOfCurvature;
        float testIndexOfRefraction = indexOfRefraction;
        MonoBehaviour.print(focalLength);

        for(int i = 0; i < 5; i++) {
            testRadiusOfCurvature = calculateRadiusOfCurvature(testIndexOfRefraction, focalLength);  
            if(testRadiusOfCurvature < Constants.minimumLensRadiusOfCurvature) {
                testIndexOfRefraction += 0.2f;
                continue;
            } else if(testRadiusOfCurvature > Constants.maximumLensRadiusOfCurvature) {
                testIndexOfRefraction -= 0.2f;
                continue;
            } else {
                indexOfRefraction = Mathf.Clamp(testIndexOfRefraction, Constants.minimumRefractiveIndex, Constants.maximumRefractiveIndex);
                radiusOfCurvature = testRadiusOfCurvature;
                return;
            }

        }
    }

    //Lens parameter accessors
    public static float getIndexOfRefraction(GameObject lens) {
        return lens.GetComponent<Renderer>().sharedMaterial.GetFloat("_IndexOfRefraction");
    }

    public static float getRadiusOfCurvature(GameObject lens) {
        return lens.GetComponent<Renderer>().sharedMaterial.GetFloat("_RadiusOfCurvature") / lens.transform.localScale.y;
    }

    public static float getThickness(GameObject lens) {
        return lens.transform.localScale.x/lens.transform.localScale.y;
    }

    public static float getScale(GameObject lens) {
        return lens.transform.localScale.y;
    }

    public static Vector2 getFocalLengthRange(GameObject lens) {
        float focalLengthMin = findFocalLength(getIndexOfRefraction(lens), Constants.minimumLensRadiusOfCurvature);
        float focalLengthMax = findFocalLength(getIndexOfRefraction(lens), Constants.maximumLensRadiusOfCurvature);

        return new Vector2(focalLengthMin, focalLengthMax);
    }

    public static Vector2 getFocalLengthRange(float indexOfRefraction) {
        float focalLengthMin = findFocalLength(indexOfRefraction, Constants.minimumLensRadiusOfCurvature);
        float focalLengthMax = findFocalLength(indexOfRefraction, Constants.maximumLensRadiusOfCurvature);

        return new Vector2(focalLengthMin, focalLengthMax);
    }

    public static float calculateRadiusOfCurvature(float indexOfRefraction, float focalLength) {
        return 2.0f * focalLength * (indexOfRefraction - 1.0f);
    }

    public static float calculateRadiusOfCurvature(float lensThickness) {
        return 0.5f / lensThickness;
    }

    public static float calculateLensThickness(float radiusOfCurvature) {
        return 0.5f / radiusOfCurvature;
    }

    //Lens parameter modifiers
    public static void setThickness(GameObject lens, float newThickness) {
        if(!checkIfConcave(lens) && newThickness < 0) {switchToBiConcaveLens(lens);}
        if(checkIfConcave(lens) && newThickness > 0) {switchToBiConvexLens(lens);}
        lens.transform.localScale = new Vector3(Mathf.Abs(newThickness) * lens.transform.localScale.y, lens.transform.localScale.y, lens.transform.localScale.z);
    }

    public static void setRadiusOfCurvature(GameObject lens, float newRadiusOfCurvature) {
        lens.GetComponent<Renderer>().sharedMaterial.SetFloat("_RadiusOfCurvature", newRadiusOfCurvature * lens.transform.localScale.y);
    }

    public static void setIndexOfRefraction(GameObject lens, float newIndexOfRefraction) {
        lens.GetComponent<Renderer>().sharedMaterial.SetFloat("_IndexOfRefraction", newIndexOfRefraction);
    }

    public static void setFocalLength(GameObject lens, float newFocalLength) {
        setRadiusOfCurvature(lens, 2.0f * newFocalLength * (getIndexOfRefraction(lens) - 1.0f));
    }

    public static void setOverallScale(GameObject lens, float lensThickness, float newOverallScale) {
        lens.transform.localScale = new Vector3(lensThickness * newOverallScale, newOverallScale, newOverallScale);
    }

    public static void setScale(GameObject lens, float newScale) {
        lens.transform.localScale = new Vector3(lens.transform.localScale.x, newScale, newScale);
    }

    public static void updateLensThickness(GameObject lens) {
        setThickness(lens, 0.5f / getRadiusOfCurvature(lens));
    }

    public static void updateRadiusOfCurvature(GameObject lens) {
        setRadiusOfCurvature(lens, 0.5f / getThickness(lens));
    }

    //Lens material utility
    public static void initializeMaterial(GameObject lens) {
        Material newLensMaterial = new Material(LaserManager.lensShader);
        lens.GetComponent<Renderer>().sharedMaterial = newLensMaterial;
    }

    public static void initializeMaterial(GameObject lens, float radiusOfCurvature) {
        initializeMaterial(lens);
        setRadiusOfCurvature(lens, radiusOfCurvature);
        setIndexOfRefraction(lens, Constants.glassRefractiveIndex);
    }

    public static void initializeMaterial(GameObject lens, float radiusOfCurvature, float indexOfRefraction) {
        initializeMaterial(lens);
        setRadiusOfCurvature(lens, radiusOfCurvature);
        setIndexOfRefraction(lens, indexOfRefraction);
    }

    //Lens mesh utility
    public static void switchToBiConvexLens(GameObject lens) {
        lens.GetComponent<MeshFilter>().sharedMesh = LaserManager.biConvexLensMesh;
    }

    public static void switchToBiConcaveLens(GameObject lens) {
        lens.GetComponent<MeshFilter>().sharedMesh = LaserManager.biConcaveLensMesh;
    }

    public static bool checkIfConcave(GameObject lens) {
        return lens.GetComponent<MeshFilter>().sharedMesh == LaserManager.biConcaveLensMesh;
    }
}
