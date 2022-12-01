using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class GravityChangeVolume : MonoBehaviour
{
    new BoxCollider2D collider;
    new Rigidbody2D rigidbody;
    private Vector2 oldUp;

    [SerializeField] private Vector2 localUp = new Vector2(0, 1);
    [SerializeField] private bool flipLocalRight = false;
    [SerializeField] private bool flipLocalRightVelocity = false;
    [SerializeField] private bool resetLocalUpOnExit = false;
    




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
            oldUp = controller.localUp;
            if(oldUp != localUp)
            {
                PlayerInput input = other.GetComponent<PlayerInput>();
                input.ResetVelocity(oldUp, localUp, flipLocalRightVelocity);
                controller.ChanceLocalUp(localUp, flipLocalRight);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 6)
        {
            if (resetLocalUpOnExit)
            {
                Controller2D controller = other.GetComponent<Controller2D>();
                controller.ChanceLocalUp(oldUp);
            }
            
        }
    }
}
