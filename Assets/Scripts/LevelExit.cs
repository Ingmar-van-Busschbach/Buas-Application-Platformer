using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class LevelExit : MonoBehaviour
{
    new BoxCollider2D collider;
    new Rigidbody2D rigidbody;
    [SerializeField] private string targetScene = "MainMenu";

    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        collider.isTrigger = true;
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        print(other.gameObject.name + " : " + gameObject.name + " : " + Time.time);
        if (other.gameObject.layer == 6)
        {
            SceneManager.LoadScene(targetScene, LoadSceneMode.Single);
        }
    }
}
