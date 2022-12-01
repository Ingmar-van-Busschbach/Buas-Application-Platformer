using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class CheckPoint : MonoBehaviour
{
    new BoxCollider2D collider;
    new Rigidbody2D rigidbody;

    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        collider.isTrigger = true;
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 6)
        {
            Controller2D controller = other.GetComponent<Controller2D>();
            controller.respawnLocation = transform.position;
        }
    }
}
