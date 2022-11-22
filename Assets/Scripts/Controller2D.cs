using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour
{
    // Requisites
    BoxCollider2D collider;
    RayCastOrigins rayCastOrigins;
    float horizontalRaySpacing;
    float verticalRaySpacing;
    Vector2 projectedVelocity;
    Vector2 localForward;

    // Constants
    const float skinWidth = 0.015f;
    [Range(2, 25)] // Clamp to make sure collision checks are made for each corner.
    public int horizontalRayCount = 4;
    [Range(2, 25)] // Clamp to make sure collision checks are made for each corner.
    public int verticalRayCount = 4;

    // Variables
    public LayerMask collisionMask;
    public CollisionInfo collisionInfo;
    public Vector2 localUp = new Vector2(0, 1);
    public bool showDebug;






    void Start()
    {
        localForward = localUp.Rotate(-90);
        collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing(); // As we only update ray spacing once, you cannot change horizontalRayCount or verticalRayCount at runtime. If those need to be changed at runtime, move this function to function Move.
    }

    // Move should only be called from function FixedUpdate to guarantee physics consistency.
    public void Move(Vector2 velocity, bool convertToWorldSpace = false)
    {
        if (convertToWorldSpace)
        {// If the input velocity is local space and does not already account for a custom LocalUp direction, convert velocity to worldspace.
            Vector2 worldVelocity = localForward * velocity.x + localUp * velocity.y;
            velocity = worldVelocity;
        }

        UpdateRayCastOrigins();

        if (showDebug) { collisionInfo.PrintData(); }
        collisionInfo.Reset();

        // Project world space velocity into a new local space velocity to do calculations with.
        projectedVelocity.x = velocity.Project(localForward);
        projectedVelocity.y = velocity.Project(localUp);

        // Check collisions only if moving. Velocity references are used to allow the collision checks to update the velocity to prevent clipping.
        if (projectedVelocity.x != 0)
        {
            HorizontalCollisions(ref velocity);
        }
        if (projectedVelocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }

        // Recombine velocities through their own projected local axis
        velocity = localForward * projectedVelocity.x + localUp * projectedVelocity.y;

        // Move
        transform.Translate(velocity);
        if (showDebug) { print("Velocity: " + velocity); }
}





    // Collision functions
    void HorizontalCollisions(ref Vector2 velocity)
    {
        int directionX = (int)Mathf.Sign(projectedVelocity.x);
        float rayLength = Mathf.Abs(projectedVelocity.x) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? rayCastOrigins.bottomLeft : rayCastOrigins.bottomRight;
            rayOrigin += localUp * horizontalRaySpacing * i  ;

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, localForward * directionX, rayLength, collisionMask);
            if (showDebug) { Debug.DrawRay(((directionX == -1) ? rayCastOrigins.bottomLeft : rayCastOrigins.bottomRight) + localUp * horizontalRaySpacing * i, localForward * directionX * 2, Color.green); }

            if (hit)
            {
                projectedVelocity.x = (hit.distance - skinWidth) * directionX;
                rayLength = hit.distance;

                collisionInfo.left = directionX == -1;
                collisionInfo.right = directionX == 1;
            }
        }
    }
    void VerticalCollisions(ref Vector2 velocity)
    {
        int directionY = (int)Mathf.Sign(projectedVelocity.y);
        float rayLength = Mathf.Abs(projectedVelocity.y) + skinWidth;

        for(int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? rayCastOrigins.bottomLeft : rayCastOrigins.topLeft;
            rayOrigin += localForward * (verticalRaySpacing * i + projectedVelocity.x);
            
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, localUp * directionY, rayLength, collisionMask);
            if (showDebug) { Debug.DrawRay(((directionY == -1) ? rayCastOrigins.bottomLeft : rayCastOrigins.topLeft) + localForward * verticalRaySpacing * i, localUp * directionY * 2, Color.red); }

            if (hit)
            {
                projectedVelocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                collisionInfo.above = directionY == 1;
                collisionInfo.below = directionY == -1;
            }
        }
    }





    // RaycastOrigins functions
    void UpdateRayCastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);
        Vector2 test1 = (Vector2)bounds.center + -localUp * bounds.extents + -localForward * bounds.extents;
        Vector2 test2 = (Vector2)bounds.center + -localUp * bounds.extents + localForward * bounds.extents;
        Vector2 test3 = (Vector2)bounds.center + localUp * bounds.extents + -localForward * bounds.extents;
        Vector2 test4 = (Vector2)bounds.center + localUp * bounds.extents + localForward * bounds.extents;
        rayCastOrigins.bottomLeft = test1;//new Vector2(bounds.min.x, bounds.min.y);
        rayCastOrigins.bottomRight = test2;//new Vector2(bounds.max.x, bounds.min.y);
        rayCastOrigins.topLeft = test3;//new Vector2(bounds.min.x, bounds.max.y);
        rayCastOrigins.topRight = test4;//new Vector2(bounds.max.x, bounds.max.y);
    }

    void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRaySpacing = bounds.size.x / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.y / (verticalRayCount - 1);
    }

    struct RayCastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }





    // Collision data functions
    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public void Reset()
        {
            above = below = false;
            left = right = false;
        }
        public void PrintData()
        {
            print("Above: " + above);
            print("Below: " + below);
            print("Left: " + left);
            print("Right: " + right);
        }
    }





    // Local up functions
    public void ChanceLocalUp(Vector2 newLocalUp)
    {
        newLocalUp.Normalize();
        localUp = newLocalUp;
        localForward = localUp.Rotate(-90);
    }
    public void RotateLocalUp(float angle)
    {
        localUp = localUp.Rotate(angle);
        localForward = localUp.Rotate(-90);
    }
}