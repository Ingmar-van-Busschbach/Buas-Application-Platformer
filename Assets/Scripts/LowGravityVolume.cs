using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[ExecuteInEditMode]
public class LowGravityVolume : MonoBehaviour
{
    new BoxCollider2D collider;
    new Rigidbody2D rigidbody;
    new SpriteRenderer spriteRenderer;
    [SerializeField] private Vector2 bottomLeftCorner;
    [SerializeField] private Vector2 topRightCorner;

    void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
        collider.isTrigger = true;
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        spriteRenderer = GetComponent<SpriteRenderer>();
        
    }

    private void Update()
    {
        transform.position = new Vector3((bottomLeftCorner.x + topRightCorner.x) / 2, (bottomLeftCorner.y + topRightCorner.y) / 2, -0.25f);
        Vector2 scale = new Vector2(topRightCorner.x - bottomLeftCorner.x, topRightCorner.y - bottomLeftCorner.y);
        collider.size = scale;
        spriteRenderer.size = scale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 6)
        {
            PlayerInput input = other.GetComponent<PlayerInput>();
            input.ChangeLowGravity(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 6)
        {
            PlayerInput input = other.GetComponent<PlayerInput>();
            input.ChangeLowGravity(false);
        }
    }
}
