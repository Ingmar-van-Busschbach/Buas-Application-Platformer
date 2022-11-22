using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Controller2D))]
public class PlayerInput : MonoBehaviour
{
    //Requisites
    Controller2D controller;
    Vector2 velocity;
    float gravity;
    float jumpVelocity;
    float currentVelocityX;
    float jumpForgivenessTimer;
    bool canJump;
    bool jumpButtonPressed;
    int currentJumpCount;

    //Constants
    const float timeToJumpApex = 0.4f;

    //Variables
    public float jumpHeight = 4;
    public float jumpForgivenessTime = 0.2f;
    public int maxJumpCount = 2;
    public float moveSpeed = 6;
    public float accelerationTimeAirborne = 0.2f;
    public float accelerationTimeGrounded = 0.1f;
    public string[] horizontalInputKeys = new string[2] { "d", "a" };
    public string jumpKey = "w";
    public bool showDebug;

    void Start()
    {
        // Get components
        controller = GetComponent<Controller2D>();

        // Setup variables
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        if (showDebug) { print("Gravity: " + gravity + "   Jump Velocity: " + jumpVelocity); }
        maxJumpCount--;
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
            if(jumpForgivenessTimer <= jumpForgivenessTime)
            {
                jumpForgivenessTimer += Time.deltaTime;
            }
            else
            {
                canJump = false;
            }
        }

        if (controller.collisionInfo.above || controller.collisionInfo.below)
        {// Reset velocity Y when touching the ground to prevent gravity accumulation
            velocity.y = 0;
        }
        if (Input.GetKey("space")){
            controller.ChanceLocalUp(new Vector2(1, 0));
        }
        else
        {
            controller.ChanceLocalUp(new Vector2(0, 1));
        }

        // Player input
        float horizontalInput = 0;
        if (Input.GetKey(horizontalInputKeys[0]))
        {
            horizontalInput += 1;
        }
        if (Input.GetKey(horizontalInputKeys[1]))
        {
            horizontalInput -= 1;
        }
        if(Input.GetKey(jumpKey))
        {
            if (!jumpButtonPressed && currentJumpCount <= maxJumpCount)
            {
                canJump = false;
                jumpButtonPressed = true;
                if (!canJump)
                {
                    currentJumpCount++;
                }

                velocity.y = jumpVelocity;
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
}
