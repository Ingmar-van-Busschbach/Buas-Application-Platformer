using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class CameraController : MonoBehaviour
{
    private GameObject camera;
    [SerializeField] private float lerpSpeed;
    [SerializeField] private Vector3 offset = new Vector3(0,0,-10);
    private Vector3 targetPosition;
    // Start is called before the first frame update
    void Start()
    {
        camera = this.gameObject.transform.GetChild(0).gameObject;
        targetPosition = camera.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        targetPosition = Vector3.Lerp(targetPosition, this.transform.position+offset, lerpSpeed * Time.deltaTime);
        camera.transform.position = targetPosition + offset;
    }
}
