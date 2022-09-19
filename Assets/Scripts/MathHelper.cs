using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathHelper
{
    // returns in degrees
    public static float GetAngleBetweenTwoVectors(Vector3 a, Vector3 b)
    {
        float dotProduct = Vector3.Dot(a, b);
        double cosineInverse = Math.Acos(dotProduct / (a.magnitude * b.magnitude));
        float angleBetween = (float) (Mathf.Rad2Deg * cosineInverse);

        // handles weird edge case that happens shortly after episodes
        if (float.IsNaN(angleBetween))
            angleBetween = 0;

        return angleBetween;
    }

    public static bool IsAngleBetweenTwoVectorsLessThan(Vector3 a, Vector3 b, float maxAngle)
    {
        return GetAngleBetweenTwoVectors(a, b) < maxAngle;
    }
}
