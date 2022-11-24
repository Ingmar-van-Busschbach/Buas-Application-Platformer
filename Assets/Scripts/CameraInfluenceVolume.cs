using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInfluenceVolume : MonoBehaviour
{
    // Requisites
    private Vector2 location;

    // Variables
    [SerializeField] private Vector2 offset; // Offsets the position at which the camera will focus from the transform position of the object.
    [SerializeField] private float cameraDistance = 10f;
    [SerializeField] private float priority = 10f; // Priority uses a comparative bias system. A 10 to 1 priority ratio will mean that the camera will focus mostly at the 10 priority volume.
    [Range(0, 35)] // Clamp to 35 as beyond that distance the current version of CameraController.cs discards the volume anyways.
    [SerializeField] private float radius = 5f;
    [Range(0.1f, 6)] // Determines how aggressive the camera will be influenced by the volume. A high exponent is more aggressive.
    [SerializeField] private float radiusExponent = 1f;

    private void Start()
    {
    }
    public float GetAdjustedPriority(Vector2 position)
    {// Converts a location into a 0-1 number, then adjusts it with an exponent, then returns a priority value based on that number.
        float distance = GetDistance(position);
        float distanceRatio = 1f - (Mathf.Pow(Mathf.Clamp(distance / radius, 0f, 1f), radiusExponent));
        return priority * distanceRatio;
    }
    public float GetDistance(Vector2 position)
    {// GetDistance does NOT take offset into consideration as the offset is to where the camera should blend and not for in which area the camera gets affected.
        return Mathf.Abs(Vector2.Distance(position, transform.position));
    }
    public float GetRadius()
    {// GetDistance does NOT take offset into consideration as the offset is to where the camera should blend and not for in which area the camera gets affected.
        return radius;
    }
    public Vector3 GetTargetPosition()
    {// Takes offset in consideration, allowing you to make the camera focus on a different object than the trigger volume itself.
        Vector2 adjustedLocation = (Vector2)transform.position + offset;
        return new Vector3(adjustedLocation.x, adjustedLocation.y, -cameraDistance);
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, (radius * Mathf.Pow(0.5f, 1/radiusExponent)));
    }
}
