using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Controller2D))]
public class PlayerInput : MonoBehaviour
{
    // Requisites
    private bool canJump; // Refers to ground jump specifically. Does not affect mid-air jumps.
    private bool jumpButtonPressed;
    private bool respawnButtonPressed;
    private bool isInLowGravity;
    private float currentVelocityX;
    private float gravity;
    private float lowGravity;
    private float jumpForgivenessTimer;
    private float jumpVelocity;
    private float lowGravityJumpVelocity;
    private int currentJumpCount;
    private Controller2D controller;
    private Vector2 velocity;

    // Variables
    [Header("Jump")]
    [SerializeField] private float jumpHeight = 4; // Jump height in units
    [SerializeField] private float jumpForgivenessTime = 0.2f; // Time in seconds within you can use your inital to jump after falling off a platform.
    [SerializeField] private float timeToJumpApex = 0.4f; // Time in seconds until the apex of the jump, after which the player falls back down.
    [SerializeField] private int maxJumpCount = 2; // Includes initial jump. 0 = no jumps.
    [SerializeField] private float antiGravityApexVelocity = 1.5f;
    [SerializeField] private float antiGravityApexAmount = 0.75f;
    [SerializeField] private float maxFallSpeed = 30f;

    [Header("LowGravity")]
    [SerializeField] private float lowGravityJumpHeight = 5;
    [SerializeField] private float lowGravityTimeToJumpApex = 0.6f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6; // Move speed in units per second.
    [SerializeField] private float accelerationTimeAirborne = 0.2f; // Time in seconds until max movemend speed is reached in air.
    [SerializeField] private float accelerationTimeGrounded = 0.1f; // Time in seconds until max movemend speed is reached on ground.

    [Header("Inputs")]
    [SerializeField] private string[] leftKeys = new string[3] { "a", "left", "j" };
    [SerializeField] private string[] rightKeys = new string[3] { "d", "right", "l" };
    [SerializeField] private string[] jumpKeys = new string[4] { "w", "up", "i", "space" };

    [Header("Debug")]
    [SerializeField] private bool showDebug; // Currently only shows what values have been calculated for gravity and jumpVelocity.
    [SerializeField] private bool overridePlayerStats = false;
    [SerializeField] private PlayerStats playerStats;
    
    
    
    
    
    void Start()
    {
        // Get components
        controller = GetComponent<Controller2D>();
        if (!overridePlayerStats && playerStats != null)
        {
            jumpHeight = playerStats.jumpHeight;
            jumpForgivenessTime = playerStats.jumpForgivenessTime;
            timeToJumpApex = playerStats.timeToJumpApex;
            maxJumpCount = playerStats.maxJumpCount;
            lowGravityJumpHeight = playerStats.lowGravityJumpHeight;
            lowGravityTimeToJumpApex = playerStats.lowGravityTimeToJumpApex;
            moveSpeed = playerStats.moveSpeed;
            accelerationTimeAirborne = playerStats.accelerationTimeAirborne;
            accelerationTimeGrounded = playerStats.accelerationTimeGrounded;
            showDebug = playerStats.showDebug;
            antiGravityApexVelocity = playerStats.antiGravityApexVelocity;
            antiGravityApexAmount = playerStats.antiGravityApexAmount;
        }
        // Setup jump variables to allow for more logical exposed settings.
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        lowGravity = -(2 * lowGravityJumpHeight) / Mathf.Pow(lowGravityTimeToJumpApex, 2);
        lowGravityJumpVelocity = Mathf.Abs(lowGravity) * lowGravityTimeToJumpApex;
        if (showDebug) { Debug.Log("Gravity: " + gravity + ", Jump Velocity: " + jumpVelocity); }
        if (showDebug) { Debug.Log("Low Gravity: " + lowGravity + ", Low Jump Velocity: " + lowGravityJumpVelocity); }
        maxJumpCount--; // Subtract initial jump from the max jump amount as it should not be included. Can delete this if maxJumps is renamed to maxAirJumps, but then it becomes unituitive.

    }

    private void FixedUpdate()
    {
        if (( Input.GetKey("r") || Input.GetKey("backspace")))
        {
            if (!respawnButtonPressed)
            {
                respawnButtonPressed = true;
                controller.ChanceLocalUp(new Vector2(0, 1));
                controller.Respawn();
            }
        }
        else
        {
            respawnButtonPressed = false;
        }
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
        // if (Input.GetKey("b")) { controller.ChanceLocalUp(new Vector2(0.5f, 0.5f)); }
        // else { controller.ChanceLocalUp(new Vector2(0, 1)); }

        // Movement input
        float horizontalInput = 0;
        if (Input.GetKey(rightKeys[0]) || Input.GetKey(rightKeys[1]) || Input.GetKey(rightKeys[2])) { horizontalInput += 1; }
        if (Input.GetKey(leftKeys[0]) || Input.GetKey(leftKeys[1]) || Input.GetKey(leftKeys[2])) { horizontalInput -= 1; }

        // Jump input
        if (Input.GetKey(jumpKeys[0]) || Input.GetKey(jumpKeys[1]) || Input.GetKey(jumpKeys[2]) || Input.GetKey(jumpKeys[3]))
        {
            if ((!jumpButtonPressed && currentJumpCount < maxJumpCount) || (!jumpButtonPressed && canJump && currentJumpCount == 0))
            {
                jumpButtonPressed = true;
                velocity.y = isInLowGravity ? lowGravityJumpVelocity : jumpVelocity;
                
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
        if(velocity.y < antiGravityApexVelocity && velocity.y > -antiGravityApexVelocity)
        {// If velocity is near 0, then have reduced gravity to allow the player to time jumps better, antigravity apex mechanic
            velocity.y += (isInLowGravity ? lowGravity : gravity) * antiGravityApexAmount * Time.deltaTime;
        }
        else
        {
            velocity.y += (isInLowGravity ? lowGravity : gravity) * Time.deltaTime;
        }
        velocity.y = Mathf.Clamp(velocity.y, -maxFallSpeed, maxFallSpeed); // Clamp falling speed

        
        // Move player
        controller.Move(velocity * Time.deltaTime, true);
    }

    public void ResetVelocity(Vector2 oldLocalUp, Vector2 newLocalUp, bool flipLocalRight = false, bool flipLocalRightVelocity = false)
    {
        float angle = Vector2.Angle(oldLocalUp, newLocalUp);
        print(angle);
        velocity = velocity.Rotate(angle);
        float projectedVelocity = velocity.Project(newLocalUp.Rotate((flipLocalRight) ? 90:-90));
        if (flipLocalRight) { velocity -= newLocalUp.Rotate(90) * projectedVelocity * 2; }
    }

    public void ChangeLowGravity(bool newGravity)
    {
        isInLowGravity = newGravity;
    }
}
