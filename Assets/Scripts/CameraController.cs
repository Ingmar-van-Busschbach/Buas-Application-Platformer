using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject camera;
    private Rigidbody2D rigidBody;
    [SerializeField] private float lerpSpeed;
    [SerializeField] private float lerpStrengthX;
    [SerializeField] private float lerpStrengthY;
    [SerializeField] private float maxOffset;
    [SerializeField] private Vector3 offset = new Vector3(0,0,-10);
    private Vector3 targetPosition;
    // Start is called before the first frame update
    void Start()
    {
        camera = this.gameObject.transform.GetChild(0).gameObject;
        rigidBody = this.gameObject.GetComponent<Rigidbody2D>();
        targetPosition = camera.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        targetPosition = Vector3.Lerp(targetPosition, Vector3.ClampMagnitude(new Vector3(rigidBody.velocity.x * lerpStrengthX, -Mathf.Abs(rigidBody.velocity.y) * lerpStrengthY, offset.z), maxOffset), lerpSpeed * Time.deltaTime);
        camera.transform.localPosition = targetPosition + offset;
    }
}
