using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2Extension
{

    public static Vector2 Rotate(this Vector2 v, float angle)
    {
        float sin = Mathf.Sin(angle * Mathf.Deg2Rad);
        float cos = Mathf.Cos(angle * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
    public static Vector2 RotateVector(Vector2 v, float angle)
    {
        float sin = Mathf.Sin(angle * Mathf.Deg2Rad);
        float cos = Mathf.Cos(angle * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
    public static float Project(this Vector2 projectedVector, Vector2 targetVector)
    {
        float dotProduct = Vector2.Dot(targetVector, projectedVector);
        return dotProduct / targetVector.magnitude;
    }
    public static float ProjectVector(Vector2 projectedVector, Vector2 targetVector)
    {
        float dotProduct = Vector2.Dot(targetVector, projectedVector);
        return dotProduct / projectedVector.magnitude;
    }
}