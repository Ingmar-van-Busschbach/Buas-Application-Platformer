using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private string forward = "d";
    [SerializeField] private string backward = "a";
    [SerializeField] private string jump = "space";
    private Vector2 upDirection = new Vector2(0,1);
    private Vector2 forwardDirection;
    private Vector2 velocity;
    private Rigidbody2D rigidBody;
    [SerializeField] private float Gravity = 987;
    [SerializeField] private float WalkSpeed;
    [SerializeField] private float drag;
    [SerializeField] private float airDrag;
    [SerializeField] private float JumpStrength; 
    [SerializeField] private float maxVelocity = 50f;

    void Start()
    {
        rigidBody = this.gameObject.GetComponent<Rigidbody2D>();

        forwardDirection = upDirection.Rotate(-90f);
    }

    void Update()
    {
        velocity = new Vector2(0, 0);
        if (rigidBody.IsTouchingLayers()) //If on ground
        {
            if (Input.GetKeyDown(jump))
            {
                //RigidBody.AddForce(upDirection * Gravity * Time.deltaTime);
                velocity += upDirection * JumpStrength;
            }
            else if (!Input.GetKey(forward) && !Input.GetKey(backward) || Input.GetKey(forward) && Input.GetKey(backward))
            {
                velocity += forwardDirection * velocity.Project(forwardDirection) * -drag * Time.deltaTime; //Friction
            }
            if (Input.GetKey(forward))
            {
                velocity += forwardDirection * WalkSpeed * Time.deltaTime;
            }
            if (Input.GetKey(backward))
            {
                velocity -= forwardDirection * WalkSpeed * Time.deltaTime;
            }
        }
        else //If in air
        {
            velocity += upDirection * -Gravity * Time.deltaTime; //Gravity

            if (!Input.GetKey(forward) && !Input.GetKey(backward) || Input.GetKey(forward) && Input.GetKey(backward))
            {
                velocity += forwardDirection * velocity.Project(forwardDirection) * -airDrag * Time.deltaTime; //Drag
            }
            if (Input.GetKey(forward))
            {
                velocity += forwardDirection * WalkSpeed * Time.deltaTime;
            }
            if (Input.GetKey(backward))
            {
                velocity -= forwardDirection * WalkSpeed * Time.deltaTime;
            }
        }
        //velocity = Vector3.ClampMagnitude(velocity, maxVelocity);
        rigidBody.AddForce(velocity);
    }
}
