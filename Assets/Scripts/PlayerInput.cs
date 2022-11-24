using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Controller2D))]
public class PlayerInput : MonoBehaviour
{
    // Requisites
    bool canJump; // Refers to ground jump specifically. Does not affect mid-air jumps.
    bool jumpButtonPressed;
    float currentVelocityX;
    float gravity;
    float jumpForgivenessTimer;
    float jumpVelocity;
    int currentJumpCount;
    Controller2D controller;
    Vector2 velocity;

    // Variables
    // Jumps
    [SerializeField] private float jumpHeight = 4; // Jump height in units
    [SerializeField] private float jumpForgivenessTime = 0.2f; // Time in seconds within you can use your inital to jump after falling off a platform.
    [SerializeField] private int maxJumpCount = 2; // Includes initial jump. 0 = no jumps.
    [SerializeField] private float timeToJumpApex = 0.4f; // Time in seconds until the apex of the jump, after which the player falls back down.
    // Movement
    [SerializeField] private float moveSpeed = 6; // Move speed in units per second.
    [SerializeField] private float accelerationTimeAirborne = 0.2f; // Time in seconds until max movemend speed is reached in air.
    [SerializeField] private float accelerationTimeGrounded = 0.1f; // Time in seconds until max movemend speed is reached on ground.
    // Keybinds
    [SerializeField] private string[] horizontalInputKeys = new string[2] { "d", "a" };
    [SerializeField] private string jumpKey = "w";
    // Debug
    [SerializeField] private bool showDebug; // Currently only shows what values have been calculated for gravity and jumpVelocity.

    void Start()
    {
        // Get components
        controller = GetComponent<Controller2D>();

        // Setup jump variables to allow for more logical exposed settings.
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        if (showDebug) { print("Gravity: " + gravity + "   Jump Velocity: " + jumpVelocity); }
        maxJumpCount--; // Subtract initial jump from the max jump amount as it should not be included. Can delete this if maxJumps is renamed to maxAirJumps, but then it becomes unituitive.
    }

    private void FixedUpdate()
    {
        if (controller.collisionInfo.below)
        {// Reset jump data if grunded
            canJump = true;
            currentJumpCount = 0;
            jumpForgivenessTimer = 0;
        }
        else
        {// If player is not grounded, tick up to the jumpForgivenessTime, before disabling canJump. If maxJumpCount == 1, then this prevents jumping entirely. If maxJumpCount > 1, then the player will use an air jump.
            if (jumpForgivenessTimer <= jumpForgivenessTime) { jumpForgivenessTimer += Time.deltaTime; }
            else { canJump = false; }
        }

        if (controller.collisionInfo.above || controller.collisionInfo.below)
        {// Reset velocity Y when touching the ground to prevent gravity accumulation
            velocity.y = 0;
        }

        // Test code to adjust local up direction of controller. Discard for production.
        //if (Input.GetKey("space")) { controller.ChanceLocalUp(new Vector2(1, 0)); }
        //else { controller.ChanceLocalUp(new Vector2(0, 1)); }

        // Movement input
        float horizontalInput = 0;
        if (Input.GetKey(horizontalInputKeys[0])) { horizontalInput += 1; }
        if (Input.GetKey(horizontalInputKeys[1])) { horizontalInput -= 1; }

        // Jump input
        if(Input.GetKey(jumpKey))
        {
            if (!jumpButtonPressed && currentJumpCount < maxJumpCount)
            {
                jumpButtonPressed = true;
                velocity.y = jumpVelocity;
                
                if (!canJump)
                {
                    currentJumpCount++;
                }
                canJump = false;
            }
        }
        else
        {
            jumpButtonPressed = false;
        }

        // Acceleration
        float targetVelocityX = horizontalInput * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref currentVelocityX, (controller.collisionInfo.below)? accelerationTimeGrounded : accelerationTimeAirborne);
        
        // Gravity
        velocity.y += gravity * Time.deltaTime;

        // Move player
        controller.Move(velocity * Time.deltaTime, true);
    }

    public void ResetVelocity(Vector2 oldLocalUp, Vector2 newLocalUp, bool flipLocalRight = false)
    {
        float angle = Vector2.Angle(oldLocalUp, newLocalUp);
        print(angle);
        velocity = velocity.Rotate(angle);
        float projectedVelocity = velocity.Project(newLocalUp.Rotate((flipLocalRight) ? 90:-90));
        velocity -= newLocalUp.Rotate((flipLocalRight) ? 90 : -90) * projectedVelocity * 2;
    }
}
