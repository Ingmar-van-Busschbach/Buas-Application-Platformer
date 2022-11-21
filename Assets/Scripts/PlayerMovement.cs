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
    private Rigidbody2D rigidBody;
    private Collider2D collider;
    [SerializeField] private float Gravity = 987;
    [SerializeField] private float WalkSpeed;
    [SerializeField] private float drag;
    [SerializeField] private float airDrag;
    [SerializeField] private float JumpStrength; 
    [SerializeField] private float maxVelocity = 50f;

    void Start()
    {
        rigidBody = this.gameObject.GetComponent<Rigidbody2D>();
        collider = this.gameObject.GetComponent<Collider2D>();

        forwardDirection = upDirection.Rotate(-90f);

    }

    void Update()
    {
        if (rigidBody.IsTouchingLayers()) //If on ground
        {
            if (Input.GetKeyDown(jump))
            {
                //RigidBody.AddForce(upDirection * Gravity * Time.deltaTime);
                rigidBody.AddForce(upDirection * JumpStrength);
            }
            else if (!Input.GetKey(forward) && !Input.GetKey(backward) || Input.GetKey(forward) && Input.GetKey(backward))
            {
                rigidBody.AddForce(forwardDirection * rigidBody.velocity.Project(forwardDirection) * -drag * Time.deltaTime); //Friction
            }
            if (Input.GetKey(forward))
            {
                rigidBody.AddForce(forwardDirection * Mathf.Clamp((rigidBody.velocity.Project(forwardDirection)), -1f, 0f) * -drag * Time.deltaTime); //Apply friction IF going left while pressing right
                rigidBody.AddForce(forwardDirection * WalkSpeed * Time.deltaTime);
            }
            if (Input.GetKey(backward))
            {
                rigidBody.AddForce(forwardDirection * Mathf.Clamp((rigidBody.velocity.Project(forwardDirection)), 0f, 1f) * -drag * Time.deltaTime); //Apply friction IF going right while pressing left
                rigidBody.AddForce(-forwardDirection * WalkSpeed * Time.deltaTime);
            }
        }
        else //If in air
        {
            rigidBody.AddForce(upDirection * -Gravity * Time.deltaTime);

            if (!Input.GetKey(forward) && !Input.GetKey(backward) || Input.GetKey(forward) && Input.GetKey(backward))
            {
                rigidBody.AddForce(forwardDirection * rigidBody.velocity.Project(forwardDirection) * -airDrag * Time.deltaTime);
            }
            if (Input.GetKey(forward))
            {
                rigidBody.AddForce(forwardDirection * Mathf.Clamp((rigidBody.velocity.Project(forwardDirection)), -1f, 0f) * -airDrag * Time.deltaTime); //Apply friction IF going left while pressing right
                rigidBody.AddForce(forwardDirection * WalkSpeed * Time.deltaTime);
            }
            if (Input.GetKey(backward))
            {
                rigidBody.AddForce(forwardDirection * Mathf.Clamp((rigidBody.velocity.Project(forwardDirection)), 0f, 1f) * -airDrag * Time.deltaTime); //Apply friction IF going right while pressing left
                rigidBody.AddForce(-forwardDirection * WalkSpeed * Time.deltaTime);
            }
        }
        rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, maxVelocity);
    }
}
