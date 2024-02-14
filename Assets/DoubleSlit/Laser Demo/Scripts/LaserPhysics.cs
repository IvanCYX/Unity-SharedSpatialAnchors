//Laser Physics Class -- Contains all of the computational physics methods for reflecting and refracting lasers

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LaserPhysics {
    public static Vector3 reflectLaser(GameObject hitObject, Vector3 rayDirection) {
        //Find the correct normal direction of the mirror
        Vector3 normalDirection = hitObject.transform.forward;
        if(Vector3.Angle(-normalDirection, rayDirection) > Vector3.Angle(normalDirection, rayDirection)) {normalDirection = -normalDirection;}

        //Find the outgoing ray direction using the law of reflection
        Vector3 reflectionDirection = 2.0f * Vector3.Dot(normalDirection, -rayDirection) * normalDirection + rayDirection;
        return reflectionDirection;
    }

    public static Vector3 lensRefractLaser(GameObject hitObject, Vector3 hitPoint, Vector3 rayDirection) {
        Vector3 normalDirection = -hitObject.transform.forward;
        Vector3 upDirection = hitObject.transform.right;
        Vector3 rightDirection = hitObject.transform.up;

        //Axis correction if we hit the back of a lens
        if(Vector3.Angle(-normalDirection, rayDirection) > Vector3.Angle(normalDirection, rayDirection)) {normalDirection = -normalDirection; upDirection = -upDirection;}

        //Find the focal point of the lens and the relative hitpoint of the laser on the lens
        float focalLength = LensPhysics.findFocalLength(hitObject.transform.parent.gameObject);
 
        Vector3 hitPointOnLens = hitObject.transform.position - hitPoint;
        
        float yHit = Vector3.Dot(upDirection, hitPointOnLens);
        float zHit = Vector3.Dot(rightDirection,  hitPointOnLens);

        //Calculate the refraction angles through the two axis perpendicular to the lens normal using the thin lens equation
        float y1 = (yHit/focalLength);
        float a1 = (90.0f - Vector3.Angle(rayDirection, -upDirection)) * Mathf.Deg2Rad;
        float b1 = Mathf.Atan(Mathf.Tan(a1) - y1) * Mathf.Rad2Deg;

        float y2 = (zHit/focalLength);
        float a2 = (90.0f - Vector3.Angle(rayDirection, -rightDirection)) * Mathf.Deg2Rad;
        float b2 = Mathf.Atan(Mathf.Tan(a2) - y2) * Mathf.Rad2Deg;

        //Deflect laser away from the negative normal direction at the correct angle
        Vector3 refractionDirection = -normalDirection;
        refractionDirection = Quaternion.AngleAxis(b1 / hitObject.transform.parent.gameObject.transform.localScale.y, -rightDirection) * refractionDirection;
        refractionDirection = Quaternion.AngleAxis(b2 / hitObject.transform.parent.gameObject.transform.localScale.y, upDirection) * refractionDirection;

        /*
        //Focal Points
        Debug.DrawLine(hitObject.transform.position + normalDirection * focalLength - upDirection * 0.01f,hitObject.transform.position + normalDirection * focalLength + upDirection * 0.01f, Color.green);
        Debug.DrawLine(hitObject.transform.position + normalDirection * focalLength - rightDirection * 0.01f, hitObject.transform.position + normalDirection * focalLength + rightDirection * 0.01f, Color.green);
        Debug.DrawLine(hitObject.transform.position + normalDirection * -focalLength - upDirection * 0.01f,hitObject.transform.position + normalDirection * -focalLength + upDirection * 0.01f, Color.green);
        Debug.DrawLine(hitObject.transform.position + normalDirection * -focalLength - rightDirection * 0.01f, hitObject.transform.position + normalDirection * -focalLength + rightDirection * 0.01f, Color.green);

        //Normal Vector
        Debug.DrawLine(hitObject.transform.position, hitObject.transform.position + 0.5f * normalDirection,  Color.magenta);
        */
 
        return refractionDirection;
    }

    public static Vector3 prismRefractLaser(GameObject hitObject, Vector3 rayDirection, ref float partialReflectance, ref Vector3 partialReflectionDirection) {
        Vector3 normalDirection = hitObject.transform.forward;
        Vector3 upDirection = hitObject.transform.right;
        Vector3 rightDirection = hitObject.transform.up;

        float n1 = Constants.airRefractiveIndex;
        float n2 = LensPhysics.getIndexOfRefraction(hitObject.transform.parent.gameObject);

        //Check if the laser is hitting a backface and correct axis and refraction indexes if needed
        if(Vector3.Angle(-normalDirection, rayDirection) > Vector3.Angle(normalDirection, rayDirection)) {
            n1 = n2;
            n2 = Constants.airRefractiveIndex;   
            normalDirection = -normalDirection; 
            rightDirection = -rightDirection;
        }

        //Calculate outgoing ray direction using Snell's law
        float angleFromXNormal = (90.0f - Vector3.Angle(rayDirection, -rightDirection)) * Mathf.Deg2Rad;
        float angleFromYNormal = (90.0f - Vector3.Angle(rayDirection, -upDirection)) * Mathf.Deg2Rad;
        float refractionAngle = Mathf.Asin((n1/n2) * Mathf.Sin(angleFromXNormal));
        
        //Check for total internal refraction
        if(float.IsNaN(refractionAngle)) {
            return reflectLaser(hitObject, rayDirection);
        }

        //Compute partial reflection through Schlick's approximation of the Fresnel equation
        //float r0 = ((n1 - n2)/(n1 + n2)) * ((n1 - n2)/(n1 + n2));
        //partialReflectance = r0 + (1.0f - r0) * Mathf.Pow((1.0f - Mathf.Cos(angleFromXNormal)), 5.0f);

        //Compute partial reflection through the Fresnel equation
        partialReflectance = Mathf.Abs((n2 * Mathf.Cos(angleFromXNormal) - n1 * Mathf.Cos(refractionAngle))/(n2 * Mathf.Cos(angleFromXNormal) + n1 * Mathf.Cos(refractionAngle)));
        partialReflectance *= partialReflectance;

        if(partialReflectance >= Constants.laserBrightnessCutoff) {partialReflectionDirection = reflectLaser(hitObject, rayDirection);}

        //Deflect laser away from the negative normal direction at the correct angle
        Vector3 refractionDirection = -normalDirection;
        refractionDirection = Quaternion.AngleAxis(refractionAngle * Mathf.Rad2Deg, -upDirection) * refractionDirection;
        refractionDirection = Quaternion.AngleAxis(-angleFromYNormal * Mathf.Rad2Deg, -rightDirection) * refractionDirection;
        return refractionDirection;
    }
}
