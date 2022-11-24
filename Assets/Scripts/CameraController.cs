using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class CameraController : MonoBehaviour
{
    // Requisites
    new GameObject camera;
    CameraInfluenceVolume[] cameraInfluenceVolumes;
    Vector3 targetPosition;
    Vector3 smoothVelocity;

    // Variables
    [SerializeField] private float lerpSpeed = 3;
    [SerializeField] private float lerpDistance = 10;
    [SerializeField] private Vector3 defaultOffset = new Vector3(0, 0, -10);
    [SerializeField] private bool showDebug = false;






    void Start()
    {
        camera = this.gameObject.transform.GetChild(0).gameObject;
        targetPosition = camera.transform.position;

        // Get all CameraInfluenceVolumes in scene
        cameraInfluenceVolumes = FindObjectsOfType<CameraInfluenceVolume>();
        if (showDebug) { print("Camera Influence Volumes found: " + cameraInfluenceVolumes.Length); }
    }

    void FixedUpdate()
    {
        Vector3 newPosition = transform.position;
        int volumesInRange = 0;
        List<int> indexVolumesInRange = new List<int>();
        for (int i = 0; i < cameraInfluenceVolumes.Length; i++)
        {
            float distance = cameraInfluenceVolumes[i].GetDistance(transform.position);
            if (distance < 35 && distance < cameraInfluenceVolumes[i].GetRadius())
            {
                volumesInRange++;
                indexVolumesInRange.Add(i);
            }
        }
        if (showDebug) { print("Volumes in range: " + volumesInRange); }
        switch (volumesInRange)
        {
            case 0: // Fail case, if no volumes have been found, simply have the camera follow the parent object.
                newPosition = transform.position + defaultOffset;
                break;
            case 1: // If only 1 volume is in range, track that volume.
                newPosition = Vector3.Lerp(transform.position + defaultOffset, cameraInfluenceVolumes[indexVolumesInRange[0]].GetTargetPosition(), cameraInfluenceVolumes[indexVolumesInRange[0]].GetAdjustedPriority(transform.position) / 10);
                break;
            default:
                // Reset priority values.
                float[] maxPriority = new float[3] { 0f, 0f, 0f };
                CameraInfluenceVolume[] maxPriorityVolumes = new CameraInfluenceVolume[3] {null, null, null };

                if (showDebug) { print("Checking 2 or more volumes..."); }

                // Itterate through available volumes. Having many volumes near each other in the scene is not recommended for performance reasons.
                for (int i = 0; i < indexVolumesInRange.Count; i++)
                {// Only consider volumes within 35 units distance as that's a realistic limit for the camera's viewing angle/distance, and it would be unlikely that a volume would have significant influence at that distance anyways.
                    int index = indexVolumesInRange[i];
                    float currentPriority = cameraInfluenceVolumes[index].GetAdjustedPriority(transform.position);

                    // Check if the new volume is in the top 3 priority among volumes checked so far.
                    if (currentPriority >= maxPriority[0])
                    {
                        // Shift the top priority volume to second place and second place to third place.
                        maxPriority[2] = maxPriority[1];
                        maxPriorityVolumes[2] = maxPriorityVolumes[1];
                        maxPriority[1] = maxPriority[0];
                        maxPriorityVolumes[1] = maxPriorityVolumes[0];

                        // Add newly found volume to first place
                        maxPriority[0] = currentPriority;
                        maxPriorityVolumes[0] = cameraInfluenceVolumes[index];
                    }
                    else if (currentPriority >= maxPriority[1])
                    {
                        maxPriority[2] = maxPriority[1];
                        maxPriorityVolumes[2] = maxPriorityVolumes[1];

                        maxPriority[1] = currentPriority;
                        maxPriorityVolumes[1] = cameraInfluenceVolumes[index];
                    }
                    else if (currentPriority >= maxPriority[2])
                    {
                        maxPriority[2] = currentPriority;
                        maxPriorityVolumes[2] = cameraInfluenceVolumes[index];
                    }
                }
                if (showDebug) { print("Priorities Sorted: Priority 0: " + maxPriority[0] + "Priority 1: " + maxPriority[1] + "Priority 2: " + maxPriority[2]); }
                
                // Blending behavior if there's 2 (default) volumes in range
                float blendRatio = maxPriority[0] / (maxPriority[0] + maxPriority[1] + maxPriority[2]);
                Vector3 blendPositionResult = maxPriorityVolumes[1].GetTargetPosition();
                Vector3 blendPosition = Vector3.Lerp(blendPositionResult, maxPriorityVolumes[0].GetTargetPosition(), blendRatio);

                // Blending behavior if there's 3 volumes in range
                if (volumesInRange > 2)
                {
                    float blendRatio2 = maxPriority[1] / (maxPriority[0] + maxPriority[1] + maxPriority[2]);
                    float blendRatio3 = maxPriority[2] / (maxPriority[0] + maxPriority[1] + maxPriority[2]);
                    Vector3 blendPosition2 = Vector3.Lerp(maxPriorityVolumes[0].GetTargetPosition(), maxPriorityVolumes[1].GetTargetPosition(), blendRatio2);
                    Vector3 blendPosition3 = Vector3.Lerp(maxPriorityVolumes[0].GetTargetPosition(), maxPriorityVolumes[2].GetTargetPosition(), blendRatio3);
                    float blendRatioResult = maxPriority[1] / (maxPriority[1] + maxPriority[2]);
                    blendPosition = Vector3.Lerp(blendPosition2, blendPosition3, blendRatioResult);
                }

                // Calculate the ratio between the two highest priority volumes and blend a target position between them.
                newPosition = blendPosition;
                break;
        }

        float targetDistance = Vector3.Distance(targetPosition, newPosition);
        float distanceRatio = Mathf.Clamp(targetDistance / lerpDistance, 1, 4);
        
        targetPosition = Vector3.Lerp(targetPosition, newPosition, (distanceRatio * lerpSpeed) * Time.deltaTime); // Lerp over time yields better results than lerp over distance.
        
        camera.transform.position = targetPosition;
    }
}