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

    // Variables
    [SerializeField] private float lerpSpeed = 3;
    




    void Start()
    {
        camera = this.gameObject.transform.GetChild(0).gameObject;
        targetPosition = transform.position;

        // Get all CameraInfluenceVolumes in scene
        cameraInfluenceVolumes = FindObjectsOfType<CameraInfluenceVolume>();
        print(cameraInfluenceVolumes.Length);
    }

    void FixedUpdate()
    {
        Vector3 newPosition = transform.position;
        int volumesInRange = 0;
        List<int> indexVolumesInRange = new List<int>();
        for (int i = 0; i < cameraInfluenceVolumes.Length; i++)
        {
            float distance = cameraInfluenceVolumes[i].GetDistance(transform.position);
            print(i + ": " + distance);
            if (distance < 35 && distance < cameraInfluenceVolumes[i].GetRadius())
            {
                volumesInRange++;
                indexVolumesInRange.Add(i);
            }
        }
        print(volumesInRange);
        switch (volumesInRange)
        {
            case 0: // Fail case, if no volumes have been found, simply have the camera follow the parent object.
                newPosition = transform.position;
                break;
            case 1: // If only 1 volume is in range, track that volume.
                newPosition = cameraInfluenceVolumes[indexVolumesInRange[0]].GetTargetPosition();
                break;
            default:
                // Reset priority values.
                float[] maxPriority = new float[3] { 0f, 0f, 0f };
                int[] maxPriorityIndex = new int[3] { 0, 0, 0 };

                // Itterate through available volumes. Having many volumes near each other in the scene is not recommended for performance reasons.
                for (int i = 0; i < indexVolumesInRange.Count; i++)
                {// Only consider volumes within 35 units distance as that's a realistic limit for the camera's viewing angle/distance, and it would be unlikely that a volume would have significant influence at that distance anyways.
                    int index = indexVolumesInRange[i];
                    float currentPriority = cameraInfluenceVolumes[index].GetAdjustedPriority(transform.position);

                    // Check if the new volume is in the top 3 priority among volumes checked so far.
                    if (currentPriority >= maxPriority[2] && currentPriority < maxPriority[1])
                    {
                        maxPriority[2] = currentPriority;
                        maxPriorityIndex[2] = index;
                    }
                    else if (currentPriority >= maxPriority[1] && currentPriority < maxPriority[0])
                    {
                        maxPriority[2] = maxPriority[1];
                        maxPriorityIndex[2] = maxPriorityIndex[1];

                        maxPriority[1] = currentPriority;
                        maxPriorityIndex[1] = index;
                    }
                    else if (currentPriority >= maxPriority[0])
                    {
                        // Shift the top priority volume to second place and second place to third place.
                        maxPriority[2] = maxPriority[1];
                        maxPriorityIndex[2] = maxPriorityIndex[1];
                        maxPriority[1] = maxPriority[0];
                        maxPriorityIndex[2] = maxPriorityIndex[0];

                        // Add newly found volume to first place
                        maxPriority[0] = currentPriority;
                        maxPriorityIndex[0] = index;
                    }
                }
                float blendRatio = maxPriority[0] / (maxPriority[0] + maxPriority[1] + maxPriority[2]);
                Vector3 blendPositionResult = cameraInfluenceVolumes[maxPriorityIndex[1]].GetTargetPosition();
                if (volumesInRange > 2)
                {
                    float blendRatio2 = maxPriority[1] / (maxPriority[0] + maxPriority[1] + maxPriority[2]);
                    float blendRatio3 = maxPriority[2] / (maxPriority[0] + maxPriority[1] + maxPriority[2]);
                    Vector3 blendPosition2 = Vector3.Lerp(cameraInfluenceVolumes[maxPriorityIndex[0]].GetTargetPosition(), cameraInfluenceVolumes[maxPriorityIndex[1]].GetTargetPosition(), blendRatio2);
                    Vector3 blendPosition3 = Vector3.Lerp(cameraInfluenceVolumes[maxPriorityIndex[0]].GetTargetPosition(), cameraInfluenceVolumes[maxPriorityIndex[2]].GetTargetPosition(), blendRatio3);
                    float blendRatioResult = maxPriority[1] / (maxPriority[1] + maxPriority[2]);
                    blendPositionResult = Vector3.Lerp(blendPosition2, blendPosition3, blendRatioResult);
                }
                // Calculate the ratio between the two highest priority volumes and blend a target position between them.
                Vector3 blendPosition = Vector3.Lerp(blendPositionResult, cameraInfluenceVolumes[maxPriorityIndex[0]].GetTargetPosition(), blendRatio);
                newPosition = blendPosition;
                break;
        }

        // Select the blended position or the highest priority volume position depending on how many volumes in range, to prevent issues if there's only 1 available volume.
        targetPosition = Vector3.Lerp(targetPosition, newPosition, lerpSpeed * Time.deltaTime);
        camera.transform.position = targetPosition;
    }
}